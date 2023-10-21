using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace SheepControl.UI.CustomComponents
{
    /*internal class CustomStringSetting : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return $"SheepControl.UI.Views.{GetType().Name}.bsml";
        }

        [UIObject("EditButtonTransform")] GameObject m_EditButtonTransform = null;
        [UIComponent("HorizontalLayout")] HorizontalLayoutGroup m_HorizontalLayout = null;
        [UIComponent("TextValuePreview")] TextMeshProUGUI m_PreviewText = null;

        Button m_EditButton = null;

        CustomKeyboard m_Keyboard = null;

        public event Action<string, string, string> OnChange;

        public int StringSettingMaxCharacters { get; private set; }
        public string Text { get; set; }
        public string Item { get; set; }

        public void SetValue(string p_Value, string p_Item)
        {
            Text = p_Value;
            m_PreviewText.text = p_Value;
            Item = p_Item;
        }

        public void Setup(string p_InitialValue,string p_Item ,int p_MaxCharacter, bool p_RecreateNewKeyboard, CustomKeyboard p_Keyboard = null)
        {
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_HorizontalLayout.gameObject, 0.4f);

            m_EditButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_EditButtonTransform.transform, "📝", OpenKeyboard, "Edit text");

            if (p_RecreateNewKeyboard)
                m_Keyboard = Create<CustomKeyboard>(this.transform, true);
            else if (p_Keyboard != null)
                m_Keyboard = p_Keyboard;
            else
                throw new Exception("Keyboard has to be non null to do a reference");

            m_Keyboard.OnKeyboardEnterPressed += (p_Item, p_OldValue, p_Value) => { if (p_OldValue == Text) ApplyValue(p_Value, p_Item); };

            StringSettingMaxCharacters = p_MaxCharacter;
            Text = p_InitialValue;
            m_PreviewText.text = Text;

            Item = p_Item;
        }

        public void OpenKeyboard()
        {
            m_Keyboard.Open(Text, Item);
        }

        public void ApplyValue(string p_Value, string p_Item)
        {
            if (p_Item != Item) return;
            m_PreviewText.text = CutString(p_Value, StringSettingMaxCharacters);
            OnChange?.Invoke(p_Item, Text, p_Value);
            Text = p_Value;
        }

        string CutString(string p_Value, int p_CharChounts)
        {
            if (p_Value.Length <= p_CharChounts)
                return p_Value;

            return p_Value.Substring(0, p_CharChounts) + "...";
        }
    }*/
}
