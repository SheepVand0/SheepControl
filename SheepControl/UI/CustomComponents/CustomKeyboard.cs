using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepControl.UI.CustomComponents
{
    public class CustomKeyboard : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return $"SheepControl.UI.Views.{GetType().Name}.bsml";
        }

        [UIComponent("KeyboardModal")] ModalKeyboard m_ModalKeyboard = null;
        [UIValue("InputKeyboardValue")] private string m_KeyboardValue { get => string.Empty; set { EnterPressed(value); } }

        public event Action<string, string, string> OnKeyboardEnterPressed;

        public string m_OldValue = string.Empty;
        public string m_Item = string.Empty;
        public void Open(string p_OpenText, string p_Item)
        {
            m_ModalKeyboard.modalView.Show(true, true);
            m_ModalKeyboard.SetText(p_OpenText);
            m_OldValue = p_OpenText;
            m_Item = p_Item;
        }

        private void EnterPressed(string p_Value)
        {
            OnKeyboardEnterPressed?.Invoke(m_Item, m_OldValue, p_Value);
            m_OldValue = p_Value;
        }
    }
}
