using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Kingwaytek.TrafficFlow
{
    public class PositioningService
    {
        private readonly string BaseAddress = "https://gistraffic.tycg.gov.tw";

        private readonly string GetTownsUri = "/TytfmWeb/Content/data.json";

        private readonly string GetByTownUri = "/TytfmWeb/api/SelectAPI/GetIntnodelist1?town={0}";

        private readonly string GetByIntersectionUri = "/TytfmWeb/api/SelectAPI/GetIntnodelist2?town={0}&rn1={1}";

        private readonly string PositioningUri = "/TytfmWeb/api/SelectAPI/GetIntnodeposition?town={0}&rn1={1}&rn2={2}";

        /// <summary>
        /// 取得桃園鄉鎮區清單
        /// </summary>
        /// <returns></returns>
        public List<PositioningTownDto> GetTowns()
        {
            var models = new List<PositioningTownDto>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(GetTownsUri).Result;
                if (response.IsSuccessStatusCode == false)
                {
                    return models;
                }
                var content = response.Content.ReadAsStringAsync().Result;

                if (content.IsNullOrEmpty())
                {
                    return models;
                }

                models = content.ToTypedObject<List<PositioningTownDto>>();
                return models;
            }
        }

        /// <summary>
        /// 根據鄉鎮名稱取得區域內的路名清單
        /// </summary>
        /// <param name="town">鄉鎮名稱</param>
        /// <returns>路名清單</returns>
        public List<string> GetRoadsByTown(string town)
        {
            var models = new List<string>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Format(GetByTownUri, town)).Result;
                if (response.IsSuccessStatusCode == false)
                {
                    return models;
                }
                var content = response.Content.ReadAsStringAsync().Result;

                if (content.IsNullOrEmpty())
                {
                    return models;
                }

                models = content.ToTypedObject<List<string>>();
                return models;
            }
        }

        /// <summary>
        /// 根據路名取得交叉路口
        /// </summary>
        /// <param name="town">town name</param>
        /// <param name="road">road name</param>
        /// <returns>交叉路口清單</returns>
        public List<string> GetRoadsByIntersection(string town, string road)
        {
            var models = new List<string>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Format(GetByIntersectionUri, town, road)).Result;
                if (response.IsSuccessStatusCode == false)
                {
                    return models;
                }
                var content = response.Content.ReadAsStringAsync().Result;

                if (content.IsNullOrEmpty())
                {
                    return models;
                }

                models = content.ToTypedObject<List<string>>();
                return models;
            }
        }

        /// <summary>
        /// 交叉路口定位
        /// </summary>
        /// <param name="town">town name</param>
        /// <param name="road1">the one road of intersection</param>
        /// <param name="road2">the other road of intersection</param>
        /// <returns>positioning data</returns>
        public PositioningDto GetPositioningByIntersection(string town, string road1, string road2)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Format(PositioningUri, town, road1, road2)).Result;
                if (response.IsSuccessStatusCode == false)
                {
                    return null;
                }
                var content = response.Content.ReadAsStringAsync().Result;

                if (content.IsNullOrEmpty())
                {
                    return null;
                }

                var model = content.ToTypedObject<PositioningDto>();
                return model;
            }
        }
    }
}