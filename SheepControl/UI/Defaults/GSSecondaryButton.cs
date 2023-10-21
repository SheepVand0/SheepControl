﻿using CP_SDK.UI.Components;
using SheepControl.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using IPA.Utilities;
using System;
using TMPro;
using UnityEngine;

namespace SheepControl.UI.Defaults
{
    internal class GSSecondaryButton : CP_SDK.XUI.XUISecondaryButton
    {
        int m_Width;
        int m_Height;

        protected GSSecondaryButton(string p_Name, string p_Label, int p_Width, int p_Height, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(SetupStyle);
            m_Width = p_Width;
            m_Height = p_Height;
        }

        public static GSSecondaryButton Make(string p_Label, int p_Width, int p_Height, string p_Name = "SheepControlButton", Action p_OnClick = null)
        {
            return new GSSecondaryButton(p_Name, p_Label, p_Width, p_Height, p_OnClick);
        }

        static Texture2D s_WhiteTexture = null;

        public virtual Color GetColor() => Color.black.ColorWithAlpha(0.7f);

        private async void SetupStyle(CSecondaryButton p_Button)
        {
            SetWidth(m_Width);
            SetHeight(m_Height);
            Texture2D l_Tex = new Texture2D(m_Width * 7, m_Height * 7);

            for (int l_X = 0; l_X < l_Tex.width; l_X++)
            {
                for (int l_Y = 0; l_Y < l_Tex.height; l_Y++)
                {
                    l_Tex.SetPixel(l_X, l_Y, Color.white);
                }
            }
            Texture2D l_NewTex = await Utils.TextureUtils.CreateRoundedTexture(await Utils.TextureUtils.Gradient(l_Tex, new Color(1, 1, 1, 0.7f), new Color(1f, 1f, 1f, 1), p_UseAlpha: true), 10);
            Sprite l_Sprite = Sprite.Create(l_NewTex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2(0, 0), 1000, 0, SpriteMeshType.FullRect);
            p_Button.SetBackgroundColor(GetColor());
            p_Button.SetBackgroundSprite(l_Sprite);
            GSText.PatchText(p_Button.gameObject.GetComponentInChildren<TextMeshProUGUI>());
        }
    }
}
