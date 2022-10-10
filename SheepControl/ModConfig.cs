using CP_SDK.Config;
using Newtonsoft.Json;
using SheepControl.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace SheepControl
{
    internal class SConfig : JsonConfig<SConfig>
    {
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        //Config itself

        [JsonProperty] internal bool IsEnabled = true;

        [JsonProperty] internal bool AskForCommands = true;

        [JsonProperty] internal bool IsCommandsEnabledInGame = true;

        [JsonProperty] internal bool BobbyAutoRonde = true;

        [JsonProperty] internal float BobbyMoveDuration = 5.0f;

        [JsonProperty] internal float BobbyStealDuration = 3.0f;

        [JsonProperty] internal float BobbyTurnDuration = 1.0f;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] internal List<string> WhitelistNames = new List<string>() { };
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] internal List<string> BannedWords = new List<string>() { };
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] internal List<string> BannedCommands = new List<string>() { };
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] internal List<string> BannedQueries = new List<string>() { };


        internal static SConfig GetDefault()
        {
            return new SConfig();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        //Config Init

        public override string GetRelativePath() => $"{CP_SDK.ChatPlexSDK.ProductName}/Sheep/Config";

        protected override void OnInit(bool p_OnCreation)
        {
            if (WhitelistNames == null)
                WhitelistNames = new List<string>() { "sheepvand" };
            if (BannedWords == null)
                BannedWords = new List<string>() { "negro", "pute", "putain", "merde",
                        "hitler", "feur", "feuse", "pryd__", "pryd","bobby",
                        "uwu", "smh", "btw", "micheal jackson",   "dragonne"};
            if (BannedCommands == null)
                BannedCommands = new List<string>() { "deletegm" };

            if (BannedQueries == null)
                BannedQueries = new List<string>() { "rich" };

            Save();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal SConfig GetModSettings()
        {
            return this;
        }

        internal static SConfig GetStaticModSettings()
        {
            return Instance;
        }

        internal new void Reset()
        {
            WhitelistNames = null;
            BannedWords = null;
            BannedCommands = null;
            BannedQueries = null;
            OnInit(false);
        }

        public bool IsBannedCommand(string p_Command)
        {
            try
            {
                foreach (var l_Current in BannedCommands)
                {
                    if (l_Current != p_Command) continue;

                    return true;
                }
            }
            catch (Exception l_E)
            {
                Plugin.Log.Error("Error from here");
                Plugin.Log.Error($"{l_E}");
            }
            return false;
        }
        public bool IsBannedQuery(string p_Query)
        {
            try
            {
                foreach (var l_Current in BannedQueries)
                {
                    if (l_Current != p_Query) continue;

                    return true;
                }
            }
            catch (Exception l_E)
            {
                Plugin.Log.Error("Error from here");
                Plugin.Log.Error($"{l_E}");
            }
            return false;
        }


    }
}
