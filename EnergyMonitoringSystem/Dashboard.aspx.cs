using EnergyMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EnergyMonitoringSystem
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string selectedFromDate;
        private string selectedShift;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFromDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                List<string> allShifts = DataBaseAccess.GetAllShift();
                ddlShift.DataSource = allShifts;
                ddlShift.DataBind();
                btnProcess_Click(null, null);
            }

        }

        private void bindTable()
        {
            selectedFromDate = DataBaseAccess.GetLogicalDay(txtFromDate.Text);
            DateTime fromdateTime = DateTime.ParseExact(txtFromDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            selectedFromDate = fromdateTime.ToString("yyyy-MM-dd");
            selectedShift = ddlShift.Text.ToString();
            DataTable dt = DataBaseAccess.GetDataForDateShift(selectedFromDate, selectedShift, true);
            if (dt != null && dt.Rows.Count > 0)
            {
                ClearDataGridColumns();
                BoundField boundField1 = new BoundField();
                boundField1.HeaderText = "Machine ID";
                boundField1.DataField = dt.Columns[0].ColumnName;
                gvDashboard.Columns.Add(boundField1);
                List<GridSettings> gridSettings = DataBaseAccess.GetGridInformation();
                foreach (GridSettings settings in gridSettings)
                {
                    BoundField boundField = new BoundField();
                    boundField.HeaderText = settings.ColumnText;
                    if (settings.ColumnName.Equals("PowerFactor"))
                        boundField.DataField = dt.Columns["PF"].ColumnName;
                    else if (settings.ColumnName.Equals("ProductionTime"))
                        boundField.DataField = dt.Columns["UtilisedTime"].ColumnName;
                    else if (settings.ColumnName.Equals("Volt1"))
                        boundField.DataField = dt.Columns["Volt1"].ColumnName;
                    else if (settings.ColumnName.Equals("Volt2"))
                        boundField.DataField = dt.Columns["Volt2"].ColumnName;
                    else if (settings.ColumnName.Equals("Volt3"))
                        boundField.DataField = dt.Columns["Volt3"].ColumnName;
                    else if (settings.ColumnName.Equals("ProductionCount"))
                        boundField.DataField = dt.Columns["Components"].ColumnName;
                    else if (settings.ColumnName.Equals("Energy"))
                        boundField.DataField = dt.Columns["Energy"].ColumnName;
                    else if (settings.ColumnName.Equals("Cost"))
                        boundField.DataField = dt.Columns["Cost"].ColumnName;
                    boundField.Visible = settings.Visibility;
                    gvDashboard.Columns.Add(boundField);
                }
            }
            gvDashboard.DataSource = dt;
            gvDashboard.DataBind();
            if (ddlShift.SelectedIndex != 0)
            {
                foreach (GridViewRow row in gvDashboard.Rows)
                {
                    HyperLink hyperLink = new HyperLink();
                    hyperLink.ID = "hlHourwise";
                    hyperLink.Text = row.Cells[0].Text;
                    hyperLink.ToolTip = "View Hourwise Energy For (" + hyperLink.Text + ")";
                    row.Cells[0].Controls.Add(hyperLink);
                }

            }

        }

        //private void LinkButton_Click(object sender, EventArgs e)
        //{
        //    LinkButton linkButton = (LinkButton)sender;
        //    string machineId = linkButton.Text;
        //    DateTime datePeriod = DateTime.ParseExact(txtFromDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    string date = datePeriod.ToString("yyyy-MM-dd");
        //    string shift = ddlShift.SelectedValue;
        //    lblShiftnDate.Text = date + " (Shift - " + shift + ")";
        //    lblHourwiseHeader.Text = "Hourwise Energy Cockpit For - " + machineId;
        //    gvHourwiseMonitoring.DataSource = DataBaseAccess.GetHourWiseEnergyData(machineId, date, shift, true);
        //    gvHourwiseMonitoring.DataBind();
        //    ScriptManager.RegisterStartupScript(this, GetType(), "OpenHourwiseMonitoringPopup", "OpenHourwiseMonitoring();", true);
        //}

        private void ClearDataGridColumns()
        {
            if (gvDashboard.Columns.Count > 0)
            {
                for (int i = gvDashboard.Columns.Count - 1; i >= 0; i--)
                {
                    gvDashboard.Columns.RemoveAt(i);
                }
            }
        }
        protected void btnProcess_Click(object sender, EventArgs e)
        {
            bool timerOff = false;
            if (timerToAutoRefresh.Enabled)
            {
                timerToAutoRefresh.Enabled = false;
                timerOff = true;
            }
            bindTable();
            ScriptManager.RegisterStartupScript(this, GetType(), "CallbtnProcess", "BtnProcessClick();", true);
            if (timerOff)
            {
                timerToAutoRefresh.Enabled = true;
                timerToAutoRefresh.Interval = 1000 * ConnectionManager.refreshData;
            }

        }

        protected void timerToAutoRefresh_Tick(object sender, EventArgs e)
        {
            btnProcess_Click(null, null);
        }

        protected void cbAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutoRefresh.Checked)
            {

                timerToAutoRefresh.Enabled = true;
                timerToAutoRefresh.Interval = 1000 * ConnectionManager.refreshData;
            }
            else
            {
                timerToAutoRefresh.Enabled = false;
            }
            btnProcess_Click(null, null);
        }
        #region "Get Hourwise Monitoring Data"
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<HourwiseMonitoringData> GetHourwiseMonitoring(string machine, string date, string shift)
        {
            List<HourwiseMonitoringData> hourwiseMonitoringDatas = new List<HourwiseMonitoringData>();
            DateTime fromdateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            date = fromdateTime.ToString("yyyy-MM-dd");
            hourwiseMonitoringDatas = DataBaseAccess.GetHourWiseEnergyData(machine, date, shift, true);
            for (int i = 0; i < hourwiseMonitoringDatas.Count; i++)
            {
                string a = hourwiseMonitoringDatas[i].ShiftHourID;
            }
            return hourwiseMonitoringDatas;
        }
        #endregion

        #region "Plot Hourwise Energy Bar Chart"
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static DataForEnergy GetHourwiseEnergy(string machine, string date, string shift)
        {
            List<HourwiseMonitoringData> hourwiseMonitoringDatas = new List<HourwiseMonitoringData>();
            DateTime fromdateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            date = fromdateTime.ToString("yyyy-MM-dd");
            hourwiseMonitoringDatas = DataBaseAccess.GetHourWiseEnergyData(machine, date, shift, true);
            DataForEnergy chartForEnergy = new DataForEnergy();
            try
            {
                chartForEnergy.TitleEnergy = "Energy Graph";
                chartForEnergy.xAxisTitleEnergy = "Hours";
                chartForEnergy.yAxisTitleEnergy = "Energy(KwH)";

                chartForEnergy.EnergyCategories = new List<string>();
                ChartSeries series1 = new ChartSeries();
                ChartSeries series2 = new ChartSeries();

                if (hourwiseMonitoringDatas != null && hourwiseMonitoringDatas.Count > 0)
                {
                    series1.name = "Energy";
                    series2.name = "Target";
                    series1.data = new List<double>();
                    series2.data = new List<double>();
                    foreach (HourwiseMonitoringData data in hourwiseMonitoringDatas)
                    {
                        chartForEnergy.EnergyCategories.Add(data.ShiftHourID);
                        series1.data.Add(Convert.ToDouble(data.Energy));
                        series2.data.Add(Convert.ToDouble(data.Target));
                    }
                    chartForEnergy.seriesEnergy = new List<ChartSeries>();
                    chartForEnergy.seriesEnergy.Add(series1);
                    chartForEnergy.seriesEnergy.Add(series2);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

            return chartForEnergy;
        }
        #endregion

        #region "Plot Hourwise Summary Chart"
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static DataForSummery GetHourwiseSummary(string machine, string date, string shift)
        {
            List<HourwiseMonitoringData> hourwiseMonitoringDatas = new List<HourwiseMonitoringData>();
            DateTime fromdateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            date = fromdateTime.ToString("yyyy-MM-dd");
            hourwiseMonitoringDatas = DataBaseAccess.GetHourWiseEnergyData(machine, date, shift, true);
            DataForSummery chartForSummery = new DataForSummery();
            chartForSummery.data = new List<pieData>();

            try
            {
                if (hourwiseMonitoringDatas != null && hourwiseMonitoringDatas.Count > 0)
                {
                    chartForSummery.name = "Energy";
                    foreach (HourwiseMonitoringData data in hourwiseMonitoringDatas)
                    {
                        pieData pData = new pieData();
                        pData.name = data.ShiftHourID;
                        if (Convert.ToDouble(data.Energy) < 0)
                        {
                            pData.y = Convert.ToDouble(data.Energy) * -1;
                            pData.positive = false;
                        }

                        else
                        {
                            pData.y = Convert.ToDouble(data.Energy);
                            pData.positive = true;
                        }

                        chartForSummery.data.Add(pData);
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

            return chartForSummery;
        }
        #endregion

        #region "Get Pie chart for Status measure"
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static DataForSummery GetSummeryPieData(string fromDate, string selectedShift)
        {
            DateTime fromdateTime = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            fromDate = fromdateTime.ToString("yyyy-MM-dd");
            DataTable dt = DataBaseAccess.GetDataForDateShift(fromDate, selectedShift, true);
            DataForSummery chartForSummery = new DataForSummery();
            chartForSummery.data = new List<pieData>();

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    chartForSummery.name = dt.Columns["Energy"].ColumnName;
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        pieData pData = new pieData();
                        pData.name = dataRow["MachineID"].ToString();
                        if (Convert.ToDouble(dataRow["Energy"].ToString()) < 0)
                        {
                            pData.y = Convert.ToDouble(dataRow["Energy"].ToString()) * -1;
                            pData.positive = false;
                        }

                        else
                        {
                            pData.y = Convert.ToDouble(dataRow["Energy"].ToString());
                            pData.positive = true;
                        }

                        chartForSummery.data.Add(pData);
                    }

                }
                else
                {
                    chartForSummery.name = "Machine";
                    chartForSummery.data = new List<pieData>
                    {
                        new pieData{ name="FANUC", y=286}
                    };
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

            return chartForSummery;
        }
        #endregion

        #region "Get Bar chart for Energy measure"
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static DataForEnergy GetEnergyBarData(string fromDate, string selectedShift)
        {
            DateTime fromdateTime = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            fromDate = fromdateTime.ToString("yyyy-MM-dd");
            DataTable dt = DataBaseAccess.GetDataForDateShift(fromDate, selectedShift, true);
            DataForEnergy chartForEnergy = new DataForEnergy();
            try
            {
                chartForEnergy.TitleEnergy = "Energy Graph";
                chartForEnergy.xAxisTitleEnergy = "Machine";
                chartForEnergy.yAxisTitleEnergy = "Energy(KwH)";

                chartForEnergy.EnergyCategories = new List<string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        chartForEnergy.EnergyCategories.Add(dataRow[0].ToString());
                    }
                    chartForEnergy.seriesEnergy = new List<ChartSeries>();
                    ChartSeries series1 = new ChartSeries();
                    ChartSeries series2 = new ChartSeries();
                    series1.name = dt.Columns["Energy"].ColumnName;
                    series2.name = dt.Columns["Target"].ColumnName;
                    series1.data = new List<double>();
                    series2.data = new List<double>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        series1.data.Add(Convert.ToDouble(dt.Rows[i]["Energy"].ToString()));
                        series2.data.Add(Convert.ToDouble(dt.Rows[i]["Target"].ToString()));
                    }
                    chartForEnergy.seriesEnergy.Add(series1);
                    chartForEnergy.seriesEnergy.Add(series2);
                }
                else
                {
                    chartForEnergy.seriesEnergy = new List<ChartSeries>
                    {
                        new ChartSeries{ name="Energy", data=new List<double>{34,54,35,23,53,23,32,35,75,76,56,36,76,86,34,54,35,23,53,23,32,35,75,76,89}},

                    };
                    for (int i = 0; i <= 24; i++)
                    {
                        chartForEnergy.EnergyCategories.Add("FANUC");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

            return chartForEnergy;
        }
        #endregion

        protected void gvDashboard_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
    public class HourwiseMonitoringData
    {
        public string ShiftHourID { get; set; }
        public string UtilisedTime { get; set; }
        public string Components { get; set; }
        public string PF { get; set; }
        public string Cost { get; set; }
        public string Energy { get; set; }
        public string Volt1 { get; set; }
        public string Volt2 { get; set; }
        public string Volt3 { get; set; }
        public string Target { get; set; }
    }
    public class ChartSeries
    {
        public string name { get; set; }
        public List<double> data { get; set; }
    }
    public class DataForEnergy
    {
        public string TitleEnergy { get; set; }
        public string xAxisTitleEnergy { get; set; }
        public string yAxisTitleEnergy { get; set; }
        public int minValueEnergy { get; set; }
        public int maxValueEnergy { get; set; }

        public List<string> EnergyCategories { get; set; }
        public List<ChartSeries> seriesEnergy { get; set; }
    }
    public class pieData
    {
        public string name { get; set; }
        public double y { get; set; }
        public bool positive { get; set; }
    }
    public class DataForSummery
    {
        public string name { get; set; }
        public List<pieData> data { get; set; }
    }
}