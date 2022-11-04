using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SheepControl.Core.ObjectsGrabber;
using static SheepControl.TUtils.TUtils;
using BeatSaberMarkupLanguage;
using UnityEngine.SceneManagement;
using SheepControl.Trucs;
using IPA.Utilities;
using static SheepControl.CommandHandler;
using BeatSaberPlus.SDK.Game;
using System.IO;
using SheepControl.Bundle;

namespace SheepControl.Core
{
    internal class CommandResult
    {
        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }

        internal void Init(bool p_IsError, string p_ErrorMessage = "")
        {
            IsError = p_IsError;
            ErrorMessage = p_ErrorMessage;
        }

    }

    internal class Command
    {
        public string Name { get; }

        protected Action<CommandHandler, string[], string> Function;

        public Command(string p_Name, Action<CommandHandler, string[], string> p_Function)
        {
            Name = p_Name;
            Function = p_Function;
        }

        public void Execute(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            if (Function == null) return;

            Function.Invoke(p_Handler, p_Args, p_Query);
        }
    }

    internal class Commands
    {
        public List<Command> m_Commands = new List<Command>();

        public void BindCommands()
        {
            m_Commands.Add(new Command("fakemiss",FakeMiss));
            m_Commands.Add(new Command("pause",Pause));
            m_Commands.Add(new Command("resume",Resume));
            m_Commands.Add(new Command("color", Color));
            m_Commands.Add(new Command("moverings", MoveRings));
            m_Commands.Add(new Command("rotaterings", RotateRings));
            m_Commands.Add(new Command("property", Property));
            m_Commands.Add(new Command("enable", Enable));
            m_Commands.Add(new Command("disable", Disable));
            m_Commands.Add(new Command("findtext", FindText));
            m_Commands.Add(new Command("findtextinobj", FindTextInObj));
            m_Commands.Add(new Command("findsprite", FindSprite));
            m_Commands.Add(new Command("findspriteinobj", FindSpriteInObj));
            m_Commands.Add(new Command("tdelete", TransformDesactive));
            m_Commands.Add(new Command("tactive", TransformActive));
            m_Commands.Add(new Command("delete", Delete));
            m_Commands.Add(new Command("show", Show));
            m_Commands.Add(new Command("position", Position));
            m_Commands.Add(new Command("rotation", Rotation));
            m_Commands.Add(new Command("scale", Scale));
            m_Commands.Add(new Command("loadingame", LoadInGame));
            m_Commands.Add(new Command("unloadingame", UnloadInGame));
            m_Commands.Add(new Command("bobbysteal", BobbySteal));
            m_Commands.Add(new Command("bobbyrelease", BobbyRelease));
            m_Commands.Add(new Command("bobbymove", BobbyMove));
            m_Commands.Add(new Command("bobbyturn", BobbyRot));
            m_Commands.Add(new Command("bobbyreset", BobbyReset));
            m_Commands.Add(new Command("bobbycolor", BobbyColor));
            m_Commands.Add(new Command("killbobby", KillBobby));
            m_Commands.Add(new Command("createbobby", CreateBobby));
            m_Commands.Add(new Command("songtime", SongTime));
            m_Commands.Add(new Command("play", Play));
            m_Commands.Add(new Command("e", E));
            m_Commands.Add(new Command("reload", Reload));
            m_Commands.Add(new Command("offset", Offset));
            m_Commands.Add(new Command("eoconfigs", Configs));
            m_Commands.Add(new Command("readuserdatafile", ReadUserDataFile));
            m_Commands.Add(new Command("obamium", Obamium));

        }

        //CommandHandler p_Handler, string[] p_Args, string p_Query

        protected void FakeMiss(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            NoteController l_Controller = Resources.FindObjectsOfTypeAll<NoteController>().First();
            Vector3 vector = l_Controller.noteTransform.position;
            Quaternion worldRotation = l_Controller.worldRotation;
            vector = l_Controller.inverseWorldRotation * vector;
            vector.z = 2;
            vector = worldRotation * vector;
            FlyingSpriteSpawner l_MissFlying = Resources.FindObjectsOfTypeAll<FlyingSpriteSpawner>()[0];
            l_MissFlying.SpawnFlyingSprite(vector, l_Controller.worldRotation, l_Controller.inverseWorldRotation);
        }

        protected void Pause(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Resources.FindObjectsOfTypeAll<SongController>()[0].PauseSong();
        }

        protected void Resume(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Resources.FindObjectsOfTypeAll<SongController>()[1].ResumeSong();
        }

        protected void Color(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            p_Handler.SetProperty(p_Handler, p_Args[1], Utils.ParseColor(p_Args[2]));
        }

        protected void MoveRings(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            TrackLaneRing[] l_Rings = Resources.FindObjectsOfTypeAll<TrackLaneRing>();
            for (int l_i = 0; l_i < l_Rings.Length; l_i++)
            {
                l_Rings[l_i].SetPosition(float.Parse(p_Args[1]) * l_i, float.Parse(p_Args[2]));
            }
        }

        protected void RotateRings(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            TrackLaneRing[] l_RRings = Resources.FindObjectsOfTypeAll<TrackLaneRing>();
            foreach (var l_Current in l_RRings)
            {
                l_Current.SetDestRotation(float.Parse(p_Args[1]), float.Parse(p_Args[2]));
            }
        }

        protected void Property(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            p_Handler.SetProperty(p_Handler, p_Args[1], p_Args[2]);
        }

        protected void Enable(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            p_Handler.SetProperty(p_Handler, p_Args[1], true);
        }

        protected void Disable(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            p_Handler.SetProperty(p_Handler, p_Args[1], false);
        }

        protected void FindText(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            TextMeshProUGUI[] l_Text = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            foreach (var l_Current in l_Text)
            {
                if (l_Current.name == p_Args[1])
                {
                    if (p_Query == "rich")
                        l_Current.richText = true;
                    l_Current.text = p_Handler.TransformPercentToSpaces(p_Args[2]);
                    if (p_Query == "rich")
                        l_Current.richText = false;
                    break;

                }
            }
        }

        protected void FindTextInObj(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_Obj = FindGm(p_Args[1]);
            TextMeshProUGUI l_FoundTextInObj = l_Obj.GetComponentInChildren<TextMeshProUGUI>();
            if (p_Query == "rich")
                l_FoundTextInObj.richText = true;
            l_FoundTextInObj.text = p_Handler.TransformPercentToSpaces(p_Args[2]);
            if (p_Query == "rich")
                l_FoundTextInObj.richText = false;
        }

        protected void FindTextColorInObj(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            TextMeshProUGUI[] l_TextCol = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            foreach (var l_Current in l_TextCol)
            {
                if (l_Current.name == p_Args[1])
                {
                    l_Current.color = Utils.ParseColor(p_Args[2]);
                    break;
                }
            }
        }

        protected void FindSpriteInObj(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_SpriteParent = FindGm(p_Args[1]);
            l_SpriteParent.GetComponentInChildren<Image>().SetImage(p_Args[2]);
        }

        protected void FindSprite(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            var l_Sprites = Resources.FindObjectsOfTypeAll<Image>();
            foreach (var l_Current in l_Sprites)
            {
                if (l_Current.name == p_Args[1])
                    l_Current.SetImage("https://" + p_Args[2]);
            }
        }

        protected void Delete(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_HGm = GameObject.Find(p_Handler.TransformPercentToSpaces(p_Args[1]));
            foreach (var l_Current in l_HGm.GetComponentsInChildren<Renderer>())
            {
                l_Current.enabled = false;
            }
        }

        protected void Show(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_SGm = FindGm(p_Handler.TransformPercentToSpaces(p_Args[1]));
            foreach (var l_Current in l_SGm.GetComponentsInChildren<Renderer>())
            {
                l_Current.enabled = true;
            }
        }

        protected void TransformDesactive(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            foreach (Transform l_Current in FindGm(p_Args[1]).transform)
            {
                l_Current.gameObject.SetActive(false);
            }
        }

        protected void TransformActive(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            foreach (Transform l_Current in FindGm(p_Args[1]).transform)
            {
                l_Current.gameObject.SetActive(true);
            }
        }

        protected void Position(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_PGm = FindGm(p_Args[1]);
            l_PGm.Move(Utils.Parse(p_Args[2]), Utils.ParseFloat(p_Query));
        }

        protected void Rotation(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_PGm = FindGm(p_Args[1]);
            l_PGm.Turn(Utils.Parse(p_Args[2]), Utils.ParseFloat(p_Query));
        }

        protected void Scale(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_PGm = FindGm(p_Args[1]);
            l_PGm.Scale(Utils.Parse(p_Args[2]), Utils.ParseFloat(p_Query));
        }

        protected void LoadInGame(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_ToLoad = FindGm(p_Args[1]);
            GameObject.DontDestroyOnLoad(l_ToLoad);
        }

        protected void UnloadInGame(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_ToUnload = FindGm(p_Args[1]);
            SceneManager.MoveGameObjectToScene(l_ToUnload, SceneManager.GetActiveScene());
        }

        protected void BobbySteal(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GameObject l_GmToSteal = FindGm(p_Args[1]);
            Bobby.m_Instance.IntelligentSteal(l_GmToSteal, Utils.ParseFloat(p_Query));
        }

        protected void BobbyRelease(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Bobby.m_Instance.ReleaseAll();
        }

        protected void BobbyMove(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Bobby.m_Instance.IntelligentMove(Utils.Parse(p_Args[1]), Utils.ParseFloat(p_Query));
        }

        protected void BobbyRot(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Bobby.m_Instance.Turn(Utils.Parse(p_Args[1]), Utils.ParseFloat(p_Query));
        }

        protected void BobbyRonde(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Bobby.m_Instance.m_EnableRandomMoves = true;
            Bobby.m_Instance.Ronde();
        }

        protected void BobbyReset(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Bobby.m_Instance.ReleaseAll();
            Bobby.m_Instance.StopCurrentAnims();
            Bobby.m_Instance.m_EnableRandomMoves = false;
        }

        protected void BobbyColor(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Bobby.m_Instance.SetColor(Utils.ParseColor(p_Args[1]));
        }

        protected void CreateBobby(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            if (Bobby.m_Instance != null) return;
            new GameObject("Bobby").AddComponent<Bobby>();
        }

        protected void KillBobby(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            if (Bobby.m_Instance == null) return;
            Bobby.m_Instance.StopAllCoroutines();
            Bobby.m_Instance.StopCurrentAnims();
            Bobby.m_Instance.ReleaseAll();
            GameObject.DestroyImmediate(GameObject.Find("Bobby"));
        }

        protected void SongTime(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            GrabObjects();
            float l_NewTime = float.Parse(p_Args[1]);
            NoteCutSoundEffectManagerFix.m_Disabled = false;
            GameSongControllerObj.SeekTo(l_NewTime);
            Utils.DissolveAllObjectsRaw(0f);
            CallbacksController.SetField("_prevSongTime", l_NewTime);
            StartCoroutineWithDelay(Utils.DisableNotCutSoundFix(), 1);
        }

        protected void Play(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            string l_Id = p_Handler.TransformPercentToSpaces(p_Args[1]);
            float l_Time = float.Parse(p_Args[3]);

            Utils.PlaySongFromId(l_Id, p_Args[2], l_Time,
            Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData,
            (p_SceneTransitionSetup, p_LevelCompletionsReults, p_DifficultyBeatmap) => { });
        }

        protected void E(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            new GameObject("E").AddComponent<EStarter>().StartCo(p_Args[1], p_Args[2]);
        }

        protected void Reload(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault().RestartGame();
        }

        protected void Offset(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("UWU");
            IJumpOffsetYProvider l_JumpOffsetYProvider = ObjectsSpawnMovementData.GetField<IJumpOffsetYProvider, BeatmapObjectSpawnMovementData>("_jumpOffsetYProvider");
            BeatmapObjectSpawnControllerHelpers.GetNoteJumpValues(
                new PlayerSpecificSettings(false, 0, false, 0, true, false, true, true, false, 1,
                NoteJumpDurationTypeSettings.Static, Utils.ParseFloat(p_Args[2]), 0, true, true,
                EnvironmentEffectsFilterPreset.AllEffects, EnvironmentEffectsFilterPreset.NoEffects), 0, out var l_NoteJumpvalueType, out var l_NoteJumpValue);
            ObjectsSpawnMovementData.Init(4, Utils.ParseFloat(p_Args[1]), Logic.LevelData.Data.difficultyBeatmap.level.beatsPerMinute, l_NoteJumpvalueType, l_NoteJumpValue
                , l_JumpOffsetYProvider, Vector3.right, Vector3.forward);
        }

        protected async void Configs(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            IEnumerable<string> l_Configs = Directory.EnumerateFiles("UserData/EasyOffset/Presets");
            foreach(var l_Config in l_Configs)
            {
                SheepControl.m_ServerManager.SendMessage(l_Config, "[NOTICE]");
                await Task.Delay(100);
            }
        }

        protected void ReadUserDataFile(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            using (FileStream l_Stream = new FileStream($"UserData/{p_Args[1]}", FileMode.Open))
            {
                string l_FileContent = string.Empty;
                byte[] l_BytesContent = new byte[l_Stream.Length];
                l_Stream.Read(l_BytesContent, 0, l_BytesContent.Length);
                l_FileContent = System.Text.Encoding.UTF8.GetString(l_BytesContent, 0, l_BytesContent.Length);
                SheepControl.m_ServerManager.SendMessage(l_FileContent, "[INFO]");
            }
        }

        protected void Obamium(CommandHandler p_Handler, string[] p_Args, string p_Query)
        {
            if (GameObject.Find("Obamium") != null) return;
            GameObject l_Obamium = ObamiumLoader.LoadObamium();
            l_Obamium.name = "Obamium";
        }
    }

}
