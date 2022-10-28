using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using BeatSaberPlus.SDK.Game;
using SheepControl.Trucs;
using IPA.Utilities;
using TMPro;
using HarmonyLib;
using HMUI;
using UnityEngine.UI;
using BeatSaberMarkupLanguage;

namespace SheepControl
{
    //public static

    public static class ControllerUtilsExtensions
    {
        static Button m_ContinueButton;

        static Button m_RestartButton;

        static Button m_MenuButton;

        static ButtonBinder m_Binder;

        static string m_LastSongName;

        static string m_LastSongAuthor;

        static TextMeshProUGUI m_LastSongText;

        static TextMeshProUGUI m_SongAuthorText;

        public static IEnumerator ChangePauseMenuTexts()
        {
            yield return new WaitForSeconds(0.2f);

            m_LastSongName = "Pausing is cheating";
            m_LastSongAuthor = "Kuurama";

            m_LastSongText.text = "Enable commands for this map ?";
            m_SongAuthorText.text = "Le dev en mousse";

            yield return null;
        }

        public static IEnumerator SetCustomTexts(string p_SongName, string p_SongAuthor)
        {
            m_LastSongName = p_SongName;
            m_LastSongAuthor = p_SongAuthor;

            m_LastSongText.text = m_LastSongName;
            m_SongAuthorText.text = m_LastSongAuthor;

            yield return null;
        }

        public static void ShowForCommands(this PauseMenuManager p_PauseMenuManager)
        {
            PauseController l_PauseController = Resources.FindObjectsOfTypeAll<PauseController>().First();
            IGamePause l_GamePause = l_PauseController.GetField<IGamePause, PauseController>("_gamePause");
            BeatmapObjectManager l_ObjectsManager = l_PauseController.GetField<BeatmapObjectManager, PauseController>("_beatmapObjectManager");

            LevelBar l_LevelBar = p_PauseMenuManager.GetField<LevelBar, PauseMenuManager>("_levelBar");
            m_LastSongText = l_LevelBar.GetField<TextMeshProUGUI, LevelBar>("_songNameText");
            m_SongAuthorText = l_LevelBar.GetField<TextMeshProUGUI, LevelBar>("_authorNameText");

            if (!SConfig.GetStaticModSettings().AskForCommands)
            {
                SheepControlController.Instance.StartCoroutine(SetCustomTexts("Pausing is cheating", "Kuurama"));
                return;
            }

            Resources.FindObjectsOfTypeAll<SheepControlController>().First().StartCoroutine(ChangePauseMenuTexts());

            l_PauseController.SetField("_paused", true);
            l_GamePause.Pause();
            p_PauseMenuManager.ShowMenu();
            l_ObjectsManager.HideAllBeatmapObjects(hide: true);
            l_ObjectsManager.PauseAllBeatmapObjects(pause: true);

            m_Binder = p_PauseMenuManager.GetField<ButtonBinder, PauseMenuManager>("_buttonBinder");

            m_ContinueButton = p_PauseMenuManager.GetField<Button, PauseMenuManager>("_continueButton");

            m_RestartButton = p_PauseMenuManager.GetField<Button, PauseMenuManager>("_restartButton");

            m_MenuButton = p_PauseMenuManager.GetField<Button, PauseMenuManager>("_backButton");

            m_Binder.ClearBindings();

            m_ContinueButton.SetButtonText("YES");
            m_RestartButton.SetButtonText("🍪");
            m_MenuButton.SetButtonText("NO");

            m_Binder.AddBinding(m_ContinueButton, EnableCommands);
            m_Binder.AddBinding(m_MenuButton, DisableCommands);
        }

        static void EnableCommands()
        {
            CommandHandler.IsCommandEnabled = true;
            ResumePauseForCommands();
        }

        static void DisableCommands()
        {
            CommandHandler.IsCommandEnabled = false;
            ResumePauseForCommands();
        }

        static void ResumePauseForCommands()
        {
            m_Binder.ClearBindings();

            PauseMenuManager l_PauseMenuManager = Resources.FindObjectsOfTypeAll<PauseMenuManager>().First();

            m_LastSongText.text = m_LastSongName;
            m_SongAuthorText.text = m_LastSongAuthor;

            m_ContinueButton.SetButtonText("CONTINUE");
            m_RestartButton.SetButtonText("RESTART");
            m_MenuButton.SetButtonText("MENU");

            m_Binder.AddBinding(m_ContinueButton, l_PauseMenuManager.ContinueButtonPressed);
            m_Binder.AddBinding(m_RestartButton, l_PauseMenuManager.RestartButtonPressed);
            m_Binder.AddBinding(m_MenuButton, l_PauseMenuManager.MenuButtonPressed);

            /*SongController l_SongController = Resources.FindObjectsOfTypeAll<SongController>().First();
            l_SongController.ResumeSong();*/

            PauseController l_PauseController = Resources.FindObjectsOfTypeAll<PauseController>().First();

            l_PauseController.HandlePauseMenuManagerDidPressContinueButton();
        }
    }

    [HarmonyPatch(typeof(BeatmapObjectSpawnController), nameof(BeatmapObjectSpawnController.Start))]
    class OnSongStart
    {
        static void Postfix()
        {
            //GameScenesManager l_GameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().First();
            //if (l_GameScenesManager.GetField<HashSet<string>, GameScenesManager>("_neverUnloadScenes").Contains("MenuCore")) return;

            if (SConfig.GetStaticModSettings().AskForCommands == false) { CommandHandler.IsCommandEnabled = SConfig.GetStaticModSettings().IsCommandsEnabledInGame; }

            if (SConfig.GetStaticModSettings().AskForCommands)
                Resources.FindObjectsOfTypeAll<SheepControlController>().First().StartCoroutine(Resources.FindObjectsOfTypeAll<SheepControlController>().First().PauseSong());

            PauseMenuManager l_PauseMenuManager = Resources.FindObjectsOfTypeAll<PauseMenuManager>().First();
            l_PauseMenuManager.ShowForCommands();
        }
    }

    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class SheepControlController : MonoBehaviour
    {
        public static SheepControlController Instance { get; private set; }

        public static void DestroyAll(string p_Name)
        {
            try
            {
                DestroyAll(p_Name);
            } catch (Exception l_E) { }
        }

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
                DestroyImmediate(this);
            else
                Instance = this;

            Logic.OnSceneChange += (p_Scene) =>
            {

                StartCoroutine(RondBobby());
                if (p_Scene == Logic.SceneType.Menu)
                    CommandHandler.IsCommandEnabled = true;

            };

            GameObject.DontDestroyOnLoad(this);
        }
        #endregion

        public IEnumerator PauseSong()
        {
            yield return new WaitForSeconds(0.7f);

            Resources.FindObjectsOfTypeAll<SongController>()[0].PauseSong();

            yield return null;
        }

        private IEnumerator RondBobby()
        {
            yield return new WaitForSeconds(0.5f);

            Bobby.m_Instance.StartCoroutine(Bobby.m_Instance.Ronde());
        }
    }
}
