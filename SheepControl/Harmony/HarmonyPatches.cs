using HarmonyLib;
using UnityEngine;
using IPA.Utilities;
using TMPro;
using System.Threading.Tasks;
using SheepControl.Installers;
using System.Collections.Generic;
using SiraUtil.Affinity;
using Zenject;
using static BeatmapObjectSpawnMovementData;
using BeatSaberAPI.DataTransferObjects;

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

    [HarmonyPatch(typeof(ColorNoteVisuals), nameof(ColorNoteVisuals.HandleNoteControllerDidInit))]
    class NotesPatches
    {
        private static void Postfix(ColorNoteVisuals __instance, ref MaterialPropertyBlockController[] ____materialPropertyBlockControllers)
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


    [HarmonyPatch(typeof(NoteController), "HandleNoteDidStartJump")]
    class DisolvePatch
    {
        private static void Postfix(NoteController __instance)
        {
            if (SheepControl.m_CommandHandler.BigNotes)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("∞∛∞∔∗∝√∇∄∍∝∢∢∻∸∵∲∯∯∙");
                __instance.transform.localScale = Vector3.one * 1.5f;
            }
            else if (SheepControl.m_CommandHandler.SmallNotes)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("∞∛∞∔∗∝√∇∄∍∝∢∢∻∸∵∲∯∯∙");
                __instance.transform.localScale = RandomUtils.RandomVector3(0.7f, 1f);
            }

            if (SheepControl.m_CommandHandler.AutoDissolveNotes)
            {
                BaseNoteVisuals l_Visuals = __instance.GetComponent<BaseNoteVisuals>();
                l_Visuals.AnimateCutout(0, 1, 0.5f);
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
                __instance.transform.localRotation = Quaternion.Euler(0, 0, l_Rot.z + Random.Range(-15f, 15f));
            }

            if (SheepControl.m_CommandHandler.MappingExtension)
            {
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("∞∛∞∔∗∝√∇∄∍∝∢∢∻∸∵∲∯∯∙");
                __instance.transform.GetChild(0).localPosition = new Vector3(__instance.transform.localPosition.x + Random.Range(-0.1f, 0.1f), 0, 0);
            }
        }
    }

    /*[HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
    class ObstaclesPatches
    {
        private static void Prefix(ObstacleController __instance, ref StretchableObstacle ____stretchableObstacle)
        {
            if (Plugin.m_CommandHandler.RandomColorWalls)
            {
                var l_streachableObstacle = __instance.GetField<StretchableObstacle, ObstacleController>("_stretchableObstacle");

                l_streachableObstacle.SetSizeAndColor(__instance.width, __instance.height, __instance.length, ColorsUtils.RandomColor());
            }
        }
    }*/

    [HarmonyPatch(typeof(LightWithIdManager), nameof(LightWithIdManager.LateUpdate))]
    class LightsPatch
    {
        private static void Postfix(LightWithIdManager __instance)
        {

            if (SheepControl.m_CommandHandler.RandomLights)
            {
                for (int l_i = 0; l_i < __instance.GetLightsArray().Length; l_i++)
                    __instance.SetColorForId(l_i, RandomUtils.RandomColor());
                SheepControl.m_CommandHandler.RandomLights = false;
            }

            if (SheepControl.m_CommandHandler.SpecifiedLights)
            {
                for (int l_i = 0; l_i < __instance.GetLightsArray().Length; l_i++)
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
        public static bool m_Disabled = false;

        static bool Prefix() => true;
    }

    [HarmonyPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasSpawned))]
    class NoteCutSoundEffectManagerFix2
    {
        static bool Prefix() => true;
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
