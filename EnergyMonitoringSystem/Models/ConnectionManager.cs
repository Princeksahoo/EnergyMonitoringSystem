using Elmah;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace EnergyMonitoringSystem
{
    public class ConnectionManager
    {
        #region "Create Connection String---"
        static string conString = WebConfigurationManager.ConnectionStrings["TPMTrakConnString"].ToString();
        public static int refreshData = Convert.ToInt32(WebConfigurationManager.AppSettings["autoRefreshInterval"].ToString());
        public static bool timeOut = false;
        public static SqlConnection GetConnection()
        {
            bool writeDown = false;
            DateTime dt = DateTime.Now;
            SqlConnection conn = null;
            if (HttpContext.Current == null || HttpContext.Current.Session == null || HttpContext.Current.Session["connectionString"] == null)
            {
                conn = new SqlConnection(conString);
            }
            else
            {
                conString = HttpContext.Current.Session["connectionString"] as string;
                conString = WebConfigurationManager.ConnectionStrings[conString].ToString();
                conn = new SqlConnection(conString);
            }
            do
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    if (writeDown == false)
                    {
                        dt = DateTime.Now.AddSeconds(60);
                        Logger.WriteErrorLog(ex.Message);
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        writeDown = true;

                    }
                    if (dt < DateTime.Now)
                    {
                        Logger.WriteErrorLog(ex.Message);
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        throw;
                    }
                    Thread.Sleep(1000);
                }

            } while (conn.State != ConnectionState.Open);
            return conn;
        }
        #endregion
    }
}