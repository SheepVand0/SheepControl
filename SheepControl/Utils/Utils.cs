using SheepControl.AnimsUtils;
using SheepControl.Trucs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SheepControl.TUtils
{
    internal static class TUtils
    {
        public static Vector3Animation Turn(this GameObject p_GameObject, Vector3 p_Rotation, float p_Duration)
        {
            Vector3Animation.AddAnim(p_GameObject, out Vector3Animation l_RAnim);
            l_RAnim.Init(p_GameObject.transform.localRotation.eulerAngles, p_Rotation, p_Duration);
            l_RAnim.OnVectorChange += (p_Value) =>
            {
                p_GameObject.transform.localRotation = Quaternion.Euler(p_Value);
            };
            l_RAnim.Play();
            return l_RAnim;
        }

        public static void Move(this GameObject p_GameObject, Vector3 p_Position, float p_Duration)
        {
            Vector3Animation.AddAnim(p_GameObject, out Vector3Animation l_RAnim);
            l_RAnim.Init(p_GameObject.transform.localPosition, p_Position, p_Duration);
            l_RAnim.OnVectorChange += (p_Value) =>
            {
                p_GameObject.transform.localPosition = p_Value;
            };
            l_RAnim.Play();
        }

        public static void Scale(this GameObject p_GameObject, Vector3 p_Scale, float p_Duration)
        {
            Vector3Animation.AddAnim(p_GameObject, out Vector3Animation l_RAnim);
            l_RAnim.Init(p_GameObject.transform.localScale, p_Scale, p_Duration);
            l_RAnim.OnVectorChange += (p_Value) =>
            {
                p_GameObject.transform.localScale = p_Value;
            };
            l_RAnim.Play();
        }

        public static GameObject FindGm(string p_Query)
        {
            GameObject l_lastGM = null;
            string[] l_Gms = p_Query.Split('.');
            foreach (string l_Current in l_Gms)
            {
                if (l_lastGM == null)
                {
                    l_lastGM = GameObject.Find(l_Gms[0]);
                    continue;
                }

                l_lastGM = l_lastGM.transform.Find(l_Current).gameObject;

            }

            return l_lastGM;
        }

        public static void AnimateCutout(this NoteControllerBase p_NoteController, float p_Start, float p_End, float p_Duration)
        {
            p_NoteController.GetComponent<BaseNoteVisuals>().AnimateCutout(p_End, p_Start, p_Duration);
        }

        private static IEnumerator Delay(IEnumerator p_Coroutine, float p_Seconds)
        {
            yield return new WaitForSeconds(p_Seconds);

            SheepControlController.Instance.StartCoroutine(p_Coroutine);

            yield return null;
        }

        public static Coroutine StartCoroutineWithDelay(IEnumerator p_Coroutine, float p_Seconds)
        {
            return SheepControlController.Instance.StartCoroutine(Delay(p_Coroutine, p_Seconds));
        }
    }
}
