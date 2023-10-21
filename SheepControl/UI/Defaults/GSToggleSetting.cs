using CP_SDK.UI.Components;
using CP_SDK.XUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheepControl.UI.Defaults
{
    internal class GSToggleSetting : XUIToggle
    {
        protected GSToggleSetting(string p_Name) : base(p_Name)
        {
            OnReady(OnCreation);
            OnValueChanged(OnChange);
        }

        public static new GSToggleSetting Make()
        {
            return new GSToggleSetting("SheepControlToggle");
        }

        private void OnCreation(CToggle p_Toggle)
        {
            
        }

        private void OnChange(bool p_Value)
        {

        } 

    }
}
