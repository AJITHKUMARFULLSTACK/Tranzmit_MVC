using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace TranzmitUI.Models
{
    public class DAutoID
    {
        public string DID { get; set; }
        public string GID { get; set; }
        public string OID { get; set; }
    }
    public class DeviceMaster
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DeviceMaster));

       
      
        public string DeviceID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string DeviceIP { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string DevicePort { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string DeviceMake { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string Bandwidth { get; set; }
        public string LocationID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string LocationName { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public string CountryID { get; set; }
        public string CountryName { get; set; }
        public string StateID { get; set; }
        public string StateName { get; set; }

        public string Gname { get; set; }
        public string ObjId { get; set; }
        public List<SelectListItem> LocationSelect { get; set; }
        public List<SelectListItem> GroupSelect { get; set; }

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

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



       

        public List<DeviceMaster> DeviceList()
        {
            List<TranzmitUI.Models.DeviceMaster> device = new List<Models.DeviceMaster>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "select device_id,device_ip,device_port,device_make,location_id,bandwidth from device_master  where  obj_id = '" + UserOraganization + "'";
                }
                else
                {
                    query = "select device_id,device_ip,device_port,device_make,location_id,bandwidth from device_master";
                }
                log.Info("the devicelist query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Device master in Devicelist ");
                dataAdapter.Fill(dataTable);
                connection.Close();
                foreach (DataRow dr in dataTable.Rows)
                {
                    //String dip = Convert.ToString(dr["device_ip"]);
                    //bool contains = dataTable.AsEnumerable().Any(row =>dip == row.Field<string>("dip"));
                    device.Add(new Models.DeviceMaster
                    {
                        DeviceID=Convert.ToString(dr["device_id"]),
                        DeviceIP= Convert.ToString(dr["device_ip"]),
                        DeviceMake = Convert.ToString(dr["device_make"]),
                        DevicePort = Convert.ToString(dr["device_port"]),
                        LocationID = Convert.ToString(dr["location_id"]),
                        Bandwidth = Convert.ToString(dr["bandwidth"]),
                       // GroupID = Convert.ToString(dr["group_id"]),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return device;
        }
        public bool dependancyeCheck(string id)
        {
            string dpcheck = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from schedule_master where obj_id = '" + id + "'";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in depandancy check");

            dpcheck = command.ExecuteScalar().ToString();
            connection.Close();


            if (dpcheck == "0")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool ExitsvalueCheck(string deviceip,string deviceport,string id)
        {
            string evalue = string.Empty;
            
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = "select count(*)  from device_master where device_id !='"+id+"' and  device_ip='" + deviceip+"' and device_port='"+deviceport+"' ";
                log.Info("the devicelist query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                //OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                //DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Device master in existsvalue check");

                evalue = command.ExecuteScalar().ToString();

                //string dip = DeviceIP;

                //DataRow dw = dataTable.AsEnumerable().FirstOrDefault(row => dip == row.Field<string>("dip"));
                
                connection.Close();
            
           
            if (evalue == "0")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public int DeviceInsert(DeviceMaster deviceInsert)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

           

            try
            {
                
                if (ExitsvalueCheck(deviceInsert.DeviceIP ,deviceInsert.DevicePort,"0"))
                {
                    
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Device master in deviceinsert");

                        string query = "insert into device_master(device_id,device_ip,device_port,device_make," +
                            "location_id,city_id,state_id,country_id,device_status,bandwidth,obj_id)values('" + deviceInsert.DeviceID + "'," +
                            "'" + deviceInsert.DeviceIP + "','" + deviceInsert.DevicePort + "'," +
                            "'" + deviceInsert.DeviceMake + "','" + deviceInsert.LocationID + "'," +
                            "'" + deviceInsert.CityID + "','" + deviceInsert.StateID + "','" + deviceInsert.CountryID + "','B','" + Bandwidth + "','" + UserOraganization + "')";
                        log.Info("the deviceinsert query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            count = command.ExecuteNonQuery();
                            Log.Info("Command executed successfully");
                        }
                        connection.Close();
                    }
                }

                else
                {
                   
                    
                }
            }
            catch (Exception ex)
            {

            }
            return count;
        }

        public int UpdateDevice(DeviceMaster update, string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;


            try
            {

                if (ExitsvalueCheck(update.DeviceIP, update.DevicePort, "0"))
                {

                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Device master in updatedevice");


                        if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                        {
                            query = "update device_master set device_ip='" + update.DeviceIP + "',device_port='" + update.DevicePort + "'," +
                               "device_make='" + update.DeviceMake + "',location_id='" + update.LocationID + "',bandwidth='" + update.Bandwidth + "', " +
                               "city_id='" + update.CityID + "', state_id = '" + update.StateID + "', country_id='" + update.CountryID + "'" +
                               "where device_id='" + id + "' and obj_id = '" + UserOraganization + "'";
                        }
                        else
                        {
                            query = "update device_master set device_ip='" + update.DeviceIP + "',device_port='" + update.DevicePort + "'," +
                              "device_make='" + update.DeviceMake + "',location_id='" + update.LocationID + "',bandwidth='" + update.Bandwidth + "', " +
                              "city_id='" + update.CityID + "', state_id = '" + update.StateID + "', country_id='" + update.CountryID + "'" +
                              "where device_id='" + id + "'";
                        }
                        log.Info("the updatedevice query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            count = command.ExecuteNonQuery();
                            log.Info("Command executed successfully");
                        }
                        connection.Close();
                    }
                }
                
               
            }
            catch(Exception ex)
            {

            }
            return count;
        }

        public int DeleteDevice(string id)
        {
            int count = 0;
            try
            {
                //delete.DeviceIP = DeviceIP;
                //delete.DevicePort = DevicePort;
                if (dependancyeCheck(id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Device master in deletedevice");
                        string query = "Delete  from device_master where device_id='" + id + "'";
                        log.Info("the Deletedevice query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            count = command.ExecuteNonQuery();
                            log.Info("Command executed successfully");

                        }
                        connection.Close();
                    }
                }
                else
                {

                }
            }
            catch(Exception ex)
            {

            }
            return count;
        }

    }
}