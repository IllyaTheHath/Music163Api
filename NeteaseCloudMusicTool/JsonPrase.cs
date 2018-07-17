using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NeteaseCloudMusicTool
{
    /// <summary>  
    /// 对获取到的Json数据进行反序列化
    /// </summary>
    public static class JsonPrase
    {
        /// <summary>
        /// 对Json数据进行反序列化
        /// </summary>
        /// <typeparam name="T">序列化的类</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns>对Json反序列化后生成的类</returns>
        public static T Prase<T>(String jsonString)
        {
            //var serializer = new DataContractJsonSerializer(typeof(T));
            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                //return (T) serializer.ReadObject(mStream);
                return (T) new DataContractJsonSerializer(typeof(T)).ReadObject(mStream);
            }
        }
    } // class JsonPrase

}
