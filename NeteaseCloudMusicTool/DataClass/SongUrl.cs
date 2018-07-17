using System;
using System.Runtime.Serialization;

namespace NeteaseCloudMusicTool.DataClass
{
    [DataContract]
    public class SongUrl
    {
        [DataMember(Order = 0)]
        public Data[] data { get; set; }

        [DataMember(Order = 1)]
        public String code { get; set; }
    }

    [DataContract]
    public class Data
    {
        [DataMember(Order = 0)]
        public String url { get; set; }

        [DataMember(Order = 1)]
        public String br { get; set; }
    }
}
