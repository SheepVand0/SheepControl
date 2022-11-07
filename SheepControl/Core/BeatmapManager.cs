using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BeatSaberPlus.SDK.Game;
using CP_SDK.Unity;
using CP_SDK.Unity.Extensions;
using IPA.Utilities;
using SheepControl.Core;
using SheepControl.TUtils;
using UnityEngine;
using static SheepControl.Core.ObjectsGrabber;

namespace SheepControl.Core
{
    public static class BeatmapManager
    {
        internal static bool s_ChangingBeatmap = false;

        /*public static void SwitchBeatmap(IBeatmapLevel p_BeatmapLevel, string p_Mode, string p_SerializedDifficulty, float p_Time, float p_DissolveTime)
        {
            MTCoroutineStarter.Start(SwitchBeatmapCoroutine(p_BeatmapLevel, p_Mode, p_SerializedDifficulty, p_Time, p_DissolveTime));
        }

        public static IEnumerator SwitchBeatmapCoroutine(IBeatmapLevel p_BeatmapLevel, string p_Mode, string p_SerializedDifficulty, float p_Time, float p_DissolveTime)
        {
            s_ChangingBeatmap = true;

            ///Get difficulty beatmap by serialized difficulty
            IDifficultyBeatmap l_Difficulty = null;

            ///Get IDifficultyBeatmap from p_SerializedDifficulty
            foreach (var l_BeatmapSet in p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets)
            {
                if (!l_BeatmapSet.beatmapCharacteristic.name.ToLower().Contains(p_Mode.ToLower())) continue;

                foreach (var l_DiffBeatmap in l_BeatmapSet.difficultyBeatmaps)
                {
                    if (l_DiffBeatmap.difficulty.ToString().ToLower() != p_SerializedDifficulty.ToLower()) continue;

                    l_Difficulty = l_DiffBeatmap;
                }
            }

            if (l_Difficulty == null)
            {
                Plugin.Log.Warn("No beatmap with the selected difficulty was found");
                l_Difficulty = p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets.First().difficultyBeatmaps.Last();
            }

            ///Get useful objects
            GrabObjects();

            ///Fix Unity "Run out of virtual channels bug"
            NoteCutSoundEffectManagerFix.m_Disabled = false;

            ///Dissolve current objets
            DissolveAllObjectsRaw(p_DissolveTime);

            ///wait that all beatmap items are dissolved
            yield return new WaitForSeconds(p_DissolveTime*1.1f);

            ///Pausing song
            //AudioTimeSyncControlleObj.Pause();

            ///Gettings new beatmap info
            Task<IReadonlyBeatmapData> l_TaskNewReadonlyBeatmapData;
            BeatmapData l_NewReadonlyBeatmapData;

            l_TaskNewReadonlyBeatmapData = l_Difficulty.GetBeatmapDataAsync(p_BeatmapLevel.environmentInfo, GamePlayerData.playerSpecificSettings);
            l_TaskNewReadonlyBeatmapData.Wait();
            l_NewReadonlyBeatmapData = (BeatmapData)l_TaskNewReadonlyBeatmapData.Result;

            ///Replace old beatmap
            CallbacksController.ReplaceBeatmapData(l_NewReadonlyBeatmapData, p_Time);

            ///Reset audio, and time
            ResetTimeSync(GameAudioSource, p_BeatmapLevel.beatmapLevelData.audioClip, p_Time, 0, 1);
            AudioTimeSyncControlleObj.SeekTo(p_Time);

            ///Change offset
            IJumpOffsetYProvider l_JumpOffsetYProvider = ObjectsSpawnMovementData.GetField<IJumpOffsetYProvider, BeatmapObjectSpawnMovementData>("_jumpOffsetYProvider");
            BeatmapObjectSpawnControllerHelpers.GetNoteJumpValues(GamePlayerData.playerSpecificSettings, l_Difficulty.noteJumpStartBeatOffset, out var l_NoteJumpvalueType, out var l_NoteJumpValue);
            ObjectsSpawnMovementData.Init(l_NewReadonlyBeatmapData.numberOfLines, l_Difficulty.noteJumpMovementSpeed, p_BeatmapLevel.beatsPerMinute, l_NoteJumpvalueType, l_NoteJumpValue
                , l_JumpOffsetYProvider, Vector3.right, Vector3.forward);

            ///Change bpm
            CallbacksController.TriggerBeatmapEvent(new BPMChangeBeatmapEventData(p_Time, p_BeatmapLevel.beatsPerMinute));

            //AudioTimeSyncControlleObj.Resume();

            ///Restore hitsounds
            yield return new WaitForSeconds(0.8f);

            NoteCutSoundEffectManagerFix.m_Disabled = true;

            s_ChangingBeatmap = false;

            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("💥⅟⅕⅗Ⅳ₈ⅴⅴ₂ⅫⅫ⅝∂∋∉∋∈∌∜∖∞∢∥∡≊≓≄≁!sudo");
        }*/


        /// <summary>
        /// Change actual beatmap by another
        /// </summary>
        /// <param name="p_BeatmapLevel">New beatmaplevel</param>
        /// <param name="p_SerializedDifficulty">Difficulty</param>
        /// <param name="p_Time">Start time</param>
        /// <param name="p_DissolveTime">Old beatmap dissolve time</param>
        public async static Task<Task> SwitchBeatmap(IBeatmapLevel p_BeatmapLevel, string p_Mode, string p_SerializedDifficulty, float p_Time, float p_DissolveTime)
        {
            s_ChangingBeatmap = true;

            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("a");

            await Task.Delay(200);

            DissolveAllObjectsRaw(p_DissolveTime);

            await Task.Delay((int)p_DissolveTime * 1000 + 500);

            ///Fix Unity "Run out of virtual channels bug"
            NoteCutSoundEffectManagerFix.m_Disabled = false;

            ///Get difficulty beatmap by serialized difficulty
            IDifficultyBeatmap l_Difficulty = null;

            ///Get IDifficultyBeatmap from p_SerializedDifficulty
            foreach (var l_BeatmapSet in p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets)
            {
                if (!l_BeatmapSet.beatmapCharacteristic.name.ToLower().Contains(p_Mode.ToLower())) continue;

                foreach (var l_DiffBeatmap in l_BeatmapSet.difficultyBeatmaps)
                {
                    if (l_DiffBeatmap.difficulty != Levels.SerializedToDifficulty(p_SerializedDifficulty)) continue;

                    l_Difficulty = l_DiffBeatmap;
                }
            }

            if (l_Difficulty == null)
            {
                Plugin.Log.Warn("No beatmap with the selected difficulty was found");
                l_Difficulty = p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets.First().difficultyBeatmaps.Last();
            }

            ///Get useful objects
            GrabObjects();

            ///wait that all beatmap items are dissolved
            //await Task.Delay((int)p_DissolveTime * 1000);

            ///Pausing song
            //AudioTimeSyncControlleObj.Pause();

            ///Gettings new beatmap info
            Task<IReadonlyBeatmapData> l_TaskNewReadonlyBeatmapData;
            BeatmapData l_NewReadonlyBeatmapData = null;
            l_TaskNewReadonlyBeatmapData = l_Difficulty.GetBeatmapDataAsync(p_BeatmapLevel.environmentInfo, GamePlayerData.playerSpecificSettings);
            l_TaskNewReadonlyBeatmapData.Wait();
            l_NewReadonlyBeatmapData = (BeatmapData)l_TaskNewReadonlyBeatmapData.Result;

            ///Reset audio, and time
            ResetTimeSync(GameAudioSource, p_BeatmapLevel.beatmapLevelData.audioClip, p_Time, 0, 1);

            ///Replace old beatmap
            CallbacksController.ReplaceBeatmapData(l_NewReadonlyBeatmapData, p_Time);

            CallbacksController.ResetCallbackController();

            AudioTimeSyncControlleObj.SeekTo(p_Time/60);

            ///Dissolve current objets
            DestroyAllObjectsRaw(0);

            ///Change offset
            IJumpOffsetYProvider l_JumpOffsetYProvider = ObjectsSpawnMovementData.GetField<IJumpOffsetYProvider, BeatmapObjectSpawnMovementData>("_jumpOffsetYProvider");
            BeatmapObjectSpawnControllerHelpers.GetNoteJumpValues(GamePlayerData.playerSpecificSettings, l_Difficulty.noteJumpStartBeatOffset, out var l_NoteJumpvalueType, out var l_NoteJumpValue);
            ObjectsSpawnMovementData.Init(l_NewReadonlyBeatmapData.numberOfLines, l_Difficulty.noteJumpMovementSpeed, p_BeatmapLevel.beatsPerMinute, l_NoteJumpvalueType, l_NoteJumpValue
                , l_JumpOffsetYProvider, Vector3.right, Vector3.forward);

            ///Change bpm
            CallbacksController.TriggerBeatmapEvent(new BPMChangeBeatmapEventData(p_Time, p_BeatmapLevel.beatsPerMinute));

            //AudioTimeSyncControlleObj.Resume();

            ///Restore hitsounds

            TUtils.TUtils.StartCoroutineWithDelay(DisableNotCutSoundFix(), 1);

            await Task.Delay(1000);

            s_ChangingBeatmap = false;

            return Task.CompletedTask;
        }

        public static (string, string) GetRandomModeAndDifficultyByBeatmapLevel(IBeatmapLevel p_BeatmapLevel)
        {
            string l_Mode;
            string l_Difficulty;

            IDifficultyBeatmapSet l_DiffSet = null;
            IDifficultyBeatmap l_Beatmap = null;

            if (p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets.Count == 1)
                l_DiffSet = p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets[0];
            else
                l_DiffSet =
                p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets.ElementAt(
                    UnityEngine.Random.Range(0, p_BeatmapLevel.beatmapLevelData.difficultyBeatmapSets.Count));

            if (l_DiffSet.difficultyBeatmaps.Count > 1)
                l_Beatmap = l_DiffSet.difficultyBeatmaps.ElementAt(UnityEngine.Random.Range(0, l_DiffSet.difficultyBeatmaps.Count));
            else l_Beatmap = l_DiffSet.difficultyBeatmaps[0];

            l_Mode = l_DiffSet.beatmapCharacteristic.name;
            l_Difficulty = DifficultyToSerialized(l_Beatmap.difficulty);

            return (l_Mode, l_Difficulty);
        }

        public static string DifficultyToSerialized(BeatmapDifficulty p_Diff)
        {
            switch (p_Diff)
            {
                case BeatmapDifficulty.Easy: return "easy";
                case BeatmapDifficulty.Normal: return "normal";
                case BeatmapDifficulty.Hard: return "hard";
                case BeatmapDifficulty.Expert: return "expert";
                case BeatmapDifficulty.ExpertPlus: return "expertplus";
                default: return "expertplus";
            }
        }

        /// <summary>
        /// Change audio and time
        /// </summary>
        /// <param name="p_Source">AudioSource component</param>
        /// <param name="p_Clip">New clip</param>
        /// <param name="p_Time">New time</param>
        /// <param name="p_TimeOffset">Time offset</param>
        /// <param name="p_TimeScale">Time scale (speed)</param>
        public static void ResetTimeSync(AudioSource p_Source, AudioClip p_Clip, float p_Time, float p_TimeOffset, float p_TimeScale)
        {
            AudioTimeSyncController l_Controller = AudioTimeSyncControlleObj;
            AudioTimeSyncController.InitData initData =
                l_Controller.GetField<AudioTimeSyncController.InitData, AudioTimeSyncController>("_initData");
            AudioTimeSyncController.InitData newData = new AudioTimeSyncController.InitData(p_Clip,
                            p_Time, p_TimeOffset, 1f);
            p_Source.clip = p_Clip;
            l_Controller.SetField("_initData", newData);
            l_Controller.SetField("_timeScale", p_TimeScale);
            l_Controller.SetField("_startSongTime", p_Time);
            l_Controller.SetField("_audioStartTimeOffsetSinceStart", /*timeSync.GetProperty<float, AudioTimeSyncController>("timeSinceStart") - */p_Time);
            l_Controller.SetField("_fixingAudioSyncError", true);
            l_Controller.SetField("_playbackLoopIndex", 0);
            l_Controller.SetField("_audioStarted", false);
            p_Source.pitch = p_TimeScale;
            //timeSync.SeekTo(p_Time * (60 / p_Clip.length));
        }

        /// <summary>
        /// Repalce current beatmap data
        /// </summary>
        /// <param name="p_CallbacksController">Callbacks controller</param>
        /// <param name="p_BeatmapData">New beatmap data</param>
        /// <param name="p_Time">New time</param>
        public static void ReplaceBeatmapData(this BeatmapCallbacksController p_CallbacksController, BeatmapData p_BeatmapData, float p_Time)
        {
            p_CallbacksController.SetField("_beatmapData", p_BeatmapData as IReadonlyBeatmapData);

            p_CallbacksController.SetField("_startFilterTime", p_Time);
            p_CallbacksController.SetField("_prevSongTime", p_Time);
        }

        /// <summary>
        /// Reset callbacks controller
        /// </summary>
        /// <param name="p_CallbacksController">CallbacksController</param>
        public static void ResetCallbackController(this BeatmapCallbacksController p_CallbacksController)
        {
            Dictionary<float, CallbacksInTime> l_Callbacks = p_CallbacksController.GetField<Dictionary<float, CallbacksInTime>, BeatmapCallbacksController>("_callbacksInTimes");
            foreach (var l_Index in l_Callbacks.Values)
            {
                l_Index.lastProcessedNode = null;
            }
        }

        /// <summary>
        /// Dissolve current beatmap objects
        /// </summary>
        /// <param name="p_Duration"></param>
        public static void DestroyAllObjectsRaw(float p_Duration)
        {
            var l_Notes = ObjectManager.GetField<MemoryPoolContainer<GameNoteController>, BasicBeatmapObjectManager>("_basicGameNotePoolContainer");
            var l_SliderHeads = ObjectManager.GetField<MemoryPoolContainer<GameNoteController>, BasicBeatmapObjectManager>("_burstSliderHeadGameNotePoolContainer");
            var l_Sliders = ObjectManager.GetField<MemoryPoolContainer<BurstSliderGameNoteController>, BasicBeatmapObjectManager>("_burstSliderGameNotePoolContainer");
            var l_Sliderfills = ObjectManager.GetField<MemoryPoolContainer<BurstSliderGameNoteController>, BasicBeatmapObjectManager>("_burstSliderFillPoolContainer");
            var l_Bombs = ObjectManager.GetField<MemoryPoolContainer<BombNoteController>, BasicBeatmapObjectManager>("_bombNotePoolContainer");
            var l_Walls = ObjectManager.GetField<MemoryPoolContainer<ObstacleController>, BasicBeatmapObjectManager>("_obstaclePoolContainer");
            foreach (var l_Note in l_Notes.activeItems)
            {
                if (l_Note == null /*|| !l_Note.gameObject.activeInHierarchy*/) continue;
                l_Note.Dissolve(p_Duration);
            }
            foreach (var l_SliderHead in l_SliderHeads.activeItems)
            {
                if (l_SliderHead == null /*|| !l_SliderHead.gameObject.activeInHierarchy*/) continue;
                l_SliderHead.Dissolve(p_Duration);
            }
            foreach (var l_Slider in l_Sliders.activeItems)
            {
                if (l_Slider == null /*|| !l_Slider.gameObject.activeInHierarchy*/ ) continue;
                l_Slider.Dissolve(p_Duration);
            }
            foreach (var l_Slider in l_Sliderfills.activeItems)
            {
                if (l_Slider == null /*|| !l_Slider.gameObject.activeInHierarchy*/) continue;
                l_Slider.Dissolve(p_Duration);
            }
            foreach (var l_Bomb in l_Bombs.activeItems)
            {
                if (l_Bomb == null /*|| !l_Bomb.gameObject.activeInHierarchy*/) continue;
                l_Bomb.Dissolve(p_Duration);
            }
            foreach (var l_Wall in l_Walls.activeItems)
            {
                if (l_Wall == null /*|| !l_Wall.gameObject.activeInHierarchy*/) continue;
                l_Wall.Dissolve(p_Duration);
            }
        }

        /// <summary>
        /// Dissolve current beatmap objects
        /// </summary>
        /// <param name="p_Duration"></param>
        public static void DissolveAllObjectsRaw(float p_Duration)
        {
            var l_Notes = ObjectManager.GetField<MemoryPoolContainer<GameNoteController>, BasicBeatmapObjectManager>("_basicGameNotePoolContainer");
            var l_SliderHeads = ObjectManager.GetField<MemoryPoolContainer<GameNoteController>, BasicBeatmapObjectManager>("_burstSliderHeadGameNotePoolContainer");
            var l_Sliders = ObjectManager.GetField<MemoryPoolContainer<BurstSliderGameNoteController>, BasicBeatmapObjectManager>("_burstSliderGameNotePoolContainer");
            var l_Sliderfills = ObjectManager.GetField<MemoryPoolContainer<BurstSliderGameNoteController>, BasicBeatmapObjectManager>("_burstSliderFillPoolContainer");
            var l_Bombs = ObjectManager.GetField<MemoryPoolContainer<BombNoteController>, BasicBeatmapObjectManager>("_bombNotePoolContainer");
            //var l_Walls = ObjectManager.GetField<MemoryPoolContainer<ObstacleController>, BasicBeatmapObjectManager>("_obstaclePoolContainer");
            foreach (var l_Note in l_Notes.activeItems)
            {
                if (l_Note == null /*|| !l_Note.gameObject.activeInHierarchy*/) continue;
                l_Note.AnimateCutout(1, 0, p_Duration);
            }
            foreach (var l_SliderHead in l_SliderHeads.activeItems)
            {
                if (l_SliderHead == null /*|| !l_SliderHead.gameObject.activeInHierarchy*/) continue;
                l_SliderHead.AnimateCutout(1, 0, p_Duration);
            }
            foreach (var l_Slider in l_Sliders.activeItems)
            {
                if (l_Slider == null /*|| !l_Slider.gameObject.activeInHierarchy*/ ) continue;
                l_Slider.AnimateCutout(1, 0, p_Duration);
            }
            foreach (var l_Slider in l_Sliderfills.activeItems)
            {
                if (l_Slider == null /*|| !l_Slider.gameObject.activeInHierarchy*/) continue;
                l_Slider.AnimateCutout(1, 0, p_Duration);
            }
            foreach (var l_Bomb in l_Bombs.activeItems)
            {
                if (l_Bomb == null /*|| !l_Bomb.gameObject.activeInHierarchy*/) continue;
                l_Bomb.AnimateCutout(1, 0, p_Duration);
            }
        }

        /// <summary>
        /// Stop HitSound fix
        /// </summary>
        /// <returns></returns>
        public static IEnumerator DisableNotCutSoundFix()
        {
            NoteCutSoundEffectManagerFix.m_Disabled = true;
            yield return null;
        }

        /// <summary>
        /// Get a speed modifier by speed
        /// </summary>
        /// <param name="p_BaseModifiers">Base player modifiers</param>
        /// <param name="p_SpeedInPercent">New speed in percent</param>
        /// <returns></returns>
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

    }
}
