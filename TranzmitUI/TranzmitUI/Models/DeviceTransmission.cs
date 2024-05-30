using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Mvc;
//using System.Linq;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net;
using System.IO;
using TranzmitUI.Controllers;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TranzmitUI.Models
{
    public class DeviceTransmission
    {
    

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DeviceTransmission));
       
        //[Required(ErrorMessage = "Please fill the detail")]
        //public string FromDate { get; set; }

        //[Required(ErrorMessage = "Please fill the detail")]
        //public string ToDate { get; set; }
        public string DeviceID { get; set; }

        public string DeviceIP { get; set; }

        public string DevicePort { get; set; }

        public string Type { get; set; }

        public string ScheduleDate { get; set; }

        public string ScheduleTime { get; set; }

        public string TransStatus { get; set; }
        public string ScheduleID { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string Status { get; set; }
        public string strDateTime { get; set; }
        public string RepNo { get; set; }

        public string filesize { get; set; }

        public string SNo { get; set; }
        public string Bankname { get; set; }
        public string ATMID { get; set; }
        public string ATMIP { get; set; }
        public string ATMPORT { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }

        public string EJDate { get; set; }

        public string EJStatus { get; set; }

        public string ReceiveDatetime { get; set; }

        public string RescheduleStatus { get; set; }

        public string EJFilePath { get; set; }

        public string EJDownloadDate { get; set; }

        public string EJDownloadUser { get; set; }

        public string UserID { get; set; }
        public string Uid { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string Organization { get; set; }
      
        public string Role { get; set; }

        public static string RescheduleAccessFlag = "N";

        //public string TranzmitEJDate { get; set; }

        public static List<TranzmitUI.Models.DeviceTransmission> TranzmitEJDate { get; set; } = new List<Models.DeviceTransmission>();


        //public string path { get; set; }

        public static List<TranzmitUI.Models.DeviceTransmission> filestatus { get; set; } = new List<Models.DeviceTransmission>();

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        DirectoryInfo directoryInfo = null;
        public static string DT = string.Empty;
        public List<DeviceTransmission> GetDevice()
        {
            List<TranzmitUI.Models.DeviceTransmission> device = new List<Models.DeviceTransmission>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);

                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {

                     query = "select * from device_transmission dt LEFT JOIN device_master de ON dt.device_id = de.device_id LEFT JOIN schedule_master sm ON dt.schedule_code = sm.schedule_code where dt.obj_id = '" + UserOraganization + "'";
                }
                else
                {
                     query = "select * from device_transmission dt LEFT JOIN device_master de ON dt.device_id = de.device_id LEFT JOIN schedule_master sm ON dt.schedule_code = sm.schedule_code";
                }
                log.Info("the device transmission  query result  is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Device Transmission  in getdevice");
                dataAdapter.Fill(dataTable);
                connection.Close();
                foreach (DataRow dr in dataTable.Rows)
                {
                    string trstatus = "Transfered";
                    device.Add(new Models.DeviceTransmission
                    {
                        DeviceIP = Convert.ToString(dr["device_ip"]),
                        DevicePort = Convert.ToString(dr["device_port"]),
                        SourcePath = Convert.ToString(dr["files_name"]),
                        Type = Convert.ToString(dr["schedule_type"]),
                        ScheduleDate = Convert.ToString(dr["schedule_date"]),
                        ScheduleTime = Convert.ToString(dr["schedule_time"]),
                        TransStatus = trstatus,
                        strDateTime = Convert.ToString(dr["end_time"]),

                    });


                }
            }
            catch (Exception ex)
            {

            }
            return device;
        }
       
        public List<DeviceTransmission> GetfileStatus()
        {
            List<TranzmitUI.Models.DeviceTransmission> filestatus = new List<Models.DeviceTransmission>();
            var Orag = System.Web.HttpContext.Current.Session["UserOraganization"];
           
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = "";
                int recount = RescheduleCheck();
                if (recount == 0)
                {

                    RescheduleAccessFlag = "N";
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        if (HomeController.filter == "Y")
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.bankname = '" + Orag + "' and es.ej_date BETWEEN  TO_DATE('" + HomeController.fd + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.td + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);

                        }
                        else
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.bankname = '" + Orag + "' and es.ej_date BETWEEN  TO_DATE('" + HomeController.FilterFrom + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.FilterTo + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                    }
                    else
                    {
                        if (HomeController.filter == "Y")
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.ej_date BETWEEN  TO_DATE('" + HomeController.fd + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.td + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                        else
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es   where es.ej_date BETWEEN  TO_DATE('" + HomeController.FilterFrom + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.FilterTo + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                    }
                }
               else
                {
                    RescheduleAccessFlag = "Y";

                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        if (HomeController.filter == "Y")
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.bankname = '" + Orag + "' and es.ej_date BETWEEN  TO_DATE('" + HomeController.fd + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.td + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                        else
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.bankname = '" + Orag + "' and es.ej_date BETWEEN  TO_DATE('" + HomeController.FilterFrom + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.FilterTo + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                    }
                    else
                    {
                        if (HomeController.filter == "Y")
                        {

                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.ej_date BETWEEN  TO_DATE('" + HomeController.fd + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.td + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                        else
                        {
                            query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where es.ej_date BETWEEN  TO_DATE('" + HomeController.FilterFrom + "','dd/MM/yyyy') AND  TO_DATE('" + HomeController.FilterTo + "','dd/MM/yyyy') order by filereceive_date desc";
                            log.Info("the ejfile status  query result  is:" + query);
                        }
                    }
                }

                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Device Transmission in getfilestatus");
                dataAdapter.Fill(dataTable);
                connection.Close();
                int icount = 0;
                string dt = string.Empty;
                foreach (DataRow dr in dataTable.Rows)
                {
                    if (dr["ej_date"].ToString().Equals(string.Empty))
                    {
                        dt = string.Empty;

                    }
                    else
                    {
                        dt = Convert.ToDateTime(dr["ej_date"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    icount++;
                    filestatus.Add(new Models.DeviceTransmission
                    {
                        SNo = Convert.ToString(icount),
                        Bankname = Convert.ToString(dr["bankname"]),
                        ATMID = Convert.ToString(dr["atm_id"]),
                        ATMIP = Convert.ToString(dr["atm_ip"]),
                        ATMPORT = Convert.ToString(dr["atm_port"]),
                        FileName = Convert.ToString(dr["ej_filename"]),
                        FileSize = Convert.ToString(dr["ej_filesize"]),
                        EJDate = dt,
                        ReceiveDatetime = Convert.ToString(dr["filereceive_date"]),
                        EJStatus = Convert.ToString(dr["ej_status"]),
                        RescheduleStatus = Convert.ToString(dr["reschedule_status"]),
                        TransStatus = Convert.ToString(dr["trans_status"]),

                    });


                }
            }
            catch (Exception ex)
            {

            }
            return filestatus;
        }

        public int RescheduleCheck()
        {
            int ecount = 0;
            var rstatus = "P";
            using (OracleConnection connection = new OracleConnection(connectionstring))
            {
                string query = "select count(*)  from reschedule_log where reschedule_status = '" + rstatus + "'";
                log.Info("the reschedule query result is:" + query);
                

                connection.Open();
                log.Info("Connection Opened Successfully for DeviceTransmission reschedulecheck condition");
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    ecount = Convert.ToInt16(command.ExecuteScalar());
                    log.Info("Command executed successfully");
                }

                connection.Close();
            }
            

            return ecount;

        }
        public List<DeviceTransmission> GetUserList()
        {
            List<TranzmitUI.Models.DeviceTransmission> UserList = new List<Models.DeviceTransmission>();
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = "select * from User_id_mapping uim LEFT JOIN user_master um ON uim.system_user_id = um.user_id LEFT JOIN role_master  rm ON um.user_type = rm.role_code";
                log.Info("the UserList  query result  is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for UserList");
                dataAdapter.Fill(dataTable);
                connection.Close();
                int icount = 0;
                foreach (DataRow dr in dataTable.Rows)
                {
                    icount++;
                    UserList.Add(new Models.DeviceTransmission
                    {
                        Uid = Convert.ToString(icount),
                        UserName = Convert.ToString(dr["login_user_id"]),
                        EmailId = Convert.ToString(dr["email"]),
                        Organization = Convert.ToString(dr["user_groups"]),
                        Role = Convert.ToString(dr["Role_name"]),
                        UserID = Convert.ToString(dr["user_id"])


                    });


                } 
            }
            catch (Exception ex)
            {

            }
            return UserList;
        }
        public List<DeviceTransmission> GetEJFileDownloadStatus()
        {
            List<TranzmitUI.Models.DeviceTransmission> filestatus = new List<Models.DeviceTransmission>();
            var Orag = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {
               
                var today = DateTime.Today;
                var Tommorrowdate = today.AddDays(1);
                string dd = Tommorrowdate.Day.ToString();
                string MM = Tommorrowdate.Month.ToString();
                string yy = Tommorrowdate.Year.ToString();
                string dt1 = MM + "/" + dd + "/" + yy;
                string Firstdt = dd + "/" + MM + "/" + yy;
                DateTime month = DateTime.Parse(Firstdt);
                
                var first = month.AddMonths(-1);
                string dd1 = first.Day.ToString();
                string MM1 = first.Month.ToString();
                string yy1 = today.Year.ToString();
                string seconddt = dd1 + "/" + MM1 + "/" + yy1;
                OracleConnection connection = new OracleConnection(connectionstring);
               
                
                string query = "";
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "select atm_id, ej_date, ej_filename, download_date, system_ip, download_user from ejfile_download where download_date BETWEEN TO_DATE('" + seconddt + "','dd/MM/yyyy') AND TO_DATE('" + Firstdt + "','dd/MM/yyyy') and bankname = '" + Orag + "' order by download_date desc";
                    log.Info("the ejfile  download status  query result  is:" + query);

                }
                else
                {
                    query = "select atm_id, ej_date, ej_filename, download_date, system_ip, download_user from ejfile_download where download_date BETWEEN TO_DATE('" + seconddt + "','dd/MM/yyyy') AND TO_DATE('" + Firstdt + "','dd/MM/yyyy') order by download_date desc";
                    log.Info("the ejfile  download status  query result  is:" + query);

                }
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Device Transmission in  ejfiledownload status");
                dataAdapter.Fill(dataTable);
                connection.Close();
                int icount = 0;
                string dt = string.Empty;
                foreach (DataRow dr in dataTable.Rows)
                {
                    if (dr["ej_date"].ToString().Equals(string.Empty))
                    {
                        dt = string.Empty;

                    }
                    else
                    {
                        dt = Convert.ToDateTime(dr["ej_date"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    icount++;
                    filestatus.Add(new Models.DeviceTransmission
                    {
                        SNo = Convert.ToString(icount),
                        ATMID = Convert.ToString(dr["atm_id"]),
                        EJDate = dt,
                        FileName = Convert.ToString(dr["ej_filename"]),
                        EJDownloadDate = Convert.ToString(dr["download_date"]),
                        EJDownloadUser = Convert.ToString(dr["download_user"]),
                        

                    });


                }
            }
            catch (Exception ex)
            {

            }
            return filestatus;
        }

        public List<DeviceTransmission> downloadEJFile()
        {
            List<TranzmitUI.Models.DeviceTransmission> ejfilestatus = new List<Models.DeviceTransmission>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                string varc = "C";
                OracleConnection connection = new OracleConnection(connectionstring);
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where trans_status='" + varc + "' and es.bankname = '" + UserOraganization + "'  order by filereceive_date desc ";
                }
                else
                {
                    query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es where trans_status='" + varc + "'  order by filereceive_date desc ";
                }
                log.Info("the ejfile status  query result  is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Device Transmission in getfilestatus");
                dataAdapter.Fill(dataTable);
                connection.Close();
                int icount = 0;
                string dt = string.Empty;
                string filepath = string.Empty;
                foreach (DataRow dr in dataTable.Rows)
                {
                    if (dr["ej_date"].ToString().Equals(string.Empty))
                    {
                        dt = string.Empty;

                    }
                    else
                    {
                        dt = Convert.ToDateTime(dr["ej_date"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    icount++;
                    if (!dr["destination_folder"].ToString().Equals(string.Empty))
                    {
                        string[] df = dr["destination_folder"].ToString().Split('\\');
                        filepath = df[0] + "\\" + df[1] + "\\" + df[2] + "\\";
                    }
                    ejfilestatus.Add(new Models.DeviceTransmission
                    {
                        SNo = Convert.ToString(icount),
                        Bankname = Convert.ToString(dr["bankname"]),
                        ATMID = Convert.ToString(dr["atm_id"]),
                        ATMIP = Convert.ToString(dr["atm_ip"]),
                        ATMPORT = Convert.ToString(dr["atm_port"]),
                        FileName = Convert.ToString(dr["ej_filename"]),
                        FileSize = Convert.ToString(dr["ej_filesize"]),
                        EJDate = dt,
                        EJFilePath= filepath,
                        EJStatus = Convert.ToString(dr["ej_status"]),
                        RescheduleStatus = Convert.ToString(dr["reschedule_status"]),
                        TransStatus = Convert.ToString(dr["trans_status"]),

                    });


                }
            }
            catch (Exception ex)
            {

            }
            return ejfilestatus;
        }
        public void downloadfile(string id)
        {
            try
            {
                List<TranzmitUI.Models.DeviceTransmission> Translist = new List<Models.DeviceTransmission>();
                Translist = downloadEJFile();
                var result = Translist.Find(x => x.SNo == id);
                string path = result.EJFilePath;
                string fname = result.FileName;
                string directory = result.DestinationPath;
                
                string pathname = path + fname;
                string store = @"C:\\EJFile\\"+result.Bankname+"\\\\"+result.ATMID+"\\\\";
                log.Info("the store path value is:" + store);
                string checkfile = store + fname;
                HomeController.path=store ;
                if (!Directory.Exists(store))
                {
                   directoryInfo = Directory.CreateDirectory(store);
                    {
                        if (File.Exists(checkfile))
                        {

                            WebClient client = new WebClient();
                            client.DownloadFile(pathname, store + fname);
                            DownloadEJFileData(id);
                            log.Info("File Download  Data Inserted successfully");
                        }
                        else
                        {
                            HomeController.fileexists = "Y";
                        }
                    }

                }
                else
                {
                    if (File.Exists(checkfile))
                    {

                        WebClient client = new WebClient();
                        client.DownloadFile(pathname, store + fname);
                        DownloadEJFileData(id);
                        log.Info("File Download  Data Inserted successfully");
                    }
                    else
                    {
                        HomeController.fileexists = "Y";
                    }
                }

            }
            catch(Exception ex)
            {

            }
           
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static string downloaddate()
        {
            string dt=string.Empty;
            try
            {
                var today = DateTime.Now;
                string dd = today.Day.ToString();
                string MM = today.Month.ToString();
                string yy = today.Year.ToString();
                string hh = today.Hour.ToString();
                string mm = today.Minute.ToString();
                string ss = today.Second.ToString();
                dt = dd + "/" + MM + "/" + yy + " " + hh + ":" + mm + ":" + ss;
                log.Info("the currentdate is" + dt);


            }
            catch(Exception ex)
            {

            }
            return dt;
            
        }
        public int DownloadEJFileData( string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {
                var today = DateTime.Now;

                string dd = today.Day.ToString();
                string MM = today.Month.ToString();
                string yy = today.Year.ToString();
                 string dt = dd + "/" + MM + "/" + yy;
                List<TranzmitUI.Models.DeviceTransmission> Translist = new List<Models.DeviceTransmission>();
                Translist = downloadEJFile();

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    connection.Open();
                    var result = Translist.Find(x => x.SNo == id);
                    var ip = GetLocalIPAddress();
                    var downloaddt = downloaddate();
                    var user = HomeController.UserName;
                    log.Info("Connection Opened Successfully for Ejfile Download Data insert");
                    string query = "insert into ejfile_download(ej_filename,ej_date,atm_id,download_date,system_ip,download_user,datedt,bankname,obj_id)values('" +result.FileName  + "',TO_DATE('" + result.EJDate + "','dd/MM/yyyy'),'" + result.ATMID+ "'," +
                        "TO_DATE('" + downloaddt + "','dd/MM/yyyy HH24:MI:SS'),'" + ip + "','" +user + "',TO_DATE('" + dt + "','dd/MM/yyyy'),'" + result.Bankname + "','"+UserOraganization+"')";
                    log.Info("the insert query result is:" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        count = command.ExecuteNonQuery();
                        log.Info("Command executed successfully");
                    }
                    connection.Close();
                }
            }

            catch (Exception ex)
            {

            }
            return count;
        }

        public bool GetRescheduleDetails( string id)
        {
            int fcount = 0;
            var today = DateTime.Now;
            string dd = today.Day.ToString();
            string MM = today.Month.ToString();
            string yy = today.Year.ToString();
            string hh = today.Hour.ToString();
            string mm = today.Minute.ToString();
            string ss = today.Second.ToString();

            //var status = "P";
            var assign1 = "P";

            List<TranzmitUI.Models.DeviceTransmission> Translist = new List<Models.DeviceTransmission>();
            var Orag = System.Web.HttpContext.Current.Session["UserOraganization"];
            Translist = GetfileStatus();

             DT = dd + "/" + MM + "/" + yy + " " + hh + ":" + mm + ":" + ss;
            var result = Translist.Find(x=>x.SNo == id);
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    connection.Open();
                    log.Info("Connection Opened Successfully for Repository master in Repositoryinsert");

                    string query = "";
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "Select count(*) from reschedule_log where atm_id ='" + result.ATMID + "' and atm_ip='" + result.ATMIP + "' and atm_port='" + result.ATMPORT + "' and ej_date=TO_DATE('" + result.EJDate + "','dd/MM/yyyy') and bankname='"+ Orag + "'";
                        log.Info("the  query result is:" + query);

                    }
                    else
                    {
                        query = "Select count(*) from reschedule_log where atm_id ='" + result.ATMID + "' and atm_ip='" + result.ATMIP + "' and atm_port='" + result.ATMPORT + "' and ej_date=TO_DATE('" + result.EJDate + "','dd/MM/yyyy')";
                        log.Info("the  query result is:" + query);

                    }
                    log.Info("the  query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query, connection))
                    {

                        fcount = Convert.ToInt16(cmd.ExecuteScalar());
                        log.Info("the count value is:" + fcount);
                        if (fcount == 0)
                        {
                            string query1 = "insert into reschedule_log(atm_id,atm_ip,atm_port,ej_date,ej_filename,reschedule_status,reschedule_dt,bankname) values('" + result.ATMID + "','" + result.ATMIP + "','" + result.ATMPORT + "', TO_DATE('" + result.EJDate + "','dd/MM/yyyy'),'" + result.FileName + "','" + assign1 + "',TO_DATE('" + DT + "','dd/MM/yyyy HH24:MI:SS'),'" + result.Bankname + "')";

                            log.Info("the getrescheduledetails query result is:" + query1);
                            using (OracleCommand command = new OracleCommand(query1, connection))
                            {

                                command.ExecuteNonQuery();
                                log.Info("Command executed successfully");


                            }
                        }
                        else
                        {
                            string query2 = "update reschedule_log set reschedule_status='" + assign1 + "',reschedule_dt=TO_DATE('" + DT + "','dd/MM/yyyy HH24:MI:SS') where atm_id ='" + result.ATMID+"' and atm_ip='"+result.ATMIP+"' and atm_port='"+result.ATMPORT+"' and ej_date=TO_DATE('"+result.EJDate+"','dd/MM/yyyy') and reschedule_status!='" + assign1 + "' and bankname='" + result.Bankname + "'"; 

                            log.Info("the getrescheduledetails query result is:" + query2);
                            using (OracleCommand command = new OracleCommand(query2, connection))
                            {
                                command.ExecuteNonQuery();
                                log.Info("Command executed successfully");


                            }
                        }
                    }
                    connection.Close();
                }
                return true;
                
            }
            catch(Exception ex)
            {
                return false;

            }
           

            
            
        }

        



        //protected void Page_Error(object sender,EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();
        //    Sever

        //}


        //public class GetDevice
        //{
        //    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        //    public List<DeviceTransmission> GetTodayDeviceList()
        //    {
        //        List<DeviceTransmission> getToday = new List<DeviceTransmission>();
        //        try
        //        {
        //            //string query = "SELECT DEVICE_ID,SCHEDULE_CODE,FILES_NAME,DESTINATION_FOLDER,END_TIME,TRANS_STATUS,REPNO FROM device_transmission WHERE END_TIME = TO_DATE(SYSDATE)";
        //            //using (OracleConnection con = new OracleConnection(connectionstring))
        //            //{
        //            //    con.Open();
        //            //    using (OracleCommand cmd = new OracleCommand(query,con))
        //            //    {


        //            //        OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(cmd.CommandText, con);
        //            //        DataTable dataTable = new DataTable();
        //            //        oracleDataAdapter.Fill(dataTable);
        //            //        int rows = dataTable.Rows.Count;
        //            //        //using (OracleDataReader dr = cmd.ExecuteReader())
        //            //        //{
        //            //        //    while (dr.Read())
        //            //        //    {
        //            //        //        getToday.Add(new DeviceTransmission
        //            //        //        {
        //            //        //            DeviceID = Convert.ToString(dr["device_id"]),
        //            //        //            ScheduleID = Convert.ToString(dr["schedule_code"]),
        //            //        //            SourcePath = Convert.ToString(dr["files_name"]),
        //            //        //            DestinationPath = Convert.ToString(dr["destination_folder"]),
        //            //        //            Status = Convert.ToString(dr["trans_status"]),
        //            //        //            DateTime = Convert.ToString(dr["end_time"]),
        //            //        //            RepNo = Convert.ToString(dr["repno"]),
        //            //        //        });
        //            //        //    }
        //            //        //}

        //            //    }
        //            //    con.Close();

        //            // }
        //            using (OracleConnection connection = new OracleConnection(ConnectionString))
        //            {

        //                connection.Open();
        //                string query = "SELECT * FROM device_transmission_upt";
        //                using (OracleCommand command = new OracleCommand(query, connection))
        //                {
        //                    OracleDataAdapter dataAdapter = new OracleDataAdapter(command.CommandText, connection);
        //                    System.Data.DataTable dataTable1 = new System.Data.DataTable();
        //                    dataAdapter.Fill(dataTable1);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return getToday;
        //    }
        //}

        //public class GetWeekDevice
        //{
        //    string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        //    public List<DeviceTransmission> GetWeekDeviceList()
        //    {
        //        List<DeviceTransmission> getWeek = new List<DeviceTransmission>();
        //        try
        //        {
        //            string query = "SELECT DEVICE_ID,SCHEDULE_CODE,FILES_NAME,DESTINATION_FOLDER," +
        //                "END_TIME,TRANS_STATUS,REPNO FROM device_transmission " +
        //                "WHERE DATE(END_TIME)=CURDATE() + INTERVAL 1 DAY";
        //            using (OracleConnection con = new OracleConnection(connectionstring))
        //            {
        //                using (OracleCommand cmd = new OracleCommand(query))
        //                {
        //                    cmd.Connection = con;
        //                    con.Open();
        //                    using (OracleDataReader dr = cmd.ExecuteReader())
        //                    {
        //                        while (dr.Read())
        //                        {
        //                            getWeek.Add(new DeviceTransmission
        //                            {
        //                                DeviceID = Convert.ToString(dr["device_id"]),
        //                                ScheduleID = Convert.ToString(dr["schedule_code"]),
        //                                SourcePath = Convert.ToString(dr["files_name"]),
        //                                DestinationPath = Convert.ToString(dr["destination_folder"]),
        //                                Status = Convert.ToString(dr["trans_status"]),
        //                                DateTime = Convert.ToString(dr["end_time"]),
        //                                RepNo = Convert.ToString(dr["repno"]),
        //                            });
        //                        }
        //                    }
        //                    con.Close();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return getWeek;
        //    }
        //}

        //public class GetMonthDevice
        //{
        //    string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        //    public List<DeviceTransmission> GetMonthDeviceList()
        //    {
        //        List<DeviceTransmission> getMonth = new List<DeviceTransmission>();
        //        try
        //        {
        //            string query = "SELECT DEVICE_ID,SCHEDULE_CODE,FILES_NAME,DESTINATION_FOLDER," +
        //                "END_TIME,TRANS_STATUS,REPNO FROM device_transmission WHERE " +
        //                "MONTH(END_TIME)=MONTH(CURRENT_DATE())";
        //            using (OracleConnection con = new OracleConnection(connectionstring))
        //            {
        //                using (OracleCommand cmd = new OracleCommand(query))
        //                {
        //                    cmd.Connection = con;
        //                    con.Open();
        //                    using (OracleDataReader dr = cmd.ExecuteReader())
        //                    {
        //                        while (dr.Read())
        //                        {
        //                            getMonth.Add(new DeviceTransmission
        //                            {
        //                                DeviceID = Convert.ToString(dr["device_id"]),
        //                                ScheduleID = Convert.ToString(dr["schedule_code"]),
        //                                SourcePath = Convert.ToString(dr["files_name"]),
        //                                DestinationPath = Convert.ToString(dr["destination_folder"]),
        //                                Status = Convert.ToString(dr["trans_status"]),
        //                                DateTime = Convert.ToString(dr["end_time"]),
        //                                RepNo = Convert.ToString(dr["repno"]),
        //                            });
        //                        }
        //                    }
        //                    con.Close();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return getMonth;
        //    }
        //}

        //public class GetYearDevice
        //{
        //    string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        //    public List<DeviceTransmission> GetYearDeviceList()
        //    {
        //        List<DeviceTransmission> getYear = new List<DeviceTransmission>();
        //        try
        //        {
        //            string query = "SELECT DEVICE_ID,SCHEDULE_CODE,FILES_NAME,DESTINATION_FOLDER," +
        //                "END_TIME,TRANS_STATUS,REPNO FROM device_transmission WHERE " +
        //                "YEAR(END_TIME)=YEAR(CURRENT_DATE())";
        //            using (OracleConnection con = new OracleConnection(connectionstring))
        //            {
        //                using (OracleCommand cmd = new OracleCommand(query))
        //                {
        //                    cmd.Connection = con;
        //                    con.Open();
        //                    using (OracleDataReader dr = cmd.ExecuteReader())
        //                    {
        //                        while (dr.Read())
        //                        {
        //                            getYear.Add(new DeviceTransmission
        //                            {
        //                                DeviceID = Convert.ToString(dr["device_id"]),
        //                                ScheduleID = Convert.ToString(dr["schedule_code"]),
        //                                SourcePath = Convert.ToString(dr["files_name"]),
        //                                DestinationPath = Convert.ToString(dr["destination_folder"]),
        //                                Status = Convert.ToString(dr["trans_status"]),
        //                                DateTime = Convert.ToString(dr["end_time"]),
        //                                RepNo = Convert.ToString(dr["repno"]),
        //                            });
        //                        }
        //                    }
        //                    con.Close();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return getYear;
        //    }
    }

    public class filter
    {
        public string TranzmitEJDate { get; set; }
    }

    public class FilterViewModel
    {
        public List<TranzmitUI.Models.DeviceTransmission> deviceTransmissions { get; set; } = new List<TranzmitUI.Models.DeviceTransmission>();
        public filter filter { get; set; }
        
        //public IEnumerable<DeviceTransmission> deviceTransmissions { get; set; }


    }
}
