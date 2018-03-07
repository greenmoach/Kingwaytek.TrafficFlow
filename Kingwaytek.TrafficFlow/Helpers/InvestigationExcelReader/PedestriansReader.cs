using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Kingwaytek.TrafficFlow
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

                if (IsValidDirection(workSheet) == false)
                {
                    return models;
                }

                // 站號
                models.IntersectionId = workSheet.Cells[6, 2].Text;

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
                        AC = workSheet.Cells[i, 5].Text.ToInt(),
                        AB2CD = workSheet.Cells[i, 6].Text.ToInt(),
                        AD2BC = workSheet.Cells[i, 7].Text.ToInt()
                    };

                    models.Data.Add(rowData);
                }
            }

            return models;
        }

        /// <summary>
        /// 有效的指向數
        /// </summary>
        /// <returns></returns>
        private bool IsValidDirection(ExcelWorksheet workSheet)
        {
            var directions = new List<string> { "A", "B", "C", "D" };
            var directionNumber = workSheet.Cells[12, 1, workSheet.Dimension.End.Row, 1].Count(x => directions.Contains(x.Text.ToUpper()));
            return directionNumber == 4
                && workSheet.Cells[11, 4].Text.ToUpper().Equals("BD")
                && workSheet.Cells[11, 5].Text.ToUpper().Equals("AC");
        }
    }
}