﻿using CP_SDK.UI.Components;
using CP_SDK.XUI;
using SheepControl.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SheepControl.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class GSText : XUIText
    {
        protected GSText(string p_Name, string p_Text) : base(p_Name, p_Text)
        {
            OnReady(PatchText);
        }

        public static new GSText Make(string p_Text)
        {
            return new GSText("SheepControlText", p_Text);
        }

        public GSText Bind(ref GSText p_Value)
        {
            p_Value = this;
            return this;
        }

        public static TMP_FontAsset TextFont = null;

        public static void PatchText(CText p_Text)
        {
            PatchText(p_Text.GetComponentInChildren<TextMeshProUGUI>());
        }

        public static void PatchText(TextMeshProUGUI p_Text)
        {
            if (TextFont == null)
            {
                byte[] l_FontBytes = AssemblyUtils.LoadFileFromAssembly("SheepControl.Resources.Teko-Medium.ttf");
                string l_Folder = "./UserData/SheepControl/Font";
                string l_FileName = $"{l_Folder}/SheepControlFont.ttf";
                if (!Directory.Exists(l_Folder))
                    Directory.CreateDirectory(l_Folder);

                File.WriteAllBytes(l_FileName, l_FontBytes);

                CP_SDK.Unity.FontManager.AddFontFile(l_FileName, out string l_FamilyName);
                CP_SDK.Unity.FontManager.TryGetTMPFontAssetByFamily(l_FamilyName, out TextFont);
            }

            p_Text.font = TextFont;
        }
    }
}
