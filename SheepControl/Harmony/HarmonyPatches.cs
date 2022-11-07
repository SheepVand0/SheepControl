using HarmonyLib;
using UnityEngine;
using IPA.Utilities;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
using SiraUtil.Affinity;
using Zenject;
using static BeatmapObjectSpawnMovementData;
using BeatSaberAPI.DataTransferObjects;
using System.Linq;
using CP_SDK.Unity;
using System.Collections;
using SheepControl.TUtils;

namespace SheepControl
{

    [HarmonyPatch(typeof(CoreGameHUDController), nameof(CoreGameHUDController.Start))]
    class Rank
    {

        private static void Prefix(CoreGameHUDController __instance, ref GameObject ____immediateRankGO, ref GameObject ____relativeScoreGO)
        {
            CommandHandler.m_ScoreRef = ____relativeScoreGO.GetComponent<TextMeshProUGUI>();
            CommandHandler.m_RankRef = ____immediateRankGO.GetComponent<TextMeshProUGUI>();
        }
    }

    [HarmonyPatch(typeof(ComboUIController), nameof(ComboUIController.Start))]
    class Combo
    {

        private static void Prefix(ComboUIController __instance)
        {
            TextMeshProUGUI l_RankText = __instance.GetField<TextMeshProUGUI, ComboUIController>("_comboText");

            CommandHandler.m_ComboValueRef = l_RankText;
            CommandHandler.m_ComboTextRef = __instance.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    [HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.ShowMenu))]
    class PausePatch
    {
        private static void Postfix(ref LevelBar ____levelBar)
        {
            TextMeshProUGUI _songNameText = ____levelBar.GetField<TextMeshProUGUI, LevelBar>("_songNameText");
            TextMeshProUGUI _authorNameText = ____levelBar.GetField<TextMeshProUGUI, LevelBar>("_authorNameText");

            MTCoroutineStarter.Start(TextsCoroutine(_songNameText, _authorNameText));
        }

        static IEnumerator TextsCoroutine(TextMeshProUGUI p_SongName, TextMeshProUGUI p_Author)
        {
            yield return new WaitForSeconds(0.5f);
            p_SongName.text = "Pausing is cheating";
            p_Author.text = "Kuurama";
        }
    }

    [HarmonyPatch(typeof(ColorNoteVisuals), nameof(ColorNoteVisuals.HandleNoteControllerDidInit))]
    class NotesPatches
    {
        private static void Postfix(ColorNoteVisuals __instance, ref NoteControllerBase ____noteController, ref MaterialPropertyBlockController[] ____materialPropertyBlockControllers)
        {
            if (SheepControl.m_CommandHandler.RandomNotesColors)
            {
                foreach (MaterialPropertyBlockController l_Current in ____materialPropertyBlockControllers)
                {
                    l_Current.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), RandomUtils.RandomColor());
                    l_Current.ApplyChanges();
                }
            }

            if (SheepControl.m_CommandHandler.SpecifiedNotesColor)
            {
                foreach (MaterialPropertyBlockController l_Current in ____materialPropertyBlockControllers)
                {
                    l_Current.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), SheepControl.m_CommandHandler.NotesColor);
                    l_Current.ApplyChanges();
                }
            }

            if (SheepControl.m_CommandHandler.SpecifiedHandNotesColor)
            {
                Color l_Color = (____noteController.noteData.colorType == ColorType.ColorA) ? SheepControl.m_CommandHandler.LeftNotesColor : SheepControl.m_CommandHandler.RightNotesColor;

                foreach (MaterialPropertyBlockController l_Current in ____materialPropertyBlockControllers)
                {
                    l_Current.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), l_Color);
                    l_Current.ApplyChanges();
                }
            }
        }
    }

    [HarmonyPatch(typeof(PreviewBeatmapLevelSO), nameof(PreviewBeatmapLevelSO.InitData))]
    class Cover
    {
        private static void Postfix(PreviewBeatmapLevelSO __instance)
        {
            Plugin.Log.Info(__instance.levelID);
        }
    }

    [HarmonyPatch(typeof(GameNoteController), nameof(GameNoteController.Init))]
    class ResolveFirstPatch
    {
        private static void Postfix(GameNoteController __instance)
        {
            if (SheepControl.m_CommandHandler.AutoResolveNotes == false)
            {
                Transform l_Object = __instance.transform.GetChild(0).Find("NoteArrow");
                Transform l_Object2 = __instance.transform.GetChild(0).Find("NoteArrowGlow");
                Transform l_Object3 = __instance.transform.GetChild(0).Find("NoteCircleGlow");

                l_Object.gameObject.SetActive(__instance.noteData.cutDirection != NoteCutDirection.Any);
                l_Object2.gameObject.SetActive(__instance.noteData.cutDirection != NoteCutDirection.Any);
                l_Object3.gameObject.SetActive(__instance.noteData.cutDirection == NoteCutDirection.Any);
            }

            if (!SheepControl.m_CommandHandler.AutoResolveNotes) return;

            foreach (var l_Current in __instance.GetComponentsInChildren<Renderer>())
                l_Current.enabled = false;
        }
    }

    [HarmonyPatch(typeof(GameNoteController), "NoteDidStartJump")]
    class DisolvePatch
    {
        private static void Postfix(GameNoteController __instance, BoxCuttableBySaber[] ____bigCuttableBySaberList, BoxCuttableBySaber[] ____smallCuttableBySaberList)
        {
            foreach (var l_Render in __instance.GetComponentsInChildren<Renderer>())
            {
                l_Render.enabled = true;
            }

            Transform l_Object = __instance.transform.GetChild(0).Find("NoteArrow");
            Transform l_Object2 = __instance.transform.GetChild(0).Find("NoteArrowGlow");
            Transform l_Object3 = __instance.transform.GetChild(0).Find("NoteCircleGlow");

            l_Object.gameObject.SetActive(__instance.noteData.cutDirection != NoteCutDirection.Any);
            l_Object2.gameObject.SetActive(__instance.noteData.cutDirection != NoteCutDirection.Any);
            l_Object3.gameObject.SetActive(__instance.noteData.cutDirection == NoteCutDirection.Any);


            if (SheepControl.m_CommandHandler.BigNotes)
            {
                __instance.transform.localScale = Vector3.one * 1.5f;
                foreach (var l_Current in ____bigCuttableBySaberList)
                {
                    l_Current.transform.localScale = Vector3.one * 0.5f;
                }
                foreach (var l_Current in ____smallCuttableBySaberList)
                {
                    l_Current.transform.localScale = Vector3.one * 0.5f;
                }
            }
            else if (SheepControl.m_CommandHandler.SmallNotes)
            {
                float l_RandomNumber = UnityEngine.Random.Range(0.7f, 1f);
                __instance.transform.localScale = Vector3.one * l_RandomNumber;
                foreach (var l_Current in ____bigCuttableBySaberList)
                {
                    l_Current.transform.localScale = Vector3.one * (1 / l_RandomNumber);
                }
                foreach (var l_Current in ____smallCuttableBySaberList)
                {
                    l_Current.transform.localScale = Vector3.one * (1 / l_RandomNumber);
                }
            }

            if (SheepControl.m_CommandHandler.AutoDissolveNotes)
            {
                BaseNoteVisuals l_Visuals = __instance.GetComponent<BaseNoteVisuals>();
                l_Visuals.AnimateCutout(0, 1, 0.5f);
            }
            if (SheepControl.m_CommandHandler.AutoResolveNotes)
            {
                SheepControl.m_CommandHandler.AutoDissolveNotes = false;
                BaseNoteVisuals l_Visuals = __instance.GetComponent<BaseNoteVisuals>();
                l_Visuals.AnimateCutout(1, 0, 0.2f);
            }


            if (SheepControl.m_CommandHandler.PauseRandomNote)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("∞∛∞∔∗∝√∇∄∍∝∢∢∻∸∵∲∯∯∙");
                __instance.Pause(true);
                SheepControl.m_CommandHandler.PauseRandomNote = false;
                var t = Task.Run(async delegate
                {
                    await Task.Delay(1000);
                    __instance.Pause(false);
                });
            }

            if (SheepControl.m_CommandHandler.MakeRandomNoteRotation)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("∞∛∞∔∗∝√∇∄∍∝∢∢∻∸∵∲∯∯∙");
                Quaternion l_Rot = __instance.transform.localRotation;
                float l_RandomRange = Random.Range(-15f, 15f);
                float l_OldZ = 0f;
                __instance.gameObject.Turn(new Vector3(0, 0, l_Rot.z + l_RandomRange), 1f).OnVectorChange += (p_CurrentVector) =>
                {
                    foreach (var l_Box in ____bigCuttableBySaberList)
                    {
                        l_Box.transform.localRotation = Quaternion.Euler(l_Box.transform.localRotation.x, l_Box.transform.localRotation.y, l_Box.transform.localRotation.z + l_OldZ - p_CurrentVector.z);
                    }

                    foreach (var l_Box in ____smallCuttableBySaberList)
                    {
                        l_Box.transform.localRotation = Quaternion.Euler(l_Box.transform.localRotation.x, l_Box.transform.localRotation.y, l_Box.transform.localRotation.z + l_OldZ - p_CurrentVector.z);
                    }
                    l_OldZ = p_CurrentVector.z;
                };
            }

            if (SheepControl.m_CommandHandler.MappingExtension)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("∞∛∞∔∗∝√∇∄∍∝∢∢∻∸∵∲∯∯∙");
                __instance.transform.GetChild(0).localPosition = new Vector3(__instance.transform.localPosition.x + Random.Range(-0.1f, 0.1f), 0, 0);
            }
        }
    }

    [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
    class ObstaclesPatches
    {
        private static void Postfix(ObstacleController __instance, ref StretchableObstacle ____stretchableObstacle)
        {
            if (SheepControl.m_CommandHandler.RandomColorWalls || SheepControl.m_CommandHandler.SHIT)
            {
                //var l_streachableObstacle = __instance.GetField<StretchableObstacle, ObstacleController>("_stretchableObstacle");

                ____stretchableObstacle.SetSizeAndColor(__instance.width, __instance.height, __instance.length, RandomUtils.RandomColor());
            }
        }
    }

    [HarmonyPatch(typeof(EnvironmentSceneSetup), nameof(EnvironmentSceneSetup.InstallBindings))]
    class OnSceneSetup
    {
        private static void Postfix()
        {
            LightsPatchSide.s_PlayerColorScheme = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData.colorSchemesSettings.GetSelectedColorScheme();
        }
    }

    [HarmonyPatch(typeof(LightWithIdManager), nameof(LightWithIdManager.SetColorForId))]
    [HarmonyPriority(int.MinValue)]
    class LightsPatchSide
    {
        public static ColorScheme s_PlayerColorScheme;

        private static void Postfix(LightWithIdManager __instance, int lightId, Color color)
        {
            if (!SheepControl.m_CommandHandler.SpecifiedLightsSide) return;

            if (s_PlayerColorScheme == null) return;

            Color l_TestColor = new Color(color.r, color.g, color.b);

            if (l_TestColor == s_PlayerColorScheme.environmentColor0)
                __instance.SetColorForId(lightId, SheepControl.m_CommandHandler.LeftLightsColor.ColorWithAlpha(color.a));
            if (l_TestColor == s_PlayerColorScheme.environmentColor1)
                __instance.SetColorForId(lightId, SheepControl.m_CommandHandler.RightLightsColor.ColorWithAlpha(color.a));
        }
    }


    [HarmonyPatch(typeof(LightWithIdManager), nameof(LightWithIdManager.LateUpdate))]
    class LightsPatch
    {
        private static void Postfix(LightWithIdManager __instance)
        {

            if (SheepControl.m_CommandHandler.RandomLights)
            {
                for (int l_i = 0; l_i < __instance.GetField<List<ILightWithId>[], LightWithIdManager>("_lights").Length; l_i++)
                    __instance.SetColorForId(l_i, RandomUtils.RandomColor());
                SheepControl.m_CommandHandler.RandomLights = false;
            }

            if (SheepControl.m_CommandHandler.SpecifiedLights)
            {
                for (int l_i = 0; l_i < __instance.GetField<List<ILightWithId>[], LightWithIdManager>("_lights").Length; l_i++)
                    __instance.SetColorForId(l_i, SheepControl.m_CommandHandler.LightsColor);
                SheepControl.m_CommandHandler.SpecifiedLights = false;
            }

            if (SheepControl.m_CommandHandler.SpecifiedLightId)
            {
                __instance.SetColorForId(SheepControl.m_CommandHandler.LightId, SheepControl.m_CommandHandler.LightsColor);
                SheepControl.m_CommandHandler.SpecifiedLightId = false;
            }
        }
    }

    [HarmonyPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasCut))]
    class NoteCutSoundEffectManagerFix
    {
        public static bool m_Disabled = true;

        static bool Prefix() => m_Disabled;
    }

    [HarmonyPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasSpawned))]
    class NoteCutSoundEffectManagerFix2
    {
        static bool Prefix() => NoteCutSoundEffectManagerFix.m_Disabled;
    }

    public static class RandomUtils
    {
        public static Color RandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        public static Vector3 RandomVector3(float p_Min, float p_Max)
        {
            return new Vector3(Random.Range(p_Min, p_Max), Random.Range(p_Min, p_Max), Random.Range(p_Min, p_Max));
        }

        public static Quaternion RandomQuat(float p_Min, float p_Max)
        {
            return Quaternion.Euler(RandomVector3(p_Min, p_Max));
        }

        public static bool RandomBool()
        {
            return (UnityEngine.Random.Range(0, 1) == 1);
        }
    }
}
