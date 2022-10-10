using BeatSaberPlus.SDK.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SheepControl.UI.CustomComponents
{
    internal class CustomTextSegmentedControl
    {
        List<GameObject> m_Tabs = new List<GameObject>();

        internal HMUI.TextSegmentedControl m_TabManager;

        internal CustomTextSegmentedControl(RectTransform p_Parent, bool p_HideCellBackground, List<string> p_Texts, List<GameObject> m_Tabs)
        {
            m_TabManager = BeatSaberPlus.SDK.UI.TextSegmentedControl.Create(p_Parent, false, p_Texts.ToArray());
            m_TabManager.ReloadData();
            m_TabManager.didSelectCellEvent += (p_SegmentedControl, p_Index) => {
                for (int l_i = 0; l_i < m_TabManager.cells.Count;l_i++)
                {
                    if (m_Tabs.ElementAt(l_i) != null)
                    {
                        if (l_i == p_Index)
                            m_Tabs.ElementAt(l_i).gameObject.SetActive(true);
                        else
                            m_Tabs.ElementAt(l_i).gameObject.SetActive(false);
                    }

                }
            };
            foreach (var l_Current in m_Tabs)
                l_Current.gameObject.SetActive(false);

            if (m_Tabs.ElementAt(0) != null) m_Tabs.ElementAt(0).gameObject.SetActive(true);
        }

    }
}
