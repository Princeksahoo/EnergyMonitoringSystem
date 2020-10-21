using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EnergyMonitoringSystem
{
    public partial class Logout : System.Web.UI.Page
    {
        static int tick;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            tick = Timer1.Interval;
            Session.Clear();
            Session.Abandon();
            Response.Cache.SetExpires(DateTime.Now.AddMinutes(-60));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            try
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Buffer = true;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
                Response.Expires = -1000;
                Response.CacheControl = "no-cache";
                //Response.Redirect("login.aspx", true);       
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            if (tick >= 1000)
            {
                Response.Redirect("~/SignIn.aspx", false);
            }
        }
    }
}