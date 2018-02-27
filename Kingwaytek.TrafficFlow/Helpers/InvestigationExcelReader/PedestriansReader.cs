using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Kingwaytek.TrafficFlow.Helpers.InvestigationExcelReader
{
    public class PedestriansReader
    {
        public InvestigateModel<PedestriansInvestigateModel> Convert(Stream fileStream)
        {
            var models = new InvestigateModel<PedestriansInvestigateModel>();

            using (var excel = new ExcelPackage(fileStream))
            {
                var workSheet = excel.Workbook.Worksheets.FirstOrDefault();

                if (workSheet == null)
                {
                    return models;
                }

                // The weather data
                models.Weather = workSheet.Cells[5, 2].Text;

                for (int i = 12; i <= workSheet.Dimension.End.Row; i++)
                {
                    if (workSheet.Cells[i, 2].Text.IsNullOrEmpty() ||
                        workSheet.Cells[i, 3].Text.IsNullOrEmpty()) continue;
                    var rowData = new PedestriansInvestigateModel
                    {
                        DirectionCode = workSheet.Cells[i, 1].Text.IsNullOrEmpty() ? models.Data[models.Data.Count - 1].DirectionCode : workSheet.Cells[i, 1].Text,
                        StartTime = workSheet.Cells[i, 2].Text,
                        EndTime = workSheet.Cells[i, 3].Text,
                        BD = workSheet.Cells[i, 4].Text.ToInt(),
                        AC = workSheet.Cells[i, 4].Text.ToInt(),
                        AB2CD = workSheet.Cells[i, 4].Text.ToInt(),
                        AD2BC = workSheet.Cells[i, 4].Text.ToInt()
                    };

                    models.Data.Add(rowData);
                }
            }

            return models;
        }
    }
}