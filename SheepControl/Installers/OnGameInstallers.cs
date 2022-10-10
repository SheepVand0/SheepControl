using Zenject;
using System.Reflection;
using HarmonyLib;

namespace SheepControl.Installers
{
    class OnGameInstaller : Installer<OnGameInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ObjectsGrabber>().AsSingle().NonLazy();
        }
    }

    public static class InstallersUtils {
        private static readonly PropertyInfo ContainerPropertyInfo = typeof(MonoInstallerBase).GetProperty(
            "Container",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        public static DiContainer GetContainer(this MonoInstallerBase monoInstallerBase)
        {
            return (DiContainer)ContainerPropertyInfo.GetValue(monoInstallerBase);
        }
    }

    public class ObjectsGrabber
    {
        public static BeatmapCallbacksController m_BeatmapCallbacksControlller;
        public static BeatmapObjectSpawnController m_BeatmapObjectSpawnController;
        public static SongSpeedData m_SongSpeedData;

        [Inject]
        public ObjectsGrabber(BeatmapCallbacksController beatmapCallbacksController, BeatmapObjectSpawnController spawnController, SongSpeedData songSpeedData)
        {
            m_BeatmapCallbacksControlller = beatmapCallbacksController;
            m_BeatmapObjectSpawnController = spawnController;
            m_SongSpeedData = songSpeedData;
        }
    }
}
