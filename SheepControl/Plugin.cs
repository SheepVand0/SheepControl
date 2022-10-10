using IPA;
using System;
using System.Windows.Forms;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using BS_Utils.Utilities;
using SiraUtil.Zenject;
using Newtonsoft.Json;
using System.IO;
using HarmonyLib;
using System.Net;
using IPALogger = IPA.Logging.Logger;
using System.Collections.Generic;
using IPA.Utilities;
using SheepControl.Trucs;
using SheepControl.UI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.GameplaySetup;

namespace SheepControl
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        #region Init
        [Init]
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            Log.Info("SheepControl initialized.");

            SheepControl.m_Zenjector = zenjector;
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {
        }
        #endregion

    }
}
