using EnergyMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EnergyMonitoringSystem
{
    public partial class Settings : System.Web.UI.Page
    {
        List<string> allplantid = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSettingsInformations();
            }
        }

        private void BindSettingsInformations()
        {
            BindGridSettingsInformations();
            BindTargetSettingsInformations();

        }

        private void BindTargetSettingsInformations()
        {
            try
            {
                allplantid = DataBaseAccess.GetAllPlants();
                if (allplantid.Count > 0)
                {
                    ddlPlant.DataSource = allplantid;
                    ddlPlant.DataBind();
                    ddlPlant.SelectedIndex = 0;
                }
                if (string.IsNullOrEmpty(ddlPlant.Text.ToString())) return;
                DataTable dt = DataBaseAccess.GetEnergyTargetDetails(ddlPlant.Text.ToString());
                foreach (DataColumn col in dt.Columns)
                {
                    col.ReadOnly = false;
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    gvTargetSettings.DataSource = dt;
                    gvTargetSettings.DataBind();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

        }

        private void BindGridSettingsInformations()
        {
            List<GridSettings> gridSettings = DataBaseAccess.GetGridInformation();
            gvGridColumnSettings.DataSource = gridSettings;
            gvGridColumnSettings.DataBind();
        }

        protected void btnUpdateTarget_Click(object sender, EventArgs e)
        {
            bool isSuccessFailure = false;
            try
            {
                foreach (GridViewRow row in gvTargetSettings.Rows)
                {
                    string machineid = (row.FindControl("lblMachineId") as Label).Text;
                    string target = (row.FindControl("txtKwh") as TextBox).Text;
                    DataBaseAccess.SaveEnergyTargetDetails(machineid, target, out isSuccessFailure);
                }
                BindTargetSettingsInformations();
                if (!isSuccessFailure)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageNotOk", "messageTargetNotOk();", true);
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "messageTargetOk();", true);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        protected void btnUpdateColVisibility_Click(object sender, EventArgs e)
        {
            int count = 0;
            try
            {
                foreach (GridViewRow row in gvGridColumnSettings.Rows)
                {
                    string columnName = (row.FindControl("lblGridColumn") as Label).Text;
                    string columnText = (row.FindControl("txtColumnText") as TextBox).Text;
                    bool visibility = (row.FindControl("cbVisibility") as CheckBox).Checked;
                    count = DataBaseAccess.SaveGridColumnsSettingsVals("EM_DataGridColumnVals", columnName, columnText, visibility);
                }
                BindGridSettingsInformations();
                if (count < 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messageNotOk", "messageGridNotOk();", true);
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "messageGridOk();", true);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }

        }


        protected void gvTargetSettings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox textBox = e.Row.FindControl("txtKwh") as TextBox;
                textBox.TextMode = TextBoxMode.Number;

            }
        }
    }
}