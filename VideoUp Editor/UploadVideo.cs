/*
 * Copyright 2015 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using VideoUp;
using System.Windows.Forms;

namespace Google.Apis.YouTube.Samples
{
    internal class UploadVideo
    {
        private string videoTitle, videoDesc, videoPath;
        private bool finished = false;

        public void passValues(string title, string desc, string path)
        {
            videoTitle = title;
            videoDesc = desc;
            videoPath = path;
        }

        [STAThread]
        public void startUpload()
        {
            try
            {
                //new UploadVideo().Run(videoTitle, videoDesc, videoPath).Wait(); // leads to deadlock
                new UploadVideo().Run(videoTitle, videoDesc, videoPath).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        private async Task Run(string title, string desc, string path)
        {
            UserCredential credential;
      
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None
            );
          }

          var youtubeService = new YouTubeService(new BaseClientService.Initializer()
          {
            HttpClientInitializer = credential,
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
          });

          var video = new Video();
          video.Snippet = new VideoSnippet();
          video.Snippet.Title = title;
          video.Snippet.Description = desc;
          video.Snippet.Tags = new string[] {"deaf community of cape town", "dcct", "deaf community", "heathfield"};
          video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
          video.Status = new VideoStatus();
          video.Status.PrivacyStatus = "unlisted";
          var filePath = path;

        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
            videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

            await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                  //MessageBox.Show(string.Format("{0:0} MB sent.", progress.BytesSent * 0.000001));
                  //sendResponse(string.Format("{0:0} MB sent.", progress.BytesSent * 0.000001));
                  break;

                case UploadStatus.Failed:
                  //MessageBox.Show(string.Format("An error prevented the upload from completing.\n{0}", progress.Exception));
                  //sendResponse(string.Format("An error prevented the upload from completing.\n{0}", progress.Exception));
                  break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            MessageBox.Show("The video '" + videoTitle + "' was successfully uploaded");
            finished = true;
            //sendResponse("The video '" + videoTitle + "' was successfully uploaded");
        }
    }
}
