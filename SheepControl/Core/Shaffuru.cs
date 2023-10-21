using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.CompilerServices;
using BeatSaberPlus.SDK.Game;

namespace SheepControl.Core
{
    internal class Shaffuru : MonoBehaviour
    {
        public const string MAPSHASH_LINK = "UserData/BeatSaberPlus/Sheep/Maps.json";

        public static List<string> s_PlayableHash = new List<string>()
        {
        };

        internal static float s_OldBeatmapDudration = 0f;

        public static Shaffuru Instance;

        public void Awake()
        {
            if (Instance != null) GameObject.DestroyImmediate(this);
            Instance = this;
            GetMaps();

            Logic.OnSceneChange += (p_Scene) =>
            {
                if (p_Scene == Logic.ESceneType.Playing)
                {
                    s_OldBeatmapDudration = Logic.LevelData.Data.difficultyBeatmap.level.songDuration;
                }
            };
        }

        public static void GetMaps()
        {
            using (WebClient l_Client = new WebClient())
            {
                l_Client.DownloadFileAsync(new System.Uri("https://github.com/SheepVand0/SheepControl/raw/main/MapsHash.json"), MAPSHASH_LINK);
                l_Client.DownloadFileCompleted += (p_Sender, p_EventArgs) =>
                {
                    if (p_EventArgs.Error != null) return;

                    string l_MapsHash = System.IO.File.ReadAllText(MAPSHASH_LINK);
                    s_PlayableHash = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(l_MapsHash);
                    Instance.Loop();
                };
            }
        }

        public async void Loop()
        {
            await Task.Delay(UnityEngine.Random.Range(10000, 40000));

            if (Logic.ActiveScene != Logic.ESceneType.Playing) { Loop(); return; }

            if (BeatmapManager.s_ChangingBeatmap) { Loop(); return; }

            await BeatSaberPlus.SDK.Game.Levels.LoadSong($"custom_level_{s_PlayableHash[UnityEngine.Random.Range(0, s_PlayableHash.Count)]}", async (p_Beatmap) =>
            {
                (string l_Mode, string l_Diff) = BeatmapManager.GetRandomModeAndDifficultyByBeatmapLevel(p_Beatmap);

                float l_Duration = 0f;
                if (s_OldBeatmapDudration > p_Beatmap.songDuration)
                {
                    l_Duration = p_Beatmap.songDuration;
                    s_OldBeatmapDudration = l_Duration;
                }
                else
                {
                    l_Duration = s_OldBeatmapDudration;
                }

                await BeatmapManager.SwitchBeatmap(p_Beatmap, l_Mode, l_Diff, l_Duration * UnityEngine.Random.Range(0.2f, 0.8f), 0.5f);
            });

            Loop();
        }

    }
}
