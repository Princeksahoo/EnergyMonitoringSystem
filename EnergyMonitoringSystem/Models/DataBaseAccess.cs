using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Elmah;
using EnergyMonitoringSystem.Models;

namespace EnergyMonitoringSystem.Models
{
    public class DataBaseAccess
    {
        internal static List<string> GetAllShift()
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            List<string> shiftList = new List<string>();
            try
            {
                SqlCommand cmd = new SqlCommand(@"select * from shiftDetails where running = 1", sqlConn);
                shiftList.Add("All");
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    shiftList.Add(rdr["shiftName"].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return shiftList;
        }

        internal static UserDetails GetEmployeeDetails(string value)
        {
            UserDetails userDetails = new UserDetails();
            SqlConnection conn = ConnectionManager.GetConnection();
            string query = @"select Name,upassword,isadmin from [employeeinformation] where [Employeeid]=@emp";
            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@emp", value);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    userDetails.UserID = rdr["Name"].ToString();
                    userDetails.Password = rdr["upassword"].ToString();
                    if (rdr["isadmin"].ToString().Equals("1", StringComparison.OrdinalIgnoreCase))
                        userDetails.IsAdmin = true;
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return userDetails;
        }

        internal static string GetLogicalDay(string selectedTime)
        {
            string list = string.Empty;
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "select [dbo].[f_GetLogicalDayStart](' " + selectedTime + "') as logicalDay";
                cmd = new SqlCommand(sqlQuery, conn);
                //cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 120;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (!Convert.IsDBNull(sdr["logicalDay"]))
                        {
                            list = DateTime.Parse((sdr["logicalDay"]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                else
                {
                    list = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in Retriving Machines - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return list;
        }
        internal static string GetLogicalDayEnd(string selectedTime)
        {
            string list = string.Empty;
            SqlDataReader sdr = null;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "select [dbo].[f_GetLogicalDayEnd](' " + selectedTime + "') as logicalDay";
                cmd = new SqlCommand(sqlQuery, conn);
                //cmd.CommandType = System.Data.CommandType.Text;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        if (!Convert.IsDBNull(sdr["logicalDay"]))
                        {
                            list = DateTime.Parse((sdr["logicalDay"]).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                else
                {
                    list = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog("Error in Retriving Machines - \n" + ex.ToString());
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
                if (conn != null) conn.Close();
            }
            return list;
        }
        internal static List<string> GetAllMachines(string plantId)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<string> machineList = new List<string>();
            try
            {
                cmd = new SqlCommand(@"s_GetLookups", sqlConn);

                if (!string.IsNullOrEmpty(plantId))
                {
                    if (plantId.Equals("All")) plantId = string.Empty;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", "Machine");
                    cmd.Parameters.AddWithValue("@filter", plantId);
                }
                else
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", "Machine");
                }

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    machineList.Add(rdr["machineId"].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return machineList;
        }
        internal static DataTable GetDataForDateShift(string dateVal, string shiftVal, bool isLiveData)
        {
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            DataTable table = new DataTable();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "s_GetEnergyData";
                cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                if (shiftVal.Equals("First"))
                {
                    shiftVal = "A";
                }
                else if (shiftVal.Equals("Second"))
                {
                    shiftVal = "B";
                }
                else if (shiftVal.Equals("Third"))
                {
                    shiftVal = "C";
                }

                cmd.Parameters.AddWithValue("@dDate", dateVal);
                if (shiftVal.Equals("All"))
                {
                    cmd.Parameters.AddWithValue("@Shift", "");
                    cmd.Parameters.AddWithValue("@Parameter", "Day");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Shift", shiftVal);
                    cmd.Parameters.AddWithValue("@Parameter", "Shift");
                }
                cmd.Parameters.AddWithValue("@MachineID", "");
                cmd.Parameters.AddWithValue("@PlantID", "");
                cmd.Parameters.AddWithValue("@View", "Technodashboard");

                if (isLiveData)
                {
                    cmd.Parameters.AddWithValue("@HistoryLive", "Live");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@HistoryLive", "");
                }

                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                table.Load(reader);
                table.AcceptChanges();
                reader.Close();
            }
            catch (Exception ex)
            {

                Logger.WriteErrorLog(ex.Message);
            }

            if (sqlConn != null) sqlConn.Close();
            return table;
        }
        internal static List<GridSettings> GetGridInformation()
        {
            List<GridSettings> gridSettings = new List<GridSettings>();
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlDataReader rdr = null;
            string query = @"Select ValueInText,ValueInText2,ValueInInt from ShopDefaults where Parameter = @Parameter";
            try
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Parameter", "EM_DataGridColumnVals");
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GridSettings grid = new GridSettings();
                    grid.ColumnName = rdr["ValueInText"].ToString();
                    grid.ColumnText = rdr["ValueInText2"].ToString();
                    if (rdr["ValueInInt"].ToString().Equals("0"))
                        grid.Visibility = false;
                    else
                        grid.Visibility = true;
                    gridSettings.Add(grid);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return gridSettings;
        }
        internal static List<string> GetAllPlants()
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            List<string> plantid = new List<string>();
            try
            {
                SqlCommand cmd = new SqlCommand(@"s_GetLookups", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", "Plant");
                plantid.Add("All");
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    plantid.Add(rdr["plantid"].ToString());
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return plantid;
        }
        internal static DataTable GetEnergyTargetDetails(string PlantID)
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            DataTable table = new DataTable();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                if (PlantID == "All")
                {
                    PlantID = "";
                }
                sqlQuery = @"select M.Machineid,ET.[Target] from plantmachine PM inner join machineinformation M on PM.MachineID= M.machineid left outer join Energy_Target ET on  M.MachineID = ET.MachineID               where (PlantID=@plantID or @plantid='')";
                cmd = new SqlCommand(sqlQuery, conn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@plantid", PlantID);
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                table.Load(reader);
                table.AcceptChanges();
                reader.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return table;
        }
        internal static void SaveEnergyTargetDetails(string MachineID, string Target, out bool isSuccessfull)
        {
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            isSuccessfull = false;


            try
            {
                sqlQuery = @"if not exists(select * from Energy_Target  where MachineID=@MachineID)
                    BEGIN
                    insert into Energy_Target(MachineID,Target)
                    Values(@MachineID,@Target)
                    END
                else
                    BEGIN
                    update Energy_Target set [Target]=@Target where MachineID=@MachineID
                    END";
                cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@MachineID", MachineID);
                if (Target.Equals(""))
                    cmd.Parameters.AddWithValue("@Target", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Target", Target);

                cmd.CommandTimeout = 120;


                int ret = cmd.ExecuteNonQuery();
                isSuccessfull = true;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }
        internal static int SaveGridColumnsSettingsVals(string parameter, string headerText, string gridHeaderVal, bool checkState)
        {
            int count = 0;
            SqlConnection conn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            string query = @"if not exists(select * from ShopDefaults where Parameter=@Parameter and ValueInText=@ValueInText)
                                begin
                                    INSERT INTO [dbo].[ShopDefaults]([Parameter],[ValueInText] ,[ValueInText2] ,[ValueInInt]) VALUES (@Parameter,@ValueInText,@ValueInText2,@ValueInInt)
                                end
                            else
                                begin
                                    update ShopDefaults set ValueInText2=@ValueInText2, ValueInInt=@ValueInInt where Parameter=@Parameter and ValueInText=@ValueInText
                                end";
            try
            {
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@ValueInText", headerText);
                cmd.Parameters.AddWithValue("@ValueInText2", gridHeaderVal);
                cmd.CommandTimeout = 120;
                if (checkState)
                {
                    cmd.Parameters.AddWithValue("@ValueInInt", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ValueInInt", 0);
                }

                count = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
            return count;
        }
        internal static DataTable GetMonthWiseEnergyDataToExport(string startDate, string endDate, string plantId, string selectedMachines, string shiftId, string param)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<string> machineList = new List<string>();
            try
            {
                cmd = new SqlCommand(@"s_GetMonthwiseEnergyData", sqlConn); // s_GetMonthwiseEnergyData
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(@"param", param);
                cmd.Parameters.AddWithValue(@"startTime", startDate);
                cmd.Parameters.AddWithValue(@"endTime", endDate);
                cmd.Parameters.AddWithValue(@"machineId", selectedMachines);
                cmd.CommandTimeout = 120;
                if (plantId.Equals("All")) plantId = string.Empty;
                cmd.Parameters.AddWithValue(@"plantId", plantId);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    dt.Load(rdr);
                    dt.AcceptChanges();
                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return dt;
        }
        internal static DataTable GetEnergyDataToExport(string startDate, string endDate, string plantId, string selectedMachines, string shiftId, string param)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            List<string> machineList = new List<string>();
            try
            {
                cmd = new SqlCommand(@"S_GetEnergyCockpit_CuttingDetails", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Param", param);
                cmd.Parameters.AddWithValue("@Startdate", startDate);
                cmd.Parameters.AddWithValue("@Enddate", endDate);
                cmd.Parameters.AddWithValue("@MachineId", selectedMachines);
                cmd.CommandTimeout = 120;
                if (plantId.Equals("All")) plantId = string.Empty;
                cmd.Parameters.AddWithValue("@PlantId", plantId);


                if (shiftId.Equals("All")) shiftId = string.Empty;
                cmd.Parameters.AddWithValue("@Shift", shiftId);
                //cmd.Parameters.AddWithValue(@"type", "technoreport");
                cmd.CommandTimeout = 3000;

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    dt.Load(rdr);
                    dt.AcceptChanges();
                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return dt;
        }
        internal static List<LiveDataCs> GetDataLiveData(string dayVal, string liveHistory, string viewParam)
        {
            List<LiveDataCs> liveDatas = new List<LiveDataCs>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            DataTable table = new DataTable();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "s_GetEnergyData";
                cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue(@"dDate", "");
                cmd.Parameters.AddWithValue(@"Shift", "");
                cmd.Parameters.AddWithValue(@"MachineID", "");
                cmd.Parameters.AddWithValue(@"PlantID", "");
                cmd.Parameters.AddWithValue(@"Parameter", dayVal);
                cmd.Parameters.AddWithValue(@"View", viewParam);
                cmd.Parameters.AddWithValue(@"HistoryLive", liveHistory);

                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        LiveDataCs liveData = new LiveDataCs();
                        liveData.Machineid = reader["machineid"].ToString();
                        liveData.DateTime = reader["LastArrivalTime"].ToString();
                        liveData.VLN_R = reader["V1"].ToString();
                        liveData.VLN_Y = reader["V2"].ToString();
                        liveData.VLN_B = reader["V3"].ToString();
                        liveData.R_AMP = reader["AR"].ToString();
                        liveData.Y_AMP = reader["AY"].ToString();
                        liveData.B_AMP = reader["AB"].ToString();
                        liveData.PowerFactor = reader["PF"].ToString();
                        liveData.Kw = reader["KW"].ToString();
                        liveData.Kwh = reader["KWH"].ToString();
                        liveData.LastArrival_TS = reader["LastArrivalTime"].ToString();

                        liveDatas.Add(liveData);
                    }
                }
                table.Load(reader);
                table.AcceptChanges();
                reader.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return liveDatas;
        }
        internal static DataTable GetEnergyData(string fromDateTime, string toDateTime, string machineId)
        {
            List<DateTime> timeStamps = new List<DateTime>();
            DataTable dt = new DataTable();

            SqlConnection sqlConn = ConnectionManager.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Select gtime,watt,AmpereR,AmpereY, AmpereB,pf,Volt1,Volt2,Volt3 from tcs_energyconsumption where gtime >= @gtimeStart  and gtime <= @gtimeEnd  and machineID =@machineId order by gtime ", sqlConn);
                cmd.Parameters.AddWithValue("@gtimeStart", fromDateTime);//"2015-01-20 6:00:00");//
                cmd.Parameters.AddWithValue("@gtimeEnd", toDateTime);//"2015-01-20 14:00:00");//
                cmd.Parameters.AddWithValue("@Machineid", machineId);
                cmd.CommandTimeout = 120;
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    dt.Load(sdr);
                    dt.AcceptChanges();
                }

                sdr.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.ToString());
                throw;
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return dt;
        }
        internal static List<HourwiseMonitoringData> GetHourWiseEnergyData(string machineId, string dateVal, string shiftVal, bool liveData)
        {
            List<HourwiseMonitoringData> hourwiseMonitoringDatas = new List<HourwiseMonitoringData>();
            SqlConnection sqlConn = ConnectionManager.GetConnection();
            SqlCommand cmd = null;
            string sqlQuery = string.Empty;
            try
            {
                sqlQuery = "s_GetEnergyData";
                cmd = new SqlCommand(sqlQuery, sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue(@"dDate", dateVal);
                cmd.Parameters.AddWithValue(@"Shift", shiftVal);
                cmd.Parameters.AddWithValue(@"Parameter", "Hour");

                cmd.Parameters.AddWithValue(@"MachineID", machineId);
                cmd.Parameters.AddWithValue(@"PlantID", "");
                cmd.Parameters.AddWithValue(@"View", "Technodashboard");

                if (liveData)
                {
                    cmd.Parameters.AddWithValue(@"HistoryLive", "Live");
                }
                else
                {
                    cmd.Parameters.AddWithValue(@"HistoryLive", "");
                }

                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        HourwiseMonitoringData data = new HourwiseMonitoringData();
                        data.ShiftHourID = reader["ShiftHourID"].ToString();
                        data.Volt1 = reader["Volt1"].ToString();
                        data.Volt2 = reader["Volt2"].ToString();
                        data.Volt3 = reader["Volt3"].ToString();
                        data.PF = reader["PF"].ToString();
                        data.Components = reader["Components"].ToString();
                        data.UtilisedTime = reader["UtilisedTime"].ToString();
                        data.Energy = reader["Energy"].ToString();
                        data.Cost = reader["Cost"].ToString();
                        data.Target = reader["Target"].ToString();
                        hourwiseMonitoringDatas.Add(data);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
            finally
            {
                if (sqlConn != null) sqlConn.Close();
            }
            return hourwiseMonitoringDatas;
        }
    }
}