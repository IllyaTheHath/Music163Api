using Microsoft.Win32;
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

using NeteaseCloudMusicTool.DataClass;
using System.Windows.Controls;
using System.Windows.Media;

namespace NeteaseCloudMusicTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        String jsonDetail = String.Empty;
        String jsonUrl = String.Empty;
        String id = String.Empty;
        String br = "320000";

        //String sId = String.Empty;
        String sUrl = String.Empty;
        String sBr = String.Empty;

        String sName = String.Empty;
        //String sBitrate = String.Empty;
        String sArtist = String.Empty;
        String sAlbum = String.Empty;
        String sDuration = String.Empty;

        String directory = String.Empty;

        MediaPlayer player = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtId.Text == String.Empty)
            {
                MessageBox.Show("请输入ID", "网易云音乐工具");
                return;
            }
            
            if (txtId.Text.Trim().StartsWith("http") || txtId.Text.Trim().StartsWith("https"))
            {
                try
                {
                    id = GetIdFromLink(txtId.Text);
                }
                catch (Exception ee) when (String.Equals(ee.Message, "Wrong Url"))
                {
                    MessageBox.Show("歌曲链接格式错误，无法解析出歌曲id");
                    return;
                }
            }
            else
            {
                id = txtId.Text;
            }

            // 获取歌曲详情
            try
            {
                jsonDetail = await MusicApi.GetDetialAsync(id);
                // jsonDetial = MusicApi.GetDetial(id);
            }
            catch (WebException ee) when (String.Equals(ee.Message, "No Internet Exception"))
            {
                MessageBox.Show("无法连接到网络", "网易云音乐工具");
                return;
            }
            // 歌曲不存在
            if (jsonDetail == "{\"songs\":[],\"equalizers\":{},\"code\":200}")
            {
                MessageBox.Show("没有找到对应歌曲", "网易云音乐工具");
                return;
            }
            else if (jsonDetail == "{\"code\":400}")
            {
                MessageBox.Show("没有找到对应歌曲", "网易云音乐工具");
                return;
            }

            // 获取歌曲链接
            try {
                jsonUrl = await MusicApi.GetUrlAsync(id, br);
                // jsonUrl = MusicApi.GetUrl(id, br);
            }
            catch (WebException ee) when (String.Equals(ee.Message, "No Internet Exception"))
            {
                MessageBox.Show("无法连接到网络", "网易云音乐工具");
                return;
            }
            catch (Exception ee) when (String.Equals(ee.Message, "Error No Response"))
            {
                MessageBox.Show("向服务器请求数据失败，请重试", "网易云音乐工具");
                return;
            }

            // 解析歌曲信息
            SongDetail sdetail = JsonPrase.Prase<SongDetail>(jsonDetail);
            sName = sdetail.songs[0].name;
            sArtist = String.Empty;
            foreach (var artist in sdetail.songs[0].artists) { sArtist += artist.name + ","; }
            sArtist = sArtist.Substring(0,sArtist.Length-1);
            sAlbum = sdetail.songs[0].album.name;
            sDuration = sdetail.songs[0].duration;

            // 解析歌曲链接
            SongUrl surl = JsonPrase.Prase<SongUrl>(jsonUrl);
            sUrl = surl.data[0].url;
            sBr = surl.data[0].br;

            // 更新界面数据
            txbName.Text = sName;
            txbBitRate.Text = sBr;
            txbArtist.Text = sArtist;
            txbAlbum.Text = sAlbum;
            txbDuration.Text = sDuration;
            progressBar.Value = 0;
            if (String.Equals(sUrl, null)) { txbUrl.Text = "无法获取下载地址，歌曲可能受版权保护"; }
            else { txbUrl.Text = sUrl; }

            if (sUrl != null)
            {
                btnPlay.IsEnabled = true;
                btnPlay.Content = " 播放 ";
                player.Open(new Uri(sUrl));
                btnDownload.IsEnabled = true;
            }
        }

        private String GetIdFromLink(String text)
        {
            String start1 = "song?id=";
            String end1 = "&userid";
            String start2 = "song/";
            String end2 = "/?userid";
            String result = String.Empty;
            
            if (text.IndexOf(start1) >= 0)
            {
                var indexs = text.IndexOf(start1);
                var indexe = text.IndexOf(end1);
                result = text.Substring(indexs + start1.Length, indexe - (indexs + start1.Length)).Trim();
            }
            else if (text.IndexOf(start2) >= 0)
            {
                var indexs = text.IndexOf(start2);
                var indexe = text.IndexOf(end2);
                result = text.Substring(indexs + start2.Length, indexe - (indexs + start2.Length)).Trim();
            }
            else
            {
                throw new Exception("Wrong Url");
            }
            return result;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "mp3文件|*.mp3|wav文件|*.wav|m4a文件|*.m4a|所有文件|*.*";
            sfd.FileName = sArtist + " - " + sName;
            if (sfd.ShowDialog() == true)
            {
                directory = sfd.FileName;
                DownloadProgress();
            }
        }

        private void DownloadProgress()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // 进度条取消隐藏
                    progressBar.Visibility = Visibility.Visible;
                    // 进度条控制
                    client.DownloadProgressChanged +=
                        delegate (object sender, DownloadProgressChangedEventArgs e)
                        {
                            progressBar.Value = e.ProgressPercentage;
                            txbProgress.Text = "已下载" + progressBar.Value + "%";
                        };

                    // 下载完成事件
                    client.DownloadFileCompleted +=
                        delegate (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                        {
                            if (e.Error == null) { txbProgress.Text = "下载完成"; }
                            else { txbProgress.Text = "下载失败"; }
                            //MessageBox.Show("下载完成", "网易云音乐工具");
                        };
                    // 异步下载文件
                    client.DownloadFileAsync(new Uri(sUrl), directory);
                }
            }
            catch (WebException)
            {
                MessageBox.Show("网络错误", "网易云音乐工具");
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "网易云音乐工具");
                return;
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if ((String)((Button)sender).Content == " 暂停 ")
            {
                ((Button)sender).Content = " 播放 ";
                if (player.CanPause)
                {
                    player.Pause();
                }
            }
            else if((String)((Button)sender).Content == " 播放 ")
            {
                ((Button)sender).Content = " 暂停 ";
                player.Open(new Uri(sUrl));
                player.Play();
            }
        }
            
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow();
            aw.ShowDialog();
            //new AboutWindow().ShowDialog();
        }
    }
}
