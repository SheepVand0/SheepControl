using UnityEngine;
using System.Reflection;
using System.Linq;
using TMPro;

namespace SheepControl.Bundle
{
    public class ObamiumLoader
    {
        public static GameObject LoadObamium()
        {
            AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("SheepControl.Bundle.troll_bundle"));
            GameObject l_Obamium = l_Bundle.LoadAsset<GameObject>("Obamium.prefab");
            l_Bundle.Unload(false);
            return l_Obamium;
        }

        static AssetBundle m_Bundle;

        public static AssetBundle LoadBundle()
        {
            if (m_Bundle == null)
                m_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("SheepControl.Bundle.sheepcontrol"));
            return m_Bundle;
        }

        public static T GetElement<T>(string p_Name) where T : Object
        {
            AssetBundle l_Bundle = LoadBundle();
            return (T)l_Bundle.LoadAsset(p_Name);
        }
    }
}
