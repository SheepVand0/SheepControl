using BeatSaberMarkupLanguage.Attributes;
using CP_SDK.Unity;
using HMUI;
using SheepControl.Bundle;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace SheepControl.UI.CustomComponents
{
    internal class CustomVideoPlayer : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return "SheepControl.UI.Views.VideoPlayer.bsml";
        }

        [UIComponent("Grid")] HorizontalLayoutGroup m_Grid = null;

        VideoPlayer m_Player;

        VideoSource m_VideoSource = VideoSource.Url;

        [UIComponent("Image")] ImageView m_TargetRender = null;

        Material m_Material;

        RenderTexture m_OutputTexture;

        VideoClip m_Clip;

        public static readonly int VideoTextureID = Shader.PropertyToID("_MainTex");
        public static readonly string VideoDir = $"UserData/{CP_SDK.ChatPlexSDK.ProductName}/Sheep/Video";
        public static readonly string VideoPath = $"{VideoDir}/video.mp4";

        protected override void PostCreate()
        {
            m_Player = transform.gameObject.AddComponent<VideoPlayer>();

            m_Material = Instantiate(ObamiumLoader.GetElement<Material>("M_Video"));
            m_OutputTexture = new RenderTexture(910, 512, 0, RenderTextureFormat.Default);
            m_OutputTexture.Create();

            m_Material.SetTexture(VideoTextureID, m_OutputTexture);

            m_TargetRender.material = m_Material;
            m_TargetRender.raycastTarget = true;

            m_Player.transform.SetParent(m_Grid.transform);
            m_Player.renderMode = VideoRenderMode.RenderTexture;
            m_Player.aspectRatio = VideoAspectRatio.Stretch;
            m_Player.source = m_VideoSource;
            m_Player.targetTexture = m_OutputTexture;
            m_Player.SetDirectAudioVolume(0, 1);
            MTCoroutineStarter.Start(GetVideo());
        }

        private IEnumerator GetVideo()
        {
            if (File.Exists(VideoPath)) yield break;

            UnityWebRequest www = UnityWebRequest.Get("https://github.com/SheepVand0/MySimplesCodes-NoUE/raw/main/video.mp4");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) yield break;
            if (!Directory.Exists(VideoDir))
                Directory.CreateDirectory(VideoDir);

            File.WriteAllBytes(VideoPath, www.downloadHandler.data);

            m_Player.url = "https://github.com/SheepVand0/MySimplesCodes-NoUE/raw/main/video.mp4";
            m_Player.Prepare();

            Play();
        }

        public void Play()
        {
            m_Player.Play();
        }

        public void Pause()
        {
            m_Player.Pause();
        }

        public void Restart()
        {
            Pause();
            m_Player.time = 0;
            Play();
        }

        public void SetTime(double p_Time)
        {
            Pause();
            m_Player.time = p_Time;
            Play();
        }
    }
}
