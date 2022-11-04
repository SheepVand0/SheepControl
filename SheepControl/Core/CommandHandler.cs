using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using IPA.Utilities;
using BeatSaberPlus.SDK.Game;
using System.Collections;
using UnityEngine.Networking;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Macros;
using System.Net;
using SheepControl.AnimsUtils;
using SheepControl.TUtils;
using static SheepControl.TUtils.TUtils;
using HMUI;
using UnityEngine.UI;
using SheepControl.Trucs;
using UnityEngine.SceneManagement;
using CP_SDK.Unity.Extensions;
using BeatmapEditor3D;
using static SheepControl.Core.ObjectsGrabber;
using SheepControl.Core;

namespace SheepControl
{
    public class CommandHandler
    {
        #region References
        public static TextMeshProUGUI m_ComboTextRef;
        public static TextMeshProUGUI m_ComboValueRef;
        public static TextMeshProUGUI m_RankRef;
        public static TextMeshProUGUI m_ScoreRef;

        List<GameObject> m_InactivesGms = new List<GameObject>();

        static List<ObjectProperty> m_CustomObjects = new List<ObjectProperty>()
        {
            new ObjectProperty(),
        };

        public static string NalulunaCatGameObject = "NalulunaCat(Clone)";
        public bool AutoDissolveNotes { get; set; }
        public bool AutoResolveNotes { get; set; }
        public bool RandomColorWalls { get; set; }
        public bool BigNotes { get; set; }
        public bool SmallNotes { get; set; }
        public bool UwUMode { get; set; }
        public bool RandomNotesColors { get; set; }
        public bool RandomLights { get; set; }
        public bool SpecifiedLights { get; set; }
        public int LightId { get; set; }
        public bool SpecifiedLightId { get; set; }
        public Color LightsColor { get; set; }
        public bool MakeRandomNoteRotation { get; set; }
        public bool PauseRandomNote { get; set; }
        public bool MappingExtension { get; set; }
        public bool SpecifiedNotesColor { get; set; }
        public bool SpecifiedHandNotesColor { get; set; }
        public Color NotesColor { get; set; }
        public Color RightNotesColor { get; set; }
        public Color LeftNotesColor { get; set; }

        public bool SpecifiedLightsSide { get; set; }
        public Color RightLightsColor { get; set; }
        public Color LeftLightsColor { get; set; }

        Commands m_Commands;

        public static bool IsCommandEnabled = true;

        public CommandHandler()
        {
            m_Commands = new Commands();
            m_Commands.BindCommands();
        }

        public void HandleCommand(string p_Command)
        {
            p_Command += " ";

            switch (BeatSaberPlus.SDK.Game.Logic.ActiveScene)
            {
                case Logic.SceneType.Menu:
                    if (!SConfig.GetStaticModSettings().IsCommandsEnabledInMenu) return;
                    break;
                case Logic.SceneType.Playing:
                    if (!SConfig.GetStaticModSettings().AskForCommands)
                    {
                        if (!SConfig.GetStaticModSettings().IsCommandsEnabledInGame) return;
                    }
                    else
                    {
                        if (!IsCommandEnabled) return;
                    }
                    break;
                default: return;
            }

            var l_Splited = p_Command.Split(' ');
            string l_Prefix = l_Splited[0];

            int l_IndexToAdd = 0;

            List<string> l_SplitedList = new List<string>();
            l_SplitedList.AddRange(l_Splited);

            try
            {
                if (l_Prefix == "$sudo" || l_Prefix == "!sudo")
                {
                    l_Prefix = l_Splited[1];
                    l_IndexToAdd += 1;
                    l_SplitedList.RemoveAt(0);
                }

                if (RemoveSpaces(l_Splited[1]) == "heck")
                {
                    l_IndexToAdd += 1;
                    l_Prefix = l_Splited[2];
                    l_SplitedList.RemoveAt(0);
                }

                string l_CommandQuery = string.Empty;
                if (l_Prefix.Contains("?"))
                {
                    var l_Query = l_Prefix.Split('?');
                    l_Prefix = l_Query[0];
                    if (l_Query.Length > 1)
                        l_CommandQuery = l_Query[1];
                }

                if (SConfig.Instance.IsBannedQuery(l_CommandQuery)) return;

                if (SConfig.Instance.IsBannedCommand(l_Prefix)) return;

                foreach (var l_Command in m_Commands.m_Commands)
                {
                    if (l_Command.Name == RemoveSet(l_Prefix.ToLower())) {
                        l_Command.Execute(this, l_SplitedList.ToArray(), l_CommandQuery);
                    }
                }
            }
            catch (Exception l_E)
            {
                Plugin.Log.Error($"[SHEEP_COMMAND_ERROR] : {l_E}");
            }
        }


        #endregion
        public void SetProperty(object p_Target, string p_Name, object p_Value)
        {
            PropertyInfo l_Property = p_Target.GetType().GetProperty(p_Name);
            if (l_Property == null) { Plugin.Log.Error("Property not found"); return; }

            try { p_Value = TransformPercentToSpaces((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = bool.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = int.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = float.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = Utils.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = Utils.RotParse((string)p_Value); } catch (Exception l_E) { }

            l_Property.SetValue(p_Target, p_Value);
        }

        public string RemoveSpaces(string p_Target)
        {
            if (p_Target.Contains(' '))
                return p_Target.Split(' ')[0];
            return p_Target;
        }

        public string TransformPercentToSpaces(string p_Target)
        {
            p_Target = p_Target.Replace('%', ' ');
            SConfig l_Settings = SConfig.GetStaticModSettings();
            foreach (var l_Current in l_Settings.BannedWords)
                if (p_Target.ToLower().Contains(l_Current))
                    p_Target = "BadWord";
            return p_Target;
        }

        public string RemoveSet(string p_Target)
        {
            if (p_Target.ToLower().Contains("set"))
                return p_Target.Replace("set", string.Empty);
            return p_Target;
        }

    }

    internal class EStarter : MonoBehaviour
    {
        public void StartCo(string p_PosValue, string p_RotValue)
        {
            StartCoroutine(RandomPosOfEverything(p_PosValue, p_RotValue));
        }

        IEnumerator RandomPosOfEverything(string p_PosValue, string p_Value)
        {
            AudioClip l_Clip = null;
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://github.com/SheepVand0/MySimplesCodes-NoUE/raw/main/E.mp3", AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (!www.isNetworkError)
                    l_Clip = DownloadHandlerAudioClip.GetContent(www);
            }

            AudioSource l_Source = new GameObject("AudioE").AddComponent<AudioSource>();
            l_Source.clip = l_Clip;
            l_Source.volume = 0.5f;
            l_Source.Play();

            float l_Pos = float.Parse(p_PosValue) / 100;

            foreach (var l_Current in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (l_Current.GetType() != typeof(Camera))
                {
                    if (l_Current.transform.parent != null)
                    {
                        if (l_Current.transform.parent.name != "RightSaber" || l_Current.transform.parent.name != "LeftSaber")
                            l_Current.transform.localPosition = l_Current.transform.localPosition + RandomUtils.RandomVector3(l_Pos * -1f, l_Pos);
                        l_Current.transform.localRotation = Quaternion.Euler(l_Current.transform.localRotation.eulerAngles + RandomUtils.RandomQuat(float.Parse(p_Value) / 2 * -1, float.Parse(p_Value) / 2).eulerAngles);
                    }
                    else
                    {
                        l_Current.transform.localPosition = l_Current.transform.localPosition + RandomUtils.RandomVector3(l_Pos * -1f, l_Pos);
                        l_Current.transform.localRotation = RandomUtils.RandomQuat(float.Parse(p_Value) / 2 * -1, float.Parse(p_Value) / 2);
                    }
                }
            }
        }
    }

    public static class Utils
    {
        public static readonly FieldAccessor<BeatmapDataItem, float>.Accessor _BeatMapDataItemTimeAccessor = FieldAccessor<BeatmapDataItem, float>.GetAccessor($"<{nameof(BeatmapDataItem.time)}>k__BackingField");

        internal static readonly FieldAccessor<BeatmapCallbacksController, IReadonlyBeatmapData>.Accessor _beatmapDataAccessor = FieldAccessor<BeatmapCallbacksController, IReadonlyBeatmapData>.GetAccessor("_beatmapData");

        public static readonly FieldAccessor<CallbacksInTime, Dictionary<Type, List<BeatmapDataCallbackWrapper>>>.Accessor _CallbacksAccessor = FieldAccessor<CallbacksInTime, Dictionary<Type, List<BeatmapDataCallbackWrapper>>>.GetAccessor("_callbacks");
        public static readonly FieldAccessor<BeatmapDataCallbackWrapper, float>.Accessor _CallbackWrapperAheadTimeAccessor = FieldAccessor<BeatmapDataCallbackWrapper, float>.GetAccessor("aheadTime");
        public static void PlaySongFromId(string p_Id,
            string p_SerializedDifficulty, float p_Time = 0, PlayerData p_PlayerData = null,
            Action<StandardLevelScenesTransitionSetupDataSO,
                LevelCompletionResults, IDifficultyBeatmap>
            p_PlayCallback = null)
        {
            NoteCutSoundEffectManagerFix.m_Disabled = true;
            Levels.LoadSong(p_Id, (p_Beatmap) =>
            {
                BeatmapDifficulty l_Difficulty = Levels.SerializedToDifficulty(p_SerializedDifficulty);
                BeatmapCharacteristicSO l_BeatmapCharacteristics = null;
                IDifficultyBeatmap l_DiffBeatmap = null;
                foreach (var l_Current in p_Beatmap.beatmapLevelData.difficultyBeatmapSets)
                {
                    foreach (var l_DiffSet in l_Current.difficultyBeatmaps)
                        if (l_DiffSet.difficulty == l_Difficulty)
                        {
                            l_BeatmapCharacteristics = l_Current.beatmapCharacteristic;
                            l_DiffBeatmap = l_DiffSet;
                        }
                }

                Plugin.Log.Info("Map loaded");

                switch (Logic.ActiveScene)
                {
                    case Logic.SceneType.Menu:
                        Levels.PlaySong(p_Beatmap,
                        l_BeatmapCharacteristics,
                        l_Difficulty, p_PlayerData.overrideEnvironmentSettings, p_PlayerData.colorSchemesSettings.GetSelectedColorScheme(),
                        p_PlayerData.gameplayModifiers, p_PlayerData.playerSpecificSettings, (p_StandardLevelScenesTransition, p_LevelCompletionResults, p_DifficultyBeatmap) =>
                        {
                            p_PlayCallback.Invoke(p_StandardLevelScenesTransition, p_LevelCompletionResults, p_DifficultyBeatmap);
                        });
                        break;
                    case Logic.SceneType.Playing:
                        GrabObjects();
                        NoteCutSoundEffectManagerFix.m_Disabled = false;
                        DissolveAllObjectsRaw(0f);

                        IReadonlyBeatmapData l_ReadonlyBeatmapData = CallbacksController.GetField<IReadonlyBeatmapData, BeatmapCallbacksController>("_beatmapData");

                        PlayerSpecificSettings l_Specific = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData.playerSpecificSettings;
                        Task<IReadonlyBeatmapData> l_TaskNewReadonlyBeatmapData = l_DiffBeatmap.GetBeatmapDataAsync(p_Beatmap.environmentInfo, l_Specific);
                        l_TaskNewReadonlyBeatmapData.Wait();
                        BeatmapData l_NewReadonlyBeatmapData = (BeatmapData)l_TaskNewReadonlyBeatmapData.Result;

                        CallbacksController.ReplaceBeatmapData(l_NewReadonlyBeatmapData, p_Time);

                        ResetTimeSync(GameAudioSource, p_Beatmap.beatmapLevelData.audioClip, p_Time, 0, 1);

                        IJumpOffsetYProvider l_JumpOffsetYProvider = ObjectsSpawnMovementData.GetField<IJumpOffsetYProvider, BeatmapObjectSpawnMovementData>("_jumpOffsetYProvider");
                        BeatmapObjectSpawnControllerHelpers.GetNoteJumpValues(p_PlayerData.playerSpecificSettings, l_DiffBeatmap.noteJumpStartBeatOffset, out var l_NoteJumpvalueType, out var l_NoteJumpValue);
                        ObjectsSpawnMovementData.Init(l_NewReadonlyBeatmapData.numberOfLines, l_DiffBeatmap.noteJumpMovementSpeed, p_Beatmap.beatsPerMinute, l_NoteJumpvalueType, l_NoteJumpValue
                            , l_JumpOffsetYProvider, Vector3.right, Vector3.forward);
                        CallbacksController.TriggerBeatmapEvent(new BPMChangeBeatmapEventData(p_Time, p_Beatmap.beatsPerMinute));

                        BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("💥⅟⅕⅗ⅣⅦⅦⅦⅥⅥ₈ⅴⅴ₂ⅫⅫ⅝⅝∂∂∋∉∋∈∌∜∖∞∢∥∡≊≓≄≁!sudo");

                        StartCoroutineWithDelay(DisableNotCutSoundFix(), 1);
                        break;
                }
            });
        }

        public static void ResetTimeSync(AudioSource p_Source, AudioClip p_Clip, float p_Time, float p_TimeOffset, float p_TimeScale)
        {
            AudioTimeSyncController l_Controller = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().First();
            AudioTimeSyncController.InitData initData =
                l_Controller.GetField<AudioTimeSyncController.InitData, AudioTimeSyncController>("_initData");
            AudioTimeSyncController.InitData newData = new AudioTimeSyncController.InitData(p_Clip,
                            p_Time, p_TimeOffset, 1f);
            var timeSync = l_Controller;
            p_Source.clip = p_Clip;
            l_Controller.SetField("_initData", newData);
            l_Controller.SetField("_timeScale", p_TimeScale);
            l_Controller.SetField("_startSongTime", p_Time);
            l_Controller.SetField("_audioStartTimeOffsetSinceStart", /*timeSync.GetProperty<float, AudioTimeSyncController>("timeSinceStart") - */(p_Time + newData.songTimeOffset));
            l_Controller.SetField("_fixingAudioSyncError", false);
            l_Controller.SetField("_playbackLoopIndex", 0);
            l_Controller.SetField("_audioStarted", false);
            p_Source.pitch = p_TimeScale;
            timeSync.StartSong(p_Time * (60 / p_Clip.length));
            timeSync.SeekTo(p_Time * (60 / p_Clip.length));
        }

        public static void ReplaceBeatmapData(this BeatmapCallbacksController p_CallbacksController, BeatmapData p_BeatmapData, float p_Time)
        {
            p_CallbacksController.SetField("_beatmapData", p_BeatmapData as IReadonlyBeatmapData);

            p_CallbacksController.SetField("_startFilterTime", 0f);
            p_CallbacksController.SetField("_prevSongTime", p_Time);

            p_CallbacksController.ResetCallbackController();
        }

        public static void ResetCallbackController(this BeatmapCallbacksController p_CallbacksController)
        {
            Dictionary<float, CallbacksInTime> l_Callbacks = p_CallbacksController.GetField<Dictionary<float, CallbacksInTime>, BeatmapCallbacksController>("_callbacksInTimes");
            foreach (var l_Index in l_Callbacks.Values)
            {
                l_Index.lastProcessedNode = null;
            }
        }

        public static void DissolveAllObjectsRaw(float p_Duration)
        {
            foreach (var l_Current in Resources.FindObjectsOfTypeAll<GameNoteController>())
            {
                //l_Current.HandleCut(l_Current.noteData.colorType == ColorType.ColorA ? LeftSaber : RightSaber, Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.zero, false);
                l_Current.Dissolve(p_Duration);
            }

            foreach (var l_Current in Resources.FindObjectsOfTypeAll<BurstSliderGameNoteController>())
                l_Current.Dissolve(p_Duration);

            foreach (var l_Current in Resources.FindObjectsOfTypeAll<BombNoteController>())
                l_Current.Dissolve(p_Duration);

            foreach (var l_Current in Resources.FindObjectsOfTypeAll<ObstacleController>())
                l_Current.Dissolve(p_Duration);
        }

        public static IEnumerator DisableNotCutSoundFix()
        {
            NoteCutSoundEffectManagerFix.m_Disabled = true;
            yield return null;
        }

        public static void ResetNoteCutSoundManager()
        {
            NoteCutSoundEffectManager l_Manager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().First();
            l_Manager.SetField("_prevNoteATime", -1f);
            l_Manager.SetField("_prevNoteBTime", -1f);
        }

        public static GameplayModifiers GetSpeedModifierFromValue(GameplayModifiers p_BaseModifiers, int p_SpeedInPercent)
        {
            GameplayModifiers.SongSpeed l_Speed = GameplayModifiers.SongSpeed.Normal;

            if (p_SpeedInPercent < 100)
                l_Speed = GameplayModifiers.SongSpeed.Slower;
            else
            if (p_SpeedInPercent >= 125 && p_SpeedInPercent < 150)
                l_Speed = GameplayModifiers.SongSpeed.Faster;
            else if (p_SpeedInPercent >= 150)
                l_Speed = GameplayModifiers.SongSpeed.SuperFast;

            return new GameplayModifiers(p_BaseModifiers.energyType, p_BaseModifiers.noFailOn0Energy, p_BaseModifiers.instaFail, p_BaseModifiers.failOnSaberClash,
                p_BaseModifiers.enabledObstacleType, p_BaseModifiers.noBombs, p_BaseModifiers.fastNotes, p_BaseModifiers.strictAngles, p_BaseModifiers.disappearingArrows,
                l_Speed, p_BaseModifiers.noArrows, p_BaseModifiers.ghostNotes, p_BaseModifiers.proMode, false, p_BaseModifiers.smallCubes);
        }

        public static float ParseFloat(string p_Value)
        {
            var l_Splited = p_Value.Split(';');

            int l_Divide = int.Parse(l_Splited[1]);

            return float.Parse(l_Splited[1]) / l_Divide;
        }

        public static Vector3 Parse(string p_Value)
        {
            try
            {
                var l_Splited = p_Value.Split(';');

                int l_Divide = int.Parse(l_Splited[3]);

                Vector3 l_Ret = new Vector3(float.Parse(l_Splited[0]) / l_Divide, float.Parse(l_Splited[1]) / l_Divide, float.Parse(l_Splited[2]) / l_Divide);

                return l_Ret;
            }
            catch (Exception l_E)
            {
                throw l_E;
            }
        }

        public static Color ParseColor(string p_Value)
        {
            try
            {
                var l_Splited = p_Value.Split(';');

                int l_Divide = int.Parse(l_Splited[3]);
                Color l_Ret = new Color(float.Parse(l_Splited[0]) / l_Divide, float.Parse(l_Splited[1]) / l_Divide, float.Parse(l_Splited[2]) / l_Divide);

                return l_Ret;
            }
            catch (Exception l_E)
            {
                throw l_E;
            }
        }

        public static Quaternion RotParse(string p_Value)
        {
            try
            {
                var l_Splited = p_Value.Split(';');

                int l_Divivde = int.Parse(l_Splited[3]);

                Vector3 l_Ret = new Vector3();

                Quaternion l_Rot = Quaternion.Euler(float.Parse(l_Splited[0]) / l_Divivde, float.Parse(l_Splited[1]) / l_Divivde, float.Parse(l_Splited[2]) / l_Divivde);
                return l_Rot;
            }
            catch (Exception l_E)
            {
                throw l_E;
            }
        }
    }

    public struct PropertyName
    {
        public string m_Name;
        public object m_Value;
    }

    public struct ObjectProperty
    {
        public ObjectProperty(string p_PName, object p_Target)
        {
            PrefixName = p_PName;
            Target = p_Target;
        }
        string PrefixName { get; set; }
        object Target { get; set; }
    }
}
