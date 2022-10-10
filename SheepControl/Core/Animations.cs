using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Reflection;
using IPA.Utilities;
using System.CodeDom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Assertions.Must;

namespace SheepControl.AnimsUtils
{
    unsafe class FloatAnimation : MonoBehaviour
    {
        public event Action<float> OnChange;

        public event Action<float> OnFinished;

        float m_Start;

        float m_End;

        bool m_Started;

        float m_Duration = 0;

        float m_ValueDuration = 0;

        float m_StartTime = 0;
        public void Stop()
        {
            m_Started = false;
        }

        public void Update()
        {
            if (m_Started == false) return;

            float l_Prct = (Time.realtimeSinceStartup - m_StartTime) / m_Duration;

            float l_Value = m_Start + (m_ValueDuration * l_Prct);

            OnChange?.Invoke(l_Value);

            if (l_Prct > 1) { m_Started = false; OnFinished?.Invoke(l_Value); }
        }
        public void Init(float p_Start, float p_Value, float p_Duration)
        {
            m_Start = p_Start;
            m_End = p_Value;
            m_ValueDuration = m_End - m_Start;
            m_Duration = p_Duration;
        }

        public void Play()
        {
            m_StartTime = Time.realtimeSinceStartup;
            if (m_Duration == 0 || float.IsPositiveInfinity(m_Duration) || float.IsNegativeInfinity(m_Duration))
            {
                OnChange?.Invoke(m_End);
                OnFinished?.Invoke(m_End);
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            m_Started = true;
        }
    }

    public class Vector3Animation : MonoBehaviour
    {
        public event Action<Vector3> OnVectorChange;

        public event Action<Vector3> OnFinished;

        public Vector3 m_Start;

        public Vector3 m_End;

        Vector3 m_Current;

        public float m_Duration;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public void Stop()
        {
            foreach (var l_Current in gameObject.GetComponents<FloatAnimation>())
                l_Current.Stop();
        }

        public void Play()
        {
            FloatAnimation l_XAnim = gameObject.AddComponent<FloatAnimation>();
            l_XAnim.Init(m_Start.x, m_End.x, m_Duration);
            FloatAnimation l_YAnim = gameObject.AddComponent<FloatAnimation>();
            l_YAnim.Init(m_Start.y, m_End.y, m_Duration);
            FloatAnimation l_ZAnim = gameObject.AddComponent<FloatAnimation>();
            l_ZAnim.Init(m_Start.z, m_End.z, m_Duration);
            l_XAnim.OnChange += (p_Val) =>
            {
                m_Current = new Vector3(p_Val, m_Current.y, m_Current.z);
                OnVectorChange?.Invoke(m_Current);
            };
            l_YAnim.OnChange += (p_Val) =>
            {
                m_Current = new Vector3(m_Current.x, p_Val, m_Current.z);
                OnVectorChange?.Invoke(m_Current);
            };
            l_ZAnim.OnChange += (p_Val) =>
            {
                m_Current = new Vector3(m_Current.x, m_Current.y, p_Val);
                OnVectorChange?.Invoke(m_Current);
            };
            l_XAnim.Play();
            l_YAnim.Play();
            l_ZAnim.Play();
            l_XAnim.OnFinished += (p_Val) => { OnFinished?.Invoke(m_End); };
        }

        public void Init(Vector3 p_Start, Vector3 p_Value, float p_Duration)
        {
            m_Start = p_Start;
            m_End = p_Value;

            m_Duration = p_Duration;
        }
    }
}

