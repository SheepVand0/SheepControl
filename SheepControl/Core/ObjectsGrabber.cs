using BeatSaberPlus.SDK.Game;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SheepControl.Core
{
    internal class ObjectsGrabber
    {

        public static BeatmapObjectSpawnController ObjectSpawnController;
        public static BeatmapCallbacksController CallbacksController;
        public static BeatmapObjectSpawnMovementData ObjectsSpawnMovementData;
        public static GameSongController GameSongControllerObj;
        public static AudioTimeSyncController AudioTimeSyncController;
        public static AudioSource GameAudioSource;
        public static PlayerData GamePlayerData;
        public static Saber RightSaber;
        public static Saber LeftSaber;

        public static void GrabObjects()
        {
            try
            {
                ObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
                CallbacksController = ObjectSpawnController.GetField<BeatmapCallbacksController, BeatmapObjectSpawnController>("_beatmapCallbacksController");
                GameSongControllerObj = Resources.FindObjectsOfTypeAll<GameSongController>().FirstOrDefault();
                AudioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                GamePlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
                ObjectsSpawnMovementData = ObjectSpawnController.GetField<BeatmapObjectSpawnMovementData, BeatmapObjectSpawnController>("_beatmapObjectSpawnMovementData");
                GameAudioSource = AudioTimeSyncController.GetField<AudioSource, AudioTimeSyncController>("_audioSource");
                Saber[] l_Sabers = Resources.FindObjectsOfTypeAll<Saber>();
                foreach (var l_Saber in l_Sabers)
                {
                    if (l_Saber.saberType == SaberType.SaberA)
                        LeftSaber = l_Saber;
                    else
                        RightSaber = l_Saber;
                }
            } catch( Exception l_E)
            {
                Plugin.Log.Error($"[SHEEP_COMMAND_ERROR] : {l_E.Message}");
            }
        }

    }
}
