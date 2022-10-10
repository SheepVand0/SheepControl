using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SheepControl.UI.CustomComponents
{
    class FlyingText : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return "SheepControl.UI.Views.Text.bsml";
        }

        [UIComponent("Text")] TMPro.TextMeshProUGUI m_Text;

        FloatingScreen m_Screen;

        public void Setup(string p_Text, bool p_RichText, Vector3 p_Position, Vector3 p_Rotation)
        {
            m_Screen = FloatingScreen.CreateFloatingScreen(new Vector2(40, 40), false, p_Position, Quaternion.Euler(p_Rotation));
            this.transform.SetParent(m_Screen.transform, false);
        }
    }
}
