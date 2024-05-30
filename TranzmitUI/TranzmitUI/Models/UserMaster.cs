using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using System.Web.DynamicData;

namespace TranzmitUI.Models
{
    public class UAutoID
    {
        [Required(ErrorMessage = "Please fill the detail")]
        public string Uid { get; set; }
        public string GID { get; set; }
        public string UIDs { get; set; }


    }
    public class UserMaster
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserMaster));

        [Required(ErrorMessage = "Please fill the detail")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Please fill the detail")]
        [DataType(DataType.Password)]
        public string PasswordUser { get; set; }
        public string NewPassword { get; set; }

        //[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [Required(ErrorMessage = "Please fill the detail")]
        public string ConfirmPassword { get; set; }
        public string ResetPasswordId { get; set; }
        public string Group { get; set; }
        public string Gname { get; set; }
        public string UserID { get; set; }

        
       
        public string EmpID { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public string StateID { get; set; }
        public string StateName { get; set; }
        public string CountryID { get; set; }
        public string CountryName { get; set; }
        public string GroupName { get; set; }
        public string GroupId { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

        public string Uid { get; set; }
        public string Text1 { get; set; }
        public string Pass1 { get; set; }
        [Required(ErrorMessage = "Please fill the detail")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string UserUser { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string EmpolyeeID { get; set; }
        

        public string EmailId { get; set; }
       
        public string Organization { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string Role { get; set; }
        public List<SelectListItem> RoleSelect { get; set; }
        public List<SelectListItem> OrganizationSelect { get; set; }
        public List<SelectListItem> CitySelect { get; set; }
        public List<SelectListItem> StateSelect { get; set; }
        public List<SelectListItem> CountrySelect { get; set; }
        public List<SelectListItem> GroupSelect { get; set; }

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<SelectListItem> PopulateCity()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = " SELECT city_id,city_name FROM city_master";
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

        public List<SelectListItem> PopulateState()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = " SELECT state_id,state_name FROM state_master";
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

        public List<SelectListItem> PopulateCountry()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = " SELECT Country_id,country_name FROM country_master";
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

        public List<SelectListItem> PopulateGroup()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = " SELECT group_id,group_desc FROM group_master";
                    log.Info("the populategroup query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Usermaster in populategroup");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["group_desc"].ToString(),
                                    Value = sdr["group_id"].ToString()
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


        public List<SelectListItem> PopulateRole()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = " SELECT * FROM Role_master";
                    log.Info("the populaterole query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for role master in populaterole ");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["role_name"].ToString(),

                                    Value = sdr["role_code"].ToString()
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

        public List<SelectListItem> PopulateOragnization()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = "SELECT distinct bankname FROM ej_link";
                    log.Info("the populate query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for user master in populate organization name ");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    

                                    Text = sdr["bankname"].ToString(),

                                    Value = sdr["bankname"].ToString()
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

        public bool ExitsvalueCheck(string username,string employeeid,string id)
        {
            string evalue = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from user_master where user_name='" + username.ToUpper() + "' OR emp_id='" + employeeid + "' and user_id !='" + id + "'";
            log.Info("the user query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            //OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
            //DataTable dataTable = new DataTable();
            connection.Open();
            log.Info("Connection Opened Successfully for user master in existsvalue check");

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

        public int UserInsert(UserMaster ua)
        {
            int count = 0;
            try
            {
                var assign1 = "P";
                if (ExitsvalueCheck(ua.UserUser, ua.EmpolyeeID, "0"))
                {

                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for User master in UserInsert");
                        string query = "begin insert into User_id_mapping (login_user_id,system_user_id)  values('" + ua.UserUser.ToUpper() + "','" + ua.Uid + "');" +
                            "insert into User_master (user_id, user_name, user_groups, user_status, user_type,email,emp_id) values('" + ua.Uid + "','" + ua.UserUser.ToUpper() + "','" + ua.BankName + "','" + assign1 + "','" + ua.RoleCode + "','" + ua.Email + "','" + ua.EmpolyeeID + "');" +
                            "insert into pin_master(user_id,user_password) values('" + ua.Uid + "',TRIM(CAST(standard_hash('" + ua.PasswordUser + "', 'MD5')AS VARCHAR(120))));  end; ";



                        log.Info("the countryinsert query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            count = command.ExecuteNonQuery();
                            log.Info("Command executed successfully");
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return count;
        }

        //public List<UserMaster> UserList()
        //{
        //    log.Info("the userlist method in usermaster is loading");
        //    List<TranzmitUI.Models.UserMaster> User = new List<Models.UserMaster>();
        //    try
        //    {

        //        OracleConnection connection = new OracleConnection(connectionstring);
        //        log.Info("the connnection string is:" + connection);
        //        string query = "select user_id,user_name,user_groups,city,state,country,mobile,email,emp_id from user_master";
        //        Log.Info("the userlist query result is:" + query);
        //        OracleCommand command = new OracleCommand(query, connection);
        //        OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
        //        DataTable dataTable = new DataTable();
        //        int rows = dataTable.Rows.Count;
        //        log.Info("the number of rows retrieved from datatable is:" + rows);
        //        connection.Open();
        //        log.Info("Connection Opened Successfully for Usermaster in Userlist");
        //        dataAdapter.Fill(dataTable);
        //        connection.Close();
        //        foreach (DataRow dr in dataTable.Rows)
        //        {
        //            User.Add(new Models.UserMaster
        //            {
        //                UserID = Convert.ToString(dr["user_id"]),
        //                UserName = Convert.ToString(dr["user_name"]),
        //                GroupId = Convert.ToString(dr["user_groups"]),
        //                CityName = Convert.ToString(dr["city"]),
        //                StateName = Convert.ToString(dr["state"]),
        //                CountryName = Convert.ToString(dr["country"]),
        //                Mobile = Convert.ToString(dr["mobile"]),
        //                Email = Convert.ToString(dr["email"]),
        //                EmpID = Convert.ToString(dr["emp_id"])
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return User;
        //}

        //public int UserInsert(UserMaster userInsert)
        //{
        //    int count = 0;
        //    try
        //    {
        //        using (OracleConnection connection = new OracleConnection(connectionstring))
        //        {
        //            connection.Open();
        //            log.Info("Connection Opened Successfully for Usermaster in userinsert");
        //            string query = "insert into user_master (user_id,user_name,user_groups,city,state,country," +
        //                "mobile,email,emp_id,designation,dept)values('"+userInsert.UserID+"'," +
        //                "'"+userInsert.UserName+"','"+userInsert.GroupId+"','"+userInsert.CityName+"'," +
        //                "'"+userInsert.StateName+"','"+userInsert.CountryName+"','"+userInsert.Mobile+"'," +
        //                "'"+userInsert.Email+"','"+userInsert.EmpID+"','"+userInsert.Designation+"'," +
        //                "'"+userInsert.Department+"')";
        //            log.Info("the userinsert query result is:" + query);
        //            using (OracleCommand command = new OracleCommand(query, connection))
        //            {
        //                count=command.ExecuteNonQuery();
        //                Log.Info("Command executed successfully");
        //            }
        //            connection.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return count;
        //}
        public int ResetPasswordUpdate(UserMaster update)
        {
            int count = 0;
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                {
                    connection.Open();
                    log.Info("Connection Opened Successfully for Usermaster in updateuser");
                    string query = "update pin_master set user_password= TRIM(CAST(standard_hash('" + update.PasswordUser + "', 'MD5')AS VARCHAR(120)))where user_id='" + update.UserID + "'";
                    log.Info("the updateuser query result is:" + query);
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
        public int  UpdateUser(UserMaster update, string id)
        {
            int count = 0;
            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                {
                    connection.Open();
                    log.Info("Connection Opened Successfully for Usermaster in updateuser");
                    string query = "update user_master set user_name='" + update.UserName + "',user_groups='" + update.GroupId + "',city='" + update.CityName + "',state='" + update.StateName + "',country='" + update.CountryName + "',mobile='" + update.Mobile + "',email='" + update.Email + "',emp_id='" + update.EmpID + "',designation='" + update.Designation + "',dept='" + update.Department + "' where user_id='" + id + "'";
                    log.Info("the updateuser query result is:" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        count=command.ExecuteNonQuery();
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

        public int DeleteUser(string id)
        {
            int count = 0;
            try
            {
               
                 OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for deleteuser");
                    string query = "Begin Delete  from user_id_mapping where system_user_id='"+ id + "';Delete  from user_master where user_id='"+ id + "';Delete from pin_master where user_id='"+ id +"';end;";
                        log.Info("the Deletedevice query result is:" + query);
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

    }
}