﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SheepControl.Utils
{
    internal class AssemblyUtils
    {

        public static Texture2D LoadTextureFromAssembly(string p_Path)
        {
            Texture2D l_Texture = new Texture2D(10, 10);
            byte[] l_Bytes = LoadFileFromAssembly(p_Path);

            l_Texture.LoadImage(l_Bytes);
            return l_Texture;
        }

        public static byte[] LoadFileFromAssembly(string p_path)
        {
            Assembly l_Assembly = Assembly.GetExecutingAssembly();
            var l_Stream = l_Assembly.GetManifestResourceStream(p_path);

            byte[] l_Bytes = new byte[l_Stream.Length];

            l_Stream.Read(l_Bytes, 0, (int)l_Stream.Length);

            return l_Bytes;
        }
    }
}
