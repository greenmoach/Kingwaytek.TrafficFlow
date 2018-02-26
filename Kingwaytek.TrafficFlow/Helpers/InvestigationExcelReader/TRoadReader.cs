using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Kingwaytek.TrafficFlow
{
    public class TRoadReader
    {
        public List<VehicleInvestigateModel> Convert(Stream fileStream)
        {
            var models = new List<VehicleInvestigateModel>();

            using (var excel = new ExcelPackage(fileStream))
            {
                var workSheet = excel.Workbook.Worksheets.FirstOrDefault();

                if (workSheet == null)
                {
                    return models;
                }

                for (int i = 12; i <= workSheet.Dimension.End.Row; i++)
                {
                    if (workSheet.Cells[i, 2].Text.IsNullOrEmpty() ||
                        workSheet.Cells[i, 3].Text.IsNullOrEmpty()) continue;
                    var rowData = new VehicleInvestigateModel
                    {
                        DirectionCode = workSheet.Cells[i, 1].Text.IsNullOrEmpty() ? models[models.Count - 1].DirectionCode : workSheet.Cells[i, 1].Text,
                        StartTime = workSheet.Cells[i, 2].Text,
                        EndTime = workSheet.Cells[i, 3].Text,
                        LargeVehicle = new[]
                        {
                            ToInt(workSheet.Cells[i, 4].Text),
                            ToInt(workSheet.Cells[i, 5].Text),
                            ToInt(workSheet.Cells[i, 6].Text)
                        },
                        LightVehicle = new[]
                        {
                            ToInt(workSheet.Cells[i, 7].Text),
                            ToInt(workSheet.Cells[i, 8].Text),
                            ToInt(workSheet.Cells[i, 9].Text)
                        },
                        Motocycle = new[]
                        {
                            ToInt(workSheet.Cells[i, 10].Text),
                            ToInt(workSheet.Cells[i, 11].Text),
                            ToInt(workSheet.Cells[i, 12].Text)
                        },
                        Bicycle = new[]
                        {
                            ToInt(workSheet.Cells[i, 13].Text),
                            ToInt(workSheet.Cells[i, 14].Text),
                            ToInt(workSheet.Cells[i, 15].Text)
                        },
                    };

                    models.Add(rowData);
                }
            }

            return models;
        }

        private int ToInt(string text)
        {
            if (text.IsNullOrEmpty())
            {
                return 0;
            }

            int.TryParse(text, out var value);

            return value;
        }
    }
}