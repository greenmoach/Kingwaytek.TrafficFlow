using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingwaytek.TrafficFlow.Repositories;

namespace Kingwaytek.TrafficFlow
{
    public class InvestigateService
    {
        private const string TurnLeft = "左轉";
        private const string TurnRight = "右轉";
        private const string Straight = "直行";

        private readonly InvestigationRepository _investigationRepository;

        private readonly InvestigationDataRepository _investigationDataRepository;

        private readonly MemoryCacheProvider _cacheProvider;

        public InvestigateService()
        {
            _cacheProvider = new MemoryCacheProvider();
            _investigationRepository = new InvestigationRepository();
            _investigationDataRepository = new InvestigationDataRepository();
        }

        /// <summary>
        /// 根據編號取得一筆調查資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Investigation GetById(int id)
        {
            var entity = _investigationRepository.GetAvailable().FirstOrDefault(x => x.Id == id);
            return entity;
        }

        /// <summary>
        /// 調查資料建立或更新
        /// </summary>
        /// <param name="viewModel"></param>
        public void CreateOrUpdate(DataCreatedViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                var editEntity = _investigationRepository.GetAvailable().FirstOrDefault(x => x.Id == viewModel.Id);
                if (editEntity == null)
                {
                    return;
                }

                editEntity.InvestigationTypeEnum = viewModel.Type;
                editEntity.PositioningId = viewModel.PositioningId;
                editEntity.PositioningCity = viewModel.City;
                editEntity.PositioningTown = viewModel.Town;
                editEntity.PositioningRoad1 = viewModel.Road1;
                editEntity.PositioningRoad2 = viewModel.Road2;
                editEntity.PositioningLatitude = viewModel.Latitude;
                editEntity.PositioningLongitude = viewModel.Longitude;
                editEntity.Positioning = viewModel.Positioning;
                editEntity.InvestigaionTime = viewModel.InvestigaionTime;
                editEntity.TrafficControlNote = viewModel.TrafficControlNote;

                if (editEntity.FileName != viewModel.FileIdentification)
                {
                    editEntity.Weather = viewModel.Weather;
                    editEntity.FileName = viewModel.FileIdentification;
                    editEntity.IntersectionId = viewModel.IntersectionId;
                }

                _investigationRepository.Update(editEntity);

                // 調查資料更新
                if (editEntity.FileName != viewModel.FileIdentification)
                {
                    // delete all data
                    var data = _investigationDataRepository.GetAvailable()
                        .Where(x => x.InvestigationId == viewModel.Id);
                    _investigationDataRepository.DeleteRange(data);

                    // add new data
                    AddInvestigationData(viewModel.Type, viewModel.FileIdentification, viewModel.Id);
                }
                ;
            }
            else
            {
                var investigation = new Investigation
                {
                    InvestigationTypeEnum = viewModel.Type,
                    PositioningId = viewModel.PositioningId,
                    PositioningCity = viewModel.City,
                    PositioningTown = viewModel.Town,
                    PositioningRoad1 = viewModel.Road1,
                    PositioningRoad2 = viewModel.Road2,
                    PositioningLatitude = viewModel.Latitude,
                    PositioningLongitude = viewModel.Longitude,
                    Positioning = viewModel.Positioning,
                    Weather = viewModel.Weather,
                    InvestigaionTime = viewModel.InvestigaionTime,
                    TrafficControlNote = viewModel.TrafficControlNote,
                    FileName = viewModel.FileIdentification,
                    IntersectionId = viewModel.IntersectionId
                };
                var investigateEntity = _investigationRepository.Add(investigation);
                AddInvestigationData(viewModel.Type, viewModel.FileIdentification, investigateEntity.Id);
            }
        }

        /// <summary>
        /// 調查資料查詢
        /// </summary>
        /// <param name="viewModel"></param>
        public SingleDateQueryViewModel Query(DataQueryViewModel viewModel)
        {
            var entities = _investigationRepository.GetAvailable()
                                                   .Include(x => x.InvestigationData)
                                                   .Where(x => x.PositioningId == viewModel.PositioningId);
            entities = viewModel.QueryType == InvestigationQueryTypeEnum.Vehicle
                ? entities.Where(x => x.InvestigationType != (int)InvestigationTypeEnum.Pedestrians)
                : entities.Where(x => x.InvestigationType == (int)InvestigationTypeEnum.Pedestrians);

            entities = entities.OrderByDescending(x => x.InvestigaionTime);

            var entity = viewModel.InvestigaionTime.HasValue ? entities.FirstOrDefault(x => x.InvestigaionTime == viewModel.InvestigaionTime) : entities.FirstOrDefault();

            if (entity == null)
            {
                return null;
            }

            var otherInvestigaionTime =
                entities.Select(x => x.InvestigaionTime).ToList().Select(x => x.ToString("yyyy/MM/dd"));

            // 每小時的不同交通工具與路口轉向統計量
            var hourlyData = entity.InvestigationData
                                   .GroupBy(x => x.HourlyInterval)
                                   .Select(x =>
                new SingleDateHourlyViewModel
                {
                    HourlyInterval = x.Key,
                    LargeVehicle = x.Where(m => m.TargetTypeEnum == TargetTypeEnum.LargeVehicle).Sum(m => m.Amount),
                    LightVehicle = x.Where(m => m.TargetTypeEnum == TargetTypeEnum.LightVehicle).Sum(m => m.Amount),
                    Motocycle = x.Where(m => m.TargetTypeEnum == TargetTypeEnum.Motocycle).Sum(m => m.Amount),
                    Bicycle = x.Where(m => m.TargetTypeEnum == TargetTypeEnum.Bicycle).Sum(m => m.Amount),
                    TrafficData = x.GroupBy(m => new { m.Direction, m.Intersection })
                                   .Select(m => new TrafficViewModel
                                   {
                                       Direction = m.Key.Direction,
                                       Intersection = m.Key.Intersection,
                                       Amount = Convert.ToInt32(Math.Ceiling(m.Sum(k => k.Amount * k.TargetTypeEnum.GetAttributeOfType<TargetWeightAttribute>().Weight)))
                                   })
                });

            var queryResult = new SingleDateQueryViewModel
            {
                InvestigationType = entity.InvestigationTypeEnum,
                InvestigaionTime = entity.InvestigaionTime.ToString("yyyy/MM/dd"),
                OtherInvestigaionTime = otherInvestigaionTime,
                HourlyIntervals = hourlyData,
                IntersectionId = entity.IntersectionId,
                IntersectionName = $"{entity.PositioningTown}{entity.PositioningRoad1}與{entity.PositioningRoad2}",
                Weather = entity.Weather,
                FileIdentification = entity.FileName,
                Positioning = entity.Positioning,
                TrafficControlNote = entity.TrafficControlNote
            };

            return queryResult;
        }

        /// <summary>
        /// 調查資料查詢分頁列表
        /// </summary>
        /// <param name="queryOption"></param>
        public void GetInvestigationList(
            PagedQueryModel<Investigation, InvestigateListFilterViewModel> queryOption)
        {
            var query = _investigationRepository.GetAvailable();
            if (queryOption.Filter.Town.IsNullOrEmpty() == false)
            {
                query = query.Where(x => x.PositioningTown == queryOption.Filter.Town);
            }

            if (queryOption.Filter.Type.HasValue)
            {
                query = query.Where(x => x.InvestigationType == (int)queryOption.Filter.Type);
            }

            if (queryOption.Filter.CreatedYear.HasValue && queryOption.Filter.CreatedYear.Value != 0)
            {
                query = query.Where(x => x.InvestigaionTime.Year == queryOption.Filter.CreatedYear);
            }

            if (queryOption.Filter.Keyword.IsNullOrEmpty() == false)
            {
                query = query.Where(x => x.PositioningRoad1.Contains(queryOption.Filter.Keyword)
                                         || x.PositioningRoad2.Contains(queryOption.Filter.Keyword)
                                         || x.PositioningTown.Contains(queryOption.Filter.Keyword)
                                         || x.TrafficControlNote.Contains(queryOption.Filter.Keyword));
            }

            queryOption.Apply(query);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var entity = _investigationRepository.GetAvailable().FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return;
            }

            _investigationRepository.Delete(entity);
        }

        /// <summary>
        /// 取得目前調查資料的建置年度清單
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllInvestigationYears()
        {
            var years = _investigationRepository.GetAvailable()
                                                .Select(x => x.InvestigaionTime.Year)
                                                .Distinct()
                                                .ToList();

            return years;
        }

        /// <summary>
        /// 取得調查目標(車輛)歷次資料
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public List<VehicleHistoricalViewModel> VehicleHistoricalQuery(VehicleHistoricalQueryViewModel viewModel)
        {
            var models = _investigationRepository
                .GetAvailable()
                .Where(x => x.PositioningId == viewModel.PositioningId
                            && x.InvestigationType != (int)InvestigationTypeEnum.Pedestrians
                            && x.InvestigationData.Any(m => m.HourlyInterval == viewModel.HourlyInterval))
                .OrderBy(x => x.InvestigaionTime)
                .ToList()
                .Select(x => new VehicleHistoricalViewModel
                {
                    InvestigaionTime = x.InvestigaionTime.ToString("yyyy/MM/dd"),
                    LargeVehicle = x.InvestigationData.Where(m => m.TargetTypeEnum == TargetTypeEnum.LargeVehicle && m.HourlyInterval == viewModel.HourlyInterval)
                        .Sum(m => m.Amount),
                    LightVehicle = x.InvestigationData.Where(m => m.TargetTypeEnum == TargetTypeEnum.LightVehicle && m.HourlyInterval == viewModel.HourlyInterval)
                        .Sum(m => m.Amount),
                    Motocycle = x.InvestigationData.Where(m => m.TargetTypeEnum == TargetTypeEnum.Motocycle && m.HourlyInterval == viewModel.HourlyInterval)
                        .Sum(m => m.Amount),
                    Bicycle = x.InvestigationData.Where(m => m.TargetTypeEnum == TargetTypeEnum.Bicycle && m.HourlyInterval == viewModel.HourlyInterval)
                        .Sum(m => m.Amount)
                })
                .ToList();
            return models;
        }

        /// <summary>
        /// 取得調查目標(行人)歷次資料
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public List<PedestriansHistoricalViewModel> PedestriansHistoricalQuery(VehicleHistoricalQueryViewModel viewModel)
        {
            var models = _investigationRepository
                .GetAvailable()
                .Where(x => x.PositioningId == viewModel.PositioningId
                            && x.InvestigationType == (int)InvestigationTypeEnum.Pedestrians
                            && x.InvestigationData.Any(m => m.HourlyInterval == viewModel.HourlyInterval))
                .OrderBy(x => x.InvestigaionTime)
                .ToList()
                .Select(x => new PedestriansHistoricalViewModel
                {
                    InvestigaionTime = x.InvestigaionTime.ToString("yyyy/MM/dd"),
                    Pedestrians = x.InvestigationData.Where(m => m.HourlyInterval == viewModel.HourlyInterval).Sum(m => m.Amount)
                })
                .ToList();
            return models;
        }

        /// <summary>
        /// 取得調查路口歷年轉向資料
        /// </summary>
        /// <param name="viewModel"></param>
        public IEnumerable<DirectHistoricalViewModel> DirectionHistoricalQuery(DirectHistoricalQueryViewModel viewModel)
        {
            var models = _investigationRepository
                .GetAvailable()
                .Where(x => x.PositioningId == viewModel.PositioningId
                            && x.InvestigationType != (int)InvestigationTypeEnum.Pedestrians
                            && x.InvestigationData.Any(m => m.HourlyInterval == viewModel.HourlyInterval))
                .OrderBy(x => x.InvestigaionTime)
                .ToList()
                .Select(x => new DirectHistoricalViewModel
                {
                    InvestigaionTime = x.InvestigaionTime.ToString("yyyy/MM/dd"),
                    Directions = x.InvestigationData.Where(m => m.Intersection == viewModel.Intersection)
                                                    .GroupBy(m => m.Direction)
                                                    .ToDictionary(m => m.Key, m => m.Sum(a => a.Amount))
                });
            return models;
        }

        #region private methods - create

        private void AddInvestigationData(InvestigationTypeEnum type, string fileIdentification, int investigateId)
        {
            switch (type)
            {
                case InvestigationTypeEnum.TRoad:
                    var tRoadData = _cacheProvider.Get<InvestigateModel<VehicleInvestigateModel>>($"Investigation:Model:{fileIdentification}");
                    var tRoadEntities = TRoadProcess(investigateId, tRoadData);
                    _investigationDataRepository.AddRange(tRoadEntities);
                    break;

                case InvestigationTypeEnum.Intersection:
                    var intersectionData = _cacheProvider.Get<InvestigateModel<VehicleInvestigateModel>>($"Investigation:Model:{fileIdentification}");
                    var intersectionEntities = IntersectionProcess(investigateId, intersectionData);
                    _investigationDataRepository.AddRange(intersectionEntities);
                    break;

                case InvestigationTypeEnum.FiveWay:
                    var fiveWayData = _cacheProvider.Get<InvestigateModel<VehicleInvestigateModel>>($"Investigation:Model:{fileIdentification}");
                    var fiveWayEntities = FiveWayProcess(investigateId, fiveWayData);
                    _investigationDataRepository.AddRange(fiveWayEntities);
                    break;

                case InvestigationTypeEnum.Pedestrians:
                    var pedestriansData = _cacheProvider.Get<InvestigateModel<PedestriansInvestigateModel>>($"Investigation:Model:{fileIdentification}");
                    var pedestriansEntities = PedestriansProcess(investigateId, pedestriansData);
                    _investigationDataRepository.AddRange(pedestriansEntities);
                    break;
            }
        }

        private IEnumerable<InvestigationData> TRoadProcess(int investigateId, InvestigateModel<VehicleInvestigateModel> model)
        {
            var investigationData = new List<InvestigationData>();

            var intersections = model.Data
                                     .Select(x => x.DirectionCode)
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();

            var hours = model.Data
                             .Select(x => x.StartTime.Substring(0, 2))
                             .Distinct()
                             .OrderBy(x => x)
                             .ToList();

            var directionArray = new List<string> { TurnLeft, Straight, TurnRight };

            foreach (var intersection in intersections)
            {
                foreach (var hour in hours)
                {
                    var hourData = model.Data.Where(x => x.StartTime.StartsWith(hour) && x.DirectionCode == intersection).ToList();

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.LargeVehicle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.LightVehicle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.Motocycle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.Bicycle, hourData, hour));
                }
            }

            return investigationData;
        }

        private IEnumerable<InvestigationData> IntersectionProcess(int investigateId, InvestigateModel<VehicleInvestigateModel> model)
        {
            var investigationData = new List<InvestigationData>();

            var intersections = model.Data
                                     .Select(x => x.DirectionCode)
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();

            var hours = model.Data
                             .Select(x => x.StartTime.Substring(0, 2))
                             .Distinct()
                             .OrderBy(x => x)
                             .ToList();

            var directionArray = new List<string> { TurnLeft, Straight, TurnRight };

            foreach (var intersection in intersections)
            {
                foreach (var hour in hours)
                {
                    var hourData = model.Data.Where(x => x.StartTime.StartsWith(hour) && x.DirectionCode == intersection).ToList();

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, Straight, TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.LargeVehicle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, Straight, TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.LightVehicle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, Straight, TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.Motocycle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnLeft, TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, Straight, TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, TurnRight, TargetTypeEnum.Bicycle, hourData, hour));
                }
            }

            return investigationData;
        }

        private IEnumerable<InvestigationData> FiveWayProcess(int investigateId, InvestigateModel<VehicleInvestigateModel> model)
        {
            var investigationData = new List<InvestigationData>();

            var intersections = model.Data
                                     .Select(x => x.DirectionCode)
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();

            var hours = model.Data
                             .Select(x => x.StartTime.Substring(0, 2))
                             .Distinct()
                             .OrderBy(x => x)
                             .ToList();

            var directionArray = new List<string> { "A", "B", "C", "D", "E" };

            foreach (var intersection in intersections)
            {
                foreach (var hour in hours)
                {
                    var hourData = model.Data.Where(x => x.StartTime.StartsWith(hour) && x.DirectionCode == intersection).ToList();

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "A", TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "B", TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "C", TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "D", TargetTypeEnum.LargeVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "E", TargetTypeEnum.LargeVehicle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "A", TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "B", TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "C", TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "D", TargetTypeEnum.LightVehicle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "E", TargetTypeEnum.LightVehicle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "A", TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "B", TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "C", TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "D", TargetTypeEnum.Motocycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "E", TargetTypeEnum.Motocycle, hourData, hour));

                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "A", TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "B", TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "C", TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "D", TargetTypeEnum.Bicycle, hourData, hour));
                    investigationData.Add(GetVehicleData(directionArray, investigateId, intersection, "E", TargetTypeEnum.Bicycle, hourData, hour));
                }
            }

            return investigationData;
        }

        private IEnumerable<InvestigationData> PedestriansProcess(int investigateId,
            InvestigateModel<PedestriansInvestigateModel> model)
        {
            var investigationData = new List<InvestigationData>();

            var intersections = model.Data
                                     .Select(x => x.DirectionCode)
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();

            var hours = model.Data
                             .Select(x => x.StartTime.Substring(0, 2))
                             .Distinct()
                             .OrderBy(x => x)
                             .ToList();

            foreach (var intersection in intersections)
            {
                foreach (var hour in hours)
                {
                    var hourData = model.Data.Where(x => x.StartTime.StartsWith(hour) && x.DirectionCode == intersection).ToList();

                    investigationData.Add(GetPedestriansData(investigateId, intersection, "BD", TargetTypeEnum.Pedestrians, hourData, hour));
                    investigationData.Add(GetPedestriansData(investigateId, intersection, "AC", TargetTypeEnum.Pedestrians, hourData, hour));
                    investigationData.Add(GetPedestriansData(investigateId, intersection, "AB2CD", TargetTypeEnum.Pedestrians, hourData, hour));
                    investigationData.Add(GetPedestriansData(investigateId, intersection, "AD2BC", TargetTypeEnum.Pedestrians, hourData, hour));
                }
            }

            return investigationData;
        }

        private InvestigationData GetPedestriansData(
            int investigateId,
            string intersection,
            string direction,
            TargetTypeEnum type,
            List<PedestriansInvestigateModel> hourData,
            string hour)
        {
            var adjustment = 4 / hourData.Count;
            var h = Convert.ToInt32(hour);

            int amount = 0;
            int? firstQuarterCount = null;
            int? secondQuarterCount = null;
            int? thirdQuarterCount = null;
            int? fourthQuarterCount = null;
            switch (direction)
            {
                case "BD":
                    amount = hourData.Select(x => x.BD).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.BD;
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.BD;
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.BD;
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.BD;
                    break;

                case "AC":
                    amount = hourData.Select(x => x.AC).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.AC;
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.AC;
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.AC;
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.AC;
                    break;

                case "AB2CD":
                    amount = hourData.Select(x => x.AB2CD).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.AB2CD;
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.AB2CD;
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.AB2CD;
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.AB2CD;
                    break;

                case "AD2BC":
                    amount = hourData.Select(x => x.AD2BC).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.AD2BC;
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.AD2BC;
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.AD2BC;
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.AD2BC;
                    break;
            }

            var model = new InvestigationData
            {
                InvestigationId = investigateId,
                Intersection = intersection,
                Direction = direction,
                TargetTypeEnum = type,
                Amount = amount,
                HourlyInterval = $"{h:00}00{h + 1:00}00",
                FirstQuarterCount = firstQuarterCount,
                SecondQuarterCount = secondQuarterCount,
                ThirdQuarterCount = thirdQuarterCount,
                FourthQuarterCount = fourthQuarterCount
            };
            return model;
        }

        private InvestigationData GetVehicleData(
            List<string> directionArray,
            int investigateId,
            string intersection,
            string direction,
            TargetTypeEnum type,
            List<VehicleInvestigateModel> hourData,
            string hour)
        {
            var adjustment = 4 / hourData.Count;
            var h = Convert.ToInt32(hour);

            int index = directionArray.IndexOf(direction);

            int amount = 0;
            int? firstQuarterCount = null;
            int? secondQuarterCount = null;
            int? thirdQuarterCount = null;
            int? fourthQuarterCount = null;
            switch (type)
            {
                case TargetTypeEnum.LargeVehicle:
                    amount = hourData.Select(x => x.LargeVehicle[index]).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.LargeVehicle[index];
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.LargeVehicle[index];
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.LargeVehicle[index];
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.LargeVehicle[index];
                    break;

                case TargetTypeEnum.LightVehicle:
                    amount = hourData.Select(x => x.LightVehicle[index]).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.LightVehicle[index];
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.LightVehicle[index];
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.LightVehicle[index];
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.LightVehicle[index];
                    break;

                case TargetTypeEnum.Motocycle:
                    amount = hourData.Select(x => x.Motocycle[index]).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.Motocycle[index];
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.Motocycle[index];
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.Motocycle[index];
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.Motocycle[index];
                    break;

                case TargetTypeEnum.Bicycle:
                    amount = hourData.Select(x => x.Bicycle[index]).Sum() * adjustment;
                    firstQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}00")?.Bicycle[index];
                    secondQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}15")?.Bicycle[index];
                    thirdQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}30")?.Bicycle[index];
                    fourthQuarterCount = hourData.FirstOrDefault(x => x.StartTime == $"{hour}45")?.Bicycle[index];
                    break;
            }

            var model = new InvestigationData
            {
                InvestigationId = investigateId,
                Intersection = intersection,
                Direction = direction,
                TargetTypeEnum = type,
                Amount = amount,
                HourlyInterval = $"{h:00}00{h + 1:00}00",
                FirstQuarterCount = firstQuarterCount,
                SecondQuarterCount = secondQuarterCount,
                ThirdQuarterCount = thirdQuarterCount,
                FourthQuarterCount = fourthQuarterCount
            };
            return model;
        }

        #endregion private methods - create
    }
}