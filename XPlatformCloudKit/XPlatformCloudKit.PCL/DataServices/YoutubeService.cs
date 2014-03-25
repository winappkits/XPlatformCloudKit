﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XPlatformCloudKit.Models;
using XPlatformCloudKit.Services;
using XPlatformCloudKit.Common;
using System.Diagnostics;

namespace XPlatformCloudKit.DataServices
{
    class YoutubeService : IDataService
    {
        HttpClient httpClient = new HttpClient();
        List<Item> YoutubeData;
        UrlSource currentYoutubeSource;

        public async Task<List<Item>> GetItems()
        {
            if(AppSettings.YoutubePublicAPIKey.Length < 30 && !AppSettings.EnableRemoteUrlSourceService)
                ServiceLocator.MessageService.ShowErrorAsync("YoutubeService is enabled but YoutubePublicAPIKey appears to be invalid please check this value in AppSettings.cs", "Application Error");

            try
            {
                YoutubeData = new List<Item>();

                foreach (var youtubeSource in AppSettings.YoutubeAddressCollection)
                {
                    currentYoutubeSource = youtubeSource;
                    await Parse(youtubeSource);
                }
            }
            catch
            {
                ServiceLocator.MessageService.ShowErrorAsync("Error when retrieving items from YoutubeService", "Application Error");
            }

            return YoutubeData;
        }

        public async Task Parse(UrlSource youtubeSource)
        {
            var _UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            httpClient.DefaultRequestHeaders.Add("user-agent", _UserAgent);

            var result = await httpClient.GetStringAsync(youtubeSource.Url);

            JObject jsonDat = JObject.Parse(result);

            string nextPageToken = null;

            try
            {
                nextPageToken = jsonDat["nextPageToken"].ToString();
            }
            catch
            {
                nextPageToken = null;
            }

            JArray jsonItems = JArray.Parse(jsonDat["items"].ToString());

            string youtubeHtmlTemplate = "<p><a href=\"{0}\"><img src=\"{1}\" alt=\"\" width=300></a></p><p><a style=\"font-size: 15px; font-weight: bold; font-decoration: none;\" href=\"{0}\">{2}</a></p><p>{3}</p>";

            string youtubePrefix;
            youtubePrefix = AppSettings.ForceYoutubeVideosToLoadFullScreen ? "/watch_popup?v=" : "/watch?v=";


            foreach (var youtubeItem in jsonItems)
            {
                YoutubeData.Add(new Item
                {
                    Title = youtubeItem["snippet"]["title"].ToString(),
                    Subtitle = youtubeItem["snippet"]["publishedAt"].ToString(),
                    Description = string.Format(youtubeHtmlTemplate, "https://www.youtube.com" + youtubePrefix + youtubeItem["snippet"]["resourceId"]["videoId"], youtubeItem["snippet"]["thumbnails"]["high"]["url"].ToString(), youtubeItem["snippet"]["title"].ToString(), youtubeItem["snippet"]["description"].ToString()),
                    Image = youtubeItem["snippet"]["thumbnails"]["medium"]["url"].ToString(),
                    Group = youtubeSource.Group
                });
            }

            //For some unknown reason, likely due to the Youtube API, this request breaks at index 130 of my test playlist YMMV
            if (nextPageToken != null)
                await Parse(new UrlSource {Url = currentYoutubeSource.Url + "&pageToken=" + nextPageToken, Group = currentYoutubeSource.Group});
        }

        /// <summary>
        /// Remove any whitespace or quotes from an YoutubeSource field.
        /// </summary>
        private string cleanField(string strFld)
        {
            return (strFld.Trim().Trim('"'));
        }

        /// <summary>
        /// Parse a string that should contain a URL/group name pair into an YoutubeSource object.
        /// </summary>
        private UrlSource stringToYoutubeSource(string str)
        {
            string[] fields = str.Split(',');

            if (fields.Length != 2)
                // Invalid remote Youtube source line.
                throw new FormatException("The following line is not a valid Youtube source line (invalid field count): " + str);

            string theUrl = cleanField(fields[0]);
            string theGroup = cleanField(fields[1]);

            if (String.IsNullOrWhiteSpace(theUrl))
                throw new FormatException("The following line is not a valid Youtube source line (URL field is empty): " + str);

            if (String.IsNullOrWhiteSpace(theGroup))
                throw new FormatException("The following line is not a valid Youtube source line (Group field is empty): " + str);

            return new UrlSource() { Url = theUrl, Group = theGroup };
        }
    }
}