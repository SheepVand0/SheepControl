using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace SheepControl.API
{
    class ScoreSaberAPI
    {

        public static ScoreSaberBasicPlayerStruct GetPlayer(string p_Id)
        {
            try
            {
                using (HttpClient l_Client = new HttpClient())
                {
                    Task<string> l_Result = l_Client.GetStringAsync($"https://www.scoresaber.com/api/player/{p_Id}/basic");
                    l_Result.Wait();
                    ScoreSaberBasicPlayerStruct l_Player = JsonConvert.DeserializeObject<ScoreSaberBasicPlayerStruct>(l_Result.Result);
                    return l_Player;
                }
            } catch (Exception l_E)
            {
                Plugin.Log.Error($"Error during getting player {l_E}");
                return new ScoreSaberBasicPlayerStruct();
            }
        }

    }

    public struct ScoreSaberBasicPlayerStruct
    {
        public string id { get; set; }
        public string name { get; set; }
        public string profilePicture { get; set; }
        public string country { get; set; }
        public float pp { get; set; }
        public int rank { get; set; }
        public int countryRank { get; set; }
        public string role { get; set; }
        public List<CustomBadge> badges { get; set; }
        public string histories { get; set; }
        public object scoreStats { get; set; }
        public int permissions { get; set; }
        public bool banned { get; set; }
        public bool inactive { get; set; }
    }

    public struct CustomBadge
    {
        public string description { get; set; }
        public string image { get; set; }
    }

    public struct PlayerScoreStat
    {
        public int totalScore { get; set; }
        public int totalRankedScore { get; set; }
        public float averageRankedAccuracy { get; set; }
        public int totalPlayCount { get; set; }
        public int rankedPlayCount { get; set; }
        public int replaysWatched { get; set; }
    }
}
