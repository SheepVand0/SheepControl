﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using IPA.Utilities;
using BeatSaberPlus.SDK.Game;
using SheepControl.Installers;
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


        public static bool IsCommandEnabled = true;
        #endregion
        public void SetProperty(object p_Target, string p_Name, object p_Value)
        {
            PropertyInfo l_Property = p_Target.GetType().GetProperty(p_Name);
            if (l_Property == null) { Plugin.Log.Error("Property not found"); return; }

            try { p_Value = Transform_ToSpaces((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = bool.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = int.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = float.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = Utils.Parse((string)p_Value); } catch (Exception l_E) { }
            try { p_Value = Utils.RotParse((string)p_Value); } catch (Exception l_E) { }

            l_Property.SetValue(p_Target, p_Value);
        }

        public string RemoveSpaces(string p_Target)
        {
            return p_Target.Split(' ')[0];
        }

        public string Transform_ToSpaces(string p_Target)
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
            return p_Target.Replace("set", string.Empty);
        }

        public void HandleCommand(string p_Command)
        {
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

            try
            {
                if (l_Prefix == "$sudo" || l_Prefix == "!sudo")
                {
                    l_Prefix = l_Splited[1];
                    l_IndexToAdd += 1;
                }

                if (RemoveSpaces(l_Splited[1]) == "heck")
                {
                    l_IndexToAdd += 1;
                    l_Prefix = l_Splited[2];
                }

                var l_Query = l_Prefix.Split('?');
                string l_CommandQuery = string.Empty;
                l_Prefix = l_Query[0];
                if (l_Query.Length > 1)
                    l_CommandQuery = l_Query[1];

                if (SConfig.Instance.IsBannedQuery(l_CommandQuery)) return;

                if (SConfig.Instance.IsBannedCommand(l_Prefix)) return;

                switch (RemoveSet(l_Prefix.ToLower()))
                {
                    //Hardcoded things
                    case "rank":
                        SetProperty(m_RankRef, "text", l_Splited[l_IndexToAdd+1]);
                        break;
                    case "combovalue":
                        SetProperty(m_ComboValueRef, "text", l_Splited[l_IndexToAdd + 1]);
                        break;
                    case "combotext":
                        SetProperty(m_ComboTextRef, "text", l_Splited[l_IndexToAdd + 1]);
                        break;
                    case "score":
                        SetProperty(m_ScoreRef, "text", l_Splited[l_IndexToAdd + 1]);
                        break;
                    case "fakemiss":
                        NoteController l_Controller = Resources.FindObjectsOfTypeAll<NoteController>().First();
                        Vector3 vector = l_Controller.noteTransform.position;
                        Quaternion worldRotation = l_Controller.worldRotation;
                        vector = l_Controller.inverseWorldRotation * vector;
                        vector.z = 2;
                        vector = worldRotation * vector;
                        FlyingSpriteSpawner l_MissFlying = Resources.FindObjectsOfTypeAll<FlyingSpriteSpawner>()[0];
                        l_MissFlying.SpawnFlyingSprite(vector, l_Controller.worldRotation, l_Controller.inverseWorldRotation);
                        break;
                    case "pause":
                        Resources.FindObjectsOfTypeAll<SongController>()[0].PauseSong();
                        break;
                    case "resume":
                        Resources.FindObjectsOfTypeAll<SongController>()[0].ResumeSong();
                        break;
                    //Colors
                    case "lightscolor":
                        LightsColor = Utils.ParseColor(l_Splited[l_IndexToAdd + 1]);
                        break;
                    case "color":
                        SetProperty(this, l_Splited[1 + l_IndexToAdd], Utils.ParseColor(l_Splited[l_IndexToAdd + 2]));
                        break;
                    //Rings
                    case "moverings":
                        TrackLaneRing[] l_Rings = Resources.FindObjectsOfTypeAll<TrackLaneRing>();
                        for (int l_i = 0; l_i < l_Rings.Length; l_i++)
                        {
                            l_Rings[l_i].SetPosition(float.Parse(l_Splited[l_IndexToAdd + 1])*l_i, float.Parse(l_Splited[l_IndexToAdd + 2]));
                        }
                        break;
                    case "rotaterings":
                        TrackLaneRing[] l_RRings = Resources.FindObjectsOfTypeAll<TrackLaneRing>();
                        foreach (var l_Current in l_RRings)
                        {
                            l_Current.SetDestRotation(float.Parse(l_Splited[l_IndexToAdd + 1]), float.Parse(l_Splited[l_IndexToAdd + 2]));
                        }
                        break;
                    //Non hardcoded
                    case "property":
                        SetProperty(this, l_Splited[l_IndexToAdd + 1], l_Splited[l_IndexToAdd + 2]);
                        break;
                    case "enable":
                        SetProperty(this, l_Splited[l_IndexToAdd + 1], true);
                        break;
                    case "disable":
                        SetProperty(this, l_Splited[l_IndexToAdd + 1], false);
                        break;
                    case "find":
                        GameObject l_Gm = FindGm(RemoveSpaces(l_Splited[l_IndexToAdd + 1]));
                        SetProperty(l_Gm, RemoveSpaces(l_Splited[l_IndexToAdd + 2]), l_Splited[l_IndexToAdd + 3]);
                        break;
                    //text
                    case "findtext":
                        TextMeshProUGUI[] l_Text = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
                        foreach (var l_Current in l_Text)
                        {
                            if (l_Current.name == l_Splited[l_IndexToAdd + 1])
                            {
                                if (l_CommandQuery == "rich")
                                    l_Current.richText = true;
                                l_Current.text = Transform_ToSpaces(l_Splited[l_IndexToAdd + 2]);
                                if (l_CommandQuery == "rich")
                                    l_Current.richText = false;
                                break;

                            }
                        }
                        break;
                    case "findtextinobj":
                        GameObject l_Obj = FindGm(l_Splited[l_IndexToAdd + 1]);
                        TextMeshProUGUI l_FoundTextInObj = l_Obj.GetComponentInChildren<TextMeshProUGUI>();
                        if (l_CommandQuery == "rich")
                            l_FoundTextInObj.richText = true;
                        l_FoundTextInObj.text = Transform_ToSpaces(l_Splited[l_IndexToAdd + 2]);
                        if (l_CommandQuery == "rich")
                            l_FoundTextInObj.richText = false;
                        break;
                    case "findspriteinobj":
                        GameObject l_SpriteParent = FindGm(l_Splited[l_IndexToAdd + 1]);
                        l_SpriteParent.GetComponentInChildren<Image>().SetImage(l_Splited[l_IndexToAdd + 2]);
                        break;
                    case "findtextcolor":
                        TextMeshProUGUI[] l_TextCol = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
                        foreach (var l_Current in l_TextCol)
                        {
                            if (l_Current.name == l_Splited[l_IndexToAdd + 1])
                            {
                                l_Current.color = Utils.ParseColor(l_Splited[l_IndexToAdd + 2]);
                                break;
                            }
                        }
                        break;
                    case "findcolorinobj":
                        GameObject l_ObjCol = FindGm(l_Splited[l_IndexToAdd + 1]);
                        l_ObjCol.GetComponentInChildren<TextMeshProUGUI>().color = Utils.ParseColor(l_Splited[l_IndexToAdd + 2]);
                        break;
                    //sprite
                    case "findsprite":
                        var l_Sprites = Resources.FindObjectsOfTypeAll<Image>();
                        foreach (var l_Current in l_Sprites)
                        {
                            if (l_Current.name == l_Splited[l_IndexToAdd + 1])
                                l_Current.SetImage("https://" + l_Splited[l_IndexToAdd + 2]);
                        }
                        break;
                    //GameObjects manipulation
                    case "loadbobbyingame":
                        GameObject l_BobbyToLoad = GameObject.Find("Bobby");
                        GameObject.DontDestroyOnLoad(l_BobbyToLoad);
                        break;
                    case "loadscene":
                        UnityEngine.SceneManagement.SceneManager.LoadScene(l_Splited[l_IndexToAdd + 1], UnityEngine.SceneManagement.LoadSceneMode.Additive);
                        break;
                    case "unloadscene":
                        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(l_Splited[l_IndexToAdd + 1], UnityEngine.SceneManagement.UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                        GameScenesManager l_SceneManage = Resources.FindObjectsOfTypeAll<GameScenesManager>().First();
                        foreach (var l_Current in l_SceneManage.GetCurrentlyLoadedSceneNames())
                            Plugin.Log.Info(l_Current);
                        break;
                    case "delete":
                        GameObject l_HGm = GameObject.Find(Transform_ToSpaces(l_Splited[l_IndexToAdd + 1]));
                        foreach (var l_Current in l_HGm.GetComponentsInChildren<Renderer>())
                        {
                            l_Current.enabled = false;
                        }
                        break;
                    case "deletegm":
                        GameObject l_gGm = FindGm(Transform_ToSpaces(l_Splited[l_IndexToAdd + 1]));
                        /*m_InactivesGms.Add(l_gGm.transform.parent.gameObject);
                        l_gGm.gameObject.SetActive(false);*/

                        l_gGm.gameObject.SetActive(false);
                        break;
                    case "tdelete":
                        foreach (Transform l_Current in FindGm(l_Splited[l_IndexToAdd + 1]).transform)
                        {
                            l_Current.gameObject.SetActive(false);
                        }
                        break;
                    case "tactive":
                        foreach (Transform l_Current in FindGm(l_Splited[l_IndexToAdd + 1]).transform)
                        {
                            l_Current.gameObject.SetActive(true);
                        }
                        break;
                    case "showgm":
                        FindGm(Transform_ToSpaces(l_Splited[1 + l_IndexToAdd])).gameObject.SetActive(true);
                        break;
                    case "show":
                        GameObject l_SGm = FindGm(Transform_ToSpaces(l_Splited[l_IndexToAdd + 1]));
                        foreach (var l_Current in l_SGm.GetComponentsInChildren<Renderer>())
                        {
                            l_Current.enabled = true;
                        }
                        break;
                    case "position":
                        GameObject l_PGm = FindGm(l_Splited[l_IndexToAdd + 1]);
                        /*Vector3Animation l_Anim = new GameObject("Anim").AddComponent<Vector3Animation>();
                        l_Anim.Init(l_PGm, l_PGm.transform.localPosition, Utils.Parse(l_Splited[l_IndexToAdd + 2]), l_Duration);
                        l_Anim.Play();*/
                        Plugin.Log.Info(l_CommandQuery);
                        l_PGm.Move(Utils.Parse(l_Splited[l_IndexToAdd + 2]), Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "scale":
                        GameObject l_ScGm = FindGm(l_Splited[l_IndexToAdd + 1]);
                        l_ScGm.Scale(Utils.Parse(l_Splited[l_IndexToAdd + 2]), Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "rotation":
                        GameObject l_RcGm = FindGm(l_Splited[l_IndexToAdd + 1]);
                        l_RcGm.Turn(Utils.Parse(l_Splited[l_IndexToAdd + 2]), Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "loadingame":
                        GameObject l_ToLoad = FindGm(l_Splited[l_IndexToAdd + 1]);
                        GameObject.DontDestroyOnLoad(l_ToLoad);
                        break;
                    case "unloadingame":
                        GameObject l_ToUnload = FindGm(l_Splited[l_IndexToAdd + 1]);
                        SceneManager.MoveGameObjectToScene(l_ToUnload, SceneManager.GetActiveScene());
                        break;
                    //Obamium
                    case "spawnobamium":
                        GameObject l_Obamium = GameObject.Instantiate(Bundle.ObamiumLoader.LoadObamium());
                        l_Obamium.transform.localPosition = Utils.Parse(l_Splited[l_IndexToAdd + 1]);
                        l_Obamium.name = $"Obamium";
                        break;
                    //bobby
                    case "bobbysteal":
                        GameObject l_GmToSteal = FindGm(l_Splited[l_IndexToAdd + 1]);
                        GameObject l_Bobby = GameObject.Find("Bobby");
                        Bobby.m_Instance.IntelligentSteal(l_GmToSteal, Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "bobbyrelease":
                        Bobby.m_Instance.ReleaseAll();
                        break;
                    case "bobbycolor":
                        Bobby.m_Instance.SetColor(Utils.ParseColor(l_Splited[l_IndexToAdd + 1]));
                        break;
                    case "bobbymove":
                        GameObject l_BobbyMove = GameObject.Find("Bobby");
                        Vector3 l_Position = Utils.Parse(l_Splited[l_IndexToAdd + 1]);
                        Bobby.m_Instance.IntelligentMove(l_Position, Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "bobbyrot":
                        GameObject l_BobbyRot = GameObject.Find("Bobby");
                        Quaternion l_Rotation = Utils.RotParse(l_Splited[l_IndexToAdd + 1]);
                        Bobby.m_Instance.Turn(l_Rotation.eulerAngles, Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "bobbyronde":
                        Bobby.m_Instance.StartCoroutine(Bobby.m_Instance.Ronde());
                        break;
                    case "bobbyreset":
                        /*HandleCommand("!sudo killbobby");
                        HandleCommand("!sudo createbobby");*/
                        Bobby.m_Instance.ReleaseAll();
                        Bobby.m_Instance.StopCurrentAnims();
                        Bobby.m_Instance.m_EnableRandomMoves = false;
                        break;
                    case "createbobby":
                        if (Bobby.m_Instance != null) return;
                        new GameObject("Bobby").AddComponent<Bobby>();
                        break;
                    case "killbobby":
                        if (Bobby.m_Instance == null) return;
                        Bobby.m_Instance.StopAllCoroutines();
                        Bobby.m_Instance.StopCurrentAnims();
                        Bobby.m_Instance.ReleaseAll();
                        GameObject.DestroyImmediate(GameObject.Find("Bobby"));
                        break;
                    case "enablebobbymvmt":
                        Bobby.m_Instance.m_EnableRandomMoves = bool.Parse(l_Splited[l_IndexToAdd + 1]);
                        break;
                    //Naluluna Cat
                    case "catposition":
                        GameObject l_Cat = FindGm(NalulunaCatGameObject);
                        l_Cat.Move(Utils.Parse(l_Splited[l_IndexToAdd + 1]), Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "catrotation":
                        GameObject l_RCat = FindGm(NalulunaCatGameObject);
                        l_RCat.Move(Utils.Parse(l_Splited[l_IndexToAdd + 1]), Utils.ParseFloat(l_CommandQuery));
                        break;
                    case "catscale":
                        GameObject l_SCat = FindGm(NalulunaCatGameObject);
                        l_SCat.Move(Utils.Parse(l_Splited[l_IndexToAdd + 1]), Utils.ParseFloat(l_CommandQuery));
                        break;
                    //Song manipulation
                    case "songtime":
                        GameSongController l_AudioManager = Resources.FindObjectsOfTypeAll<GameSongController>()[0];
                        l_AudioManager.SeekTo(float.Parse(l_Splited[l_IndexToAdd + 1]));
                        break;
                    case "songspeed":
                        ObjectsGrabber.m_SongSpeedData.SetField("speedMul", float.Parse(l_Splited[l_IndexToAdd + 1]));
                        break;
                    case "play":
                        string l_Id = Transform_ToSpaces(l_Splited[l_IndexToAdd + 1]);
                        float l_Time = Utils.ParseFloat(l_Splited[l_IndexToAdd + 3]);

                        Utils.PlaySongFromId(l_Id, l_Splited[l_IndexToAdd + 2], l_Time,
                        Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData,
                        (p_SceneTransitionSetup, p_LevelCompletionsReults, p_DifficultyBeatmap) => {});
                        break;
                    case "notescolor":
                        NotesColor = Utils.ParseColor(l_Splited[l_IndexToAdd + 1]);
                        break;
                    case "e":
                        new GameObject("E").AddComponent<EStarter>().StartCo(l_Splited[l_IndexToAdd + 1], l_Splited[l_IndexToAdd + 2]);
                        break;
                    case "tryrestoree":
                        foreach (var l_Current in Resources.FindObjectsOfTypeAll<GameObject>())
                        {
                            l_Current.transform.localPosition = new Vector3(0, 0, 0);
                            l_Current.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        }
                        break;
                    case "reload":
                        Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault().RestartGame();
                        break;
                    default:
                        Plugin.Log.Error("Not command valid");
                        return;
                }
            }
            catch (Exception l_E)
            {
                Plugin.Log.Error($"Error on command : {l_E}");
            }
        }

        internal class EStarter : MonoBehaviour
        {
            public void StartCo (string p_PosValue, string p_RotValue)
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

                float l_Pos = float.Parse(p_PosValue)/100;

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
            Levels.LoadSong(p_Id, (p_Beatmap) => {
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

                switch (Logic.ActiveScene) {
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
                        try
                        {
                            NoteCutSoundEffectManagerFix.m_Disabled = false;

                            DissolveAllObjectsRaw(0f);

                            BeatmapObjectSpawnController l_BeatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();

                            BeatmapCallbacksController l_CallbacksController = l_BeatmapObjectSpawnController.GetField<BeatmapCallbacksController, BeatmapObjectSpawnController>("_beatmapCallbacksController");

                            GameSongController l_GameSongController = Resources.FindObjectsOfTypeAll<GameSongController>().FirstOrDefault();

                            BeatmapObjectSpawnMovementData l_BeatmapObjectSpawnMovementData = l_BeatmapObjectSpawnController.GetField<BeatmapObjectSpawnMovementData, BeatmapObjectSpawnController>("_beatmapObjectSpawnMovementData");

                            IReadonlyBeatmapData l_ReadonlyBeatmapData = l_CallbacksController.GetField<IReadonlyBeatmapData, BeatmapCallbacksController>("_beatmapData");

                            Task<IReadonlyBeatmapData> l_TaskNewReadonlyBeatmapData = Logic.LevelData.Data.GetTransformedBeatmapDataAsync();
                            l_TaskNewReadonlyBeatmapData.Wait();

                            BeatmapData l_NewReadonlyBeatmapData = (BeatmapData)l_TaskNewReadonlyBeatmapData.Result;

                            Plugin.Log.Info($"{l_ReadonlyBeatmapData.allBeatmapDataItems.Count}");

                            //l_CallbacksController.sendCallbacksOnBeatmapDataChange = false;

                            /*Dictionary<float, CallbacksInTime> l_Callbacks = l_CallbacksController.GetField<Dictionary<float, CallbacksInTime>, BeatmapCallbacksController>("_callbacksInTimes");
                            foreach (var l_Current in l_Callbacks.Values)
                            {
                                l_Current.lastProcessedNode = null;
                            }*/

                            l_CallbacksController.ReplaceBeatmapData(l_NewReadonlyBeatmapData, p_Time);

                            AudioTimeSyncController l_TimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().First();
                            AudioSource l_AudioSource =
                            l_TimeSyncController.GetField<AudioSource, AudioTimeSyncController>("_audioSource");
                            ResetTimeSync(l_AudioSource, p_Beatmap.beatmapLevelData.audioClip, p_Time, 0, 1);

                            IJumpOffsetYProvider l_JumpOffsetYProvider = l_BeatmapObjectSpawnMovementData.GetField<IJumpOffsetYProvider, BeatmapObjectSpawnMovementData>("_jumpOffsetYProvider");
                            BeatmapObjectSpawnControllerHelpers.GetNoteJumpValues(p_PlayerData.playerSpecificSettings, l_DiffBeatmap.noteJumpStartBeatOffset, out var l_NoteJumpvalueType, out var l_NoteJumpValue);
                            l_BeatmapObjectSpawnMovementData.Init(l_NewReadonlyBeatmapData.numberOfLines, l_DiffBeatmap.noteJumpMovementSpeed, p_Beatmap.beatsPerMinute, l_NoteJumpvalueType, l_NoteJumpValue
                                , l_JumpOffsetYProvider, Vector3.right, Vector3.forward);
                            l_CallbacksController.TriggerBeatmapEvent(new BPMChangeBeatmapEventData(p_Time, p_Beatmap.beatsPerMinute));

                            Plugin.Log.Info($"{l_CallbacksController.GetField<IReadonlyBeatmapData, BeatmapCallbacksController>("_beatmapData").allBeatmapDataItems.Count}");

                            StartCoroutineWithDelay(DisableNotCutSoundFix(), 1);

                        }
                        catch (Exception l_E)
                        {
                            Plugin.Log.Error(l_E);
                        }
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
            timeSync.StartSong(newData.songTimeOffset);
        }

        public static void ReplaceBeatmapData(this BeatmapCallbacksController p_CallbacksController, BeatmapData p_BeatmapData, float p_Time)
        {
            p_CallbacksController.SetField("_beatmapData", p_BeatmapData as IReadonlyBeatmapData);
            /*_beatmapDataAccessor(ref p_CallbacksController).allBeatmapDataItems.Clear();
            foreach (var l_Index in p_BeatmapData.allBeatmapDataItems)
                _beatmapDataAccessor(ref p_CallbacksController).allBeatmapDataItems.AddLast(l_Index);*/
            p_CallbacksController.SetField("_startFilterTime", 0f);
            p_CallbacksController.SetField("_prevSongTime", p_Time);
            Dictionary<float, CallbacksInTime> l_Callbacks = p_CallbacksController.GetField<Dictionary<float, CallbacksInTime>, BeatmapCallbacksController>("_callbacksInTimes");
            foreach (var l_Index in l_Callbacks.Values) {
                l_Index.lastProcessedNode = null;
            }
            /*foreach (var l_Index in p_BeatmapData.allBeatmapDataItems)
            {
                CallbacksInTime l_Callback = new CallbacksInTime(l_Index.time);
                //l_Callback.lastProcessedNode.Value = l_Index;
                l_Callbacks.Add(l_Index.time, l_Callback);
            }*/
            //p_CallbacksController.SetField("_callbacksInTimes", l_Callbacks);
        }

        public static void DissolveAllObjectsRaw(float p_Duration)
        {
            foreach (var l_Current in Resources.FindObjectsOfTypeAll<GameNoteController>())
                l_Current.Dissolve(p_Duration);

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
                Color l_Ret = new Color(float.Parse(l_Splited[0])/ l_Divide, float.Parse(l_Splited[1])/ l_Divide, float.Parse(l_Splited[2]) / l_Divide);

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

                Quaternion l_Rot = Quaternion.Euler(float.Parse(l_Splited[0])/l_Divivde, float.Parse(l_Splited[1]) / l_Divivde, float.Parse(l_Splited[2]) / l_Divivde);
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
