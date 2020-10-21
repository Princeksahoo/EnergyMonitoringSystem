using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnergyMonitoringSystem.Models
{
    public class DTO
    {
    }
    public class GridSettings
    {
        public string ColumnName { get; set; }
        public string ColumnText { get; set; }
        public bool Visibility { get; set; }
    }
    public class LiveDataCs
    {
        public string Machineid { get; set; }
        public string DateTime { get; set; }
        public string VLN_R { get; set; }
        public string VLN_Y { get; set; }
        public string VLN_B { get; set; }
        public string R_AMP { get; set; }
        public string Y_AMP { get; set; }
        public string B_AMP { get; set; }
        public string PowerFactor { get; set; }
        public string Kw { get; set; }
        public string Kwh { get; set; }
        public string LastArrival_TS { get; set; }
    }
}