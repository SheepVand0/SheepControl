using CP_SDK.UI.Components;
using CP_SDK.XUI;
using SheepControl.UI.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepControl.UI.CustomComponents
{
    internal class CBannedCommand : XUIHLayout
    {
        protected CBannedCommand(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static CBannedCommand Make()
        {
            return new CBannedCommand("SheepControlBannedCommand");
        }

        GSTextInput m_TextInput;
        GSSecondaryButton m_RemoveButton;

        private void OnCreation(CHOrVLayout p_Layout)
        {
            (m_TextInput = GSTextInput.Make("Name"))
                .OnValueChanged(OnChange)
                .BuildUI(Element.LElement.transform);
            (m_RemoveButton = GSSecondaryButton.Make("x", 5, 5, p_OnClick: OnRemove)).BuildUI(Element.LElement.transform);
            SetWidth(100);
        }

        string m_Name;

        private void OnChange(string p_Value)
        {
            for (int l_i = 0; l_i < SConfig.Instance.BannedCommands.Capacity; l_i++)
            {
                if (SConfig.Instance.BannedCommands[l_i] == m_Name)
                {
                    m_Name = p_Value;
                    SConfig.Instance.BannedCommands[l_i] = m_Name;
                }
            }
            SConfig.Instance.Save();
        }

        private void OnRemove()
        {
            SConfig.Instance.BannedCommands.Remove(m_Name);
            MainSettingsViewController.Instance.UpdateBannedCommands();
        }

        public void SetCommand(string p_Name)
        {
            m_Name = p_Name;
            m_TextInput.SetValue(p_Name, false);
        }
    }
}
