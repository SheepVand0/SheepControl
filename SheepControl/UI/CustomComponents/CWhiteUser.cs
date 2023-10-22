using BeatSaberPlus.SDK.Game;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using SheepControl.UI.Defaults;

namespace SheepControl.UI.CustomComponents
{
    internal class CWhiteUser : XUIHLayout
    {
        protected CWhiteUser(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static CWhiteUser Make()
        {
            return new CWhiteUser("SheepControlWhiteUser");
        }

        GSTextInput m_TextInput;
        GSSecondaryButton m_RemoveButton;

        private void OnCreation(CHOrVLayout p_Layout)
        {
            (m_TextInput = GSTextInput.Make("Name"))
                .OnValueChanged(OnChange)
                .BuildUI(Element.transform);
            (m_RemoveButton = GSSecondaryButton.Make("x", 5, 5, p_OnClick: OnRemove)).BuildUI(Element.transform);
            SetWidth(100);
        }

        string m_Name;

        private void OnChange(string p_Value)
        {
            for (int l_i = 0; l_i < SConfig.Instance.WhitelistNames.Capacity;l_i++)
            {
                if (SConfig.Instance.WhitelistNames[l_i] == m_Name)
                {
                    m_Name = p_Value;
                    SConfig.Instance.WhitelistNames[l_i] = m_Name;
                }
            }
            SConfig.Instance.Save();
        }

        private void OnRemove()
        {
            SConfig.Instance.WhitelistNames.Remove(m_Name);
            SConfig.Instance.Save();
            MainSettingsViewController.Instance.UpdateWhitelist();
        }

        public void SetName(string p_Name)
        {
            m_Name = p_Name;
            m_TextInput.SetValue(p_Name, false);
        }
    }
}
