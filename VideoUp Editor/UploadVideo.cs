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

        /// <summary>
        /// Fetches the information about a video file from the MainForm.cs class
        /// </summary>
        /// <param name="title">the name of a video file.>/param>
        /// <param name="desc">the description of a video file.>/param>
        /// <param name="path">the directory path of a video file.>/param>
        public void passValues(string title, string desc, string path)
        {
            videoTitle = title;
            videoDesc = desc;
            videoPath = path;
        }

        /// <summary>
        /// Initiates the upload process
        /// </summary>
        [STAThread]
        public void startUpload()
        {
            try
            {
                //new UploadVideo().Run(videoTitle, videoDesc, videoPath).Wait(); // leads to deadlock
                new UploadVideo().Run(videoTitle, videoDesc, videoPath).ConfigureAwait(false); // handles deadlock
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Uploads the video to YouTube
        /// </summary>
        /// <param name="title">the name of a video file.>/param>
        /// <param name="desc">the description of a video file.>/param>
        /// <param name="path">the directory path of a video file.>/param>
        private async Task Run(string title, string desc, string path)
        {
            UserCredential credential;
      
            // developer key
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
          
          // information about the video that is uploaded to YouTube
          var video = new Video();
          video.Snippet = new VideoSnippet();
          video.Snippet.Title = title;
          video.Snippet.Description = desc;
          video.Snippet.Tags = new string[] {"deaf community of cape town", "dcct", "deaf community", "heathfield", "cape town"};
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

        /// <summary>
        /// Tracks the progress of the video file being uploaded
        /// </summary>
        /// <param name="progress">the progress of the video file.>/param>
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    MessageBox.Show(string.Format("{0:0} MB sent. \n Upload status: {1}", progress.BytesSent * 0.000001, progress.Status));
                    break;

                case UploadStatus.Failed:
                    MessageBox.Show("An error prevented the upload from completing.\nPlease try again.");
                    break;
            }
        }

        /// <summary>
        /// Returns the response of the video upload
        /// </summary>
        /// <param name="video">the response of the video once the upload is complete.>/param>
        void videosInsertRequest_ResponseReceived(Video video)
        {
            MessageBox.Show("The video upload is complete. Check your YouTube account.");
        }
    }
}
