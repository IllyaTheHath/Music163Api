using System;
using System.Runtime.Serialization;

namespace NeteaseCloudMusicTool.DataClass
{
    [DataContract]
    public class SongDetail
    {
        [DataMember(Order = 0)]
        public Songs[] songs { get; set; }

        [DataMember(Order = 1)]
        public String code { get; set; }
    }

    [DataContract]
    public class Songs
    {
        [DataMember(Order = 0)]
        public String name { get; set; }

        [DataMember(Order = 1)]
        public Artists[] artists { get; set; }

        [DataMember(Order = 2)]
        public Album album { get; set; }

        private String _duration;
        [DataMember(Order = 3)]
        public String duration
        {
            get
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, Int32.Parse(this._duration));
                return ts.Hours * 60 + ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds;
            }
            set => this._duration = value;
        }
    }

    [DataContract]
    public class Artists
    {
        [DataMember(Order = 0)]
        public String name { get; set; }
    }

    [DataContract]
    public class Album
    {
        [DataMember(Order = 0)]
        public String name { get; set; }
    }
}
