using MusicDownloader.Models;
using MusicPlayerClient.Core;
using MusicPlayerClient.Services;
using MusicPlayerData.DataEntities;
using MusicPlayerClient.Stores;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class DownloadSongOnYoutubeAsyncCommand : AsyncCommandBase
    {
        private readonly IYouTubeClientService _youtubeClient;
        private readonly ObservableCollection<YoutubeVideoInfoModel> _observableMedia;
        private readonly MediaStore _mediaStore;

        public DownloadSongOnYoutubeAsyncCommand(
            IYouTubeClientService youtubeClient,
            ObservableCollection<YoutubeVideoInfoModel> observableMedia,
            MediaStore mediaStore)
        {
            _youtubeClient = youtubeClient;
            _observableMedia = observableMedia;
            _mediaStore = mediaStore;

            PreventClicksWhileExecuting = false;
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            if (parameter is string url)
            {
                var dir = Directory.CreateDirectory("downloads\\");
                YoutubeVideoInfoModel? video = _observableMedia.FirstOrDefault(x => x.Url == url);
                var fileName = Path.Combine(dir.FullName, video?.Title + ".mp3");

                if (video != null && !video.Downloading)
                {
                    try
                    {
                        video.FinishedDownload = false;
                        video.Downloading = true;

                        var download = _youtubeClient.DownloadYoutubeAudioAsync(video.Url!, fileName);
                        await foreach (var progress in download)
                        {
                            video.DownloadProgress = progress;
                        }

                        video.FinishedDownload = true;

                        // 🎯 MediaEntity olarak ekle
                        var media = new MediaEntity
                        {
                            Title = video.Title ?? "İsimsiz",
                            FilePath = fileName,
                            CreatedAt = DateTime.Now
                        };

                        await _mediaStore.Add(media);
                    }
                    catch
                    {
                        video.FinishedDownload = default;
                        video.Downloading = default;
                        video.DownloadProgress = default;
                        File.Delete(fileName);
                    }
                }
                else if (video != null && video.FinishedDownload == true)
                {
                    string argument = "/select, \"" + fileName + "\"";
                    Process.Start("explorer.exe", argument);
                }
            }
        }
    }
}
