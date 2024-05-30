
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Web.Mvc;

namespace TranzmitUI.Models
{
    public class SAutoID
    {
        public string SID { get; set; }
        public string OID { get; set; }
    }
    public class ScheduleMaster
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ScheduleMaster));
        public List<SelectListItem> RSelect { get; set; }
        public string RName { get; set; }

        
        public string RepCode { get; set; }
        public string Profile { get; set; }
        public string ScheduleType { get; set; }
        public string Flag { get; set; }
        public string ScheduleCode { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string ScheduleDate { get; set; }


        [Required(ErrorMessage = "Please fill the detail")]
        public string ProfileType { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string Group { get; set; }


        [Required(ErrorMessage = "Please fill the detail")]
        public string ScheduleTime { get; set; }
        public string DestinationPath { get; set; }
        public string DeployDate { get; set; }
        public string DeployTime { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string TransferRate { get; set; }
        public string SetupFile { get; set; }
        public List<SelectListItem> DeviceSelect { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string DeviceName { get; set; }
        public string DeviceIP { get; set; }

        public string DevicePort { get; set; }
        public string DeviceID { get; set; }
        public List<SelectListItem> LocationSelect { get; set; }
        public string LocationName { get; set; }
        public string LocationId { get; set; }
        public List<SelectListItem> RepSelect { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string RepName { get; set; }
        public string RepID { get; set; }
        public List<SelectListItem> DirSelect { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string DirName { get; set; }
        public string DirID { get; set; }



        public string GroupName { get; set; }

        public string GroupOfDevice { get; set; }
        public string GroupCode { get; set; }
        public string Schedule { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string ObjID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string DirCode { get; set; }


        public List<SelectListItem> GroupSelect { get; set; }

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

       

        public List<SelectListItem> PopulateDevices()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = " SELECT device_ip || ',' || device_port device_ip,device_id FROM device_master where obj_id = '"+UserOraganization+"'";
                    }
                    else
                    {
                        query = " SELECT device_ip || ',' || device_port device_ip,device_id FROM device_master";

                    }

                    log.Info("the populatedevices query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Schedulemaster in populatedevices");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["device_ip"].ToString(),
                                    Value=sdr["device_id"].ToString()
                                    
                                });
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return items;
        }
        public List<SelectListItem> PopulateLocation()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = " SELECT location_id,location_name FROM location_master  where  obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT location_id,location_name FROM location_master";
                    }
                    log.Info("the populatelocation query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Device master in populatelocation ");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["location_name"].ToString(),

                                    Value = sdr["location_id"].ToString()
                                });
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return items;
        }

        public List<SelectListItem> PopulateGroup()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "SELECT profile_code,profile_name FROM profile_master where obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "SELECT profile_code,profile_name FROM profile_master";
                    }

                    log.Info("the  query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["profile_name"].ToString(),
                                    Value = sdr["profile_code"].ToString()

                                });
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return items;
        }


       
        public List<SelectListItem> PopulateDirectory()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = " SELECT dir_code,dir_path FROM directory_master where obj_id = '"+UserOraganization+"'";
                    }
                    else
                    {
                        query = " SELECT dir_code,dir_path FROM directory_master";
                    }
                    log.Info("the populatedirectory query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Schedulemaster in populatedirectory");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["dir_path"].ToString(),
                                    Value = sdr["dir_code"].ToString()
                                });
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return items;
        }

        public List<SelectListItem> PopulateRepositoryPath()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = " SELECT repcode,reppath FROM repository_master where obj_id = '"+UserOraganization+"'";
                    }
                    else
                    {
                        query = " SELECT repcode,reppath FROM repository_master";
                    }
                    log.Info("the populate respository path query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Schedulemaster in populate respository path");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["repcode"].ToString(),
                                    Value = sdr["reppath"].ToString()
                                });
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return items;
        }
        public List<SelectListItem> PopulateRepository()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = " SELECT repcode,repname FROM repository_master where obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT repcode,repname FROM repository_master";
                    }
                    log.Info("the populate respoitory query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Schedulemaster in populaterespository");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["repname"].ToString(),
                                    Value = sdr["repcode"].ToString()
                                });
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return items;
        }

        public List<ScheduleMaster> ScheduleList()
        {
            List<TranzmitUI.Models.ScheduleMaster> schedules = new List<Models.ScheduleMaster>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "select s.obj_id,d.device_ip,d.device_port,s.repcode,s.schedule_type,s.flag,s.schedule_code,s.schedule_date,s.schedule_time,s.destination_dir,s.setup_file,s.deploy_date," +
                   "s.deploy_time,s.profile_code,s.profile_type from schedule_master s LEFT JOIN device_master d ON  s.obj_id=d.device_id ORDER BY schedule_code DESC where s.user_id = '" + UserOraganization + "'";
                }
                else
                {
                    query = "select s.obj_id,d.device_ip,d.device_port,s.repcode,s.schedule_type,s.flag,s.schedule_code,s.schedule_date,s.schedule_time,s.destination_dir,s.setup_file,s.deploy_date," +
                 "s.deploy_time,s.profile_code,s.profile_type from schedule_master s LEFT JOIN device_master d ON  s.obj_id=d.device_id ORDER BY schedule_code DESC";
                }
                log.Info("the schedulelist query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Schedulemaster in schedulelist");
                dataAdapter.Fill(dataTable);
                connection.Close();
                foreach (DataRow dr in dataTable.Rows)
                {
                    schedules.Add(new Models.ScheduleMaster
                    {
                        ProfileType = Convert.ToString(dr["profile_type"]),
                        Group = Convert.ToString(dr["profile_code"]),
                        ObjID = Convert.ToString(dr["obj_id"]),
                        DeviceIP = Convert.ToString(dr["device_ip"]),
                        ScheduleType = Convert.ToString(dr["schedule_type"]),
                        Flag = Convert.ToString(dr["flag"]),
                        ScheduleCode = Convert.ToString(dr["schedule_code"]),
                        ScheduleDate = Convert.ToString(dr["schedule_date"]),
                        ScheduleTime = Convert.ToString(dr["schedule_time"]),
                        DevicePort = Convert.ToString(dr["device_port"]),
                        SetupFile = Convert.ToString(dr["setup_file"]),
                        DeployDate = Convert.ToString(dr["deploy_date"]),
                        DeployTime = Convert.ToString(dr["deploy_time"]),
                        RepCode = Convert.ToString(dr["repcode"]),
                        DirCode = Convert.ToString(dr["destination_dir"])
                    });
                    
                }
            }
            catch (Exception ex)
            {

            }
            return schedules;
        }

        public int  ScheduleInsert(ScheduleMaster schedule)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

           
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    connection.Open();
                    log.Info("Connection Opened Successfully for Schedulemaster in scheduleinsert");
                    string query = "insert into schedule_master(profile_code,profile_type,obj_id,repcode,destination_dir,schedule_type," +
                        "schedule_code,schedule_date,schedule_time,flag,deploy_date,deploy_time," +
                        "compress_ratio,destination_folder,setup_file,user_id)" +
                        "values('"+schedule.Group+ "','"+schedule.ProfileType+"','" + schedule.ObjID+"','"+schedule.RepCode+"','"+schedule.DirCode+"'," +
                        "'"+schedule.ScheduleType+"','"+schedule.ScheduleCode+"','"+schedule.ScheduleDate+"'," +
                        "'"+schedule.ScheduleTime+"','"+schedule.Flag+"','"+schedule.DeployDate+"','"+schedule.DeployTime+"'," +
                        "'"+schedule.TransferRate+"','"+schedule.DestinationPath+"','"+schedule.SetupFile+ "','"+UserOraganization+"')";
                    log.Info("the scheduleinsert query result is:" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        count = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return count;
        }

        public int UpdateSchedule(ScheduleMaster update, string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

          
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                {
                    connection.Open();
                    log.Info("Connection Opened Successfully for Schedulemaster in updateschedule");
                    string query = "update schedule_master set repcode='" + update.RepCode + "'," +
                        "schedule_type='" + update.ScheduleType + "',schedule_date='" + update.ScheduleDate + "'," +
                        "schedule_time='" + update.ScheduleTime + "',deploy_date='" + update.DeployDate + "'," +
                        "deploy_time='" + update.DeployTime + "',destination_dir='" + update.DirCode + "'," +
                        "setup_file='" + update.SetupFile + "',flag='" + update.Flag + "',obj_id='"+update.ObjID+"'  where schedule_code='" + id + "' and user_id = '"+UserOraganization+"'";
                    log.Info("the updateschedule query result is:" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        count=command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }
            return count;
        }

        public void DeleteSchedule(string id)
        {
            OracleConnection connection = new OracleConnection(connectionstring);
            {
                connection.Open();
                log.Info("Connection Opened Successfully for Schedulemaster in deleteschedule");
                string query = "Delete  from schedule_master where schedule_code='" + id + "'";
                log.Info("the delete schedule query result is:" + query);
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void ScheduleRestart(string id)
        {
            List<TranzmitUI.Models.ScheduleMaster> schedules = new List<Models.ScheduleMaster>();
            try
            {
                string profile = string.Empty;
                string repcode = string.Empty;
                string scheduletype = string.Empty;
                string flag = string.Empty;
                string scheduledate = string.Empty;
                string scheduletime = string.Empty;
                string dircode = string.Empty;
                string setupfile = string.Empty;
                string dploydate = string.Empty;
                string deploytime = string.Empty;
                string destination = string.Empty;
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = "select profile_code,repcode,schedule_type,flag,schedule_date," +
                    "schedule_time,destination_dir,setup_file,deploy_date,deploy_time,schedule_code,destination_folder from schedule_master where schedule_code='" + id + "' ";
                log.Info("the schedulerestart query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Schedulemaster in schedulerestart");
                dataAdapter.Fill(dataTable);
                connection.Close();
                foreach (DataRow dr in dataTable.Rows)
                {
                    profile = Convert.ToString(dr["profile_code"]);
                    repcode = Convert.ToString(dr["repcode"]);
                    scheduletype = Convert.ToString(dr["schedule_type"]);
                    flag = Convert.ToString(dr["flag"]);
                    scheduledate = Convert.ToString(dr["schedule_date"]);
                    scheduletime = Convert.ToString(dr["schedule_time"]);
                    dircode = Convert.ToString(dr["destination_dir"]);
                    setupfile = Convert.ToString(dr["setup_file"]);
                    dploydate = Convert.ToString(dr["deploy_date"]);
                    deploytime = Convert.ToString(dr["deploy_time"]);
                    destination = Convert.ToString(dr["destination_folder"]);
                }
                SAutoID GetSAuto = new SAutoID();
                using (OracleConnection connectionss = new OracleConnection(connectionstring))
                {
                    string locationss = "select count(*) from schedule_master";
                    log.Info("the schedulerestart  result is:" + locationss);
                    using (OracleCommand commandss = new OracleCommand(locationss, connectionss))
                    {
                        connectionss.Open();
                        int count = Convert.ToInt16(commandss.ExecuteScalar()) + 1;
                        if (count < 10)
                        {
                            GetSAuto.SID = "SCH" + "00" + count;
                        }
                        else if (count > 10 && count < 100)
                        {
                            GetSAuto.SID = "SCH" + "0" + count;
                        }
                        else
                        {
                            GetSAuto.SID = "SCH" + count;
                        }
                    }
                }
                 string scheduleCode = GetSAuto.SID;
                string transferrate = "1024";
                using (OracleConnection connections = new OracleConnection(connectionstring))
                {
                    connections.Open();
                    string querys = "insert into schedule_master(repcode,destination_dir,schedule_type," +
                        "schedule_code,schedule_date,schedule_time,flag,deploy_date,deploy_time," +
                        "compress_ratio,destination_folder,setup_file)" +
                        "values('" + repcode+ "','" + dircode + "'," +
                        "'" + scheduletype + "','" + scheduleCode + "','" + scheduledate + "'," +
                        "'" + scheduletime + "','" + flag + "','" + dploydate + "','" + deploytime + "'," +
                        "'" + transferrate + "','" + destination + "','" + setupfile + "')";
                    log.Info("the schedulerestart result is:" + querys);
                    using (OracleCommand commands = new OracleCommand(querys, connections))
                    {
                        commands.ExecuteNonQuery();
                    }
                    connections.Close();
                }
                OracleConnection connectionsss = new OracleConnection(connectionstring);
                {
                    connectionsss.Open();
                    string querysss = "Delete  from device_transmission where schedule_code='" + id + "'";
                    log.Info("the schedulerestart  result is:" + querysss);
                    using (OracleCommand commandsss = new OracleCommand(querysss, connectionsss))
                    {
                        commandsss.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}