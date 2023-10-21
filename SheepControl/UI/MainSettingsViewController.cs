using BeatSaberMarkupLanguage.Attributes;
using BeatSaberPlus.SDK.UI;
using CP_SDK.XUI;
using IPA.Loader;
using SheepControl.Trucs;
using SheepControl.UI.CustomComponents;
using SheepControl.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using SheepControl.UI.Defaults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepControl.UI
{
    internal class MainSettingsViewController : CP_SDK.UI.ViewController<MainSettingsViewController>
    {

        internal static new MainSettingsViewController Instance = null;

        XUITabControl m_TabController;

        internal string UpdateFileLocation = $"./UserData/{CP_SDK.ChatPlexSDK.ProductName}/Sheep/update.txt";

        string m_UpdateModUrl = string.Empty;

        protected override void OnViewCreation()
        {
            Templates.FullRectLayoutMainView(
                Templates.TitleBar("SheepControl"),

                XUITabControl.Make(
                    ("Whitelist", BuildWhitelistTab()),
                    ("Bad Words", BuildBadWordsTab()),
                    ("Banned Commands", BuildBannedCommandsTab()),
                    ("Banned Queries", BuildBannedQueriesTab()),
                    ("Other", BuildOtherTab()),
                    ("Quick Actions", BuildQuickActionsTab()),
                    ("Updates", BuildUpdatesTab())
                 )
             )
             .SetBackground(true, null, true)
             .BuildUI(transform);

            Instance = this;
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        XUIVScrollView m_WhitelistScrollView;
        List<CWhiteUser> m_WhiteUsersCells = new();

        private IXUIElement BuildWhitelistTab()
        {
            return XUIVLayout.Make(
                        XUIVLayout.Make(
                            XUIVScrollView.Make()
                            .Bind(ref m_WhitelistScrollView)
                        ).SetMinHeight(40)
                         .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                         .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = x.HOrVLayoutGroup.childControlWidth = true),
                        GSSecondaryButton.Make("Add", 20, 7, p_OnClick: () =>
                        {
                            SConfig.Instance.WhitelistNames.Add($"Name{SConfig.Instance.WhitelistNames.Count}");
                            UpdateWhitelist();
                        })
                    );
        }

        public void UpdateWhitelist()
        {
            foreach (var l_Index in m_WhiteUsersCells)
            {
                l_Index.SetActive(false);
            }

            var l_Whitenames = SConfig.GetStaticModSettings().WhitelistNames;
            for (int l_i = 0; l_i < l_Whitenames.Count;l_i++)
            {
                if (l_i > m_WhiteUsersCells.Count - 1)
                {
                    var l_WhiteUser = CWhiteUser.Make();
                    l_WhiteUser.BuildUI(m_WhitelistScrollView.Element.Container.transform);
                    m_WhiteUsersCells.Add(l_WhiteUser);
                }

                m_WhiteUsersCells[l_i].SetName(l_Whitenames[l_i]);
                m_WhiteUsersCells[l_i].SetActive(true);
            }
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        XUIVScrollView m_BadWordsScrollView;
        List<CBadWord> m_BadWordsCells = new();

        private IXUIElement BuildBadWordsTab()
        {
            return XUIVLayout.Make(
                        XUIVLayout.Make(
                            XUIVScrollView.Make()
                            .Bind(ref m_BadWordsScrollView)
                        ).SetMinHeight(40)
                         .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                         .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = x.HOrVLayoutGroup.childControlWidth = true),
                        GSSecondaryButton.Make("Add", 20, 7, p_OnClick: () =>
                        {
                            SConfig.Instance.BannedWords.Add($"Name{SConfig.Instance.BannedWords.Count}");
                            UpdateBadWords();
                        })
                    );
        }

        public void UpdateBadWords()
        {
            foreach (var l_Index in m_BadWordsCells)
            {
                l_Index.SetActive(false);
            }

            var l_Badwords = SConfig.Instance.BannedWords;
            for (int l_i = 0; l_i < l_Badwords.Count;l_i++)
            {
                if (l_i > m_BadWordsCells.Count - 1)
                {
                    var l_BadWord = CBadWord.Make();
                    l_BadWord.BuildUI(m_BadWordsScrollView.Element.Container.transform);
                    m_BadWordsCells.Add(l_BadWord);
                }

                m_BadWordsCells[l_i].SetWord(l_Badwords[l_i]);
                m_BadWordsCells[l_i].SetActive(true);
            }
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        XUIVScrollView m_BannedCommandsScrollView;
        List<CBannedCommand> m_BannedCommandsCells = new();

        private IXUIElement BuildBannedCommandsTab()
        {
            return XUIVLayout.Make(
                        XUIVLayout.Make(
                            XUIVScrollView.Make()
                            .Bind(ref m_BannedCommandsScrollView)
                        ).SetMinHeight(40)
                         .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                         .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = x.HOrVLayoutGroup.childControlWidth = true),
                        GSSecondaryButton.Make("Add", 20, 7, p_OnClick: () =>
                        {
                            SConfig.Instance.BannedCommands.Add($"Name{SConfig.Instance.BannedCommands.Count}");
                            UpdateBannedCommands();
                        })
                    );
        }

        public void UpdateBannedCommands()
        {
            foreach (var l_Index in m_BadWordsCells)
            {
                l_Index.SetActive(false);
            }

            var l_BannedCommands = SConfig.Instance.BannedCommands;
            for (int l_i = 0; l_i < l_BannedCommands.Count; l_i++)
            {
                if (l_i > m_BannedCommandsCells.Count - 1)
                {
                    var l_BannedCommand = CBannedCommand.Make();
                    l_BannedCommand.BuildUI(m_BannedCommandsScrollView.Element.Container.transform);
                    m_BannedCommandsCells.Add(l_BannedCommand);
                }

                m_BannedCommandsCells[l_i].SetCommand(l_BannedCommands[l_i]);
                m_BannedCommandsCells[l_i].SetActive(true);
            }
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        XUIVScrollView m_BannedQueriesScrollView;
        List<CBannedQuery> m_BannedQueriesCells = new();

        private IXUIElement BuildBannedQueriesTab()
        {
            return XUIVLayout.Make(
                        XUIVLayout.Make(
                            XUIVScrollView.Make()
                            .Bind(ref m_BannedQueriesScrollView)
                        ).SetMinHeight(40)
                         .OnReady(x => x.CSizeFitter.horizontalFit = x.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained)
                         .OnReady(x => x.HOrVLayoutGroup.childForceExpandHeight = x.HOrVLayoutGroup.childControlWidth = true),
                        GSSecondaryButton.Make("Add", 20, 7, p_OnClick: () =>
                        {
                            SConfig.Instance.BannedQueries.Add($"Name{SConfig.Instance.BannedQueries.Count}");
                            UpdateBannedQueries();
                        })
                    );
        }

        public void UpdateBannedQueries()
        {
            foreach (var l_Index in m_BadWordsCells)
            {
                l_Index.SetActive(false);
            }

            var l_BannedQueries = SConfig.Instance.BannedQueries;
            for (int l_i = 0; l_i < l_BannedQueries.Count; l_i++)
            {
                if (l_i > m_BannedQueriesCells.Count - 1)
                {
                    var l_Query = CBannedQuery.Make();
                    l_Query.BuildUI(m_BannedQueriesScrollView.Element.Container.transform);
                    m_BannedQueriesCells.Add(l_Query);
                }

                m_BannedQueriesCells[l_i].SetQuery(l_BannedQueries[l_i]);
                m_BannedQueriesCells[l_i].SetActive(true);
            }
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        private IXUIElement BuildOtherTab()
        {
            return XUIVLayout.Make(
                GSText.Make("Enable Bobby's AI:"),
                GSToggleSetting.Make()
                    .OnValueChanged(x =>
                    {
                        SConfig.Instance.BobbyAutoRonde = x;
                        SConfig.Instance.Save();
                        UpdateBobby();
                    }),
                GSText.Make("Bobby's move speed:"),
                GSSlider.Make()
                    .OnValueChanged(x =>
                    {
                        SConfig.Instance.BobbyMoveDuration = 5 / x;
                        SConfig.Instance.Save();
                        UpdateBobby();
                    }),
                GSText.Make("Bobby's steal duration:"),
                GSSlider.Make()
                    .OnValueChanged(x =>
                    {
                        SConfig.Instance.BobbyStealDuration = 5 / x;
                        SConfig.Instance.Save();
                        UpdateBobby();
                    }),
                GSText.Make("Bobby's rotation speed:"),
                GSSlider.Make()
                    .OnValueChanged(x =>
                    {
                        SConfig.Instance.BobbyTurnDuration = 5 / x;
                        SConfig.Instance.Save();
                        UpdateBobby();
                    }),
                XUIHLayout.Make(
                    XUIVLayout.Make(
                        GSText.Make("Ask for commands:"),
                        GSToggleSetting.Make()
                            .OnValueChanged(x =>
                            {
                                SConfig.Instance.AskForCommands = x;
                                SConfig.Instance.Save();
                            })
                        ),
                        XUIVLayout.Make(
                            GSText.Make("Is commands enabled:"),
                            GSText.Make("(In game)"),
                            GSToggleSetting.Make()
                                .OnValueChanged(x =>
                                {
                                    SConfig.Instance.IsCommandsEnabledInGame = x;
                                    SConfig.Instance.Save();
                                })
                            ),
                        XUIVLayout.Make(
                            GSText.Make("Is commands enabled:"),
                            GSText.Make("(In menu)"),
                            GSToggleSetting.Make()
                                .OnValueChanged(x =>
                                {
                                    SConfig.Instance.IsCommandsEnabledInMenu = x;
                                    SConfig.Instance.Save();
                                })
                            )
                    )
                );
        }

        private void UpdateBobby()
        {
            if (Bobby.m_Instance == null) return;

            Bobby.m_Instance.ApplyConfig();
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        private IXUIElement BuildQuickActionsTab()
        {
            return XUIVLayout.Make(
                GSSecondaryButton.Make("Random lights", 40, 7, p_OnClick: () =>
                {
                    SheepControl.m_CommandHandler.HandleCommand("!sudo enable RandomLights");
                }),
                GSSecondaryButton.Make("Force Bobby's AI", 40, 7, p_OnClick: () =>
                {
                    Bobby.m_Instance.StartCoroutine(Bobby.m_Instance.Ronde());
                }),
                GSSecondaryButton.Make("Force Bobby to release objects", 40, 7, p_OnClick: () =>
                {
                    Bobby.m_Instance.ReleaseAll();
                }),
                GSSecondaryButton.Make("Reset Bobby position", 40, 7, p_OnClick: () =>
                {
                    Bobby.m_Instance.Reset();
                }),
                GSSecondaryButton.Make("Reset config", 40, 7, p_OnClick: () =>
                {
                    Reset();
                }),
                GSSecondaryButton.Make("Enable auto dissolve", 40, 7, p_OnClick: () =>
                {
                    SheepControl.m_CommandHandler.HandleCommand("!sudo enable AutoDissolveNotes");
                }),
                GSSecondaryButton.Make("Enable auto dissolve", 40, 7, p_OnClick: () =>
                {
                    SheepControl.m_CommandHandler.HandleCommand("!sudo disable AutoDissolveNotes");
                })
            );
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        GSText m_ModVersionText;
        GSText m_UpdateText;
        GSSecondaryButton m_DownloadUpdateButton;

        private IXUIElement BuildUpdatesTab()
        {
            return XUIVLayout.Make(
                GSText.Make($"Current version: {PluginManager.GetPluginFromId("SheepControl").HVersion}"),
                m_UpdateText = GSText.Make(string.Empty),
                GSSecondaryButton.Make("Check for updates", 40, 7, p_OnClick: CheckForUpdates),
                (m_DownloadUpdateButton = GSSecondaryButton.Make("Download", 40, 7, p_OnClick: DownloadUpdate))
                .SetInteractable(false)
                );
        }

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

        private void DownloadUpdate()
        {
            string l_ModFolderPath = "IPA/Pending/Plugins/";
            string l_ModPath = $"{l_ModFolderPath}/SheepControl.dll";

            using (System.Net.WebClient l_Client = new())
            {
                if (!Directory.Exists(l_ModFolderPath))
                    Directory.CreateDirectory(l_ModFolderPath);

                m_DownloadUpdateButton.SetInteractable(false);

                l_Client.DownloadFileAsync(new Uri(m_UpdateModUrl), l_ModPath);
                l_Client.DownloadFileCompleted += (p_Sender, p_EventARg) =>
                {
                    m_DownloadUpdateButton.SetInteractable(true);
                    m_DownloadUpdateButton.SetText("Redownload");
                    m_UpdateText.SetText("Updated");
                };
                l_Client.DownloadProgressChanged += (p_Sender, p_EventArg) =>
                {
                    m_DownloadUpdateButton.SetText($"{p_EventArg.ProgressPercentage}%");
                };
            }
        }

        private void OnDownloadFnished(object p_Sender, System.ComponentModel.AsyncCompletedEventArgs p_EventArg)
        {
            if (p_EventArg.Error != null)
            {
                m_UpdateText.SetText("Error during getting update");
                return;
            }

            StreamReader l_File = File.OpenText(UpdateFileLocation);

            string l_FirstLine = string.Empty;

            if ((l_FirstLine = l_File.ReadLine()) == null) { m_UpdateText.SetText("Error during getting updates"); return; }

            string[] l_Splited = l_FirstLine.Split(';');

            string l_Version = l_Splited[0];

            m_UpdateModUrl = l_Splited[1];

            PluginMetadata l_ModMetadata = PluginManager.GetPluginFromId("SheepControl");

            if (l_ModMetadata.HVersion != new Hive.Versioning.Version(l_Version))
            {
                m_DownloadUpdateButton.SetInteractable(true);
                m_UpdateText.SetText($"New update to : {l_Version}");
            }
            else
            {
                m_UpdateText.SetText("No update needed");
            }

            l_File.Dispose();
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////
        

        public void Reset()
        {
            SConfig.Instance.Reset();
            UpdateBadWords();
            UpdateWhitelist();
            UpdateBannedCommands();
            UpdateBannedQueries();
        }

    }
}
