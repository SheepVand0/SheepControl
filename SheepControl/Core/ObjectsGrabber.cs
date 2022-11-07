using BeatSaberPlus.SDK.Game;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace SheepControl.Core
{
    internal class ObjectsGrabber
    {

        public static BeatmapObjectSpawnController ObjectSpawnController;
        public static BasicBeatmapObjectManager ObjectManager;
        public static BeatmapCallbacksController CallbacksController;
        public static BeatmapObjectSpawnMovementData ObjectsSpawnMovementData;
        public static GameSongController GameSongControllerObj;
        public static AudioTimeSyncController AudioTimeSyncControlleObj;
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
                AudioTimeSyncControlleObj = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                GamePlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
                ObjectsSpawnMovementData = ObjectSpawnController.GetField<BeatmapObjectSpawnMovementData, BeatmapObjectSpawnController>("_beatmapObjectSpawnMovementData");
                GameAudioSource = AudioTimeSyncControlleObj.GetField<AudioSource, AudioTimeSyncController>("_audioSource");
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

    internal class ZenjectGrabber
    {
        public ZenjectGrabber(BasicBeatmapObjectManager p_BasicBeatmapObjectManager)
        {
            ObjectsGrabber.ObjectManager = p_BasicBeatmapObjectManager;
        }
    }

    internal class ZenjectGrabberInstaller : Installer<ZenjectGrabberInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ZenjectGrabber>().AsSingle().NonLazy();
        }
    }

}
