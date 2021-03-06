﻿using Kingwaytek.TrafficFlow.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kingwaytek.TrafficFlow.Controllers
{
    public class HomeController : ControllerBase
    {
        private string InvestigationFileFolder = "InvestigationFiles";

        private readonly MemoryCacheProvider _cacheProvider;

        private readonly InvestigateService _investigateService;

        public HomeController()
        {
            _cacheProvider = new MemoryCacheProvider();
            _investigateService = new InvestigateService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PagedQueryModel<Investigation, InvestigateListFilterViewModel> queryOption)
        {
            _investigateService.GetInvestigationList(queryOption);
            return View(queryOption);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DataCreatedViewModel viewModel)
        {
            _investigateService.CreateOrUpdate(viewModel);
            return Json("");
        }

        public ActionResult Edit(int id)
        {
            var model = _investigateService.GetById(id);

            return View("Create", model);
        }

        public ActionResult Query()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Query(DataQueryViewModel viewModel)
        {
            var model = _investigateService.Query(viewModel);
            return Json(model);
        }

        public ActionResult Delete(int id)
        {
            _investigateService.Delete(id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult VehicleHistoricalData(VehicleHistoricalQueryViewModel viewModel)
        {
            if (viewModel.QueryType == InvestigationQueryTypeEnum.Vehicle)
            {
                var vehicleModels = _investigateService.VehicleHistoricalQuery(viewModel);
                return Json(vehicleModels);
            }
            var pedestriansModels = _investigateService.PedestriansHistoricalQuery(viewModel);
            return Json(pedestriansModels);
        }

        [HttpPost]
        public ActionResult DirectHistoricalData(DirectHistoricalQueryViewModel viewModel)
        {
            var models = _investigateService.DirectionHistoricalQuery(viewModel);

            return Json(models);
        }

        [HttpPost]
        public ActionResult UploadInvestigation(HttpPostedFileBase file, InvestigationTypeEnum type, DateTime date, int positioningId)
        {
            var fileId = GetFileIdentification();
            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension.IsNullOrEmpty() == false)
            {
                fileId += fileExtension;
            }

            SaveFile(file.InputStream, fileId);

            var hours = _investigateService.GetExistInvestigationHourlys(type, date, positioningId);
            List<string> investigationHoursly;
            switch (type)
            {
                case InvestigationTypeEnum.TRoad:
                case InvestigationTypeEnum.Intersection:
                    var intersectionReader = new IntersectionReader();
                    var intersectionModel = intersectionReader.Convert(file.InputStream, type);
                    investigationHoursly = intersectionModel.Data.Select(x => x.StartTime.Substring(0, 2)).ToList();
                    intersectionModel.HasOverlayData = hours.Intersect(investigationHoursly).Any();
                    intersectionModel.FileIdentification = fileId;
                    _cacheProvider.Set($"Investigation:Model:{fileId}", intersectionModel, 30 * 60);
                    return View("CrossRoadPreview", intersectionModel);

                case InvestigationTypeEnum.Pedestrians:
                    var pedestriansReader = new PedestriansReader();
                    var pedestriansModel = pedestriansReader.Convert(file.InputStream);
                    investigationHoursly = pedestriansModel.Data.Select(x => x.StartTime.Substring(0, 2)).ToList();
                    pedestriansModel.HasOverlayData = hours.Intersect(investigationHoursly).Any();
                    pedestriansModel.FileIdentification = fileId;
                    _cacheProvider.Set($"Investigation:Model:{fileId}", pedestriansModel, 30 * 60);
                    return View("PedestriansPreview", pedestriansModel);

                case InvestigationTypeEnum.FiveWay:
                    var fivewayReader = new FivewayReader();
                    var fivewayModel = fivewayReader.Convert(file.InputStream);
                    investigationHoursly = fivewayModel.Data.Select(x => x.StartTime.Substring(0, 2)).ToList();
                    fivewayModel.HasOverlayData = hours.Intersect(investigationHoursly).Any();
                    fivewayModel.FileIdentification = fileId;
                    _cacheProvider.Set($"Investigation:Model:{fileId}", fivewayModel, 30 * 60);
                    return View("FivewayPreview", fivewayModel);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public ActionResult DownloadInvestigation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var filePath = Path.Combine(AppDataPath, InvestigationFileFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                throw new ArgumentNullException("Kingwaytek.TrafficFlow.Controllers.HomeController.DownloadInvestigation", "File not Exists");
            }

            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private string GetFileIdentification()
        {
            string key = Guid.NewGuid().ToString().Replace("-", string.Empty);
            return key;
        }

        private void SaveFile(Stream stream, string fileName)
        {
            var folder = Path.Combine(AppDataPath, InvestigationFileFolder);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filePath = Path.Combine(AppDataPath, InvestigationFileFolder, fileName);
            FileStream fileStream = System.IO.File.Create(filePath, (int)stream.Length);
            // Initialize the bytes array with the stream length and then fill it with data
            byte[] bytesInStream = new byte[stream.Length];
            stream.Read(bytesInStream, 0, bytesInStream.Length);
            // Use write method to write to the file specified above
            fileStream.Write(bytesInStream, 0, bytesInStream.Length);
        }
    }
}