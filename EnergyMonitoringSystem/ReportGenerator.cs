using Elmah;
using EnergyMonitoringSystem.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EnergyMonitoringSystem
{
    public class ReportGenerator
    {
        private static DataTable DATATABLE;
        static string APP_PATH = HttpContext.Current.Server.MapPath("~/Reports");

        public static string SafeFileName(string name)
        {
            StringBuilder str = new StringBuilder(name);
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                str = str.Replace(c, '_');
            }
            return str.ToString();
        }
        private static void DownloadMultipleFile(string fileName, byte[] byteArray)
        {
            try
            {

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + Path.GetFileName(fileName) + "\"");
                HttpContext.Current.Response.OutputStream.Write(byteArray, 0, byteArray.Length);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                //HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                //Logger.WriteErrorLog("GENERATED ERROR : \n" + "Report generation Failed Error: " + ex.ToString());
            }
        }
        private static string FROM_DATE;
        private static string TO_DATE;
        private static string MACHINE_ID;
        private static string PLANT_ID;
        private static string FORMAT_TYPE;

        private static string REPORT_TYPE = string.Empty;
        private static string TEMPLATE = string.Empty;
        private static string DASHBOARD_PRINT = string.Empty;

        public static string GenerateEnergyReport(string formatType, string reportType, DataTable dt, string fromDate, string toDate, string plantID, string selectedMachines, string shiftId, string param, string dashboard = "") // param - Day Or Shift
        {
            string isGenerated = "NotGenerated";
            DATATABLE = dt;
            FROM_DATE = fromDate;
            TO_DATE = toDate;

            if (selectedMachines.Contains(","))
            {
                selectedMachines = selectedMachines.Replace(',', '|');
            }

            if (selectedMachines.Contains("'"))
            {
                selectedMachines = selectedMachines.Replace("'", "");
            }

            MACHINE_ID = selectedMachines;
            REPORT_TYPE = reportType;
            PLANT_ID = plantID;
            FORMAT_TYPE = formatType;
            DASHBOARD_PRINT = dashboard;
            string Filename = string.Empty;
            if (REPORT_TYPE.Equals("Format - I"))
            {
                if (FORMAT_TYPE.Equals("Day"))
                {
                    TEMPLATE = "EnergyReportForDay_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx";
                    Filename = "EnergyReportForDay.xlsx";
                }
                else if (FORMAT_TYPE.Equals("Month"))
                {
                    TEMPLATE = "EnergyReportForMonth_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx";
                    Filename = "EnergyReportForMonth.xlsx";
                }
                else
                {
                    TEMPLATE = "EnergyReport_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx";
                    Filename = "EnergyReport.xlsx";
                }
            }
            string Source = string.Empty, destination = string.Empty, Template = string.Empty;

            Source = Util.GetReportPath(Filename);
            destination = Path.Combine(APP_PATH, "Temp", SafeFileName(TEMPLATE));
            if (!File.Exists(Source))
            {
                Logger.WriteDebugLog("Engery Report- \n " + Source);
            }
            if (DATATABLE != null && DATATABLE.Rows.Count > 0)
            {
                FileInfo newFile = new FileInfo(Source);
                ExcelPackage pck = new ExcelPackage(newFile, true);

                var wsDt = pck.Workbook.Worksheets[1];

                if (DASHBOARD_PRINT.Equals("DashboardPrint"))
                {
                    DashboardExcelPrint(wsDt, DATATABLE);
                }
                else
                {
                    if (FORMAT_TYPE.Equals("Day"))
                    {
                        WriteDayDataToCells(pck, REPORT_TYPE, wsDt, DATATABLE);
                    }
                    else if (FORMAT_TYPE.Equals("Month"))
                    {
                        wsDt.Name = "MachineWise - Energy Data";
                        WriteMachineWiseMonthDataToCells(pck, REPORT_TYPE, wsDt, DATATABLE);
                    }
                    else
                    {
                        WriteDataToCell(REPORT_TYPE, wsDt, DATATABLE);
                    }
                }

                SetPrinterSettings(wsDt);
                DownloadMultipleFile(destination, pck.GetAsByteArray());
                isGenerated = "Generated";
            }
            return isGenerated;
        }

        internal static void WriteGridDataToExcel()
        {
            #region
            string templateFile = string.Empty;

            templateFile = Path.Combine(APP_PATH, "ReportTemplates\\" + TEMPLATE + ".xlsx");
            string GeneratedReportPath = Path.Combine(APP_PATH, "GeneratedReports");

            if (!File.Exists(templateFile))
            {

                return;
            }
            #endregion

            #region
            if (!Directory.Exists(GeneratedReportPath))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(GeneratedReportPath);
                }
                catch (Exception)
                {

                }
            }
            #endregion

            string excelFilePath = Path.Combine(GeneratedReportPath, REPORT_TYPE + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");

            FileInfo newFile = new FileInfo(templateFile);
            ExcelPackage pck = new ExcelPackage(newFile, true);

            var wsDt = pck.Workbook.Worksheets[1];

            if (DASHBOARD_PRINT.Equals("DashboardPrint"))
            {
                DashboardExcelPrint(wsDt, DATATABLE);
            }
            else
            {
                if (FORMAT_TYPE.Equals("Day"))
                {
                    WriteDayDataToCells(pck, REPORT_TYPE, wsDt, DATATABLE);
                }
                else if (FORMAT_TYPE.Equals("Month"))
                {
                    wsDt.Name = "MachineWise - Energy Data";
                    WriteMachineWiseMonthDataToCells(pck, REPORT_TYPE, wsDt, DATATABLE);
                }
                else
                {
                    WriteDataToCell(REPORT_TYPE, wsDt, DATATABLE);
                }
            }

            SetPrinterSettings(wsDt);

            var fi = new FileInfo(excelFilePath);
            if (fi.Exists)
            {
                fi.Delete();
            }

            pck.Workbook.Worksheets[1].Select();

            pck.SaveAs(fi);

        }

        private static void WriteMachineWiseMonthDataToCells(ExcelPackage pck, string reportType, ExcelWorksheet wsDt, DataTable dt)
        {
            int rowPos = 0, colPosition = 1;

            WriteDefaultDayValuesToExcelSheet(wsDt, "Month-Wise");

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            #region

            for (int i = 0; i < columnNames.Length; i++)
            {
                wsDt.Cells[9, colPosition].Value = columnNames[i];
                colPosition++;
            }

            setFontStylesForCells(wsDt, 10, 9, columnNames.Length, "Cols");

            if (reportType.Equals("Format - I"))
            {
                rowPos = 11;
                int colCount = dt.Columns.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        wsDt.Cells[rowPos, j + 1].Value = dt.Rows[i][j];
                    }

                    rowPos++;
                }
            }

            //string col1 = string.Format("C11:C{0}", dt.Rows.Count + 12);
            //wsDt.Cells[col1].Style.Numberformat.Format = "0";
            //string col2 = string.Format("D11:D{0}", dt.Rows.Count + 12);
            //wsDt.Cells[col2].Style.Numberformat.Format = "0";

            #endregion

            for (int i = 1; i < columnNames.Length + 1; i++)
            {
                wsDt.Column(i).Width = 15;
                wsDt.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("MonthWise - Energy Data", wsDt);

            rowPos = 11;
            int rowStartForGraph = dt.Rows.Count + 12;
            int colStartForGraph = 0;
            int firstRowGraphs = 3;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == firstRowGraphs)
                {
                    colStartForGraph = 0;
                    rowStartForGraph = rowStartForGraph + 10;
                    firstRowGraphs = firstRowGraphs + 3;
                }

                PlotMonthDataGraphsMachineWise(dt.Rows[i]["MachineId"].ToString(), wsDt, rowPos, columnNames.Length, rowStartForGraph, colStartForGraph);
                rowPos++;
                colStartForGraph = colStartForGraph + 4;
            }
            WriteMonthWiseMonthDataToCells(pck, REPORT_TYPE, ws, DATATABLE);
            SetPrinterSettings(ws);
        }

        private static void WriteMonthWiseMonthDataToCells(ExcelPackage pck, string reportType, ExcelWorksheet wsDt, DataTable dt)
        {
            int rowPos = 11;
            int rowStartForGraph = dt.Rows.Count + 12;
            int colStartForGraph = 0;
            int firstRowGraphs = 3;
            int colPos = 2;

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            for (int i = 1; i < columnNames.Length; i++)
            {
                if (i == firstRowGraphs + 1)
                {
                    colStartForGraph = 0;
                    rowStartForGraph = rowStartForGraph + 10;
                    firstRowGraphs = firstRowGraphs + 3;
                }

                PlotMonthDataGraphsMonthWise(columnNames[i].ToString(), wsDt, rowPos, dt.Rows.Count + 10, colPos, rowStartForGraph, colStartForGraph);
                rowPos++;
                colPos++;
                colStartForGraph = colStartForGraph + 4;
            }
        }

        private static void PlotMonthDataGraphsMonthWise(string machineName, ExcelWorksheet wsDt, int startPos, int noOfMonths, int colPos, int rowStartForGraphPos, int columnStartForGraphPos)
        {
            var chart = (ExcelBarChart)wsDt.Drawings.AddChart(machineName, eChartType.ColumnClustered);
            chart.Border.LineStyle = OfficeOpenXml.Drawing.eLineStyle.Solid;

            chart.SetSize(400, 200);
            chart.Title.Font.Bold = true;
            chart.Title.Font.Size = 11;
            chart.Title.Text = machineName;

            var serie1 = chart.Series.Add(ExcelRange.GetAddress(11, colPos, noOfMonths, colPos), ExcelRange.GetAddress(11, 1, noOfMonths, 1));
            chart.YAxis.Title.Text = "KWh";
            chart.YAxis.Title.Font.Bold = true;
            chart.YAxis.Title.Font.Size = 8;
            chart.XAxis.Font.Size = 8;

            chart.Legend.Remove();
            chart.SetPosition(rowStartForGraphPos, 0, columnStartForGraphPos, 0);
        }

        private static void PlotMonthDataGraphsMachineWise(string monthName, ExcelWorksheet wsDt, int startPos, int noOfMonths, int rowStartForGraphPos, int columnStartForGraphPos)
        {
            var chart = (ExcelBarChart)wsDt.Drawings.AddChart(monthName, eChartType.ColumnClustered);
            chart.Border.LineStyle = OfficeOpenXml.Drawing.eLineStyle.Solid;

            chart.SetSize(400, 200);
            chart.Title.Font.Bold = true;
            chart.Title.Font.Size = 11;
            chart.Title.Text = monthName;

            var serie1 = chart.Series.Add(ExcelRange.GetAddress(startPos, 2, startPos, noOfMonths), ExcelRange.GetAddress(9, 2, 10, noOfMonths));
            chart.YAxis.Title.Text = "KWh";
            chart.YAxis.Title.Font.Bold = true;
            chart.YAxis.Title.Font.Size = 8;
            chart.XAxis.Font.Size = 8;

            chart.Legend.Remove();
            chart.SetPosition(rowStartForGraphPos, 0, columnStartForGraphPos, 0);
        }

        private static void DashboardExcelPrint(ExcelWorksheet wsDt, DataTable dt)
        {
            int rowPos = 0, colPosition = 1;

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            #region

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (columnNames[i].Equals("PF")) columnNames[i] = "Power Factor";
                else if (columnNames[i].Equals("MachineID", StringComparison.OrdinalIgnoreCase)) columnNames[i] = "Machine ID";
                else if (columnNames[i].Equals("Energy", StringComparison.OrdinalIgnoreCase)) columnNames[i] = "Energy (KWH)";
                else if (columnNames[i].Equals("Components", StringComparison.OrdinalIgnoreCase)) columnNames[i] = "Production Count";
                else if (columnNames[i].Equals("UtilisedTime", StringComparison.OrdinalIgnoreCase)) columnNames[i] = "Production Time";
                else if (columnNames[i].Equals("Cost", StringComparison.OrdinalIgnoreCase)) columnNames[i] = "Cost (INR)";
                else if (columnNames[i].Equals("Volt1", StringComparison.OrdinalIgnoreCase)) columnNames[i] = @"VLN-R  (Min.\Max.)";
                else if (columnNames[i].Equals("Volt2", StringComparison.OrdinalIgnoreCase)) columnNames[i] = @"VLN-Y  (Min.\Max.)";
                else if (columnNames[i].Equals("Volt3", StringComparison.OrdinalIgnoreCase)) columnNames[i] = @"VLN-B  (Min.\Max.)";

                wsDt.Cells[9, colPosition].Value = columnNames[i];

                colPosition++;
            }
            setFontStylesForCells(wsDt, 10, 9, columnNames.Length, "Cols");
            #endregion

            try
            {
                #region
                rowPos = 11;
                int colCount = dt.Columns.Count;
                HideColumns(wsDt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        wsDt.Cells[rowPos, j + 1].Value = dt.Rows[i][j];
                    }
                    rowPos++;
                }

                for (int i = 11; i < rowPos; i++)
                {
                    for (int j = 1; j <= colCount; j++)
                    {
                        wsDt.Cells[i, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        if (i % 2 == 0)
                        {
                            wsDt.Cells[i, j].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 231, 245));
                        }
                        else
                        {
                            wsDt.Cells[i, j].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(186, 202, 222));
                        }
                        wsDt.Row(i).Height = 25;
                        wsDt.Cells[i, j].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                        wsDt.Cells[i, j].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                        wsDt.Cells[i, j].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                        wsDt.Cells[i, j].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                    }
                }

                for (int i = 3; i < columnNames.Length - 2; i++)
                {
                    wsDt.Column(i).Width = 17;
                }

                wsDt.Column(10).Hidden = true;
                wsDt.Column(11).Hidden = true;
                wsDt.Column(12).Hidden = true;

                wsDt.Row(3).Hidden = true;
                wsDt.Row(4).Hidden = true;
                wsDt.Row(5).Hidden = true;
                wsDt.Row(6).Hidden = true;
                wsDt.Row(7).Hidden = true;

                wsDt.Row(8).Height = 30;

                wsDt.Cells[1, 10].Value = DateTime.Now.ToString("dd-MMM-yyyy HH:mm tt");
                wsDt.Cells[1, 1].Value = "From Date : " + FROM_DATE;
                wsDt.Cells[2, 1].Value = "To date : " + TO_DATE;

                wsDt.Column(1).Width = 35;
                HideColumns(wsDt);
                PlotGraphs(wsDt, 11, rowPos - 1, 8);

                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(" Error - \n" + ex.ToString());

            }

        }

        private static void HideColumns(ExcelWorksheet wsDt)
        {
            List<GridSettings> gridSettings = DataBaseAccess.GetGridInformation();
            if (DASHBOARD_PRINT.Equals("DashboardPrint"))
            {
                foreach (GridSettings settings in gridSettings)
                {
                    if (settings.Visibility)
                    {
                        if (settings.ColumnName.Equals("Power Factor", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(5).Hidden = true;
                        else if (settings.ColumnName.Equals(@"VLN - B(Min.\Max.)", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(4).Hidden = true;
                        else if (settings.ColumnName.Equals(@"VLN-R  (Min.\Max.)", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(2).Hidden = true;
                        else if (settings.ColumnName.Equals(@"VLN-Y  (Min.\Max.)", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(3).Hidden = true;
                        else if (settings.ColumnName.Equals("Production Count", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(7).Hidden = true;
                        else if (settings.ColumnName.Equals("Production Time", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(6).Hidden = true;
                        if (settings.ColumnName.Equals("Cost (INR)", StringComparison.OrdinalIgnoreCase))
                            wsDt.Column(9).Hidden = true;
                    }
                }
            }
            else
            {
                foreach (GridSettings settings in gridSettings)
                {
                    if (settings.Visibility)
                    {
                        if (FORMAT_TYPE.Equals("Day"))
                        {
                            if (settings.ColumnName.Equals("Power Factor", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(6).Hidden = true;
                            else if (settings.ColumnName.Equals("Production Count", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(5).Hidden = true;
                            else if (settings.ColumnName.Equals("Production Time", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(3).Hidden = true;
                            else if (settings.ColumnName.Equals("Cost (INR)", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(7).Hidden = true;
                        }
                        else
                        {
                            if (settings.ColumnName.Equals("Power Factor", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(8).Hidden = true;
                            else if (settings.ColumnName.Equals("Production Count", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(7).Hidden = true;
                            else if (settings.ColumnName.Equals("Production Time", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(5).Hidden = true;
                            else if (settings.ColumnName.Equals("Cost (INR)", StringComparison.OrdinalIgnoreCase))
                                wsDt.Column(9).Hidden = true;
                        }

                    }
                }
            }
            #region
            //vals = DataBaseAccess.GetGridColumnSettings(out colVals);
            //{
            //    if (vals.Count > 0 && colVals.Count > 0)
            //    {

            //        if (DASHBOARD_PRINT.Equals("DashboardPrint"))
            //        {
            //            #region
            //            for (int i = 0; i < vals.Count; i++)
            //            {
            //                string val = vals[i];

            //                if (colVals[i] == "0")
            //                {
            //                    if (val.Equals("Power Factor"))
            //                    {
            //                        wsDt.Column(5).Hidden = true;
            //                    }
            //                    else if (val.Equals(@"VLN-B  (Min.\Max.)"))
            //                    {
            //                        wsDt.Column(4).Hidden = true;
            //                    }
            //                    else if (val.Equals(@"VLN-R  (Min.\Max.)"))
            //                    {
            //                        wsDt.Column(2).Hidden = true;
            //                    }
            //                    else if (val.Equals(@"VLN-Y  (Min.\Max.)"))
            //                    {
            //                        wsDt.Column(3).Hidden = true;
            //                    }
            //                    else if (val.Equals("Production Count"))
            //                    {
            //                        wsDt.Column(7).Hidden = true;
            //                    }
            //                    else if (val.Equals("Production Time"))
            //                    {
            //                        wsDt.Column(6).Hidden = true;
            //                    }

            //                    if (val.Equals("Cost (INR)"))
            //                    {
            //                        wsDt.Column(9).Hidden = true;
            //                    }
            //                }
            //            }
            //            #endregion
            //        }
            //        else
            //        {
            //            #region
            //            for (int i = 0; i < vals.Count; i++)
            //            {
            //                string val = vals[i];

            //                if (colVals[i] == "0")
            //                {
            //                    if (FORMAT_TYPE.Equals("Day"))
            //                    {
            //                        if (val.Equals("Power Factor"))
            //                        {
            //                            wsDt.Column(6).Hidden = true;
            //                        }
            //                        else if (val.Equals("Production Count"))
            //                        {
            //                            wsDt.Column(5).Hidden = true;
            //                        }
            //                        else if (val.Equals("Production Time"))
            //                        {
            //                            wsDt.Column(3).Hidden = true;
            //                        }
            //                        else if (val.Equals("Cost (INR)"))
            //                        {
            //                            wsDt.Column(7).Hidden = true;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (val.Equals("Power Factor"))
            //                        {
            //                            wsDt.Column(8).Hidden = true;
            //                        }
            //                        else if (val.Equals("Production Count"))
            //                        {
            //                            wsDt.Column(7).Hidden = true;
            //                        }
            //                        else if (val.Equals("Production Time"))
            //                        {
            //                            wsDt.Column(5).Hidden = true;
            //                        }
            //                        else if (val.Equals("Cost (INR)"))
            //                        {
            //                            wsDt.Column(9).Hidden = true;
            //                        }
            //                    }
            //                }
            //            }
            //            #endregion
            //        }
            //    }
            //}
            #endregion
        }

        private static void WriteDayDataToCells(ExcelPackage pck, string reportType, ExcelWorksheet wsDt, System.Data.DataTable dt)
        {
            int rowPos = 0, colPosition = 1, sheetId = 1;
            string prevMachineId = string.Empty;

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            #region


            for (int i = 2; i < columnNames.Length; i++)
            {
                if (columnNames[i].Equals("PF")) columnNames[i] = "Power Factor";
                else if (columnNames[i].Equals("energy")) columnNames[i] = "Energy (KWH)";
                else if (columnNames[i].Equals("components")) columnNames[i] = "Production Count";
                else if (columnNames[i].Equals("UtilisedTime")) columnNames[i] = "Production Time";
                else if (columnNames[i].Equals("Cost")) columnNames[i] = "Cost (INR)";
                else if (columnNames[i].Equals("Volt1")) columnNames[i] = @"VLN-R  (Min.\Max.)";
                else if (columnNames[i].Equals("Volt2")) columnNames[i] = @"VLN-Y  (Min.\Max.)";
                else if (columnNames[i].Equals("Volt3")) columnNames[i] = @"VLN-B  (Min.\Max.)";
                else if (columnNames[i].Equals("Cutting_Time")) columnNames[i] = @"Cutting Time";
                //added(Pramod)
                else if (columnNames[i].Equals("CompOpn")) columnNames[i] = "Component<Operation>";

                wsDt.Cells[9, colPosition].Value = columnNames[i];

                colPosition++;
            }
            setFontStylesForCells(wsDt, 10, 9, columnNames.Length - 2, "Cols");
            #endregion

            try
            {
                #region
                if (reportType.Equals("Format - I"))
                {
                    rowPos = 11;
                    int colCount = dt.Columns.Count;
                    WriteDefaultDayValuesToExcelSheet(wsDt, dt.Rows[0]["MachineId"].ToString());
                    HideColumns(wsDt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (prevMachineId == string.Empty || prevMachineId != dt.Rows[i]["MachineId"].ToString())
                        {
                            prevMachineId = dt.Rows[i]["MachineId"].ToString();
                            if (i > 0)
                            {

                                for (int ii = 3; ii < columnNames.Length - 1; ii++)
                                {
                                    wsDt.Column(ii).Width = 20;
                                }
                                HideColumns(wsDt);
                                //component column width
                                wsDt.Column(2).Width = 25;
                                wsDt.Column(3).Width = 35;
                                PlotGraphsForDayType(wsDt, 11, rowPos - 1, 9);
                                setFontStylesForCells(wsDt, rowPos + 1, 11, columnNames.Length - 2, "Rows");

                                string col1 = string.Format("A11:A{0}", dt.Rows.Count + 12);
                                wsDt.Cells[col1].Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                                string col2 = string.Format("B11:B{0}", dt.Rows.Count + 12);
                                wsDt.Cells[col2].Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";

                            }
                            sheetId++;
                            rowPos = 11;
                            wsDt.Workbook.Worksheets.Copy(wsDt.Workbook.Worksheets[wsDt.Workbook.Worksheets.Count].Name, sheetId.ToString());
                            pck.Workbook.Worksheets[wsDt.Workbook.Worksheets.Count - 1].Name = prevMachineId;
                            wsDt = pck.Workbook.Worksheets[wsDt.Workbook.Worksheets.Count - 1];
                            WriteDefaultDayValuesToExcelSheet(wsDt, pck.Workbook.Worksheets[wsDt.Workbook.Worksheets.Count - 1].Name);

                        }

                        int colVal = 1;
                        for (int j = 2; j < colCount; j++)
                        {
                            wsDt.Cells[rowPos, colVal].Value = dt.Rows[i][j];
                            colVal++;
                        }
                        rowPos++;

                    }

                    for (int i = 3; i < columnNames.Length - 1; i++)
                    {
                        wsDt.Column(i).Width = 20;
                    }
                    //component column width
                    wsDt.Column(2).Width = 25;
                    wsDt.Column(3).Width = 35;

                    HideColumns(wsDt);
                    PlotGraphsForDayType(wsDt, 11, rowPos - 1, 9);
                    wsDt.Workbook.Worksheets.Delete(wsDt.Workbook.Worksheets.Count);
                    WriteDefaultDayValuesToExcelSheet(wsDt, pck.Workbook.Worksheets[wsDt.Workbook.Worksheets.Count].Name);
                    setFontStylesForCells(wsDt, rowPos + 1, 11, columnNames.Length - 2, "Rows");

                    string col11 = string.Format("A11:A{0}", dt.Rows.Count + 12);
                    wsDt.Cells[col11].Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    string col22 = string.Format("B11:B{0}", dt.Rows.Count + 12);
                    wsDt.Cells[col22].Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";





                }
                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(" Error - \n" + ex.ToString());
            }
        }

        private static void WriteDefaultDayValuesToExcelSheet(ExcelWorksheet wsDt, string macID)
        {
            if (FORMAT_TYPE.Equals("Month"))
            {
                wsDt.Cells[1, 10].Value = Environment.NewLine + DateTime.Now.ToString("dd-MMM-yyyy HH:mm tt");
            }
            else
            {
                wsDt.Cells[1, 9].Value = Environment.NewLine + DateTime.Now.ToString("dd-MMM-yyyy HH:mm tt");
            }
            wsDt.Cells[7, 2].Value = PLANT_ID;
            wsDt.Cells[5, 2].Value = FROM_DATE;
            wsDt.Cells[6, 2].Value = TO_DATE;
            wsDt.Cells[1, 3].Value = "TPM-Trak - Energy Consumption Report for - " + Environment.NewLine + macID.ToUpper();
        }

        private static void WriteDataToCell(string reportType, ExcelWorksheet wsDt, System.Data.DataTable dt)
        {
            int rowPos = 0, colPosition = 1;
            int mergeShiftStartPos = 11;
            string prevShift = string.Empty;
            string rangeShift = string.Format("B{0}:B{1}", rowPos - 1, rowPos - 1);

            int mergeMachineIdStartPos = 11;
            string prevMachineId = string.Empty;
            string rangeMachineId = string.Format("A{0}:A{1}", rowPos - 1, rowPos - 1);

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            #region


            for (int i = 0; i < columnNames.Length; i++)
            {
                if (columnNames[i].Equals("PF")) columnNames[i] = "Power Factor";
                else if (columnNames[i].Equals("shift")) columnNames[i] = "Shift";
                else if (columnNames[i].Equals("energy")) columnNames[i] = "Energy (KWH)";
                else if (columnNames[i].Equals("components")) columnNames[i] = "Production Count";
                else if (columnNames[i].Equals("UtilisedTime")) columnNames[i] = "Production Time";
                else if (columnNames[i].Equals("Cost")) columnNames[i] = "Cost (INR)";
                else if (columnNames[i].Equals("Volt1")) columnNames[i] = @"VLN-R  (Min.\Max.)";
                else if (columnNames[i].Equals("Volt2")) columnNames[i] = @"VLN-Y  (Min.\Max.)";
                else if (columnNames[i].Equals("Volt3")) columnNames[i] = @"VLN-B  (Min.\Max.)";
                else if (columnNames[i].Equals("Cutting_Time")) columnNames[i] = @"Cutting Time";

                //added(Pramod)
                else if (columnNames[i].Equals("CompOpn")) columnNames[i] = "Component<Operation>";

                wsDt.Cells[9, colPosition].Value = columnNames[i];

                colPosition++;
            }
            setFontStylesForCells(wsDt, 10, 9, columnNames.Length, "Cols");
            #endregion

            try
            {
                #region
                if (reportType.Equals("Format - I"))
                {
                    rowPos = 11;
                    int colCount = dt.Columns.Count;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (prevShift == string.Empty) prevShift = dt.Rows[i]["Shift"].ToString();
                        if (prevMachineId == string.Empty) prevMachineId = dt.Rows[i]["MachineId"].ToString();

                        for (int j = 0; j < colCount; j++)
                        {
                            wsDt.Cells[rowPos, j + 1].Value = dt.Rows[i][j];
                        }

                        if (prevShift != dt.Rows[i]["Shift"].ToString() || dt.Rows.Count == i)
                        {
                            rangeShift = string.Format("B{0}:B{1}", mergeShiftStartPos, rowPos - 1);
                            wsDt.Cells[rangeShift].Merge = true;
                            mergeShiftStartPos = rowPos;
                            prevShift = dt.Rows[i]["Shift"].ToString();
                        }

                        if (prevMachineId != dt.Rows[i]["MachineId"].ToString() || dt.Rows.Count == i)
                        {
                            rangeMachineId = string.Format("A{0}:A{1}", mergeMachineIdStartPos, rowPos - 1);
                            wsDt.Cells[rangeMachineId].Merge = true;
                            mergeMachineIdStartPos = rowPos;
                            prevMachineId = dt.Rows[i]["MachineId"].ToString();
                        }

                        rowPos++;

                    }

                    rangeShift = string.Format("B{0}:B{1}", mergeShiftStartPos, rowPos - 1);
                    wsDt.Cells[rangeShift].Merge = true;

                    rangeMachineId = string.Format("A{0}:A{1}", mergeMachineIdStartPos, rowPos - 1);
                    wsDt.Cells[rangeMachineId].Merge = true;
                    WriteDefaultValuesToExcelSheet(wsDt);
                    HideColumns(wsDt);
                    setFontStylesForCells(wsDt, dt.Rows.Count + 12, 11, columnNames.Length, "Rows");

                    if (FORMAT_TYPE.Equals("Time Consolidated"))
                    {
                        wsDt.Column(3).Hidden = true;
                        wsDt.Column(4).Hidden = true;
                        for (int i = 5; i < columnNames.Length - 2; i++)
                        {
                            wsDt.Column(i).Width = 22;
                        }
                        PlotGraphs(wsDt, 11, rowPos - 1, 11);
                    }

                    if (FORMAT_TYPE.Equals("Shift"))
                    {
                        wsDt.Column(3).Width = 25;
                        wsDt.Column(4).Width = 25;
                    }

                    //component column width
                    wsDt.Column(5).Width = 35;
                }
                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(" Error - \n" + ex.ToString());
            }


        }

        private static void PlotGraphs(ExcelWorksheet wsDt, int startPos, int series, int EnergyColIndex)
        {
            var chart = (ExcelBarChart)wsDt.Drawings.AddChart("Energy Report", eChartType.ColumnClustered);
            chart.Border.LineStyle = OfficeOpenXml.Drawing.eLineStyle.Solid;

            chart.SetSize(710, 310);
            chart.Title.Text = "Energy Report";
            chart.Legend.Remove();

            var serie1 = chart.Series.Add(ExcelRange.GetAddress(startPos, EnergyColIndex, series, EnergyColIndex), ExcelRange.GetAddress(startPos, 1, series, 1));
            chart.YAxis.Title.Text = "KWh";

            var chartz = (ExcelPieChart)wsDt.Drawings.AddChart("Energy Reportz", eChartType.Pie);
            if (EnergyColIndex <= 8)
            {
                chart.SetPosition((series + 13) * 20, 22);
                chartz.SetPosition(((series + 13) * 20), 766);
            }
            else
            {
                chart.SetPosition((series + 12) * 18, 22);
                chartz.SetPosition(((series + 12) * 18), 766);
            }
            chartz.Border.LineStyle = OfficeOpenXml.Drawing.eLineStyle.Solid;

            chartz.SetSize(420, 310);
            chartz.Title.Text = "Energy Report";

            var serie2 = chartz.Series.Add(ExcelRange.GetAddress(startPos, EnergyColIndex, series, EnergyColIndex), ExcelRange.GetAddress(startPos, 1, series, 1));
            chartz.Legend.Position = eLegendPosition.Top;
        }

        private static void PlotGraphsForDayType(ExcelWorksheet wsDt, int startPos, int series, int EnergyColIndex)
        {
            int val = 18;

            string ColValue = string.Format("A11:A{0}", series.ToString());
            //string NextColValue = string.Format("J11:J{0}", series.ToString());
            string NextColValue = string.Format("R11:R{0}", series.ToString());
            wsDt.Cells[ColValue].Copy(wsDt.Cells[NextColValue]);
            wsDt.Cells[NextColValue].Style.Numberformat.Format = "dd-MMM-yyyy";
            //wsDt.Column(10).Width = 0.3;
            wsDt.Cells[NextColValue].Style.Font.Color.SetColor(Color.White);

            var chart = (ExcelBarChart)wsDt.Drawings.AddChart("Energy Report", eChartType.ColumnClustered);
            chart.Title.Text = "Energy Report";
            chart.YAxis.Title.Text = "KWh";
            chart.Series.Add(ExcelRange.GetAddress(startPos, EnergyColIndex, series, EnergyColIndex), ExcelRange.GetAddress(startPos, val, series, val));
            chart.Border.LineStyle = OfficeOpenXml.Drawing.eLineStyle.Solid;
            chart.Legend.Remove();

            var chartz = (ExcelPieChart)wsDt.Drawings.AddChart("Energy Reportz", eChartType.Pie);
            chartz.Border.LineStyle = OfficeOpenXml.Drawing.eLineStyle.Solid;
            chartz.Title.Text = "Energy Report";
            chartz.Series.Add(ExcelRange.GetAddress(startPos, EnergyColIndex, series, EnergyColIndex), ExcelRange.GetAddress(startPos, val, series, val));

            chart.XAxis.Format = "dd-MMM-yyyy";

            if (series <= 27)
            {
                chart.SetPosition((series + 12) * 15, 22);
                chartz.SetPosition(((series + 12) * 15), 930);

                chartz.Legend.Position = eLegendPosition.TopRight;

                chart.SetSize(900, 350);
                chartz.SetSize(460, 350);
            }
            else
            {

                if (series >= 36 && series < 40)
                {
                    chart.SetPosition(((series + 12) * 15) + 55, 22);
                    chartz.SetPosition(((series + 12) * 15) + 400, 22);
                }
                else if (series >= 40)
                {
                    chart.SetPosition(((series + 12) * 15) + 270, 22);
                    chartz.SetPosition(((series + 12) * 15) + 620, 22);
                }
                else
                {
                    chart.SetPosition(((series + 12) * 15) + 100, 22);
                    chartz.SetPosition(((series + 12) * 15) + 420, 22);
                }

                chart.SetSize(1255, 300);
                chartz.SetSize(1255, 500);
            }
            chartz.Legend.Position = eLegendPosition.Top;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Logger.WriteErrorLog("Error - \n" + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void WriteDefaultValuesToExcelSheet(ExcelWorksheet wsDt)
        {
            wsDt.Cells[1, 9].Value = "Report Date " + Environment.NewLine + DateTime.Now.ToString("dd-MMM-yyyy HH:mm tt");
            wsDt.Cells[5, 2].Value = FROM_DATE;
            wsDt.Cells[6, 2].Value = TO_DATE;
            wsDt.Cells[7, 2].Value = PLANT_ID;
        }

        private static void SetPrinterSettings(ExcelWorksheet wsDt)
        {
            wsDt.PrinterSettings.Orientation = eOrientation.Landscape;
            wsDt.PrinterSettings.PaperSize = ePaperSize.A4;
            wsDt.PrinterSettings.LeftMargin = new decimal(.25);
            wsDt.PrinterSettings.RightMargin = new decimal(.25);
            wsDt.PrinterSettings.TopMargin = new decimal(.25);
            wsDt.PrinterSettings.BottomMargin = new decimal(.25);


            //wsDt.PrinterSettings.FitToPage = true;         
        }

        private static void setFontStylesForCells(ExcelWorksheet wsDt, int val, int fromRow, int toCol, string RowOrCols)
        {
            try
            {
                if (RowOrCols.Equals("Rows"))
                {
                    if (val == 14) val = 15;
                    using (var range = wsDt.Cells[fromRow, 1, val - 2, toCol])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(251, 251, 251));
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Hair;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Hair;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Hair;//(ExcelBorderStyle.Medium, Color.Red);                        
                    }
                }
                else
                {
                    using (var range = wsDt.Cells[fromRow, 1, val, toCol])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 73, 125));
                        range.Style.Font.Color.SetColor(Color.White);
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
        }

    }
}