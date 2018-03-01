using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Kingwaytek.TrafficFlow.Helpers.InvestigationExcelReader;

namespace Kingwaytek.TrafficFlow.Controllers
{
    public class HomeController : ControllerBase
    {
        private string InvestigationFileFolder = "InvestigationFiles";

        private readonly MemoryCacheProvider CacheProvider = new MemoryCacheProvider();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadInvestigation(HttpPostedFileBase file, InvestigationTypeEnum type)
        {
            var fileId = GetFileIdentification();
            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension.IsNullOrEmpty() == false)
            {
                fileId += fileExtension;
            }

            SaveFile(file.InputStream, fileId);

            switch (type)
            {
                case InvestigationTypeEnum.TRoad:
                case InvestigationTypeEnum.Intersection:
                    var intersectionReader = new IntersectionReader();
                    var intersectionModel = intersectionReader.Convert(file.InputStream, type);
                    intersectionModel.FileIdentification = fileId;
                    CacheProvider.Set($"Investigation:Model:{fileId}", intersectionModel, 30 * 60);
                    return View("CrossRoadPreview", intersectionModel);

                case InvestigationTypeEnum.Pedestrians:
                    var pedestriansReader = new PedestriansReader();
                    var pedestriansModel = pedestriansReader.Convert(file.InputStream);
                    pedestriansModel.FileIdentification = fileId;
                    CacheProvider.Set($"Investigation:Model:{fileId}", pedestriansModel, 30 * 60);
                    return View("PedestriansPreview", pedestriansModel);

                case InvestigationTypeEnum.FiveWay:
                    var fivewayReader = new FivewayReader();
                    var fivewayModel = fivewayReader.Convert(file.InputStream);
                    fivewayModel.FileIdentification = fileId;
                    CacheProvider.Set($"Investigation:Model:{fileId}", fivewayModel, 30 * 60);
                    return View("FivewayPreview", fivewayModel);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private string GetFileIdentification()
        {
            string key = Guid.NewGuid().ToString().Replace("-", string.Empty);
            return key;
        }

        private void SaveFile(Stream stream, string fileName)
        {
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