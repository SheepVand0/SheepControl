using UnityEngine;
using System.Reflection;
using System.Linq;

namespace SheepControl.Bundle
{
    public class ObamiumLoader
    {
        public static GameObject LoadObamium()
        {
            AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("SheepControl.Bundle.troll_bundle"));
            GameObject l_Obamium = l_Bundle.LoadAsset<GameObject>("Obamium");
            l_Bundle.Unload(false);
            return l_Obamium;
        }
    }
}
