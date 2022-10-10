﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using static SheepControl.TUtils.TUtils;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using SheepControl.UI;
using IPA.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SheepControl.AnimsUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using BeatSaberPlus.SDK.Game;

namespace SheepControl.Trucs
{

    unsafe public class Bobby : MonoBehaviour
    {
        public static Bobby m_Instance;

        GameNoteController m_NotePrefab;

        GameObject m_Note;

        public SettingsView m_SettingsView;

        AudioClip m_WoolSound;

        //AudioClip m_EatingSound;

        AudioSource m_AudioSource;

        public bool m_EnableRandomMoves = true;

        public float m_BobbyMoveDuration = 5.0f;

        public float m_BobbyStealDuration = 3.0f;

        public float m_BobbyTurnDuration = 1.0f;

        public static readonly List<string> STEALABLE_GAME_OBJECTS = new List<string>()
        { "Feet", "Notes", "MagicDoorSprite", "Construction", "GlowLineC", "GlowLineR", "GlowLineL",
        "Clouds", "PlayersPlace", "TrackMirror", "Logo", "Spectrograms", "PileOfNotes"};

        public static List<GameObject> m_BobbyStoleObjects = new List<GameObject>();

        public const float BOBBY_HEIGHT = 0.2f;

        IEnumerator GetClips()
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://github.com/SheepVand0/MySimplesCodes-NoUE/blob/main/Wool%20Placing%20(Nr.%204%20%20%20Minecraft%20Sound)%20-%20Sound%20Effect%20for%20editing.mp3?raw=true", AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (!www.isNetworkError)
                    m_WoolSound = DownloadHandlerAudioClip.GetContent(www);
            }

            m_AudioSource = gameObject.AddComponent<AudioSource>();
        }

        public void Awake()
        {
            m_Instance = this;

            StartCoroutine(GetClips());

            SceneManager.LoadSceneAsync("StandardGameplay", LoadSceneMode.Additive).completed += (_) =>
            {
                var l_BeatmapObjectsInstaller = Resources.FindObjectsOfTypeAll<BeatmapObjectsInstaller>().FirstOrDefault();
                var l_OriginalNotePrefab = l_BeatmapObjectsInstaller.GetField<GameNoteController, BeatmapObjectsInstaller>("_normalBasicNotePrefab");

                m_NotePrefab = l_OriginalNotePrefab;
                m_Note = Instantiate(m_NotePrefab.transform.GetChild(0).gameObject);
                m_Note.transform.SetParent(this.transform);

                GameObject.DontDestroyOnLoad(m_Note);

                foreach (var l_MatControl in m_Note.GetComponents<MaterialPropertyBlockController>())
                {
                    l_MatControl.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), new Color(0.9f, 0, 0.9f));
                    l_MatControl.ApplyChanges();
                }

                transform.position = new Vector3(-2.38f, 0.2f, 1.5f);
                transform.rotation = Quaternion.Euler(90, 300.4f, 0);

                m_Note.GetComponentsInChildren<NoteBigCuttableColliderSize>().First().gameObject.SetActive(false);

                m_EnableRandomMoves = SConfig.GetStaticModSettings().BobbyAutoRonde;

                ApplyConfig();

                SceneManager.UnloadSceneAsync("StandardGameplay");
            };
        }

        public Vector3Animation IntelligentMove(Vector3 p_NewPos, float p_Speed)
        {
            LookAtLocation(p_NewPos);
            return Move(p_NewPos, p_Speed);
        }

        public void LookAtLocation(Vector3 p_Location)
        {
            Turn(Quaternion.LookRotation(p_Location - transform.localPosition).eulerAngles + new Vector3(90, 180, 0), 0.1f);
        }

        public Vector3Animation IntelligentSteal(GameObject p_Target, float p_Duration)
        {
            GameObject l_Target = p_Target;
            Vector3Animation l_Move = IntelligentMove(p_Target.transform.localPosition - new Vector3(0, p_Target.transform.localPosition.y - BOBBY_HEIGHT, 0), p_Duration);
            l_Move.OnFinished += (p_Pos) =>
            {
                Steal(l_Target);
            };
            return l_Move;
        }

        public void Steal(GameObject p_Gm)
        {
            m_BobbyStoleObjects.Add(p_Gm);
            p_Gm.transform.localPosition = transform.localPosition + new Vector3(0, 0.3f, 0);
            m_AudioSource.clip = m_WoolSound;
            m_AudioSource.volume = 0.5f;
            m_AudioSource.Play();
        }

        public void ReleaseAll()
        {
            foreach (var l_Object in m_BobbyStoleObjects)
            {
                l_Object.transform.SetPositionAndRotation(
                    (RandomUtils.RandomBool() == true) ? RandomUtils.RandomVector3(2.5f, 10) : RandomUtils.RandomVector3(-2.5f, -10),
                    RandomUtils.RandomQuat(-180, 180));
            }
            m_BobbyStoleObjects.Clear();
        }

        public IEnumerator Ronde()
        {
            yield return new WaitForSeconds(4.2f);

            if (m_EnableRandomMoves)
                try
                {
                    Vector3 l_RandomPos = new Vector3(UnityEngine.Random.Range(-6, 6), BOBBY_HEIGHT, UnityEngine.Random.Range(-2, 6));
                    switch (UnityEngine.Random.Range(0, 6))
                    {
                        case 0:
                            StopCurrentAnims();
                            IntelligentMove(l_RandomPos, m_BobbyMoveDuration);
                            break;
                        case 1:
                            StopCurrentAnims();
                            GameObject l_ToStealGm = FindGm(STEALABLE_GAME_OBJECTS[UnityEngine.Random.Range(0, STEALABLE_GAME_OBJECTS.Count)]);
                            IntelligentSteal(l_ToStealGm, m_BobbyStealDuration);
                            break;
                        case 2:
                            SetColor(RandomUtils.RandomColor());
                            StopCurrentAnims();
                            IntelligentMove(l_RandomPos, m_BobbyMoveDuration);
                            break;
                        case 3:
                            StopCurrentAnims();
                            Turn(new Vector3(90, 15000, 0), 112);
                            break;
                        case 4:
                            StopCurrentAnims();
                            Turn(new Vector3(90, UnityEngine.Random.Range(0, 360), 0), m_BobbyTurnDuration);
                            break;
                        case 5:
                            ReleaseAll();
                            break;
                    }
                    StartCoroutine(Ronde());
                }
                catch (System.Exception l_E)
                {
                    StartCoroutine(Ronde());
                }
        }

        public void StopCurrentAnims()
        {
            foreach (var l_Anim in gameObject.GetComponentsInChildren<Vector3Animation>())
                l_Anim.Stop();
        }

        public Vector3Animation Move(Vector3 p_Pos, float p_Duration)
        {
            Vector3Animation l_Animation = gameObject.AddComponent<Vector3Animation>();
            l_Animation.Init(transform.localPosition, p_Pos, p_Duration);
            l_Animation.Play();
            l_Animation.OnVectorChange += OnMove;
            return l_Animation;
        }
        void OnMove(Vector3 p_Value)
        {
            transform.localPosition = p_Value;
            foreach (var l_Current in m_BobbyStoleObjects)
            {
                if (l_Current == null) { m_BobbyStoleObjects.Remove(l_Current); continue; }

                l_Current.transform.localPosition = p_Value;
            }
        }

        public Vector3Animation Turn(Vector3 p_NewRot, float p_Duration)
        {
            Vector3Animation l_Animation = gameObject.AddComponent<Vector3Animation>();
            l_Animation.Init(transform.localRotation.eulerAngles, p_NewRot, p_Duration);
            l_Animation.Play();
            l_Animation.OnVectorChange += OnTurn;
            return l_Animation;
        }
        void OnTurn(Vector3 p_NewRot)
        {
            transform.localRotation = Quaternion.Euler(p_NewRot);
            foreach (var l_Current in m_BobbyStoleObjects)
            {
                if (l_Current == null) { m_BobbyStoleObjects.Remove(l_Current); continue; }

                l_Current.transform.localRotation = Quaternion.Euler(p_NewRot - new Vector3(90, transform.localRotation.y));
            }
        }
        public void SetColor(Color p_Color)
        {
            foreach (var l_MatControl in m_Note.GetComponents<MaterialPropertyBlockController>())
            {
                l_MatControl.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), p_Color);
                l_MatControl.ApplyChanges();
            }
        }

        public void Reset()
        {
            m_Instance.ReleaseAll();
            m_Instance.m_EnableRandomMoves = false;
            m_Instance.StopCurrentAnims();
            m_Instance.IntelligentMove(new UnityEngine.Vector3(0, 0, 0), 1).OnFinished += (p_Val) =>
            {
                m_Instance.Turn(new UnityEngine.Vector3(90, 180, 0), 4);
            };
        }

        public void ApplyConfig()
        {
            StopAllCoroutines();
            m_EnableRandomMoves = SConfig.Instance.GetModSettings().BobbyAutoRonde;
            if (m_EnableRandomMoves == true) { StartCoroutine(Ronde()); }
            m_BobbyMoveDuration = SConfig.GetStaticModSettings().BobbyMoveDuration;
            m_BobbyStealDuration = SConfig.GetStaticModSettings().BobbyStealDuration;
            m_BobbyTurnDuration = SConfig.GetStaticModSettings().BobbyTurnDuration;
        }
    }
}