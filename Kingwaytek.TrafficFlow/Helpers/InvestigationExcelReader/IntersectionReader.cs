using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Kingwaytek.TrafficFlow
{
    public class IntersectionReader
    {
        public InvestigateModel<VehicleInvestigateModel> Convert(Stream fileStream, InvestigationTypeEnum type)
        {
            var models = new InvestigateModel<VehicleInvestigateModel>();

            using (var excel = new ExcelPackage(fileStream))
            {
                var workSheet = excel.Workbook.Worksheets.FirstOrDefault();

                if (workSheet == null)
                {
                    return models;
                }

                if (IsValidDirection(workSheet, type) == false)
                {
                    return models;
                }

                // The weather data
                models.Weather = workSheet.Cells[5, 2].Text;

                for (int i = 12; i <= workSheet.Dimension.End.Row; i++)
                {
                    if (workSheet.Cells[i, 2].Text.IsNullOrEmpty() ||
                        workSheet.Cells[i, 3].Text.IsNullOrEmpty()) continue;
                    var rowData = new VehicleInvestigateModel
                    {
                        DirectionCode = workSheet.Cells[i, 1].Text.IsNullOrEmpty() ? models.Data[models.Data.Count - 1].DirectionCode : workSheet.Cells[i, 1].Text,
                        StartTime = workSheet.Cells[i, 2].Text,
                        EndTime = workSheet.Cells[i, 3].Text,
                        LargeVehicle = new[]
                        {
                            workSheet.Cells[i, 4].Text.ToInt(),
                            workSheet.Cells[i, 5].Text.ToInt(),
                            workSheet.Cells[i, 6].Text.ToInt()
                        },
                        LightVehicle = new[]
                        {
                            workSheet.Cells[i, 7].Text.ToInt(),
                            workSheet.Cells[i, 8].Text.ToInt(),
                            workSheet.Cells[i, 9].Text.ToInt()
                        },
                        Motocycle = new[]
                        {
                            workSheet.Cells[i, 10].Text.ToInt(),
                            workSheet.Cells[i, 11].Text.ToInt(),
                            workSheet.Cells[i, 12].Text.ToInt()
                        },
                        Bicycle = new[]
                        {
                            workSheet.Cells[i, 13].Text.ToInt(),
                            workSheet.Cells[i, 14].Text.ToInt(),
                            workSheet.Cells[i, 15].Text.ToInt()
                        },
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
        private bool IsValidDirection(ExcelWorksheet workSheet, InvestigationTypeEnum type)
        {
            var directions = new List<string> { "A", "B", "C", "D" };
            var directionNumber = workSheet.Cells[12, 1, workSheet.Dimension.End.Row, 1].Count(x => directions.Contains(x.Text.ToUpper()));
            if (type == InvestigationTypeEnum.TRoad && directionNumber == 3)
            {
                return true;
            }

            if (type == InvestigationTypeEnum.Intersection && directionNumber == 4)
            {
                return true;
            }

            return false;
        }
    }
}