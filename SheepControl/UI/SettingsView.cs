using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using SheepControl.Trucs;
using SheepControl.UI.CustomComponents;
using IPA.Config.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using BeatSaberPlus.SDK.UI;
using BeatSaberMarkupLanguage;
using System.Reflection;
using Button = UnityEngine.UI.Button;
using UnityEngine;
using BeatSaberMarkupLanguage.Parser;
using SliderSetting = BeatSaberMarkupLanguage.Components.Settings.SliderSetting;
using ToggleSetting = BeatSaberMarkupLanguage.Components.Settings.ToggleSetting;
using TMPro;
using CP_SDK.Network;
using System.Net.Http;
using System.IO;
using IPA.Loader;
using System.Windows.Forms;
using System.Collections;
using UnityEngine.Rendering;
using BeatmapEditor3D;
using HMUI;

namespace SheepControl.UI
{
    /*class WhiteUser
    {
        SettingsView m_Parent = null;

        string m_Name = string.Empty;

        [UIComponent("StrSettingHorizontal")] private HorizontalLayoutGroup m_StrSettingHorizontal = null;

        CustomStringSetting m_StrSetting = null;

        [UIValue("OnSelected")]
        string Value
        {
            get => m_Name;
            set { }
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            m_StrSetting = CustomUIComponent.Create<CustomStringSetting>(m_StrSettingHorizontal.transform, true);
            m_StrSetting.Setup(m_Name, "WhitelistNames", 16, false, m_Parent.m_Keyboard);
            m_StrSetting.OnChange += (p_Item, p_OldValue, p_Value) =>
            {
                for (int l_i = 0; l_i < SConfig.GetStaticModSettings().WhitelistNames.Count; l_i++)
                {
                    if (SConfig.GetStaticModSettings().WhitelistNames[l_i] == m_Name)
                    {
                        SConfig.GetStaticModSettings().WhitelistNames[l_i] = p_Value;
                        m_Name = p_Value;
                        SConfig.Instance.Save();
                        m_Parent.Reload();
                    }
                }
            };
        }

        [UIAction("delete")]
        private void Delete()
        {
            SConfig.GetStaticModSettings().WhitelistNames.Remove(m_Name);
            m_Parent.Reload();
        }

        public WhiteUser(string p_Name, SettingsView p_Parent)
        {
            m_Name = p_Name;
            m_Parent = p_Parent;
        }
    }
    class BadWord
    {
        string m_Name = string.Empty;

        SettingsView m_Parent;

        [UIComponent("StrSettingHorizontal")] HorizontalLayoutGroup m_StrSettingHorizontal = null;

        CustomStringSetting m_StringSetting;

        [UIValue("OnSelected")]
        string Value
        {
            get => m_Name;
            set { }
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            m_StringSetting = CustomUIComponent.Create<CustomStringSetting>(m_StrSettingHorizontal.transform, true);
            m_StringSetting.Setup(m_Name, "BannedWords", 32, false, m_Parent.m_Keyboard);
            m_StringSetting.OnChange += (p_Item, p_OldValue, p_Value) =>
            {
                for (int l_i = 0; l_i < SConfig.GetStaticModSettings().BannedWords.Count; l_i++)
                {
                    if (SConfig.GetStaticModSettings().BannedWords[l_i] == m_Name)
                    {
                        SConfig.GetStaticModSettings().BannedWords[l_i] = p_Value;
                        m_Name = p_Value;
                        SConfig.Instance.Save();
                        m_Parent.Reload();
                    }
                }
            };
        }

        [UIAction("delete")]
        private void Delete()
        {
            SConfig.GetStaticModSettings().BannedWords.Remove(m_Name);
            m_Parent.Reload();
        }

        public BadWord(string p_Name, SettingsView p_Parent)
        {
            m_Name = p_Name;
            m_Parent = p_Parent;
        }
    }
    class BannedCommand
    {
        string m_Name = string.Empty;

        SettingsView m_Parent;

        [UIComponent("StrSettingHorizontal")] HorizontalLayoutGroup m_StrSettingHorizontal = null;

        CustomStringSetting m_StringSetting;

        [UIValue("OnSelected")]
        string Value
        {
            get => m_Name;
            set { }
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            m_StringSetting = CustomUIComponent.Create<CustomStringSetting>(m_StrSettingHorizontal.transform, true);
            m_StringSetting.Setup(m_Name, "BannedCommands", 32, false, m_Parent.m_Keyboard);
            m_StringSetting.OnChange += (p_Item, p_OldValue, p_Value) =>
            {
                for (int l_i = 0; l_i < SConfig.GetStaticModSettings().BannedCommands.Count; l_i++)
                {
                    if (SConfig.GetStaticModSettings().BannedCommands[l_i] == m_Name)
                    {
                        SConfig.GetStaticModSettings().BannedCommands[l_i] = p_Value;
                        m_Name = p_Value;
                        SConfig.Instance.Save();
                        m_Parent.Reload();
                    }
                }
            };
        }

        [UIAction("delete")]
        private void Delete()
        {
            SConfig.GetStaticModSettings().BannedCommands.Remove(m_Name);
            SConfig.Instance.Save();
            m_Parent.Reload();
        }

        public BannedCommand(string p_Name, SettingsView p_Parent)
        {
            m_Name = p_Name;
            m_Parent = p_Parent;
        }
    }
    class BannedQuery
    {
        string m_Name = string.Empty;

        SettingsView m_Parent;

        [UIComponent("StrSettingHorizontal")] HorizontalLayoutGroup m_StrSettingHorizontal = null;

        CustomStringSetting m_StringSetting;

        [UIValue("OnSelected")]
        string Value
        {
            get => m_Name;
            set { }
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            m_StringSetting = CustomUIComponent.Create<CustomStringSetting>(m_StrSettingHorizontal.transform, true);
            m_StringSetting.Setup(m_Name, "BannedQueries", 32, false, m_Parent.m_Keyboard);
            m_StringSetting.OnChange += (p_Item, p_OldValue, p_Value) =>
            {
                for (int l_i = 0; l_i < SConfig.GetStaticModSettings().BannedQueries.Count; l_i++)
                {
                    if (SConfig.GetStaticModSettings().BannedQueries[l_i] == m_Name)
                    {
                        SConfig.GetStaticModSettings().BannedQueries[l_i] = p_Value;
                        m_Name = p_Value;
                        SConfig.Instance.Save();
                        m_Parent.Reload();
                    }
                }
            };
        }

        [UIAction("delete")]
        private void Delete()
        {
            SConfig.GetStaticModSettings().BannedQueries.Remove(m_Name);
            SConfig.Instance.Save();
            m_Parent.Reload();
        }

        public BannedQuery(string p_Name, SettingsView p_Parent)
        {
            m_Name = p_Name;
            m_Parent = p_Parent;
        }
    }

    public class SettingsView : ViewController<SettingsView>
    {

        internal static SettingsView m_Instance;

        internal string UpdateFileLocation = $"./UserData/{CP_SDK.ChatPlexSDK.ProductName}/Sheep/update.txt";

        string m_UpdateModUrl = string.Empty;

        [UIObject("Tabs")] private readonly GameObject m_Tabs = null;
        [UIObject("TabSelector")] GameObject m_TabSelector = null;

        CustomTextSegmentedControl m_CustomTabSelector = null;

        [UIComponent("WhiteListList")] CustomCellListTableData m_WhiteList = null;
        [UIComponent("BadWordsListList")] CustomCellListTableData m_BadWordsList = null;
        [UIComponent("BannedCommandsList")] CustomCellListTableData m_BannedCommandsList = null;
        [UIComponent("BannedQueriesList")] CustomCellListTableData m_BannedQueriesList = null;
        [UIComponent("QuickActionsTransform")] VerticalLayoutGroup m_QuickActionsTransform = null;

        [UIObject("wlistdiv")] private readonly GameObject m_WButtonParent = null;
        [UIObject("bwordsdiv")] private readonly GameObject m_BWordButtonParent = null;
        [UIObject("bcommandsdiv")] private readonly GameObject m_BCommandsButtonParent = null;
        [UIObject("bqueriesdiv")] private readonly GameObject m_BQueriesButtonParent = null;

        [UIComponent("WhitelistTab")] HorizontalLayoutGroup m_WhitelistTab = null;
        [UIComponent("BadWordsTab")] HorizontalLayoutGroup m_BadWordsTab = null;
        [UIComponent("BannedCommandsTab")] HorizontalLayoutGroup m_BannedCommandsTab = null;
        [UIComponent("BannedQueriesTab")] HorizontalLayoutGroup m_BannedQueriesTab = null;
        [UIComponent("OtherTab")] HorizontalLayoutGroup m_OthersTab = null;
        [UIComponent("QuickActionsTab")] HorizontalLayoutGroup m_QuickActionsTab = null;
        [UIComponent("Updates")] HorizontalLayoutGroup m_UpdatesTab = null;

        [UIComponent("BobbyRandomMoves")] BeatSaberMarkupLanguage.Components.Settings.ToggleSetting m_EnableBobbyMoves = null;
        [UIComponent("bobbymduration")] SliderSetting m_BobbyMoveDurationSlider = null;
        [UIComponent("bobbysduration")] SliderSetting m_BobbyStealDurationSlider = null;
        [UIComponent("bobbytduration")] SliderSetting m_BobbyTurnDurationSlider = null;

        [UIComponent("AskCommands")] ToggleSetting m_AskForCommands = null;
        [UIComponent("InGameCommandsEnabled")] ToggleSetting m_InGameCommandsEnabled = null;
        [UIComponent("InMenuCommandsEnabled")] ToggleSetting m_InMenuCommandsEnabled = null;

        [UIValue("WhiteList")] List<object> m_WhiteListContent = new List<object>();
        [UIValue("BadWords")] List<object> m_BadWordsListContent = new List<object>();
        [UIValue("BannedCommands")] List<object> m_BannedCommandsContent = new List<object>();
        [UIValue("BannedQueriesContent")] List<object> m_BannedQueriesContent = new List<object>();

        Button m_AddWhiteListNameButton;
        Button m_AddBWordNameButton;
        Button m_AddBCommandNameButton;
        Button m_AddBQueryNameButton;

        Button m_RandomLightsButton = null;
        Button m_ForceBobbyRondeButton = null;
        Button m_ResetBobbyButton = null;
        Button m_ResetConfigButton = null;
        Button m_BobbyReleaseButton = null;
        Button m_EnableAutoDissolveNotes = null;
        Button m_DisableAutoDissolveNotes = null;

        [UIObject("ModDownloadButtonObject")] GameObject m_ModDownloadButtonObject = null;
        [UIObject("ModUpdateButtonRefreshObject")] GameObject m_ModUpdateButtonRefreshObject = null;
        [UIComponent("UpdateText")] TextMeshProUGUI m_UpdateText = null;
        [UIComponent("CurrentVersionText")] TextMeshProUGUI m_CurrentVersionText = null;

        Button m_UpdateDownloadButton;
        Button m_ModUpdateRefreshButton;

        CustomVideoPlayer m_Player;

        public CustomKeyboard m_Keyboard;

        protected override sealed void OnViewCreation()
        {
            m_Instance = this;



            BSMLAction l_EnableBobbyMovesAction = new BSMLAction(this, this.GetType().GetMethod(nameof(OnBobbyMoveEnabled), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            BSMLAction l_SliderActions = new BSMLAction(this, this.GetType().GetMethod(nameof(OnSliderChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            BSMLAction l_ToggleActions = new BSMLAction(this, this.GetType().GetMethod(nameof(OnBoolChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

            ////////////////////////////////////////////////////////////////////////////

            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tabs, 0.5f);

            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableBobbyMoves, l_EnableBobbyMovesAction, SConfig.GetStaticModSettings().BobbyAutoRonde, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_AskForCommands, l_ToggleActions, SConfig.GetStaticModSettings().AskForCommands, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_InGameCommandsEnabled, l_ToggleActions, SConfig.GetStaticModSettings().IsCommandsEnabledInGame, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_InMenuCommandsEnabled, l_ToggleActions, SConfig.GetStaticModSettings().IsCommandsEnabledInMenu, true);

            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_BobbyMoveDurationSlider, l_SliderActions,
                BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Percentage, SConfig.GetStaticModSettings().BobbyMoveDuration ,true, false);

            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_BobbyStealDurationSlider, l_SliderActions,
                BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Percentage, SConfig.GetStaticModSettings().BobbyStealDuration, true, false);

            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_BobbyTurnDurationSlider, l_SliderActions,
                BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Percentage, SConfig.GetStaticModSettings().BobbyTurnDuration, true, false);

            ////////////////////////////////////////////////////////////////////////////

            m_CustomTabSelector = new CustomTextSegmentedControl(m_TabSelector.transform as RectTransform, false, new List<string>()
            { "Whitelist", "Banned Words", "Banned Commands", "Banned Queries", "Others", "Quick Actions", "Update" },
            new List<GameObject>() {
                m_WhitelistTab.gameObject, m_BadWordsTab.gameObject, m_BannedCommandsTab.gameObject,
                m_BannedQueriesTab.gameObject, m_OthersTab.gameObject ,m_QuickActionsTab.gameObject, m_UpdatesTab.gameObject });

            ////////////////////////////////////////////////////////////////////////////

            m_AddWhiteListNameButton = BeatSaberPlus.SDK.UI.Button.Create(m_WButtonParent.transform, "Add",
                () => { SConfig.GetStaticModSettings().WhitelistNames.Add("Name"); Reload(); });

            m_AddBWordNameButton = BeatSaberPlus.SDK.UI.Button.Create(m_BWordButtonParent.transform, "Add",
                () => { SConfig.GetStaticModSettings().BannedWords.Add("Word"); Reload(); });

            m_AddBCommandNameButton = BeatSaberPlus.SDK.UI.Button.Create(m_BCommandsButtonParent.transform, "Add",
                () => { SConfig.GetStaticModSettings().BannedCommands.Add("Command"); Reload(); });

            m_AddBQueryNameButton = BeatSaberPlus.SDK.UI.Button.Create(m_BQueriesButtonParent.transform, "Add",
                () => { SConfig.GetStaticModSettings().BannedQueries.Add("Query"); Reload(); });

            ////////////////////////////////////////////////////////////////////////////

            m_RandomLightsButton = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Random Lights",
                () => { SheepControl.m_CommandHandler.HandleCommand("!sudo enable RandomLights"); }, p_PreferedWidth: 40);

            m_ForceBobbyRondeButton = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Force bobby IA",
                () => { Bobby.m_Instance.StartCoroutine(Bobby.m_Instance.Ronde()); }, p_PreferedWidth: 40);

            m_BobbyReleaseButton = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Force bobby release",
                () => { Bobby.m_Instance.ReleaseAll(); }, p_PreferedWidth: 40);

            m_ResetBobbyButton = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Reset bobby position",
                () => { Bobby.m_Instance.Reset(); }, p_PreferedWidth: 40);

            m_ResetConfigButton = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Reset config",
                () => { SConfig.Instance.Reset(); Reload(); SConfig.Instance.Save(); }, p_PreferedWidth: 40); ;

            m_EnableAutoDissolveNotes = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Enable Auto dissolve",
                () => { SheepControl.m_CommandHandler.HandleCommand("!sudo enable AutoDissolveNotes"); });

            m_DisableAutoDissolveNotes = BeatSaberPlus.SDK.UI.Button.Create(m_QuickActionsTransform.transform, "Disable Auto dissolve",
                () => { SheepControl.m_CommandHandler.HandleCommand("!sudo disable AutoDissolveNotes"); });

            m_Keyboard = CustomUIComponent.Create<CustomKeyboard>(transform, true);
            Reload();

            m_UpdateDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_ModDownloadButtonObject.transform, "Download", DownloadUpdate, p_PreferedWidth: 40);
            m_ModUpdateRefreshButton = BeatSaberPlus.SDK.UI.Button.Create(m_ModUpdateButtonRefreshObject.transform, "Refresh", CheckForUpdates, p_PreferedWidth: 40);
            m_CurrentVersionText.text = $"Current version : {PluginManager.GetPluginFromId("SheepControl").HVersion.ToString()}";

            CheckForUpdates();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void OnBobbyMoveEnabled(bool p_Enabled)
        {
            SConfig.GetStaticModSettings().BobbyAutoRonde = p_Enabled;
            SConfig.Instance.Save();

            if (Bobby.m_Instance != null) { Bobby.m_Instance.ApplyConfig(); }
        }

        private void OnSliderChanged(float p_Value)
        {
            SConfig.GetStaticModSettings().BobbyMoveDuration = m_BobbyMoveDurationSlider.Value;
            SConfig.GetStaticModSettings().BobbyStealDuration = m_BobbyStealDurationSlider.Value;
            SConfig.GetStaticModSettings().BobbyTurnDuration = m_BobbyTurnDurationSlider.Value;
            SConfig.Instance.Save();
            Bobby.m_Instance.ApplyConfig();
        }

        private void OnBoolChanged(bool p_Value)
        {
            SConfig.GetStaticModSettings().AskForCommands = m_AskForCommands.Value;
            SConfig.GetStaticModSettings().IsCommandsEnabledInGame = m_InGameCommandsEnabled.Value;
            SConfig.GetStaticModSettings().IsCommandsEnabledInMenu = m_InMenuCommandsEnabled.Value;
            SConfig.Instance.Save();
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        public void Reload()
        {
            m_WhiteListContent.Clear();
            m_BadWordsListContent.Clear();
            m_BannedCommandsContent.Clear();
            m_BannedQueriesContent.Clear();
            foreach (var l_Current in SConfig.GetStaticModSettings().WhitelistNames)
            {
                m_WhiteListContent.Add(new WhiteUser(l_Current, this));
            }
            foreach (var l_Current in SConfig.GetStaticModSettings().BannedWords)
            {
                m_BadWordsListContent.Add(new BadWord(l_Current, this));
            }
            foreach (var l_Current in SConfig.GetStaticModSettings().BannedCommands)
            {
                m_BannedCommandsContent.Add(new BannedCommand(l_Current, this));
            }
            foreach (var l_Current in SConfig.GetStaticModSettings().BannedQueries)
            {
                m_BannedQueriesContent.Add(new BannedQuery(l_Current, this));
            }
            m_WhiteList.tableView.ReloadData();
            m_BadWordsList.tableView.ReloadData();
            m_BannedCommandsList.tableView.ReloadData();
            m_BannedQueriesList.tableView.ReloadData();

            SConfig.Instance.Save();
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        private void CheckForUpdates()
        {
            if (File.Exists(UpdateFileLocation))
                File.Delete(@UpdateFileLocation);

            using (System.Net.WebClient l_Client = new())
            {
                l_Client.DownloadFileAsync(new System.Uri("https://raw.githubusercontent.com/SheepVand0/SheepControl/main/Updates/Update.txt"), @UpdateFileLocation);
                l_Client.DownloadFileCompleted += OnDownloadFnished;
            }
        }

        private void OnDownloadFnished(object p_Sender, System.ComponentModel.AsyncCompletedEventArgs p_EventArg)
        {
            if (p_EventArg.Error != null)
            {
                m_UpdateText.text = "Error during getting update";
                return;
            }

            StreamReader l_File = File.OpenText(UpdateFileLocation);

            string l_FirstLine = string.Empty;

            if ((l_FirstLine = l_File.ReadLine()) == null) { m_UpdateText.text = "Error during getting updates"; return; }

            string[] l_Splited = l_FirstLine.Split(';');

            string l_Version = l_Splited[0];

            m_UpdateModUrl = l_Splited[1];

            PluginMetadata l_ModMetadata = PluginManager.GetPluginFromId("SheepControl");

            if (l_ModMetadata.HVersion != new Hive.Versioning.Version(l_Version))
            {
                m_UpdateDownloadButton.interactable = true;
                m_UpdateText.text = $"New update to : {l_Version}";
            } else
            {
                m_UpdateText.text = "No update needed";
            }

            l_File.Dispose();
        }

        private void DownloadUpdate()
        {
            string l_ModFolderPath = "IPA/Pending/Plugins/";
            string l_ModPath = $"{l_ModFolderPath}/SheepControl.dll";

            using (System.Net.WebClient l_Client = new())
            {
                if (!Directory.Exists(l_ModFolderPath))
                    Directory.CreateDirectory(l_ModFolderPath);

                m_UpdateDownloadButton.interactable = false;

                l_Client.DownloadFileAsync(new Uri(m_UpdateModUrl), l_ModPath);
                l_Client.DownloadFileCompleted += (p_Sender, p_EventARg) =>
                {
                    m_UpdateDownloadButton.interactable = true;
                    m_UpdateDownloadButton.SetButtonText("Redownload");
                    m_UpdateText.text = "Updated";
                };
                l_Client.DownloadProgressChanged += (p_Sender, p_EventArg) =>
                {
                    m_UpdateDownloadButton.SetButtonText($"{p_EventArg.ProgressPercentage}%");
                };
            }
        }
    }*/
}
