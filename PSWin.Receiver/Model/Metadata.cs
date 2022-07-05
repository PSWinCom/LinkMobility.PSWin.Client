using System.Collections.Generic;

namespace LinkMobility.PSWin.Receiver.Model
{
    public class Metadata
    {
        public Metadata(Dictionary<string, string> data)
        {
            Data = data;
        }

        public string TimeStamp => Get("TIMESTAMP");

        public string Reference => Get("REFERENCE");

        public IReadOnlyDictionary<string, string> Data { get; }

        private string Get(string key, string defaultValue = null)
        {
            if (Data.ContainsKey(key))
                return Data[key];
            return defaultValue;
        }
    }
}