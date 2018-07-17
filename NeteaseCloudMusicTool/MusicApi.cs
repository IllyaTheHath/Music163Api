using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusicTool
{
    /// <summary>  
    /// 类名称 : MusicApi
    /// 描述 : 网易云音乐API
    /// 作者 : IllyaTheHath
    /// 创始时间 : 2017-07-26 23:54:25
    /// 最后修改人 : IllyaTheHath
    /// 最后修改时间 : 2017-09-11 21:16:03
    /// 版本 : version 2.2
    /// 功能实现 : 
    ///		网易云音乐API
    /// 版权所有 (C) : IllyaTheHath
    /// </summary>
    public static class MusicApi
    {
        private static Int32 count = 0;

        #region 同步方法
        /// <summary>
        /// 获取歌曲详细信息
        /// </summary>
        /// <param name="ids">歌曲id</param>
        /// <returns>从服务器上接收到的数据</returns>
        public static String GetDetial(String ids)
        {
            try { return GetDetialApi("[" + ids + "]"); }
            catch (WebException) { throw; }
        }

        /// <summary>
        /// 获取歌曲详细信息API
        /// </summary>
        /// <param name="ids">歌曲id</param>
        /// <returns>从服务器上接收到的数据</returns>
        private static String GetDetialApi(String ids)
        {
            using (WebClient client = new WebClient())
            {
                // 必要的HTTP头
                client.Headers.Add("Cookie", "appver=1.5.2;");
                client.Headers.Add("Referer", "http://music.163.com/");
                client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.152 Safari/537.36");

                String url = "http://music.163.com/api/song/detail/";
                client.QueryString.Add("ids", ids);
                client.QueryString.Add("csrf_token", "");

                try {
                    var responsebytes = client.DownloadData(url);
                    return Encoding.UTF8.GetString(responsebytes);
                }
                catch (WebException e) {  throw new WebException("No Internet Exception", e); }
            }
        }

        /// <summary>
        /// 获取歌曲链接
        /// </summary>
        /// <param name="id">歌曲id</param>
        /// <param name="br">歌曲比特率</param>
        /// <returns>从服务器上接收到的数据</returns>
        public static String GetUrl(String id, String br)
        {
            if (count >= 9)
            {
                count = 0;
                throw new Exception("Error No Response");
            }

            String value = "{\"ids\": \"[" + id + "]\", \"br\": \"" + br + "\"}";
            String data = Encrypt.EncryptedRequest(value);

            String[] words = data.Split('\n');
            String result = String.Empty;

            try { result = GetUrlApi(words[0], words[1]); }
            catch (WebException) { throw; }

            if (result == String.Empty || result == null)
            {
                count++;
                return GetUrl(id, br);
            }
            else
            {
                count = 0;
                return result;
            }
        }

        /// <summary>
        /// 获取歌曲链接API
        /// </summary>
        /// <param name="rparams">加密后的数据</param>
        /// <param name="encSeckey">加密后的数据</param>
        /// <returns>从服务器上接收到的数据</returns>
        private static String GetUrlApi(String rparams, String encSeckey)
        {
            using (WebClient client = new WebClient())
            {
                // 必要的HTTP头
                client.Headers.Add("Cookie", "appver=1.5.2;");
                client.Headers.Add("Referer", "http://music.163.com/");
                client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.152 Safari/537.36");

                String url = "http://music.163.com/weapi/song/enhance/player/url?csrf_token=";
                var reqparm = new NameValueCollection();
                reqparm.Add("params", rparams);
                reqparm.Add("encSecKey", encSeckey);
                try {
                    var responsebytes = client.UploadValues(url, "POST", reqparm);
                    return Encoding.UTF8.GetString(responsebytes);
                }
                catch (WebException e) { throw new WebException("No Internet Exception", e); }
            }
        }
        #endregion

        #region 异步方法
        /// <summary>
        /// 异步获取歌曲详细信息
        /// </summary>
        /// <param name="ids">歌曲id</param>
        /// <returns>从服务器上接收到的数据</returns>
        public static async Task<String> GetDetialAsync(String ids)
        {
            try { return await GetDetialApiAsync("[" + ids + "]"); }
            catch (WebException) { throw; }
        }

        /// <summary>
        /// 异步获取歌曲详细信息API
        /// </summary>
        /// <param name="ids">歌曲id</param>
        /// <returns>从服务器上接收到的数据</returns>
        private async static Task<String> GetDetialApiAsync(String ids)
        {
            using (WebClient client = new WebClient())
            {
                // 必要的HTTP头
                client.Headers.Add("Cookie", "appver=1.5.2;");
                client.Headers.Add("Referer", "http://music.163.com/");
                client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.152 Safari/537.36");

                String url = "http://music.163.com/api/song/detail/";
                client.QueryString.Add("ids", ids);
                client.QueryString.Add("csrf_token", "");

                try {
                    var responsebytes =  await client.DownloadDataTaskAsync(url);
                    return Encoding.UTF8.GetString(responsebytes);
                }
                catch (WebException e) { throw new WebException("No Internet Exception", e); }
            }
        }

        /// <summary>
        /// 异步获取歌曲链接
        /// </summary>
        /// <param name="id">歌曲id</param>
        /// <param name="br">歌曲比特率</param>
        /// <returns>从服务器上接收到的数据</returns>
        public static async Task<String> GetUrlAsync(String id, String br)
        {
            if (count >= 9)
            {
                count = 0;
                throw new Exception("Error No Response");
            }

            String value = "{\"ids\": \"[" + id + "]\", \"br\": \"" + br + "\"}";
            String data = Encrypt.EncryptedRequest(value);

            String[] words = data.Split('\n');
            String result = String.Empty;

            try { result = await GetUrlApiAsync(words[0], words[1]); }
            catch (WebException) { throw; }

            if (result == String.Empty)
            {
                count++;
                return await GetUrlAsync(id, br);
            }
            else
            {
                count = 0;
                return result;
            }
        }

        /// <summary>
        /// 异步获取歌曲链接API
        /// </summary>
        /// <param name="rparams">加密后的数据</param>
        /// <param name="encSeckey">加密后的数据</param>
        /// <returns>从服务器上接收到的数据</returns>
        private static async Task<String> GetUrlApiAsync(String rparams, String encSeckey)
        {
            using (WebClient client = new WebClient())
            {
                // 必要的HTTP头
                client.Headers.Add("Cookie", "appver=1.5.2;");
                client.Headers.Add("Referer", "http://music.163.com/");
                client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.152 Safari/537.36");

                String url = "http://music.163.com/weapi/song/enhance/player/url?csrf_token=";
                var reqparm = new NameValueCollection();
                reqparm.Add("params", rparams);
                reqparm.Add("encSecKey", encSeckey);
                try {
                    var responsebytes = await client.UploadValuesTaskAsync(url, "POST", reqparm);
                    return Encoding.UTF8.GetString(responsebytes);
                }
                catch (WebException e) { throw new WebException("No Internet Exception", e); }
            }
        }
        #endregion

        /* 
         * 旧api，url，get http://music.163.com/api/song/enhance/download/url?br=320000&id=591542
         * 旧api，歌曲详情，get http://music.163.com/api/song/detail/?id=591542&ids=[591542]&csrf_token=
         * 参数
         *   id: 歌曲id,591542
         *   ids: 歌曲id,[591542]
         *   csrf_token: 非关键操作，值可为空
         *
         * 新api，歌曲详情，post http://music.163.com/weapi/v3/song/detail
         * 新api，歌词，post http://music.163.com/weapi/song/lyric?csrf_token=
         */
    }
}
