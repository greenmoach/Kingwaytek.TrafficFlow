using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using Kingwaytek.TrafficFlow.Models;
using Kingwaytek.TrafficFlow.Repositories;

namespace Kingwaytek.TrafficFlow
{
    public class PositioningService
    {
        private readonly string BaseAddress = "https://gistraffic.tycg.gov.tw";

        private readonly string GetTownsUri = "/TytfmWeb/Content/data.json";

        private readonly string GetByTownUri = "/TytfmWeb/api/SelectAPI/GetIntnodelist1?town={0}";

        private readonly string GetByIntersectionUri = "/TytfmWeb/api/SelectAPI/GetIntnodelist2?town={0}&rn1={1}";

        private readonly string PositioningUri = "/TytfmWeb/api/SelectAPI/GetIntnodeposition?town={0}&rn1={1}&rn2={2}";

        private readonly PositioningRepository _positioningRepository;

        public PositioningService()
        {
            _positioningRepository = new PositioningRepository();
        }

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
            var position = DoPositioning(town, road1, road2);
            if (position != null)
            {
                return position;
            }

            position = DoPositioning(town, road2, road1);
            return position;
        }

        private PositioningDto DoPositioning(string town, string road1, string road2)
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

        /// <summary>
        /// 取得定位點的路口定位資料
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string DirectionPositioning(int positionId, decimal latitude, decimal longitude, InvestigationTypeEnum type)
        {
            var entity = _positioningRepository.GetAvailable()
                                               .FirstOrDefault(x => x.Id == positionId && x.InvestigationType == (int)type);
            if (entity != null)
            {
                return entity.Positioning1;
            }

            switch (type)
            {
                case InvestigationTypeEnum.TRoad:
                    return CreateDirectionPositioningWithTRoad(latitude, longitude);

                case InvestigationTypeEnum.Intersection:
                case InvestigationTypeEnum.Pedestrians:
                    return CreateDirectionPositioningWithIntersection(latitude, longitude);

                case InvestigationTypeEnum.FiveWay:
                    return CreateDirectionPositioningWithFiveWay(latitude, longitude);

                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// T自路口車輛調查定位
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private string CreateDirectionPositioningWithTRoad(decimal latitude, decimal longitude)
        {
            var directViewModel = new DirectViewModel
            {
                Center = new DirectPositionViewModel
                {
                    Id = "center",
                    Latitude = latitude,
                    Longitude = longitude
                },
                Directions = new List<DirectPositionViewModel>
                {
                    new DirectPositionViewModel
                    {
                        Id = "A",
                        Latitude = latitude,
                        Longitude = longitude + 0.00015m,
                        Rotate = 270
                    },
                    new DirectPositionViewModel
                    {
                        Id = "B",
                        Latitude = latitude - 0.00015m,
                        Longitude = longitude,
                        Rotate = 0
                    },
                    new DirectPositionViewModel
                    {
                        Id = "C",
                        Latitude = latitude,
                        Longitude = longitude - 0.00015m,
                        Rotate = 90
                    },
                }
            };
            return directViewModel.ToJson();
        }

        /// <summary>
        /// 十字路口調查定位
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private string CreateDirectionPositioningWithIntersection(decimal latitude, decimal longitude)
        {
            var directViewModel = new DirectViewModel
            {
                Center = new DirectPositionViewModel
                {
                    Id = "center",
                    Latitude = latitude,
                    Longitude = longitude
                },
                Directions = new List<DirectPositionViewModel>
                {
                    new DirectPositionViewModel
                    {
                        Id = "A",
                        Latitude = latitude,
                        Longitude = longitude + 0.00015m,
                        Rotate = 270
                    },
                    new DirectPositionViewModel
                    {
                        Id = "B",
                        Latitude = latitude - 0.00015m,
                        Longitude = longitude,
                        Rotate = 0
                    },
                    new DirectPositionViewModel
                    {
                        Id = "C",
                        Latitude = latitude,
                        Longitude = longitude - 0.00015m,
                        Rotate = 90
                    },
                    new DirectPositionViewModel
                    {
                        Id = "D",
                        Latitude = latitude + 0.00015m,
                        Longitude = longitude,
                        Rotate = 180
                    }
                }
            };
            return directViewModel.ToJson();
        }

        /// <summary>
        /// 五向路口車輛調查定位
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private string CreateDirectionPositioningWithFiveWay(decimal latitude, decimal longitude)
        {
            var directViewModel = new DirectViewModel
            {
                Center = new DirectPositionViewModel
                {
                    Id = "center",
                    Latitude = latitude,
                    Longitude = longitude
                },
                Directions = new List<DirectPositionViewModel>
                {
                    new DirectPositionViewModel
                    {
                        Id = "A",
                        Latitude = latitude,
                        Longitude = longitude + 0.00015m,
                        Rotate = 270
                    },
                    new DirectPositionViewModel
                    {
                        Id = "B",
                        Latitude = latitude - 0.0001m,
                        Longitude = longitude + 0.00013m,
                        Rotate = 300
                    },
                    new DirectPositionViewModel
                    {
                        Id = "C",
                        Latitude = latitude- 0.00015m,
                        Longitude = longitude - 0.00013m,
                        Rotate = 30
                    },
                    new DirectPositionViewModel
                    {
                        Id = "D",
                        Latitude = latitude,
                        Longitude = longitude-0.00015m,
                        Rotate = 90
                    },
                    new DirectPositionViewModel
                    {
                        Id = "E",
                        Latitude = latitude + 0.00015m,
                        Longitude = longitude,
                        Rotate = 180
                    }
                }
            };
            return directViewModel.ToJson();
        }
    }
}