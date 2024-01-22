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
            public string TimeString { get; set; } = "empty";
            public string Source { get; set; }
            public string Destination { get; set; }
            public string SourcePort { get; set; }
            public string DestinationPort { get; set; }
            public string Protocol { get; set; }
            public string FrameLength { get; set; }
            public string Info { get; set; }
            public string Data { get; set; }
            List<string> Protocols = new List<string>();

            public packetData(String err)
            {
                Info = err;

            }
            public packetData(JObject layersObject)
            {
                Data = JsonConvert.SerializeObject(layersObject, Newtonsoft.Json.Formatting.None);
                foreach (var layer in layersObject)
                {
                    Protocols.Add(layer.Key.ToString());
                    //Debug.WriteLine(layer.Key);
                }
                Number = Int32.Parse((string)layersObject[Protocols[0]][$"{Protocols[0]}_{Protocols[0]}_number"]);
                //frame_frame_number

                string caputureTime = (string)layersObject[Protocols[0]][$"{Protocols[0]}_{Protocols[0]}_time"];
                caputureTime = caputureTime.Substring(0, 27);
                //精度が高すぎるので落とす

                DateTime Time = DateTime.ParseExact(caputureTime, "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                Time = Time.AddHours(9);
                TimeString = Time.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF");


                string eSource = (string)layersObject[Protocols[1]][$"{Protocols[1]}_{Protocols[1]}_src"];
                string eDestination = (string)layersObject[Protocols[1]][$"{Protocols[1]}_{Protocols[1]}_dst"];
                //ethレベルのアドレス（MACアドレス）

                Source = (string)layersObject[Protocols[2]][$"{Protocols[2]}_{Protocols[2]}_src"];
                Destination = (string)layersObject[Protocols[2]][$"{Protocols[2]}_{Protocols[2]}_dst"];

                if (Source == null)
                {
                    Source = eSource;
                    Destination = eDestination;
                }
                if (Protocols.Contains("tcp") || Protocols.Contains("udp"))
                {
                    SourcePort = (string)layersObject[Protocols[3]][$"{Protocols[3]}_{Protocols[3]}_srcport"];
                    DestinationPort = (string)layersObject[Protocols[3]][$"{Protocols[3]}_{Protocols[3]}_dstport"];
                    Info += $"{SourcePort} → {DestinationPort}";
                }

                if (Protocols.Last().Equals("data"))
                {
                    Protocol = Protocols[Protocols.Count - 2];
                }
                else
                {
                    Protocol = Protocols.Last();
                }
                Protocol = Protocol.ToUpper();

                FrameLength = (string)layersObject[Protocols[0]][$"{Protocols[0]}_{Protocols[0]}_len"];
                Info += $" {Protocols.Last()}";
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
