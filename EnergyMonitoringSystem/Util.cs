using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EnergyMonitoringSystem
{
    public class Util
    {
        static string appPath = HttpContext.Current.Server.MapPath("~/Reports");
        static string[] formats = new string[] { "dd-MM-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm", "dd-MM-yyyy", "dd-MMM-yyyy", "dd-MMM-yyyy HH:mm", "dd-MMM-yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "dd-MM-yyyyTHH:mm:ss", "dd-MM-yyyyTHH:mm", "dd-MMM-yyyyTHH:mm", "dd-MMM-yyyyTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm" };
        public static bool show16losses = false;
        public static DateTime GetDateTime(string strDatetime)
        {
            DateTime datetime = DateTime.Now;
            if (!DateTime.TryParseExact(strDatetime.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out datetime))
            {
                datetime = DateTime.Now;
                Logger.WriteErrorLog(string.Format("Not able to convert datetime string {0} to DateTime", strDatetime));
            }
            return datetime;
        }
        public static string GetReportPath(string reportName)
        {
            string src;
            if (HttpContext.Current.Session["Language"] == null)
                src = Path.Combine(appPath, "ReportTemplates", reportName);
            else
            {
                if (HttpContext.Current.Session["Language"].ToString() != "en")
                    src = Path.Combine(appPath, "ReportTemplates-" + HttpContext.Current.Session["Language"].ToString() + "", reportName);
                else
                    src = Path.Combine(appPath, "ReportTemplates", reportName);
            }
            return src;
        }
       
        public static void SetCultureForThread()
        {
            string culture = System.Globalization.CultureInfo.CurrentCulture.Name;
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["Language"] != null)
            {
                culture = Convert.ToString(HttpContext.Current.Session["Language"]);
            }
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        public static string splitfunction(string ID)
        {
            string value = "";
            string[] IDs = ID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < IDs.Length; i++)
            {
                value = value + @"'" + IDs[i] + @"',";
            }
            value = value.TrimEnd(',');
            return value;
        }
    }


    public static class ExtentionMethods
    {
        public static string ToSQLDateTimeFormat(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToSQLDateFormat(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static Boolean IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, Boolean compareTime = false)
        {
            return compareTime ?
               dt >= startDate && dt <= endDate :
               dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }



    }
}