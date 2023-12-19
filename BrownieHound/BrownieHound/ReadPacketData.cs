using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace BrownieHound
{
    public class ReadPacketData
    {
        public class packetData
        {
            public int Number { get; set; }
            public DateTime Time { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string sourcePort { get; set; }
            public string destinationPort { get; set; }
            public string Protocol { get; set; }
            public int frameLength { get; set; }
            public string Info { get; set; }
            public string Data { get; set; }
            List<string> protocols = new List<string>();

            public packetData(String err)
            {
                Info = err;

            }
            public packetData(JObject layersObject)
            {
                Data = JsonConvert.SerializeObject(layersObject, Newtonsoft.Json.Formatting.None);
                foreach (var layer in layersObject)
                {
                    protocols.Add(layer.Key.ToString());
                    //Debug.WriteLine(layer.Key);
                }
                Number = Int32.Parse((string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_number"]);
                //frame_frame_number

                string caputureTime = (string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_time"];
                caputureTime = caputureTime.Substring(0, 27);
                //精度が高すぎるので落とす

                Time = DateTime.ParseExact(caputureTime, "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                Time = Time.AddHours(9);


                string eSource = (string)layersObject[protocols[1]][$"{protocols[1]}_{protocols[1]}_src"];
                string eDestination = (string)layersObject[protocols[1]][$"{protocols[1]}_{protocols[1]}_dst"];
                //ethレベルのアドレス（MACアドレス）

                Source = (string)layersObject[protocols[2]][$"{protocols[2]}_{protocols[2]}_src"];
                Destination = (string)layersObject[protocols[2]][$"{protocols[2]}_{protocols[2]}_dst"];

                if (Source == null)
                {
                    Source = eSource;
                    Destination = eDestination;
                }
                if (protocols.Contains("tcp") || protocols.Contains("udp"))
                {
                    sourcePort = (string)layersObject[protocols[3]][$"{protocols[3]}_{protocols[3]}_srcport"];
                    destinationPort = (string)layersObject[protocols[3]][$"{protocols[3]}_{protocols[3]}_dstport"];
                    Info += $"{sourcePort} → {destinationPort}";
                }

                if (protocols.Last().Equals("data"))
                {
                    Protocol = protocols[protocols.Count - 2];
                }
                else
                {
                    Protocol = protocols.Last();
                }
                Protocol = Protocol.ToUpper();

                frameLength = Int32.Parse((string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_len"]);
                Info += $" {protocols.Last()}";
                //Debug.WriteLine($"{Number} : {time.TimeOfDay} : {Source} : {Destination} : {Protocol} : {Length} :: {Info}");
            }
        }

        public static packetData transfer(string msg)
        {
            packetData pd = null;
            try
            {
                JObject packetObject = JObject.Parse(msg);
                if (packetObject["layers"] != null)
                {
                    pd = new packetData((JObject)packetObject["layers"]);
                }
            }
            catch
            {
                pd = (new packetData(msg));
            }
            return pd;
        }
    }
}
