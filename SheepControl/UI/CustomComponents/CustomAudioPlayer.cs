using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberPlus.SDK.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using SliderSetting = BeatSaberMarkupLanguage.Components.Settings.SliderSetting;

namespace SheepControl.UI.CustomComponents
{
    /*internal class CustomAudioPlayer : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return "SheepControl.UI.Views.CustomAudioPlayer.bsml";
        }

        [UIComponent("Background")] VerticalLayoutGroup m_Background;

        [UIComponent("PlayerTransform")] HorizontalLayoutGroup m_PlayerTransform;
        [UIObject("ButtonTransform")] GameObject m_ButtonTransform;

        [UIComponent("TimeSetting")] SliderSetting m_TimeSetting;

        [UIComponent("NameText")] TextMeshProUGUI m_Name;

        AudioSource m_Source;
        AudioClip m_Clip;

        Button m_PlayButton;

        bool m_IsPlaying = false;

        protected override void PostCreate()
        {
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Background.gameObject, 0.5f);

            m_PlayButton = BeatSaberPlus.SDK.UI.Button.Create(m_ButtonTransform.transform, "▶", () =>
            {
                if (m_IsPlaying)
                {
                    m_Source.Pause();
                    m_PlayButton.SetButtonText("▶");
                } else
                {
                    m_Source.Play();
                    m_PlayButton.SetButtonText("⏸");
                }
            });

            BSMLAction l_TimeAction = new BSMLAction(this, GetType().GetMethod("SetTime"));

            Vector2 l_Rect = new Vector2(5, 1);

            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_TimeSetting, l_TimeAction, BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Time, 0, true, false, l_Rect, l_Rect);

            m_Source = gameObject.AddComponent<AudioSource>();
        }

        public void SetTime(float p_Value)
        {
            m_Source.time = p_Value;
        }

        public void Setup(AudioClip p_Clip, float p_Volume, float p_StartTime)
        {
            if (m_Source != null && m_Clip != null) return;

            m_Clip = p_Clip;
            m_Source.clip = m_Clip;
            m_Source.time = p_StartTime;
            m_Source.volume = p_Volume;

            m_Name.text = m_Clip.name;
        }

        public void Update()
        {
            if (!m_IsPlaying) return;

            m_TimeSetting.Value = m_Source.time;
            m_TimeSetting.ApplyValue();
        }
    }*/
}
