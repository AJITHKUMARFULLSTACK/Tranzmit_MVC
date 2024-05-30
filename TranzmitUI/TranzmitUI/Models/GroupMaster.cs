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
using MySqlX.XDevAPI.Common;

namespace TranzmitUI.Models
{
  
    public class GroupMaster
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DeviceMaster));

       
       
        public string GroupCode { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string GroupName { get; set; }

       

        public string GroupOwner { get; set; }

      

        public string GroupCreatedDatTime { get; set; }

       

        public string GroupCriteria { get; set; }


      
        public string GroupType { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string GroupProfileSelection { get; set; }

        //[Required(ErrorMessage = "Please fill the detail")]
        //public List<string> GroupLocationID { get; set; }


        [Required(ErrorMessage = "Please fill the detail")]
        public List<string> GroupDeviceID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public List<string> GroupLocation { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string GroupState { get; set; }


        [Required(ErrorMessage = "Please fill the detail")]
        public string GroupCity { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string GroupCountry { get; set; }




        public List<SelectListItem> GroupSelect { get; set; }

        public List<SelectListItem> GroupCountrySelect { get; set; }
        public List<SelectListItem> GroupStateSelect { get; set; }
        public List<SelectListItem> GroupCitySelect { get; set; }

        public List<SelectListItem> GroupLocationSelect { get; set; }

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;


        //private MultiSelectList _GroupDeviceList { get; set; }


        //public MultiSelectList GroupDeviceList
        //{

        //    get
        //    {
        //        if(_GroupDeviceList != null)
        //        {
        //            return _GroupDeviceList;

        //        }

        //        return new MultiSelectList(PopulateDevices(), "Text", "Value");
        //    }

        //    set { _GroupDeviceList = value; }
        //}


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
                                    Value = sdr["device_id"].ToString()

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

        public List<SelectListItem> PopulateCountry()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = " SELECT Country_id,country_name FROM country_master ";
                    log.Info("the populatecountry query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Usermaster in populatecountry");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["country_name"].ToString(),
                                    Value = sdr["Country_id"].ToString()
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

        public List<SelectListItem> PopulateState(string groupCountry)
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
                        query = " SELECT state_id,state_name FROM state_master where country_id='" + groupCountry + "' and  obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT state_id,state_name FROM state_master where country_id='" + groupCountry + "'";
                    }
                    log.Info("the populatestate query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Usermaster in populatestate");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["state_name"].ToString(),
                                    Value = sdr["state_id"].ToString()
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




        public List<SelectListItem> PopulateCity(string groupState)
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
                        query = " SELECT city_id,city_name FROM city_master where state_id = '" + groupState + "' and  obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT city_id,city_name FROM city_master where state_id = '" + groupState + "'";

                    }
                    log.Info("the populatecity query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Usermaster in populatecity");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["city_name"].ToString(),
                                    Value = sdr["city_id"].ToString()
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

      
        public List<SelectListItem> PopulateLocation(string groupCity)
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
                        query = " SELECT location_id,location_name FROM location_master where city_id = '" + groupCity + "'  and  obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT location_id,location_name FROM location_master where city_id = '" + groupCity + "'";
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




       
        public List<GroupMaster> GroupList()
        {
            List<TranzmitUI.Models.GroupMaster> device = new List<Models.GroupMaster>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "select profile_code,profile_name,obj_id,profile_criteria,profile_type from profile_master  where  obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "select profile_code,profile_name,obj_id,profile_criteria,profile_type from profile_master ";
                    }
                log.Info("the  query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Group master");
                dataAdapter.Fill(dataTable);
                connection.Close();
                foreach (DataRow dr in dataTable.Rows)
                {
                    //String dip = Convert.ToString(dr["device_ip"]);
                    //bool contains = dataTable.AsEnumerable().Any(row =>dip == row.Field<string>("dip"));
                    device.Add(new Models.GroupMaster
                    {
                      
                        GroupCode = Convert.ToString(dr["profile_code"]),
                        GroupName= Convert.ToString(dr["profile_name"]),
                        GroupCriteria = Convert.ToString(dr["profile_criteria"]),
                        GroupProfileSelection = Convert.ToString(dr["profile_type"]),
                      
                    });
                }

                }
            }
            catch (Exception ex)
            {

            }
            return device;
        }
        public bool dependancyCheck(string id)
        {
            string dpcheck = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from  schedule_master where profile_code = '" + id + "'";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully");

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
        public bool ExitsvalueCheck(string groupname, string groupcode)
        {
            string evalue = string.Empty;
            
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = "select count(*)  from profile_master where  profile_name='"+groupname+"'";
                log.Info("the  query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                //OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                //DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully");

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
        public int GroupInsert(GroupMaster group)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

          
            try
            {
                
                if (ExitsvalueCheck(group.GroupCode ,group.GroupName))
                {
                    
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully");

                        string query = "insert into profile_master(profile_code,profile_name,profile_owner,profile_type,profile_criteria,Date_And_Time,obj_id)values('" + group.GroupCode + "'," +
                            "'" + group.GroupName+ "','" + group.GroupOwner + "','" + group.GroupProfileSelection + "'," +
                            "'" + group.GroupCriteria + "',TO_TIMESTAMP('" + group.GroupCreatedDatTime + "','dd/MM/yyyy HH24:MI:SS'),'"+UserOraganization+"')";
                        log.Info("the group query result is:" + query);
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

        public int GroupUpdate(GroupMaster group, string profilecode)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;

            try
            {
                
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully");

                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                         query = "update profile_master set profile_name='" + group.GroupName + "',profile_owner='" + group.GroupOwner + "',profile_type='" + group.GroupProfileSelection + "'," +
                            "profile_criteria='" + group.GroupCriteria + "',Date_And_Time= TO_TIMESTAMP('" + group.GroupCreatedDatTime + "', 'dd/MM/yyyy HH24:MI:SS')  " +
                            "where profile_code='" + profilecode + "' and obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "update profile_master set profile_name='" + group.GroupName + "',profile_owner='" + group.GroupOwner + "',profile_type='" + group.GroupProfileSelection + "'," +
                           "profile_criteria='" + group.GroupCriteria + "',Date_And_Time= TO_TIMESTAMP('" + group.GroupCreatedDatTime + "', 'dd/MM/yyyy HH24:MI:SS')  " +
                           "where profile_code='" + profilecode + "' ";
                    }
                        log.Info("the  query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            count = command.ExecuteNonQuery();
                            log.Info("Command executed successfully");
                        }
                        connection.Close();
                    }
               
               
            }
            catch(Exception ex)
            {

            }
            return count;
        }

        public int DeleteGroup(string id)
        {
            int count = 0;
            try
            {
                //delete.DeviceIP = DeviceIP;
                //delete.DevicePort = DevicePort;
                if (dependancyCheck(id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully");
                        string query = "Delete  from profile_master where profile_code='" + id + "'";
                        log.Info("the  query result is:" + query);
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