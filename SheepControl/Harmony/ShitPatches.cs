using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IPA.Utilities;
using SheepControl.TUtils;
using SheepControl.Core;

namespace SheepControl.Shit
{
    public static class NotePatchesUtils
    {
        public static List<NoteData> m_allNotesData = new List<NoteData>();

        public static TrackLaneRingsManager m_ringsManager = Resources.FindObjectsOfTypeAll<TrackLaneRingsManager>().FirstOrDefault();

        public static LightWithIdManager m_LightsManager = Resources.FindObjectsOfTypeAll<LightWithIdManager>().First();
    }

    /// <summary>
    /// This patches ClassToPatch.MethodToPatch(Parameter1Type arg1, Parameter2Type arg2)
    /// </summary>
    [HarmonyPatch(typeof(NoteController), "HandleNoteDidStartJump")]
    public class ShitNotePatches
    {

        public static event Action e_OnNoteSpawn;

        /// <summary>
        ///
        /// </summary>
        /// <param name="__instance"></param>
        static void Prefix(NoteController __instance)
        {

            if (SheepControl.m_CommandHandler.SHIT)
            {
                e_OnNoteSpawn?.Invoke();

                var l_LightManager = Resources.FindObjectsOfTypeAll<LightWithIdManager>()[1];

                l_LightManager.SetColorForId(
                    UnityEngine.Random.Range(
                        0, 5
                        ), RandomUtils.RandomColor()
                    );

                //__instance.gameObject.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(RandomUtils.RandomVector3(-0.13f, 0.13f).x, 0, 0);
                __instance.gameObject.transform.GetChild(0).gameObject.transform.localScale = RandomUtils.RandomVector3(0.9f, 1.0f);

                if (!__instance.gameObject.activeInHierarchy) return;

                if (UnityEngine.Random.Range(0, 15) == 0)
                {
                    __instance.AnimateCutout(0.1f, 1.0f, 0.7f);
                }

                if (__instance.noteData.cutDirection == NoteCutDirection.Any)
                    __instance.noteData.SetCutDirectionAngleOffset(__instance.noteData.cutDirectionAngleOffset + UnityEngine.Random.Range(-40, 40));

                foreach (var l_currentRing in NotePatchesUtils.m_ringsManager.Rings)
                {
                    l_currentRing.SetDestRotation(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0.1f, 0.5f));
                    l_currentRing.SetPosition(UnityEngine.Random.Range(-10, 60), 0.5f);
                    l_currentRing.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.25f, 0.5f);
                    l_currentRing.FixedUpdateRing(Time.fixedDeltaTime);
                }

                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("vas manger des pancakes");
            }
        }
    }

    [HarmonyPatch(typeof(BeatmapObjectsInstaller), nameof(BeatmapObjectsInstaller.InstallBindings))]
    static class OnGameLaunched
    {
        private static void Prefix()
        {
            NotePatchesUtils.m_ringsManager = Resources.FindObjectsOfTypeAll<TrackLaneRingsManager>().FirstOrDefault();
        }
    }

    [HarmonyPatch(typeof(ColorNoteVisuals), nameof(ColorNoteVisuals.HandleNoteControllerDidInit))]
    public class NoteColorPatched
    {

        /*static List<GameObject> s_fakesNotes = new List<GameObject>();

        static void PatchFakeNote(GameObject p_note, ColorType p_noteColor)
        {
            Color l_currentColor;

            PlayerData l_currentPlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;

            ColorScheme l_currentColorScheme = l_currentPlayerData.colorSchemesSettings.GetSelectedColorScheme();

            l_currentColor = (p_noteColor == ColorType.ColorA) ? l_currentColorScheme.saberAColor : l_currentColorScheme.saberBColor;

            var l_currentMat = p_note.GetComponent<MaterialPropertyBlockController>();

            l_currentMat.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), l_currentColor);
            l_currentMat.ApplyChanges();

        }
        */
        static void Postfix(ColorNoteVisuals __instance,
            ref int ____colorId,
            ref MaterialPropertyBlockController[] ____materialPropertyBlockControllers,
            ref MeshRenderer[] ____arrowMeshRenderers,
            ref MeshRenderer[] ____circleMeshRenderers,
            ref NoteControllerBase ____noteController
            )
        {
            if (SheepControl.m_CommandHandler.SHIT)
            {
                //____noteController.gameObject.transform.localPosition = new Vector3(RandomUtils.RandomVector3(-1f, 1f).x, 0, 0);

                ///Random Color
                if (UnityEngine.Random.Range(0, 7) == 1)
                {
                    foreach (var l_currentMat in ____materialPropertyBlockControllers)
                    {
                        l_currentMat.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), RandomUtils.RandomColor());
                        l_currentMat.ApplyChanges();
                    }
                }

                ///FakeNotes
                /*if (UnityEngine.Random.Range(0, 8) == 1)
                {
                    for (int l_i = 0; l_i < UnityEngine.Random.Range(1, 2); l_i++)
                    {
                        var l_currentFake = GameObject.Instantiate(____noteController.transform.GetChild(0).gameObject);

                        l_currentFake.transform.parent = ____noteController.transform;
                        PatchFakeNote(l_currentFake, ____noteController.noteData.colorType);
                        var l_fakeLoc = l_currentFake.gameObject.transform.localPosition;
                        l_currentFake.gameObject.transform.localPosition = ____noteController.transform.localPosition + new Vector3(UnityEngine.Random.Range(-12f, 12f), l_fakeLoc.y, l_fakeLoc.z);
                        l_currentFake.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, RandomUtils.RandomVector3(-180, 180).z));
                        GameObject.Destroy(l_currentFake, 5);
                    }
                }*/
            }
        }
    }

    [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
    static class ObstaclePatch
    {

        private static void Postfix(ObstacleController __instance, ref Color ____color)
        {

            if (SheepControl.m_CommandHandler.SHIT)
            {
                //var l_shitModController = Plugin.m_modControllerRef;
                List<GameObject> l_fakeWalls = new List<GameObject>();

                float l_distanceToRandom = 35;

                __instance.gameObject.transform.localPosition = new Vector3(RandomUtils.RandomVector3(l_distanceToRandom * -1, l_distanceToRandom).x, 0, 0);
                __instance.gameObject.transform.localScale = RandomUtils.RandomVector3(0.3f, 0.3f);
                __instance.gameObject.transform.localRotation = Quaternion.Euler(RandomUtils.RandomVector3(-180f, 180f));

                ____color = RandomUtils.RandomColor();

                for (int l_i = 0; l_i < 3; l_i++)
                {
                    var l_currentFakeWall = GameObject.Instantiate(__instance.transform.GetChild(0).gameObject);

                    l_currentFakeWall.transform.SetParent(__instance.transform);
                    l_currentFakeWall.gameObject.transform.localPosition = __instance.transform.localPosition + RandomUtils.RandomVector3(l_distanceToRandom * -1, l_distanceToRandom);
                    l_currentFakeWall.gameObject.transform.localRotation = Quaternion.Euler(RandomUtils.RandomVector3(-180f, 180f));
                    l_currentFakeWall.GetComponent<Renderer>().material.color = RandomUtils.RandomColor();

                    l_fakeWalls.Add(l_currentFakeWall);
                }

                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("a");
            }
        }

    }
}
