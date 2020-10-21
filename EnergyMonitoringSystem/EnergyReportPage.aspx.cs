using EnergyMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EnergyMonitoringSystem
{
    public partial class EnergyReportPage : System.Web.UI.Page
    {
        string isGenerated = "NotGenerated";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFromDate.Text = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtFromDateTime.Text = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy HH:mm:ss");
                txtToDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                txtFromMonth.Text = DateTime.Now.AddMonths(-1).ToString("MMM-yyyy");
                txtToMonth.Text = DateTime.Now.ToString("MMM-yyyy");

                ddlPlantId.DataSource = DataBaseAccess.GetAllPlants();
                ddlPlantId.DataBind();
                ddlShift.DataSource = DataBaseAccess.GetAllShift();
                ddlShift.DataBind();

                ddlPlantId_SelectedIndexChanged(null, EventArgs.Empty);
                ddlFormat_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        protected void ddlPlantId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMachineIDs.DataSource = DataBaseAccess.GetAllMachines(ddlPlantId.SelectedValue);
            ddlMachineIDs.DataBind();
        }

        protected void ddlFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAllTimePeriods();
            if (ddlFormat.SelectedValue.Equals("Day", StringComparison.OrdinalIgnoreCase))
            {
                txtFromDate.Visible = true;
                txtToDate.Visible = true;
            }
            else if (ddlFormat.SelectedValue.Equals("Month", StringComparison.OrdinalIgnoreCase))
            {
                txtFromMonth.Visible = true;
                txtToMonth.Visible = true;
            }
            else if (ddlFormat.SelectedValue.Equals("TimeConsolidated", StringComparison.OrdinalIgnoreCase))
            {
                txtFromDateTime.Visible = true;
                txtToDateTime.Visible = true;
            }
            else
            {
                txtFromDate.Visible = true;
                txtToDate.Visible = true;
                ddlShift.Enabled = true;
            }
        }

        private void HideAllTimePeriods()
        {
            txtFromDate.Visible = false;
            txtFromDateTime.Visible = false;
            txtFromMonth.Visible = false;
            txtToDate.Visible = false;
            txtToDateTime.Visible = false;
            txtToMonth.Visible = false;
            ddlShift.Enabled = false;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            if (ValidateFeilds()) return;
            string selectedMachines = string.Empty;
            if (ddlMachineIDs != null && !ddlMachineIDs.Text.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                //selectedMachines = ddlMachineIDs.Text.Split(new string[] { "," }, StringSplitOptions.None);
                //selectedMachines = selectedMachines.Take(selectedMachines.Count()).ToArray();
                foreach (ListItem item in ddlMachineIDs.Items)
                {
                    if (item.Selected)
                    {
                        if (selectedMachines == string.Empty)
                            selectedMachines = "'" + item.Value + "'";
                        else
                            selectedMachines = selectedMachines + ",'" + item.Value.ToString().Trim() + "'";
                    }
                    //selectedMachines.Add(item.Value);

                }
            }


            DataTable dtEnergyDate = new DataTable();
            try
            {
                string startDate = string.Empty;
                string endDate = string.Empty;
                if (ddlReportType.SelectedValue.Equals("Format - I", StringComparison.OrdinalIgnoreCase))
                {
                    if (ddlFormat.SelectedValue.Equals("Month"))
                    {
                        DateTime fromDate = DateTime.Now.Date;
                        DateTime toDate = DateTime.Now.Date;
                        //startDate = DataBaseAccess.GetLogicalDay(txtFromMonth.Text);
                        //endDate = DataBaseAccess.GetLogicalDayEnd(txtToMonth.Text);
                        fromDate = DateTime.ParseExact(txtFromMonth.Text, "MMM-yyyy", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(txtToMonth.Text, "MMM-yyyy", CultureInfo.InvariantCulture);
                        startDate = fromDate.ToString("yyyy-MM-dd");
                        endDate = toDate.ToString("yyyy-MM-dd");
                        dtEnergyDate = DataBaseAccess.GetMonthWiseEnergyDataToExport(startDate, endDate, ddlPlantId.SelectedValue, selectedMachines, ddlShift.SelectedValue, ddlFormat.SelectedValue);
                    }
                    else if (ddlFormat.SelectedValue.Equals("TimeConsolidated"))
                    {
                        DateTime fromDate = DateTime.Now.Date;
                        DateTime toDate = DateTime.Now.Date;
                        //startDate = DataBaseAccess.GetLogicalDay(txtFromDateTime.Text);
                        //endDate = DataBaseAccess.GetLogicalDayEnd(txtToDateTime.Text);
                        fromDate = DateTime.ParseExact(txtFromDateTime.Text, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(txtToDateTime.Text, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        startDate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                        endDate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                        dtEnergyDate = DataBaseAccess.GetEnergyDataToExport(startDate, endDate, ddlPlantId.SelectedValue, selectedMachines, ddlShift.SelectedValue, ddlFormat.SelectedValue);
                    }
                    else
                    {
                        //startDate = DataBaseAccess.GetLogicalDay(txtFromDate.Text);
                        //endDate = DataBaseAccess.GetLogicalDayEnd(txtToDate.Text);
                        DateTime dateTime1 = DateTime.ParseExact(txtFromDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        DateTime dateTime2 = DateTime.ParseExact(txtToDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        startDate = dateTime1.ToString("yyyy-MM-dd");
                        endDate = dateTime2.ToString("yyyy-MM-dd");
                        dtEnergyDate = DataBaseAccess.GetEnergyDataToExport(startDate, endDate, ddlPlantId.SelectedValue, selectedMachines, ddlShift.SelectedValue, ddlFormat.SelectedValue);
                    }

                    if (dtEnergyDate.Columns.Count <= 0)
                    {
                        isGenerated = "NodataFound";

                    }
                    else
                    {
                        List<GridSettings> gridSettings = DataBaseAccess.GetGridInformation();
                        foreach (GridSettings settings in gridSettings)
                        {
                            if (!settings.Visibility)
                            {
                                if (settings.ColumnName.Equals("Volt1", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("Volt1");
                                else if (settings.ColumnName.Equals("Volt2", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("Volt2");
                                else if (settings.ColumnName.Equals("Volt3", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("Volt3");
                                else if (settings.ColumnName.Equals("PowerFactor", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("PF");
                                else if (settings.ColumnName.Equals("Energy", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("Energy");
                                else if (settings.ColumnName.Equals("ProductionCount", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("Components");
                                else if (settings.ColumnName.Equals("ProductionTime", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("UtilisedTime");
                                else if (settings.ColumnName.Equals("Cost", StringComparison.OrdinalIgnoreCase))
                                    dtEnergyDate.Columns.Remove("Cost");
                            }
                        }
                        isGenerated = ReportGenerator.GenerateEnergyReport(ddlFormat.SelectedValue.ToString(), "Format - I", dtEnergyDate, startDate, endDate, ddlPlantId.SelectedValue.ToString(), selectedMachines, ddlShift.SelectedValue.ToString(), ddlFormat.SelectedValue.ToString());
                    }
                    if (isGenerated.Equals("NotGenerated", StringComparison.OrdinalIgnoreCase))
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageNotOk", "messageNotOk();", true);
                    else if (isGenerated.Equals("NodataFound", StringComparison.OrdinalIgnoreCase))
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageNodata", "messageNodata();", true);
                    else if (isGenerated.Equals("Generated", StringComparison.OrdinalIgnoreCase))
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "messageOk();", true);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

        }
        private bool ValidateFeilds()
        {
            //if (chkListMachine.SelectedItems.Count == 0)
            //{
            //    CustomDialogBox frm = new CustomDialogBox("Error Message", "Please Select Machine ID.");
            //    frm.Show();
            //    chkListMachine.Focus();
            //    return true;
            //}

            if (CompareDates())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('From-Date cannot be greater than To-Date.');", true);
                return true;
            }

            if (CheckDateValues())
            {
                //if (ddlFormat.SelectedValue.Equals("Shift"))
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('Date-Time Period Cannot be greater than 7 Days.');", true);
                //}

                //else if (ddlFormat.SelectedValue.Equals("Month"))
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('Date Period Cannot be greater than 1 Year.');", true);
                //}

                //else
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('Date-Time Period Cannot be greater than 31 Days.');", true);
                //}

                return true;
            }

            return false;
        }

        private bool CompareDates()
        {
            bool isDateGreater = false;
            string dateVal1 = string.Empty;
            string dateVal2 = string.Empty;

            if (ddlFormat.SelectedValue.Equals("Shift") || ddlFormat.SelectedValue.Equals("Day"))
            {
                dateVal1 = txtFromDate.Text.ToString();
                dateVal2 = txtToDate.Text.ToString();
            }

            else if (ddlFormat.SelectedValue.Equals("Month"))
            {
                dateVal1 = txtFromMonth.Text.ToString();
                dateVal2 = txtToMonth.Text.ToString();
            }
            else
            {
                dateVal1 = txtFromDateTime.Text.ToString();
                dateVal2 = txtToDateTime.Text.ToString();
            }

            DateTime dt1 = DateTime.Parse(dateVal1);
            DateTime dt2 = DateTime.Parse(dateVal2);

            if (dt1 > dt2)
            {
                isDateGreater = true;
            }

            return isDateGreater;
        }

        private bool CheckDateValues()
        {
            string dateVal1 = txtFromDate.Text.ToString();
            string dateVal2 = txtToDate.Text.ToString();

            if (ddlFormat.SelectedValue.Equals("Shift") || ddlFormat.SelectedValue.Equals("Day"))
            {
                dateVal1 = txtFromDate.Text.ToString();
                dateVal2 = txtToDate.Text.ToString();
            }

            else if (ddlFormat.SelectedValue.Equals("Month"))
            {
                dateVal1 = txtFromMonth.Text.ToString();
                dateVal2 = txtToMonth.Text.ToString();
            }
            else
            {
                dateVal1 = txtFromDateTime.Text.ToString();
                dateVal2 = txtToDateTime.Text.ToString();
            }


            DateTime dt1 = DateTime.Parse(dateVal1);
            DateTime dt2 = DateTime.Parse(dateVal2);
            var val = (dt2 - dt1).Days;
            if (ddlFormat.SelectedValue.Equals("Shift"))
            {
                if (val > 7)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('Date-Time Period Cannot be greater than 7 Days.');", true);
                    return true;
                }
            }
            else if (ddlFormat.SelectedValue.Equals("Month"))
            {
                if (val > 366)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('Date Period Cannot be greater than 1 Year.');", true);
                    return true;
                }
            }
            else
            {
                if (val > 31)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageWarning", "messageWarning('Date-Time Period Cannot be greater than 31 Days.');", true);
                    return true;
                }
            }

            return false;
        }
    }
}