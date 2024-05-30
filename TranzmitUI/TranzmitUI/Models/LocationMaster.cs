using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TranzmitUI.Models
{
    public class Location
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Location));
        public string LocationId { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string LocationName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string CityId { get; set; }
        public string CityName { get; set; }

        public string StateName { get; set; }

        public string CountryName { get; set; }

        public string UserId { get; set; }

        public List<SelectListItem> CitySelect { get; set; }
       
                                                
        

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<SelectListItem> PopulateCity()
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
                         query = " SELECT city_id,city_name FROM city_master where  obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "SELECT city_id,city_name FROM city_master";
                    }
                    log.Info("the populatecity query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in populatecity");
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


        public List<Location> GetLocation()
        {
            List<Location> location = new List<Location>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "SELECT L.location_id,L.location_name,L.city_id,C.City_Name,S.State_Name,CM.Country_Name FROM location_master L " +
                    "LEFT JOIN city_master C ON C.CITY_ID= L.City_ID LEFT JOIN state_master S ON S.State_ID = C.State_ID LEFT JOIN " +
                    "Country_master CM ON CM.Country_ID = S.Country_ID where L.obj_id = '" + UserOraganization + "' ORDER BY L.Location_ID DESC ";
                }
                else
                {
                    query = "SELECT L.location_id,L.location_name,L.city_id,C.City_Name,S.State_Name,CM.Country_Name FROM location_master L " +
                  "LEFT JOIN city_master C ON C.CITY_ID= L.City_ID LEFT JOIN state_master S ON S.State_ID = C.State_ID LEFT JOIN " +
                  "Country_master CM ON CM.Country_ID = S.Country_ID ORDER BY L.Location_ID DESC";
                }
                log.Info("the getlocation query result is:" + query);
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in getlocation");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                location.Add(new Location
                                {
                                    LocationId = sdr["location_id"].ToString(),
                                    LocationName = sdr["location_name"].ToString(),
                                    CityId= sdr["city_id"].ToString(),
                                    CityName = sdr["city_name"].ToString(),
                                    StateName = sdr["state_name"].ToString(),
                                    CountryName = sdr["country_name"].ToString()
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
            return location;
        }
        public bool dependancyeCheck(string id)
        {
            string dpcheck = string.Empty;

            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from device_master where location_id = '" + id + "' ";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in existsvalue check");

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
        public bool ExitsvalueCheck(string locationname, string cityid,string id)
        {
            string evalue = string.Empty;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from location_master where LOWER(location_name)='" + locationname.ToLower() + "' and city_id='" + cityid + "' and location_id !='"+id+"'";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            //OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
            //DataTable dataTable = new DataTable();
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in existsvalue check");

            evalue = command.ExecuteScalar().ToString();

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
        public int  LocationInsert(AutoID autoID)
        {
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            int count = 0;
            try
            {
                if (ExitsvalueCheck(autoID.LoName, autoID.CiID,"0"))
                {
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in locationinsert");
                        string query = "insert into location_master (location_id,city_id,location_name,obj_id)values('" + autoID.LoID + "','" + autoID.CiID + "','" + autoID.LoName + "','"+ UserOraganization+"')";
                        log.Info("the locationinsert query result is:" + query);

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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int UpdateLocation(Location updatelocation, string id)
        {
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            int count = 0;
            string query = string.Empty;
            try
            {
                if (ExitsvalueCheck(updatelocation.LocationName,"0",id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in updatelocation");

                        if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                        {

                             query = "update location_master set location_name='" + updatelocation.LocationName + "',city_id='" + updatelocation.CityId + "' " +
                                "where location_id='" + id + "' and obj_id= '" + UserOraganization + "'";
                        }
                        else
                        {
                             query = "update location_master set location_name='" + updatelocation.LocationName + "',city_id='" + updatelocation.CityId + "' " +
                               "where location_id='" + id + "'";
                        }
                        log.Info("the updatelocation query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int DeleteLocation(string id)
        {
          
            int count = 0;
            try
            {
                if (dependancyeCheck(id))
                {

                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in deletelocation");
                        string query = "Delete  from location_master where location_id='" + id + "'";
                        log.Info("the deletelocation query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;

        }

        

    }
    public class AutoID
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AutoID));

       
        public string LID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string CID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string SID { get; set; }

        [Required(ErrorMessage="Please fill the detail")]
        public string COID { get; set; }

       
        public string Branch { get; set; }

        
        public List<SelectListItem> CitySelect { get; set; }

        
        public List<SelectListItem> StateSelect { get; set; }
        
        public List<SelectListItem> CountrySelect { get; set; }
        [Required(ErrorMessage = "Please fill the detail")]
        public string LoName { get; set; }

        
        public string LoID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string CiName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string CiID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string StaName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string StaID { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string CoName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string CoIDs { get; set; }

      

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<SelectListItem> PopulateCity()
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
                        query = " SELECT city_id,city_name FROM city_master  where obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT city_id,city_name FROM city_master";
                    }
                    log.Info("the populatecity query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in populatecity");
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

        public List<SelectListItem> PopulateState()
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

                        query = "SELECT state_id,state_name FROM state_master  where obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "SELECT state_id,state_name FROM state_master";
                    }
                    log.Info("the populatestate query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in populatestate");
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

        public List<SelectListItem> PopulateCountry()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try

            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                   
                   query = "SELECT Country_id,country_name FROM country_master";
                  
                    log.Info("the populatecountry query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in populatecountry");
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
    }

    public class City
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(City));
        public string CityId { get; set; }
        [Required(ErrorMessage = "Please fill the detail")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string StateId { get; set; }
        public string Statename { get; set; }

        private static readonly string UserOraganization = (string)HttpContext.Current.Session["UserOraganization"];

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<SelectListItem> StateSelect { get; set; }

        public List<SelectListItem> PopulateState()
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

                        query = "SELECT state_id,state_name FROM state_master  where obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "SELECT state_id,state_name FROM state_master";
                    }
                    log.Info("the populatestate query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in populatestate");
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
        public List<City> GetCity()
        {
            List<City> city = new List<City>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "SELECT city_id,city_name,state_id FROM city_master  where obj_id = '" + UserOraganization + "'";
                }
                else
                {
                    query = "SELECT city_id,city_name,state_id FROM city_master";
                }
                log.Info("the getcity query result is:" + query);
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in getcity");
                        using (OracleDataReader citydr = cmd.ExecuteReader())
                        {
                            while (citydr.Read())
                            {
                                city.Add(new City
                                {
                                    CityId = citydr["city_id"].ToString(),
                                    CityName = citydr["city_name"].ToString(),
                                    StateId = citydr["state_id"].ToString()
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
            return city;
        }
        public bool dependancyeCheck(string id)
        {
            string dpcheck = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from location_master where city_id = '" + id + "' ";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in existsvalue check");

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
        public bool ExitsvalueCheck(string cityname, string stateid,string id)
        {
            string evalue = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from city_master where LOWER(city_name)='" + cityname.ToLower() + "' and state_id='" + stateid + "' and city_id !='"+id+"'";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            //OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
            //DataTable dataTable = new DataTable();
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in existsvalue check");

            evalue = command.ExecuteScalar().ToString();

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
        public int  CityInsert(AutoID autoID)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {
                if (ExitsvalueCheck(autoID.CiName, autoID.StaID,"0"))
                {
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in cityinsert");
                        string query = "insert into city_master (city_id,city_name,state_id,obj_id)values('" + autoID.CiID + "','" + autoID.CiName + "','" + autoID.StaID + "','"+UserOraganization+"')";
                        log.Info("the cityinsert query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int UpdateCity(City updatecity, string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                if (ExitsvalueCheck(updatecity.CityName,"0",id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in updatecity");

                        if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                        {
                             query = "update city_master set city_name='" + updatecity.CityName + "',state_id='" + updatecity.StateId + "'" +
                                " where city_id='" + id + "' and obj_id = '" + UserOraganization + "'";
                        }
                        else
                        {
                             query = "update city_master set city_name='" + updatecity.CityName + "',state_id='" + updatecity.StateId + "'" +
                                " where city_id='" + id + "' ";
                        }
                        log.Info("the updatecity query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int DeleteCity(string id)
        {
            int count = 0;
            try
            {
                if (dependancyeCheck(id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in deletecity");
                        string query = "Delete  from city_master where city_id='" + id + "'";
                        log.Info("the deletecity query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

    }

    public class State
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(State));

        
        public string StateId { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string Statename { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string CountryId { get; set; }

        
        public string CountryName { get; set; }

     

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<SelectListItem> CountrySelect { get; set; }

        public List<SelectListItem> PopulateCountry()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try

            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                  
                    
                    query = "SELECT Country_id,country_name FROM country_master";
                  
                    log.Info("the populatecountry query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in populatecountry");
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
    

        public List<State> GetState()
        {
            List<State> state = new List<State>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "SELECT state_id,state_name,country_id FROM state_master  where obj_id = '" + UserOraganization + "'";
                }
                else
                {
                    query = "SELECT state_id,state_name,country_id FROM state_master";
                }
                log.Info("the getstate query result is:" + query);
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in getstate");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                state.Add(new State
                                {
                                    StateId = sdr["state_id"].ToString(),
                                    Statename = sdr["state_name"].ToString(),
                                    CountryId = sdr["country_id"].ToString()
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
            return state;
        }
        public bool dependancyeCheck(string id)
        {
            string dpcheck = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from city_master where state_id = '" + id + "' ";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in existsvalue check");

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
        public bool ExitsvalueCheck(string statename,string countryid,string id)
        {
            string evalue = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from state_master where LOWER(state_name)='" + statename.ToLower() + "' and country_id='"+countryid+"' and state_id !='"+id+"'";
            log.Info("the  query result is:" + query);
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

        public int StateInsert(AutoID autoID)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {
                if (ExitsvalueCheck(autoID.StaName,autoID.CoIDs,"0"))
                {
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in stateinsert");
                        string query = "insert into state_master (state_id,state_name,country_id,obj_id)values('" + autoID.StaID + "','" + autoID.StaName + "','" + autoID.CoIDs + "','"+UserOraganization+"')";
                        log.Info("the stateinsert query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int UpdateState(State updatelocation, string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;
            try
            {
                if (ExitsvalueCheck(updatelocation.Statename, "0",id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in updatestate");
                        if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                        {

                             query = "update state_master set state_name='" + updatelocation.Statename + "',country_id='" + updatelocation.CountryId + "'" +
                                " where state_id='" + id + "' and obj_id = '" + UserOraganization + "'";
                        }
                        else
                        {
                             query = "update state_master set state_name='" + updatelocation.Statename + "',country_id='" + updatelocation.CountryId + "'" +
                               " where state_id='" + id + "'";
                        }
                        log.Info("the updatestate query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int DeleteState(string id)
        {
            int count = 0;
            try
            {
                if (dependancyeCheck(id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in deletestate");
                        string query = "Delete  from state_master where state_id='" + id + "'";
                        log.Info("the deletestate query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                           count= command.ExecuteNonQuery();
                            log.Info("Command executed successfully");
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
    }

    public class Country
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Country));

        
        public string CountryId { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string CountryName { get; set; }

        //public string CoIDs { get; set; }

        public string CoName { get; set; }

        //private static readonly string UserRole = (string)HttpContext.Current.Session["UserRole"];

       

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<Country> GetCountry()
        {
            List<Country> country = new List<Country>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = "";
            try
            {
               
                query = "SELECT country_id,country_name FROM country_master";
                    
               
                log.Info("the getcountry query result is:" + query);
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Location master in getcountry");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                country.Add(new Country
                                {
                                    CountryId = sdr["country_id"].ToString(),
                                    CountryName = sdr["country_name"].ToString()
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
            return country;
        }

        public bool dependancyeCheck( string id)
        {
            string dpcheck = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from state_master where country_id = '" + id + "' ";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully for Device master in existsvalue check");

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
        public bool ExitsvalueCheck(string name,string id)
        {
            string evalue = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from country_master where LOWER(country_name)='"+name.ToLower()+"' and country_id != '"+id+"' ";
            log.Info("the  query result is:" + query);
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
        public int CountryInsert(AutoID autoID)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {
                if (ExitsvalueCheck(autoID.CoName,"0"))
                {
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in countryinsert");


                        string query = "insert into country_master (country_id,country_name,obj_id)values('" + autoID.CoIDs + "','" + autoID.CoName + "','" + UserOraganization + "')";
                        log.Info("the countryinsert query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int UpdateCountry(Country updatecountry,string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {
                if (ExitsvalueCheck(updatecountry.CountryName,id))
                {


                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in updatecountry");
                       
                            string query = "update country_master set country_name='" + updatecountry.CountryName + "' where country_id='" + id + "'";
                       
                        log.Info("the updatecountry query result is:" + query);
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
            catch (Exception ex)
            {

            }
            return count;
        }

        public int DeleteCountry(string id)
        {
            int count = 0;
            try
            {
                if (dependancyeCheck(id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Location master in deletecountry");
                        string query = "Delete  from country_master where country_id='" + id + "'";
                        log.Info("the deletecountry query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            count=command.ExecuteNonQuery();
                            log.Info("Command executed successfully");
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
    }
    
}