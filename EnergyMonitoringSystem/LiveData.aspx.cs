using EnergyMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EnergyMonitoringSystem
{
    public partial class LiveData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindLiveDataGrid();
            }
        }

        private void BindLiveDataGrid()
        {
            List<LiveDataCs> liveDatas = DataBaseAccess.GetDataLiveData("day", "live", "Technolivescreen");
            gvLiveData.DataSource = liveDatas;
            gvLiveData.DataBind();
        }

        protected void timerToAutoRefresh_Tick(object sender, EventArgs e)
        {
            BindLiveDataGrid();
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
            BindLiveDataGrid();
        }
    }
}