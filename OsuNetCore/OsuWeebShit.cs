using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace OsuNetCore
{
    public class OsuWeebShit
    {
        public static string GetRecentPlaysByUser(string user)
        {
            string data = makeApiCall("get_user_recent", "u=" + user, "limit=5");
            string dataToFormat = serializeJson(data);
            return dataToFormat;
        }

        private static string serializeJson(string data)
        {
            string returnstring = String.Empty;

            var list = JsonConvert.DeserializeObject<List<RecentOsu>>(data);

            //TODO reine serializer Klasse draus machen für alle API Calls
            foreach (RecentOsu item in list)
            {
                BeatmapCall beatmap = getMapNameById(item.beatmap_id);

                returnstring += beatmap.title +"<https://osu.ppy.sh/beatmapsets/" + beatmap.beatmapset_id + "#osu/" + beatmap.beatmap_id + ">" + "  -  " + beatmap.difficultyrating + " Sterne. \n";
            }
            //dynamic newdata = JsonConvert.DeserializeObject(data);

            return returnstring;
        }

        public static BeatmapCall getMapNameById(int mapID)
        {
            string mapname = string.Empty;
            string json = makeApiCall("get_beatmaps", "b=" + mapID.ToString());
            var jsonList = JsonConvert.DeserializeObject<List<BeatmapCall>>(json);
            foreach (BeatmapCall item in jsonList)
            {
                return item;
            }
            return new BeatmapCall();
        }


        public static string makeApiCall(string requestMode, string ID = "", string parameters = "")
        {
            string urlAddress = "https://osu.ppy.sh/api/" + requestMode + "?k=86ae1b8a0766f91dddfa8f1f5bb3e4b7f56122d9&" + ID + "&" + parameters;
            string data = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }

            return data;
        }

    }

    internal class RecentOsu
    {
        public int beatmap_id;
        public int score;
        public int count50;
        public int count100;
        public int count300;
        public int countmiss;
        public int perfect;
        public int enabled_mods;
        public DateTime date;
        public char rank;
    }

    public class BeatmapCall
    {
        public int beatmapset_id;
        public int beatmap_id;
        public string title;
        public float difficultyrating;
    }
}
