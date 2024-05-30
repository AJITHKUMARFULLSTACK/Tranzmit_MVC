using System;
using TranzmitUI.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Dynamic;
using System.Web;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using ReportViewerForMvc;
using MySqlX.XDevAPI;
using System.Web.Services.Description;
using System.Reflection;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Security.Policy;

namespace TranzmitUI.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        public static string loginaccess = "N";
        
        public static string fileexists = "N";
        public static string filter = "N";
        public static string path = string.Empty;
        public static string UserName= string.Empty;
        public static string UserUser = string.Empty;
        public static string PasswordUser = string.Empty;
        public static string fd = string.Empty, td = string.Empty;
        public static string FilterFrom = string.Empty, FilterTo = string.Empty;
        //public static string NameOfOrganization = string.Empty;
        public static string CityValue = string.Empty;

        List<string> GroupSelectedItems = new List<string>();

        List<string> GroupDevice = new List<string>();
        public ActionResult Login(UserMaster user)
        {
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                using(OracleConnection connection = new OracleConnection(ConnectionString))
                {

                    connection.Open();
                    log.Info("Connection opened successfully");
                    UserName = user.UserName;
                    log.Info("the Username is :" + UserName);
                    string Password = user.Password;
                    log.Info("the Password is :" + Password);
                    string query = "select * from User_id_mapping uim LEFT JOIN user_master um ON uim.system_user_id = um.user_id";
                    log.Info("the login query result is :" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        
                        OracleDataAdapter dataAdapter = new OracleDataAdapter(command.CommandText, connection);
                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        dataAdapter.Fill(dataTable);
                        int RowCount = dataTable.Rows.Count;
                        log.Info("the number of rows retrieved from datatable is:" + RowCount);
                        for (int i = 0; i < RowCount; i++)
                        {
                            string uname = dataTable.Rows[i]["login_user_id"].ToString();
                            log.Info("the uname value  is :" + uname);
                            string userid = dataTable.Rows[i]["system_user_id"].ToString();
                            log.Info("the userid value is :" + userid);
                            string userrole = dataTable.Rows[i]["user_type"].ToString();
                            string NameOfOrganization = dataTable.Rows[i]["user_groups"].ToString();

                            log.Info("the userid value is :" + userid);
                            if (UserName != null && UserName.ToUpper() == uname.ToUpper())
                            {
                                string query1 = "Select * FROM pin_master where user_id='" + userid + "' and user_password = TRIM(CAST(standard_hash('"+Password+"', 'MD5')AS VARCHAR(120)))";
                                log.Info("the query1 result is :" + query1);
                                using (OracleCommand command1 = new OracleCommand(query1, connection))
                                {
                                    OracleDataAdapter dataAdapter1 = new OracleDataAdapter(command1.CommandText, connection);
                                    System.Data.DataTable dataTable1 = new System.Data.DataTable();
                                    dataAdapter1.Fill(dataTable1);
                                    int RowCount1 = dataTable1.Rows.Count;
                                    log.Info("the number of rows retrieved from datatable is:" + RowCount1);
                                    if (RowCount1 > 0)
                                    {
                                        string upass = dataTable1.Rows[0]["user_password"].ToString();
                                        log.Info("the upass value  is :" + upass);
                                        //if (upass == Password)
                                        //{
                                            loginaccess = "Y";
                                        Session["UserName"] = UserName;
                                        Session["UserID"] = userid;
                                           Session["UserRole"] = userrole;
                                           Session["UserOraganization"] = NameOfOrganization;
                                           log.Info("the session value is:" + Session["UserOraganization"]);
                                            log.Info("login successfully");
                                            return RedirectToAction("DashBoard");

                                        //}
                                    }
                                }

                               Session["UserID"] = userid;
                                Session["UserRole"] = userrole;




                            }
                            
                                // TempData["testmsg"] = " Logged In Failed ";
                            
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

            }
            return View();
        }


       

        string UserNames = string.Empty;
        public ActionResult ForgotPassword(string UserName)
        {
            try
            {
                ViewBag.CurrentFilter = UserName;
            }
            catch(Exception ex)
            {

            }
            return View();
        }

        [HttpPost]

        public ActionResult ForgotPassword(UserMaster user)
        {
            try
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                OracleConnection connection = new OracleConnection(connectionstring);
                {
                    
                    connection.Open();
                    log.Info("Connection opened successfully  for forgotpassword");
                    string query = "update user_master set PASSPORT_NO='" + user.ConfirmPassword + "' where user_name='" + UserNames + "'";
                    log.Info("the Forgot password query result  is :" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        log.Info("Password change successfully");
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }
            return View();
        }

        public ActionResult Logout()
        {
            loginaccess = "N";
            try
            {

                log.Info("logout successfully");
                Session.Abandon();
                
             return RedirectToAction("Login");
            }
            catch(Exception ex)
            {

            }
            return View();
            
        }




        DeviceTransmission deviceTransmission = new DeviceTransmission();
       
        public ActionResult DashBoard()
        {
            
            

            try
            {
                if (Session["UserID"] != null)
                {

                    ViewBag.MenuHome = "class = active-nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";

                    var today = DateTime.Today;
                    var date1 = today.AddDays(-1);
                    string dd = date1.Day.ToString();
                    string MM = date1.Month.ToString();
                    string yy = date1.Year.ToString();

                    String Firstdt = dd + "/" + MM + "/" + yy;
                    FilterTo = Firstdt;
                    var date2 = today.AddDays(-7);
                    string dd1 = date2.Day.ToString();
                    string MM1 = date2.Month.ToString();
                    string yy1 = date2.Year.ToString();
                    String Seconddt = dd1 + "/" + MM1 + "/" + yy1;
                    FilterFrom = Seconddt;

                    ViewBag.IsFilter = "N";
                    String strRange = string.Empty;
                    if (filter == "Y")
                    {
                        ViewBag.IsFilter = "Y";
                        strRange = "**Date Range Between :" + fd + " and " + td;
                    }
                    else
                    {
                        ViewBag.IsFilter = "N";
                        strRange = "**Date Range Between :" + FilterFrom + " and " + FilterTo;
                    }
                    ViewBag.FilterRange = strRange;
                    //FilterViewModel filtermodel = new FilterViewModel();

                    //filtermodel.deviceTransmissions = GetfileStatus();


                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {

            }
            return View(deviceTransmission.GetfileStatus());

        }
           

             

                //return View(deviceTransmission.GetDevice());
            
        
        
        //[HttpPost]
        //public ActionResult DashBoard(DeviceTransmission transmission)
        //{
        //    try
        //    {


        //    }

        //    catch (Exception ex)
        //    {

        //    }

        //    return View(deviceTransmission.GetfileStatus());

        //}

        public ActionResult ClearFilter()
        {
            try
            {

                filter = "N";
            }

            catch (Exception ex)
            {

            }

            return RedirectToAction("DashBoard", "Home");

        }

        public ActionResult UserList()
        {



            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() != "M")
                {

                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class =active-nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";

                    ViewBag.UserSuccess = TempData["UserInsertSuccess"];

                    ViewBag.PasswordSuccess = TempData["PasswordSuccess"];

                    ViewBag.DeleteSuccess = TempData["deleteSuccess"];
                    ViewBag.DeleteFail = TempData["deleteFail"];


                    //FilterViewModel filtermodel = new FilterViewModel();

                    //filtermodel.deviceTransmissions = GetfileStatus();


                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            return View(deviceTransmission.GetUserList());

        }
        AutoID GetAutoID = new AutoID();
        UAutoID UGetAutoID = new UAutoID();
       
        UserMaster GetUserAccess = new UserMaster();

        public ActionResult AddUser()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = active-nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() != "M")
                {

                    GetUserAccess.RoleSelect = GetUserAccess.PopulateRole();
                    GetUserAccess.OrganizationSelect = GetUserAccess.PopulateOragnization();
                    // GetDevice.GroupSelect = GetDevice.PopulateGroup();
                    //ViewBag.UserSuccess = TempData["UserInsertSuccess"];
                    ViewBag.UserFail = TempData["UserInsertFail"];
                      
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetUserAccess);
        }

        [HttpPost]

        public ActionResult AddUser(UserMaster UserA)
        {
            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() != "M")
                {
                    int note = 0;


                   
                    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {
                        string country = "select count(*) from user_master where user_id  like 'USR%'";
                        log.Info("the User insert query result is:" + country);
                        using (OracleCommand commands = new OracleCommand(country, connection))
                        {
                            connection.Open();
                            log.Info("Connection opened successfully for country insertion");
                            int count = Convert.ToInt16(commands.ExecuteScalar());
                            log.Info("the User  query result is:" + count);
                            if (count == 0)
                            {
                                UGetAutoID.Uid = "USR001";
                            }
                            else
                            {
                                string query = "SELECT * FROM(SELECT user_id FROM user_master ORDER BY user_id DESC) WHERE ROWNUM = 1";

                                OracleCommand command = new OracleCommand(query, connection);
                               
                                //connection.Open();
                                log.Info("Connection Opened Successfully");

                                string lastadata = command.ExecuteScalar().ToString();
                                int lastUserId = Int32.Parse(lastadata.Substring(lastadata.Length - 3))+1;



                                if (lastUserId < 10)
                                {
                                    UGetAutoID.Uid = "USR" + "00" + lastUserId;
                                }
                                else if (lastUserId > 9 && lastUserId < 100)
                                {
                                    UGetAutoID.Uid = "USR" + "0" + lastUserId;
                                }
                                else
                                {
                                    UGetAutoID.Uid = "USR" + lastUserId;
                                }
                            }
                        }

                        GetUserAccess.RoleSelect = GetUserAccess.PopulateRole();
                        var roleItem = GetUserAccess.RoleSelect.Find(p => p.Value == UserA.RoleName.ToString());
                        if (roleItem != null)
                        {
                            roleItem.Selected = true;
                            UserA.RoleCode = roleItem.Value;
                            UserA.RoleName = roleItem.Text;
                        }
                        if (UserA.RoleCode == "M")
                        {
                            GetUserAccess.OrganizationSelect = GetUserAccess.PopulateOragnization();
                            var orgItem = GetUserAccess.OrganizationSelect.Find(p => p.Value == UserA.BankName.ToString());
                            if (orgItem != null)
                            {
                                orgItem.Selected = true;
                                UserA.BankName = orgItem.Value;
                                UserA.BankName = orgItem.Text;
                            }
                        }
                        UGetAutoID.UIDs = UGetAutoID.Uid;

                        UserA.Uid = UGetAutoID.UIDs;
                        GetUserAccess.EmpolyeeID = UserA.EmpolyeeID;
                        GetUserAccess.Email = UserA.Email;
                        GetUserAccess.UserUser = UserA.UserUser;
                        GetUserAccess.BankName = UserA.BankName;
                        GetUserAccess.RoleCode = UserA.RoleCode;
                        GetUserAccess.PasswordUser = UserA.PasswordUser;

                        note = GetUserAccess.UserInsert(UserA);


                    }


                    if (note > 0)
                    {
                        log.Info("User Inserted Successfully");
                        TempData["UserInsertSuccess"] = "User Inserted Successfully";
                        return RedirectToAction("UserList");
                    }


                    else
                    {
                        //TempData["CountryInsertFail"] = "Country Inserted Failed";
                        log.Info("User Inserted Failed");
                        TempData["UserInsertFail"] = "Insert UserName or EmployeeId already exists";
                        //MessageBox.Show("Insert Country already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return RedirectToAction("AddUser");

                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("UserList");

        }


        public ActionResult ResetPassword()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = active-nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() != "M")
                {


                   
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [HttpPost]

        public ActionResult ResetPassword(string id,UserMaster updatePass)
        {
            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() != "M")
                {

                    int note = 0;

                    //UGetAutoID.Uid = UautoIds.Uid;

                    updatePass.UserID = id;
                    GetUserAccess.ResetPasswordId = updatePass.UserID;

                    GetUserAccess.PasswordUser = updatePass.PasswordUser;
                    GetUserAccess.ConfirmPassword = updatePass.ConfirmPassword;

                    note = GetUserAccess.ResetPasswordUpdate(updatePass);





                    if (note < 0)
                    {
                        log.Info("Password Change Successfully");
                        TempData["PasswordSuccess"] = "Password Change Successfully";

                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("UserList");

        }

        public ActionResult DeleteUser(string id)
        {
            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() != "M")
                {

                    int note = 0;
                    note = GetUser.DeleteUser(id);
                    try
                    {


                    }
                    catch (Exception ex)
                    {

                    }
                    //if (note > 0)
                    //{

                        log.Info("user deleted sucessfully");
                        TempData["deleteSuccess"] = "User deleted Successfully";
                        return RedirectToAction("UserList");
                    //}
                    //else
                    //{

                    //    log.Info("user deleted Failed");
                    //    TempData["deletefail"] = "user deleted Failed";
                    //    return RedirectToAction("UserList");
                    //}
                }
                else
                {

                }
            }
            catch(Exception ex)
            {

            }

            return RedirectToAction("UserList");
        }

        [HttpGet]
        public ActionResult EJFile()
        {


            try
            {
                if (Session["UserID"] != null)
                {

                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = active-nav-link";

                    ViewBag.Download = TempData["FileDownloadSuccess"];
                    ViewBag.FileExists = TempData["FileExists"];
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {

            }
            return View(deviceTransmission.downloadEJFile());

        }

        [HttpPost]
        public ActionResult EJFile(DeviceTransmission transmission)
        {
            try
            {


            }

            catch (Exception ex)
            {

            }

            return View(deviceTransmission.downloadEJFile());

        }


        public ActionResult RescheduleFile(string id)
        {
            ViewBag.MenuHome = "class = active-nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                

                deviceTransmission.GetRescheduleDetails(id);
            }
            catch(Exception ex)
            {

            }
            
            return RedirectToAction("DashBoard");

        }

       
        
        public ActionResult Download(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = active-nav-link";



            try
            {
                
                deviceTransmission.downloadfile(id );
                log.Info("File Download successfully");
                
                string storepath = path;
                log.Info("the file storepath is:" + storepath);
                if (fileexists == "N")
                {
                    TempData["FileDownloadSuccess"] = "File Download successfully, Directory path:" + storepath;
                }
                else
                {
                    TempData["FileExists"] = "File already exists! " + storepath;
                }

            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("EJFile");

        }


        public ActionResult TransmissionList()
        {

            try
            {
                if (Session["UserID"] != null)
                {


                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = active-nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }


            }
            catch (Exception ex)
            {

            }


            return View(deviceTransmission.GetDevice());
        }

        [HttpPost]
        public ActionResult TransmissionList(DeviceTransmission transmission)
        {
            try
            {


            }

            catch (Exception ex)
            {

            }

            return View(deviceTransmission.GetDevice());

        }
       
        public ActionResult EJFileDownload()
        {

            try
            {
                if (Session["UserID"] != null)
                {


                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class = nav-link";
                     ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = active-nav-link";
                }
                else
                {
                    return RedirectToAction("Login");
                }


            }
            catch (Exception ex)
            {

            }


            return View(deviceTransmission.GetEJFileDownloadStatus());
        }

        [HttpPost]
        public ActionResult EJFileDownload(DeviceTransmission transmission)
        {
            try
            {


            }

            catch (Exception ex)
            {

            }

            return View(deviceTransmission.GetEJFileDownloadStatus());

        }
        ReportMaster reportmaster = new ReportMaster();
        public ActionResult Filter()
        {
            try
            {
                if (Session["UserID"] != null)
                {


                    ViewBag.MenuHome = "class = active-nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";
                }
                else
                {
                    return RedirectToAction("Login");
                }


            }
            catch (Exception ex)
            {

            }
            return View(reportmaster);
        }

        [HttpPost]
        public ActionResult Filter(ReportMaster report)
        {
            fd = report.FromDate;
            td = report.ToDate;
            try
            {
                if (Session["UserID"] != null)
                {


                    ViewBag.MenuHome = "class = active-nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";

                    filter = "Y";
                    ViewBag.IsFilter = "Y";
                }
                else
                {
                    return RedirectToAction("Login");
                }



            }
            catch (Exception ex)
            {

            }

            //return View(reportmaster.GetEJFileRecord(fd, td));
            //return View("DashBoard", deviceTransmission.GetEJFileRecord(fd, td));
            return RedirectToAction("DashBoard", "Home");
        }
        public ActionResult RestartSchedule(string id)
        {
            try
            {
              
                string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    
                    connection.Open();
                    log.Info("Connection opened successfully for Restartschedule");
                    string type = "IMMEDIATE";
                    string flag = "B";
                    string query = "update schedule_master set schedule_type='"+type+"',flag='"+flag+"' where schedule_code='"+id+"'";
                    Log.Info("the Restartschedule query result is:" + query);
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        log.Info("Restart Schedule Successfully");
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("DashBoard");
        }

        Location GetLocation = new Location();
        City GetCity = new City();
        State GetState = new State();
        Country GetCountry = new Country();

        public ActionResult LocationList()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = active-nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";


            dynamic model = new ExpandoObject();
            try
            {
                if (Session["UserID"] != null)
                {


                    ViewBag.LocationInsertSuccess = TempData["LocationInsertSuccess"];
                    ViewBag.CityInsertSuccess = TempData["CityInsertSuccess"];
                    ViewBag.StateInsertSuccess = TempData["StateInsertSuccess"];
                    ViewBag.CountryInsertSuccess = TempData["CountryInsertSuccess"];


                    ViewBag.LocationUpdateSuccess = TempData["LocationUpdateSuccess"];
                    //ViewBag.LocationUpdateFail = TempData["LocationUpdateFail"];
                    ViewBag.CityUpdateSuccess = TempData["CityUpdateSuccess"];
                    //ViewBag.CityUpdateFail = TempData["CityUpdateFail"];
                    ViewBag.StateUpdateSuccess = TempData["StateUpdateSuccess"];
                    //ViewBag.StateUpdateFail = TempData["StateUpdateFail"];
                    ViewBag.CountryUpdateSuccess = TempData["CountryUpdateSuccess"];
                    ViewBag.CountryUpdateFail = TempData["CountryUpdateFail"];
                    ViewBag.CouSuccess = TempData["deleteSuccess"];
                    ViewBag.CouFail = TempData["deletefail"];
                    ViewBag.StateSuccess = TempData["deleteSuccess"];
                    ViewBag.StateFail = TempData["deletefail"];
                    ViewBag.CitySuccess = TempData["deleteSuccess"];
                    ViewBag.CityFail = TempData["deletefail"];
                    ViewBag.LocSuccess = TempData["deleteSuccess"];
                    ViewBag.LocFail = TempData["deletefail"];

                    model.Location = GetLocation.GetLocation();
                    model.City = GetCity.GetCity();
                    model.State = GetState.GetState();
                    model.Country = GetCountry.GetCountry();
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        [HttpPost]

        public ActionResult LocationList(Location location)
        {
            try
            {
                ViewBag.Message = "Message";

            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult LocationDirect()
        {
            return RedirectToAction("LocationInsert");
        }

       


        public ActionResult LocationInsert()
        {
            try
            {
                if (Session["UserID"] != null )
                {

                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = active-nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";


                    //ViewBag.Default = "Cs";

                    GetAutoID.CitySelect = GetAutoID.PopulateCity();
                    ViewBag.citylist = GetAutoID.CitySelect;
                    GetAutoID.StateSelect = GetAutoID.PopulateState();
                    ViewBag.statelist = GetAutoID.StateSelect;
                    GetAutoID.CountrySelect = GetAutoID.PopulateCountry();
                    ViewBag.countrylist = GetAutoID.CountrySelect;
                    ViewBag.LocationInsertFail = TempData["LocationInsertFail"];

                    ViewBag.CityInsertFail = TempData["CityInsertFail"];

                    ViewBag.StateInsertFail = TempData["StateInsertFail"];

                    ViewBag.CountryInsertFail = TempData["CountryInsertFail"];


                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            
            
            return View(GetAutoID);
        }

        [HttpPost]

        
        public ActionResult LocationInsert(AutoID autoIDs)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    GetAutoID.CitySelect = GetAutoID.PopulateCity();

                    if (autoIDs.Branch == "Location")
                    {
                        int note = 0;
                        // string selectedvalue = string.Empty;
                        //autoIDs.CiID = selectedvalue;
                        string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            string locations = "select count(*) from location_master";
                            Log.Info("the location insert query result is:" + locations);
                            using (OracleCommand commands = new OracleCommand(locations, connection))
                            {

                                connection.Open();
                                log.Info("Connection opened successfully for location insertion");
                                int counts = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                                if (counts < 10)
                                {
                                    GetAutoID.LID = "LOC" + "00" + counts;
                                }
                                else if (counts > 10 && counts < 100)
                                {
                                    GetAutoID.LID = "LOC" + "0" + counts;
                                }
                                else
                                {
                                    GetAutoID.LID = "LOC" + counts;
                                }
                            }

                            autoIDs.LoID = GetAutoID.LID;

                            note = GetLocation.LocationInsert(autoIDs);
                            if (note >= 1)
                            {
                                log.Info("Location Inserted Successfully");
                                TempData["LocationInsertSuccess"] = "Location Inserted Successfully";
                                return RedirectToAction("LocationList");
                            }
                            else
                            {

                                log.Info("Location Inserted Failed");
                                TempData["LocationInsertFail"] = "Insert location already exists";

                                return RedirectToAction("LocationInsert");

                            }
                        }
                    }



                    if (autoIDs.Branch == "City")
                    {

                        //string selectedvalue = string.Empty;
                        //autoIDs.StaID = selectedvalue;
                        int note = 0;
                        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                        using (OracleConnection connection = new OracleConnection(connectionstring))
                        {
                            string cities = "select count(*) from city_master";
                            log.Info("the city insert query result is:" + cities);
                            using (OracleCommand commands = new OracleCommand(cities, connection))
                            {

                                connection.Open();
                                log.Info("Connection opened successfully for city insertion");
                                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                                if (count < 10)
                                {
                                    GetAutoID.CID = "CIT" + "00" + count;
                                }
                                else if (count > 10 && count < 100)
                                {
                                    GetAutoID.CID = "CIT" + "0" + count;
                                }
                                else
                                {
                                    GetAutoID.CID = "CIT" + count;
                                }
                            }

                            autoIDs.CiID = GetAutoID.CID;
                            note = GetCity.CityInsert(autoIDs);



                            if (note > 0)
                            {
                                TempData["CityInsertSuccess"] = "City Inserted Successfully";
                                log.Info("City Inserted Successfully");
                                return RedirectToAction("LocationList");
                            }
                            else
                            {
                                //TempData["CityInsertFail"] = "City Inserted Failed";
                                log.Info("City Inserted Failed");
                                TempData["CityInsertFail"] = "Insert city already exists";
                                //MessageBox.Show("Insert city already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return RedirectToAction("LocationInsert");
                            }
                        }

                    }




                    if (autoIDs.Branch == "State")
                    {
                        //GetDevice.LocationSelect = GetDevice.PopulateLocation();
                        //// GetDevice.GroupSelect = GetDevice.PopulateGroup();
                        //var locationItem = GetDevice.LocationSelect.Find(p => p.Value == device.LocationName.ToString());
                        //if (locationItem != null)
                        //{
                        //    locationItem.Selected = true;
                        //    device.LocationID = locationItem.Value;
                        //    device.LocationName = locationItem.Text;
                        //}

                        int note = 0;

                        //autoIDs.CoIDs = GetAutoID.COID;

                        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                        using (OracleConnection connection = new OracleConnection(connectionstring))
                        {
                            string states = "select count(*) from state_master";
                            log.Info("the state insert query result is:" + states);
                            using (OracleCommand commands = new OracleCommand(states, connection))
                            {

                                connection.Open();
                                log.Info("Connection opened successfully for state insertion");
                                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                                if (count < 10)
                                {
                                    GetAutoID.SID = "STA" + "00" + count;
                                }
                                else if (count > 10 && count < 100)
                                {
                                    GetAutoID.SID = "STA" + "0" + count;
                                }
                                else
                                {
                                    GetAutoID.SID = "STA" + count;
                                }

                            }
                            autoIDs.StaID = GetAutoID.SID;

                            note = GetState.StateInsert(autoIDs);


                            if (note > 0)
                            {
                                TempData["StateInsertSuccess"] = "State Inserted Succesfully";
                                log.Info("State Inserted Succesfully");
                                return RedirectToAction("LocationList");
                            }
                            else
                            {
                                //TempData["StateInsertFail"] = "State Inserted Failed";
                                log.Info("State Inserted Failed");
                                TempData["StateInsertFail"] = "Insert state already exists";
                                //MessageBox.Show("Insert state already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return RedirectToAction("LocationInsert");
                            }
                        }
                    }
                    if (autoIDs.Branch == "Country")
                    {
                        int note = 0;
                        GetAutoID.CoName = autoIDs.CoName;
                        string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            string country = "select count(*) from country_master";
                            log.Info("the country insert query result is:" + country);
                            using (OracleCommand commands = new OracleCommand(country, connection))
                            {
                                connection.Open();
                                log.Info("Connection opened successfully for country insertion");
                                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                                if (count < 10)
                                {
                                    autoIDs.COID = "COU" + "00" + count;
                                }
                                else if (count > 10 && count < 100)
                                {
                                    autoIDs.COID = "COU" + "0" + count;
                                }
                                else
                                {
                                    autoIDs.COID = "COU" + count;
                                }
                            }
                            GetAutoID.CoIDs = autoIDs.COID;


                            note = GetCountry.CountryInsert(GetAutoID);


                        }


                        if (note > 0)
                        {
                            log.Info("Country Inserted Successfully");
                            TempData["CountryInsertSuccess"] = "Country Inserted Successfully";
                            return RedirectToAction("LocationList");
                        }


                        else
                        {
                            //TempData["CountryInsertFail"] = "Country Inserted Failed";
                            log.Info("Country Inserted Failed");
                            TempData["CountryInsertFail"] = "Insert Country already exists";
                            //MessageBox.Show("Insert Country already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return RedirectToAction("LocationInsert");

                        }

                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            
            catch (Exception ex)
            {

            }

            return RedirectToAction("LocationList");
        }

        public ActionResult UpdateLocation(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = active-nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null )
                {
                    ViewData["LocationCity"] = GetAutoID.PopulateCity();
                    ViewBag.LocationCity = GetAutoID.PopulateCity();
                    ViewBag.LocationUpdateFail = TempData["LocationUpdateFail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            return View(GetLocation.GetLocation().Find(umodel => umodel.LocationId == id));
        }

       

        [HttpPost]

        
        public ActionResult UpdateLocation(Location location, string id)
        {
            try
            {
                if (Session["UserID"] != null )
                {

                    location.CitySelect = location.PopulateCity();
                    var itemCity = location.CitySelect.Find(p => p.Value == location.CityId.ToString());
                    if (itemCity != null)
                    {
                        itemCity.Selected = true;
                        location.CityId = itemCity.Value;
                        location.CityName = itemCity.Text;
                    }
                    int note = 0;
                    note = GetLocation.UpdateLocation(location, id);
                    if (note > 0)
                    {
                        TempData["LocationUpdateSuccess"] = "Location Updated Successfully";
                        log.Info("Location Updated Successfully");
                        return RedirectToAction("LocationList");
                    }
                    else
                    {
                        TempData["LocationUpdateFail"] = "update location already exists";
                        log.Info("Location Updated Failed");
                        //MessageBox.Show("update location already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return RedirectToAction("UpdateLocation");
                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
                    
               }
            
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult DeleteLocation(string id)
        {
            int note = 0;

            try
            {
               
                if (Session["UserID"] != null )
                {

                    
                    note = GetLocation.DeleteLocation(id);
                   
                }
                else
                {
                    return RedirectToAction("DashBoard");

                }
            }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {

                log.Info("location deleted sucessfully");
                TempData["deleteSuccess"] = "Location deleted Successfully";
                return RedirectToAction("LocationList");

            }
            else
            {

                log.Info("location deleted Failed");
                TempData["deletefail"] = "Failed,This is Used in Device";
                return RedirectToAction("LocationList");
            }
        }
        public ActionResult UpdateCity(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = active-nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";


            try
            {
                if (Session["UserID"] != null )
                {

                    ViewData["CityState"] = GetCity.PopulateState();
                    ViewBag.CityState = GetCity.PopulateState();
                    ViewBag.CityUpdateFail = TempData["CityUpdateFail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetCity.GetCity().Find(umodel => umodel.CityId == id));
        }
        [HttpPost]

        public ActionResult UpdateCity(City city, string id)
        {
            try
            {
                if (Session["UserID"] != null )
                {


                    city.StateSelect = city.PopulateState();
                    var itemState = city.StateSelect.Find(p => p.Value == city.ToString());
                    if (itemState != null)
                    {
                        itemState.Selected = true;
                        city.StateId = itemState.Value;
                        city.Statename = itemState.Text;
                    }
                    int note = 0;
                    note = city.UpdateCity(city, id);
                    if (note > 0)
                    {
                        TempData["CityUpdateSuccess"] = "City Updated Successfully";
                        log.Info("City Updated Successfully");
                        return RedirectToAction("LocationList");
                    }
                    else
                    {
                        TempData["CityUpdateSuccess"] = "update city already exists";
                        log.Info("City Updated Failed");
                        //MessageBox.Show("update city already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return RedirectToAction("UpdateCity");
                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
                
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult DeleteCity(string id)
        {

            int note = 0;
            try
            {
                if (Session["UserID"] != null )
                {
                    

                    note = GetCity.DeleteCity(id);
                   
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {

                log.Info("city deleted sucessfully");
                TempData["deleteSuccess"] = "City deleted Successfully";
                return RedirectToAction("LocationList");

            }
            else
            {

                log.Info("city deleted Failed");
                TempData["deletefail"] = "Failed,This is Used in Location";
                return RedirectToAction("LocationList");
            }

        }


        public ActionResult UpdateState(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = active-nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {


                    ViewData["StateCountry"] = GetState.PopulateCountry();
                    ViewBag.StateCountry = GetState.PopulateCountry();
                    ViewBag.StateUpdateFail = TempData["StateUpdateFail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            return View(GetState.GetState().Find(umodel => umodel.StateId == id));
        }

        [HttpPost]

        public ActionResult UpdateState(State state, string id)
        {
            int note = 0;
            try
            {
                if (Session["UserID"] != null)
                {


                    state.CountrySelect = state.PopulateCountry();
                    var itemCountry = state.CountrySelect.Find(p => p.Value == state.CountryId.ToString());
                    if (itemCountry != null)
                    {
                        itemCountry.Selected = true;
                        state.CountryId = itemCountry.Value;
                        state.CountryName = itemCountry.Text;
                    }
                    
                    note = state.UpdateState(state, id);
                    
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
                    
                
            }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {
                TempData["StateUpdateSuccess"] = "State Updated Successfully";
                log.Info("Location Updated Successfully");
                return RedirectToAction("LocationList");
            }
            else
            {
                TempData["StateUpdateFail"] = "update state already exists";
                log.Info("State Updated Failed");
                //MessageBox.Show("update state already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RedirectToAction("UpdateState");
            }
        }

        public ActionResult DeleteState(string id)
        {
            int note = 0;
            try
            {
                if (Session["UserID"] != null && Session["UserRole"].ToString() == "S")
                {
                    
                    note = GetState.DeleteState(id);
                    
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }

            if (note > 0)
            {

                log.Info("state deleted sucessfully");
                TempData["deleteSuccess"] = "State deleted Successfully";
                return RedirectToAction("LocationList");

            }
            else
            {

                log.Info("state deleted Failed");
                TempData["deletefail"] = "Failed,This is Used in Location";
                return RedirectToAction("LocationList");
            }

        }

        public ActionResult UpdateCountry(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = active-nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null )
                {

                    ViewBag.CountryUpdateFail = TempData["CountryUpdateFail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetCountry.GetCountry().Find(umodel => umodel.CountryId == id));
        }
        [HttpPost]

        public ActionResult UpdateCountry(string id, Country master)
        {
            int note = 0;
            try
            {
                if (Session["UserID"] != null)
                {
                    note = GetCountry.UpdateCountry(master, id);
                    log.Info("the updatecountry : " + note);
                    
                }
                else
                {

                }
                    
                
            }

            catch (Exception ex)
            {

            }
            if (note > 0)
            {
                TempData["CountryUpdateSuccess"] = "Country Updated Successfully";
                log.Info("Country Updated Successfully");
                return RedirectToAction("LocationList");

            }
            else
            {
                TempData["CountryUpdateFail"] = "update Country already exists";
                log.Info("Country Updated Failed");
                //MessageBox.Show("update Country already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RedirectToAction("UpdateCountry");
            }
        }

        public ActionResult DeleteCountry(string id)
        {
            int note = 0;

            try
            {
                
                if (Session["UserID"] != null && Session["UserRole"].ToString() == "S")
                {
                    note = GetCountry.DeleteCountry(id);
                   
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }



            }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {

                log.Info("country deleted successfully");
                TempData["deleteSuccess"] = "Country deleted Successfully";
                return RedirectToAction("LocationList");

            }
            else
            {

                log.Info("Country deleted Failed");
                TempData["deletefail"] = "Failed,This is Used in Location";
                return RedirectToAction("LocationList");
            }

        }

        DeviceMaster GetDevice = new DeviceMaster();
        DAutoID GetDAuto = new DAutoID();

        [HttpGet]

        public ActionResult DeviceList()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = active-nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null )
                {


                    ViewBag.Success = TempData["Success"];

                    ViewBag.UpdateSuccess = TempData["UpdateSuccess"];

                    ViewBag.DevSuccess = TempData["deleteSuccess"];
                    ViewBag.DevFail = TempData["deletefail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return View(GetDevice.DeviceList());
        }

        [HttpPost]

        public ActionResult DeviceList(DeviceMaster device)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            return View(GetDevice.DeviceList());
        }

        public ActionResult DeviceDirect()
        {
            return RedirectToAction("DeviceInsert");
        }

        public ActionResult DeviceInsert()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = active-nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null )
                {


                    GetDevice.LocationSelect = GetDevice.PopulateLocation();
                    // GetDevice.GroupSelect = GetDevice.PopulateGroup();
                    ViewBag.Fail = TempData["Fail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return View(GetDevice);
        }

        [HttpPost]

        public ActionResult DeviceInsert(DeviceMaster device)
        {
            int success = 0;
            try
            {
                if (Session["UserID"] != null)
                {


                    GetDevice.LocationSelect = GetDevice.PopulateLocation();
                    // GetDevice.GroupSelect = GetDevice.PopulateGroup();
                    var locationItem = GetDevice.LocationSelect.Find(p => p.Value == device.LocationName.ToString());
                    if (locationItem != null)
                    {
                        locationItem.Selected = true;
                        device.LocationID = locationItem.Value;
                        device.LocationName = locationItem.Text;
                    }
                    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {
                        string locations = "select count(*) from device_master";
                        log.Info("the deviceinsert query result is:" + locations);
                        using (OracleCommand commands = new OracleCommand(locations, connection))
                        {
                            connection.Open();
                            Log.Info("Connection Opened Successfully for Deviceinsert");
                            int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                            if (count < 10)
                            {
                                GetDAuto.DID = "DEV" + "00" + count;
                            }
                            else if (count > 10 && count < 100)
                            {
                                GetDAuto.DID = "DEV" + "0" + count;
                            }
                            else
                            {
                                GetDAuto.DID = "DEV" + count;
                            }
                        }

                    }
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {

                        string locations = "select a.location_id,a.location_name,b.city_id,b.city_name,c.state_id,c.state_name," +
                            "d.country_id,d.country_name from location_master a left join city_master b on a.city_id = b.city_id " +
                            "left join state_master c on b.state_id = c.state_id left join country_master d on c.country_id = d.country_id" +
                            " where location_id = '" + device.LocationID + "'";
                        log.Info("the deviceinsert query result is:" + locations);
                        using (OracleCommand command = new OracleCommand(locations, connection))
                        {
                            OracleDataAdapter dataAdapter = new OracleDataAdapter(command.CommandText, connection);
                            System.Data.DataTable dataTable = new System.Data.DataTable();
                            dataAdapter.Fill(dataTable);
                            int RowCount = dataTable.Rows.Count;
                            device.LocationID = dataTable.Rows[0]["LOCATION_ID"].ToString();
                            device.CityID = dataTable.Rows[0]["CITY_ID"].ToString();
                            device.StateID = dataTable.Rows[0]["STATE_ID"].ToString();
                            device.CountryID = dataTable.Rows[0]["COUNTRY_ID"].ToString();
                        }
                        device.DeviceID = GetDAuto.DID;
                        GetDevice.DeviceIP = device.DeviceIP;
                        GetDevice.DevicePort = device.DevicePort;
                        GetDevice.DeviceMake = device.DeviceMake;

                        GetDevice.Bandwidth = device.Bandwidth;

                        //int success = 0;
                        success = GetDevice.DeviceInsert(device);

                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            if (success >= 1)
            {
                log.Info("the device insert successfull");
                TempData["Success"] = " Device Inserted Successfully";
                return RedirectToAction("DeviceList");
            }
            else
            {
                TempData["Fail"] = "Device Insert ip and port already exists";
                //GetDevice.LocationSelect = GetDevice.PopulateLocation();
                log.Info("the device insert failed");
                //MessageBox.Show("Device Insert ip and port already exists","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return RedirectToAction("DeviceInsert");
            }
           
        }
        public ActionResult UpdateDevice(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = active-nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {


                    ViewData["Location"] = GetDevice.PopulateLocation();
                    ViewBag.Location = GetDevice.PopulateLocation();
                    //ViewData["Group"] = GetDevice.PopulateGroup();
                    //ViewBag.Group = GetDevice.PopulateGroup();
                    ViewBag.UpdateFail = TempData["UpdateFail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return View(GetDevice.DeviceList().Find(umodel => umodel.DeviceID == id));
        }

        [HttpPost]

        public ActionResult UpdateDevice(DeviceMaster device,string id)
        {
            int success = 0;
            try
            {
                if (Session["UserID"] != null)
                {
                    GetDevice.LocationSelect = GetDevice.PopulateLocation();
                    // GetDevice.GroupSelect = GetDevice.PopulateGroup();
                    var locationItem = GetDevice.LocationSelect.Find(p => p.Value == device.LocationID.ToString());
                    if (locationItem != null)
                    {
                        locationItem.Selected = true;
                        device.LocationID = locationItem.Value;
                        device.LocationName = locationItem.Text;
                    }
                    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {
                        string locations = "select a.location_id,a.location_name,b.city_id,b.city_name,c.state_id,c.state_name," +
                            "d.country_id,d.country_name from location_master a left join city_master b on a.city_id = b.city_id " +
                            "left join state_master c on b.state_id = c.state_id left join country_master d on c.country_id = d.country_id" +
                            " where location_id = '" + device.LocationID + "'";
                        log.Info("the updatedevice query result is:" + locations);
                        using (OracleCommand command = new OracleCommand(locations, connection))
                        {
                            OracleDataAdapter dataAdapter = new OracleDataAdapter(command.CommandText, connection);
                            System.Data.DataTable dataTable = new System.Data.DataTable();
                            dataAdapter.Fill(dataTable);
                            int RowCount = dataTable.Rows.Count;
                            log.Info("the number of rows retrieved from datatable is:" + RowCount);
                            device.LocationID = dataTable.Rows[0]["LOCATION_ID"].ToString();
                            device.CityID = dataTable.Rows[0]["CITY_ID"].ToString();
                            device.StateID = dataTable.Rows[0]["STATE_ID"].ToString();
                            device.CountryID = dataTable.Rows[0]["COUNTRY_ID"].ToString();
                        }
                    }

                    success = GetDevice.UpdateDevice(device, id);
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
                
            }
            catch (Exception ex)
            {

            }

            if (success >= 1)
            {
                TempData["UpdateSuccess"] = "Device Updated Successfully";
                log.Info("Device Updated Successfully");
                return RedirectToAction("DeviceList");

            }
            else
            {
                TempData["UpdateFail"] = "Device update ip and port already exists";
                log.Info("Device Updated failed");
                //MessageBox.Show("Device update ip and port already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RedirectToAction("UpdateDevice");
            }
            
           
        }
        public ActionResult DeleteDevice(string id)
        {
            //GetDevice.DeviceIP = device.DeviceIP;
            //GetDevice.DevicePort = device.DevicePort;
            int note = 0;
            
            try
            {
                

                if (Session["UserID"] != null)
                {
                    note = GetDevice.DeleteDevice(id);
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {

                log.Info("Device deleted sucessfully");
                TempData["deleteSuccess"] = "Device deleted Successfully";
                return RedirectToAction("DeviceList");
            }
            else
            {

                log.Info("Device deleted Failed");
                TempData["deletefail"] = "Failed,This is Used in Schedule";
                return RedirectToAction("DeviceList");
            }

        }

      

        UserMaster GetUser = new UserMaster();
        //UAutoID GetuAuto = new UAutoID();

        

        
       

        public ActionResult UserDirect()
        {
            return RedirectToAction("UserInsert");
        }

        public ActionResult UserInsert()
        {
            try
            {
                if (Session["UserID"] != null)
                {
                //    GetUser.CitySelect = GetUser.PopulateCity();
                //    GetUser.StateSelect = GetUser.PopulateState();
                //    GetUser.CountrySelect = GetUser.PopulateCountry();
                //    GetUser.GroupSelect = GetUser.PopulateGroup();
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetUser);
        }

        [HttpPost]

        public ActionResult UserInsert(UserMaster user)
        {
        //    try
        //    {
        //        GetUser.CitySelect = GetUser.PopulateCity();
        //        GetUser.StateSelect = GetUser.PopulateState();
        //        GetUser.CountrySelect = GetUser.PopulateCountry();
        //        GetUser.GroupSelect = GetUser.PopulateGroup();
        //        var cityItem = GetUser.CitySelect.Find(p => p.Value == user.CityName.ToString());
        //        if (cityItem != null)
        //        {
        //            cityItem.Selected = true;
        //            user.CityID = cityItem.Value;
        //            user.CityName = cityItem.Text;
        //        }
        //        var stateItem = GetUser.StateSelect.Find(p => p.Value == user.StateName.ToString());
        //        if (stateItem != null)
        //        {
        //            stateItem.Selected = true;
        //            user.StateID = stateItem.Value;
        //            user.StateName = stateItem.Text;
        //        }
        //        var countryItem = GetUser.CountrySelect.Find(p => p.Value == user.CountryName.ToString());
        //        if (countryItem != null)
        //        {
        //            countryItem.Selected = true;
        //            user.CountryID = countryItem.Value;
        //            user.CountryName = countryItem.Text;
        //        }
        //        string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        //        using (OracleConnection connection = new OracleConnection(ConnectionString))
        //        {
        //            string locations = "select count(*) from user_master";
        //            log.Info("the userinsert query result is:" + locations);
        //            using (OracleCommand commands = new OracleCommand(locations, connection))
        //            {
        //                connection.Open();
        //                log.Info("Connection Opened Successfully for userinsert");
        //                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
        //                log.Info("the count value is:" + count);
        //                if (count < 10)
        //                {
        //                    GetuAuto.UID = "USR" + "00" + count;
        //                }
        //                else if (count > 10 && count < 100)
        //                {
        //                    GetuAuto.UID = "USR" + "0" + count;
        //                }
        //                else
        //                {
        //                    GetuAuto.UID = "USR" + count;
        //                }
        //            }
        //        }
        //        if (user.Group == "Existing")
        //        {
        //            log.Info("the selected user is :" + user.Group);
        //            var groupItem = GetUser.GroupSelect.Find(p => p.Value == user.GroupName.ToString());
        //            if (groupItem != null)
        //            {
        //                groupItem.Selected = true;
        //                user.GroupId = groupItem.Value;
        //                user.GroupName = groupItem.Text;
        //            }
        //            user.UserID = GetuAuto.UID;
        //            int note = 0;
        //            note=GetUser.UserInsert(user);
        //            if(note>=1)
        //            {
        //                TempData["InsertSuccess"] = "User Inserted Successfully";
        //                log.Info("User Inserted Successfully");
        //            }
        //            else
        //            {
        //                TempData["InsertFail"] = "User Inserted Failed";
        //                log.Info("User Inserted Failed");
        //            }
        //        }
        //        if (user.Group == "New")
        //        {
        //            log.Info("the selected user is :" + user.Group);
        //            string Connection = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        //            using (OracleConnection connection = new OracleConnection(Connection))
        //            {
        //                string locations = "select count(*) from group_master";
        //                log.Info("the userlist new query result is:" + locations);
        //                using (OracleCommand commands = new OracleCommand(locations, connection))
        //                {
        //                    connection.Open();
        //                    log.Info("Connection Opened Successfully for userinsert new");
        //                    int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
        //                    log.Info("the count value is:" + count);
        //                    if (count < 10)
        //                    {
        //                        GetuAuto.GID = "GRP" + "00" + count;
        //                    }
        //                    else if (count > 10 && count < 100)
        //                    {
        //                        GetuAuto.GID = "GRP" + "0" + count;
        //                    }
        //                    else
        //                    {
        //                        GetuAuto.GID = "GRP" + count;
        //                    }
        //                }
        //            }
        //            user.GroupId = GetuAuto.GID;
        //            using (OracleConnection connection = new OracleConnection(Connection))
        //            {
        //                connection.Open();
        //                string query = "insert into group_master(group_id,group_desc,category)values('" + GetuAuto.GID + "','" + user.Gname + "','TYPE2')";
        //                log.Info("the  query result is:" + query);
        //                using (OracleCommand command = new OracleCommand(query, connection))
        //                {
        //                    command.ExecuteNonQuery();
        //                    log.Info("Command executed successfully");
        //                }
        //                connection.Close();
        //            }
        //            user.UserID = GetuAuto.UID;
        //            int note = 0;
        //            note = GetUser.UserInsert(user);
        //            if (note >= 1)
        //            {
        //                TempData["InsertSuccess"] = "User Inserted Successfully";
        //                log.Info("User Inserted Successfully");
        //            }
        //            else
        //            {
        //                TempData["InsertFail"] = "User Inserted Failed";
        //                log.Info("User Inserted Failed");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
            return RedirectToAction("UserList");
        }

       

        [HttpPost]

        public ActionResult UpdateUser(UserMaster user, string id)
        {
            try
            {

                GetUser.GroupSelect = GetUser.PopulateGroup();
                GetUser.CitySelect = GetUser.PopulateCity();
                GetUser.StateSelect = GetUser.PopulateState();
                GetUser.CountrySelect = GetUser.PopulateCountry();
                var groupItem = GetUser.GroupSelect.Find(p => p.Value == user.GroupName.ToString());
                if (groupItem != null)
                {
                    groupItem.Selected = true;
                    user.GroupId = groupItem.Value;
                    user.GroupName = groupItem.Text;
                }
                var cityItem = GetUser.CitySelect.Find(p => p.Value == user.CityName.ToString());
                if (cityItem != null)
                {
                    cityItem.Selected = true;
                    user.CityID = cityItem.Value;
                    user.CityName = cityItem.Text;
                }
                var stateItem = GetUser.StateSelect.Find(p => p.Value == user.StateName.ToString());
                if (stateItem != null)
                {
                    stateItem.Selected = true;
                    user.StateID = stateItem.Value;
                    user.StateName = stateItem.Text;
                }
                var countryItem = GetUser.CountrySelect.Find(p => p.Value == user.CountryName.ToString());
                if (countryItem != null)
                {
                    countryItem.Selected = true;
                    user.CountryID = countryItem.Value;
                    user.CountryName = countryItem.Text;
                }
                int note = 0;
                note=GetUser.UpdateUser(user, id);
                if (note >= 1)
                {
                    TempData["UpdateSuccess"] = "User Updated Successfully";
                    log.Info("User Updated Successfully");
                }
                else
                {
                    TempData["UpdateFail"] = "User Updated Failed";
                    log.Info("User Updated Failed");
                }
                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        //public ActionResult DeleteUser(string id)
        //{
        //    try
        //    {
        //        GetUser.DeleteUser(id);
        //        log.Info("delete user Successfully");
        //        return RedirectToAction("UserList");
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return View();
        //}

        RepositoryMaster GetRepository = new RepositoryMaster();
        RAutoID GetRAuto = new RAutoID();
        FileProcessor fileProcessor = new FileProcessor();

        [HttpGet]

        public ActionResult RepositoryList()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = active-nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";


            try
            {
                if (Session["UserID"] != null )
                {
                    ViewBag.InsertSuccess = TempData["InsertSuccess"];
                    ViewBag.InsertFail = TempData["InsertFail"];
                    ViewBag.UpdateSuccess = TempData["UpdateSuccess"];
                    ViewBag.UpdateFail = TempData["UpdateFail"];
                    ViewBag.RepSuccess = TempData["deleteSuccess"];
                    ViewBag.RepFail = TempData["deletefail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return View(GetRepository.RepositoryList());
        }

        [HttpPost]

        public ActionResult RepositoryList(RepositoryMaster repositoryMaster)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult RepositoryDirect()
        {
            return RedirectToAction("RepositoryInsert");
        }
        public ActionResult RepositoryInsert()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = active-nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {
                    GetRepository.FileSelect = GetRepository.PopulateFiles();
                    GetRepository.PathSelect = GetRepository.PopulatePath();
                    ViewBag.ddlitemlist = GetRepository.PopulateFiles();
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetRepository);
        }

   

        [HttpPost]

        public ActionResult RepositoryInsert(RepositoryMaster repository, HttpPostedFileBase[] files)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    GetRepository.FileSelect = GetRepository.PopulateFiles();
                    GetRepository.PathSelect = GetRepository.PopulatePath();
                    if (repository.File == "File")
                    {
                        log.Info("the select respository type is :" + repository.File);
                        var groupItem = GetRepository.PathSelect.Find(p => p.Value == repository.RepPath.ToString());
                        if (groupItem != null)
                        {
                            groupItem.Selected = true;
                            repository.RepPath = groupItem.Text;
                        }
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            string locations = "select count(*) from repository_master";
                            log.Info("the  Respositoryinsert result is:" + locations);
                            using (OracleCommand commands = new OracleCommand(locations, connection))
                            {
                                connection.Open();
                                log.Info("Connection Opened Successfully for respository insert");
                                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                                if (count < 10)
                                {
                                    GetRAuto.RID = "REP" + "00" + count;
                                }
                                else if (count > 10 && count < 100)
                                {
                                    GetRAuto.RID = "REP" + "0" + count;
                                }
                                else
                                {
                                    GetRAuto.RID = "REP" + count;
                                }
                            }
                        }
                        repository.RepCode = GetRAuto.RID;

                        foreach (HttpPostedFileBase file in files)
                        {

                           
                            if (file != null && file.ContentLength > 0)
                            {
                                string sdirectoryPath = "C:\\Repository\\SendFiles\\";
                                string  sfilename = file.FileName;

                                string filepath = Path.Combine(sdirectoryPath, sfilename);

                               
                                 if (!System.IO.File.Exists(filepath))
                                 {
                                    byte[] imagedatacontent;

                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        file.InputStream.CopyTo(ms);
                                        imagedatacontent = ms.ToArray();

                                    }
                                    //byte[] ImageDataFromDatabase = file.InputStream;
                                    repository.ImageData = imagedatacontent;
                                }
                                
                                else
                                {
                                   
                                   repository.ImageData = null;
                                }



                                

                               
                            }





                            string filename = string.Empty;

                            string fname = string.Empty;
                            log.Info("the fname is:" + fname);
                            for (int i = 0; i < files.Count(); i++)
                            {
                                //string repos = repository.RepPath + repository.RepPath + files[i].FileName;
                                //string[] repfilename = repos.Split('\\');
                                string[] repfilename = files[i].FileName.Split('\\');

                                int rcount = repfilename.Length;
                                if (rcount == 1)
                                {

                                    filename = files[i].FileName;
                                    log.Info("the filename is:" + filename);
                                }
                                else
                                {

                                    filename = repfilename[rcount - 1].ToString();
                                    log.Info("the filename is:" + filename);
                                }



                                if (fname == string.Empty)
                                {
                                    fname = filename;
                                }
                                else
                                {
                                    fname = fname + ',' + filename;
                                }

                            }

                            repository.RepName = fname;
                            int note = 0;
                            note = GetRepository.RepositoryInsert(repository);
                            if (note >= 1)
                            {
                                TempData["InsertSuccess"] = "Repository Inserted Successfully";
                                log.Info("Repository Inserted Successfully");
                            }
                            else
                            {
                                TempData["InsertFail"] = "Repository Inserted failed;File Name Already exists;";
                                log.Info("Repository Inserted failed");
                            }
                        }


                    }
                    if (repository.File == "Directory")
                    {
                        log.Info("the select respository type is :" + repository.File);
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            string locations = "select count(*) from directory_master";
                            log.Info("the  Respositoryinsert directory result is:" + locations);
                            using (OracleCommand commands = new OracleCommand(locations, connection))
                            {
                                connection.Open();
                                log.Info("Connection Opened Successfully for respositortinsert directory");
                                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
                                if (count < 10)
                                {
                                    GetRAuto.DID = "DIR" + "00" + count;
                                }
                                else if (count > 10 && count < 100)
                                {
                                    GetRAuto.DID = "DIR" + "0" + count;
                                }
                                else
                                {
                                    GetRAuto.DID = "DIR" + count;
                                }
                            }
                        }
                        repository.DirCode = GetRAuto.DID;
                        repository.DateTime = DateTime.Now.ToString();
                        int n = 0;
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            connection.Open();

                            string query = "insert into directory_master(dir_code,dir_path)values('" + repository.DirCode + "','" + repository.DestinationPath + "')";
                            Log.Info("the  query result is:" + query);
                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                n = command.ExecuteNonQuery();
                            }
                            connection.Close();
                        }
                        if (n >= 1)
                        {
                            TempData["InsertSuccess"] = "Directory Inserted Successfully";
                            log.Info("Directory Inserted Successfully");
                        }
                        else
                        {
                            TempData["InsertFail"] = "Directory Inserted failed";
                            log.Info("Directory Inserted failed");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");

                }


            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("RepositoryList"); 
        }

        public ActionResult RepositoryUpdate(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = active-nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {
                    ViewData["Repository"] = GetRepository.PopulateFiles();
                    ViewBag.Repository = GetRepository.PopulateFiles();
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetRepository.RepositoryList().Find(umodel => umodel.RepCode == id));
        }

        [HttpPost]

        public ActionResult RepositoryUpdate(RepositoryMaster repository, HttpPostedFileBase[]filesUpdate, string id)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    GetRepository.FileSelect = GetRepository.PopulateFiles();
                    ViewData["Repository"] = GetRepository.PopulateFiles();
                    var reposItem = GetRepository.FileSelect.Find(p => p.Text == repository.RepName.ToString());
                    if (reposItem != null)
                    {
                        reposItem.Selected = true;
                        // repository.RepName = reposItem.Text;
                    }
                    string fname = string.Empty;
                    log.Info("the fname is:" + fname);
                    for (int i = 0; i < filesUpdate.Count(); i++)
                    {
                        string filename = filesUpdate[i].FileName;
                        if (fname == string.Empty)
                        {
                            fname = filename;
                        }
                        else
                        {
                            fname = fname + ',' + filename;
                        }

                    }
                    repository.RepName = fname;
                    int note = 0;
                    note = GetRepository.UpdateRepository(repository, id);
                    if (note >= 1)
                    {
                        TempData["UpdateSuccess"] = "Repository Updated Successfully";
                        log.Info("Repository Updated Successfully");
                    }
                    else
                    {

                        TempData["UpdateFail"] = "Repository Updated Failed";
                        log.Info("Repository Updated Failed");
                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
                
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("RepositoryList");
        }

        public ActionResult RepositoryDelete(string id)
        {
            int note = 0;
            
            try
            {
                if (Session["UserID"] != null)
                {

                    note = GetRepository.DeleteRepository(id);
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

             }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {

                log.Info("Repository deleted sucessfully");
                TempData["deleteSuccess"] = "Repository deleted Successfully";
                return RedirectToAction("RepositoryList");
            }
            else
            {

                log.Info("Repository deleted Failed");
                TempData["deletefail"] = "Failed,This File Used in Schedule";
                return RedirectToAction("RepositoryList");
            }

        }
        ScheduleMaster GetSchedule = new ScheduleMaster();
        SAutoID GetSAuto = new SAutoID();

        public ActionResult ScheduleList()
        {

            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = active-nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {
                    ViewBag.InsertSuccess = TempData["InsertSuccess"];
                    ViewBag.InsertFail = TempData["InsertFail"];
                    ViewBag.UpdateSuccess = TempData["UpdateSuccess"];
                    ViewBag.UpdateFail = TempData["UpdateFail"];
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch(Exception ex)
            {

            }
            return View(GetSchedule.ScheduleList());
        }

        [HttpPost]

        public ActionResult ScheduleList(ScheduleMaster schedule)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult ScheduleDirect()
        {
            return RedirectToAction("ScheduleInsert");
        }

        public ActionResult ScheduleInsert()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = active-nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {
                    GetSchedule.RSelect = GetSchedule.PopulateRepositoryPath();
                    GetSchedule.RepSelect = GetSchedule.PopulateRepository();
                    GetSchedule.DirSelect = GetSchedule.PopulateDirectory();
                    GetSchedule.LocationSelect = GetSchedule.PopulateLocation();
                    GetSchedule.DeviceSelect = GetSchedule.PopulateDevices();

                    GetSchedule.GroupSelect = GetSchedule.PopulateGroup();
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetSchedule);
        }

        [HttpPost]

        public ActionResult ScheduleInsert(ScheduleMaster schedules,string STime,string ITime)
        {
            int note = 0;
            try
            {
                if (Session["UserID"] != null)
                {
                    GetSchedule.RepSelect = GetSchedule.PopulateRepository();
                    GetSchedule.DirSelect = GetSchedule.PopulateDirectory();
                    //GetSchedule.LocationSelect = GetSchedule.PopulateLocation();
                    GetSchedule.DeviceSelect = GetSchedule.PopulateDevices();
                    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    var repItem = GetSchedule.RepSelect.Find(p => p.Value == schedules.RepName.ToString());
                    if (repItem != null)
                    {
                        repItem.Selected = true;
                        schedules.RepID = repItem.Value;
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            string query = "select repname from repository_master where repcode='" + schedules.RepID + "'";
                            log.Info("the scheduleinsert result is:" + query);
                            OracleCommand command = new OracleCommand(query, connection);
                            OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                            DataTable dataTable = new DataTable();
                            connection.Open();
                            log.Info("Connection Opened Successfully for scheduleinsert");
                            dataAdapter.Fill(dataTable);
                            connection.Close();
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                schedules.SetupFile = Convert.ToString(dr["repname"]);
                            }
                            connection.Close();
                        }
                        schedules.RepName = repItem.Text;
                    }

                    GetSchedule.GroupSelect = GetSchedule.PopulateGroup();
                    if (schedules.Group != null && schedules.ProfileType == "GROUP")
                    {
                        var groupItem = GetSchedule.GroupSelect.Find(p => p.Value == schedules.Group.ToString());
                        if (groupItem != null)
                        {
                            groupItem.Selected = true;
                            schedules.GroupCode = groupItem.Value;

                            using (OracleConnection connection = new OracleConnection(ConnectionString))
                            {
                                string query = "select * from profile_master where profile_code='" + schedules.GroupCode + "'";
                                log.Info("the  result is:" + query);
                                OracleCommand command = new OracleCommand(query, connection);
                                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                                DataTable dataTable = new DataTable();
                                connection.Open();
                                log.Info("Connection Opened Successfully");
                                dataAdapter.Fill(dataTable);
                                connection.Close();
                                foreach (DataRow dr in dataTable.Rows)
                                {
                                    schedules.GroupOfDevice = Convert.ToString(dr["profile_criteria"]);
                                }
                                connection.Close();
                            }
                            schedules.GroupName = groupItem.Text;
                            string[] profile = schedules.GroupOfDevice.Split('*');
                            string profileSelect = profile[0];
                            string[] profileCriteria = profile[1].Split('#');
                            string CombineDevice = string.Empty;
                            if (profileSelect == "LOCATION")
                            {
                                List<string> LocationDeviceList = new List<string>(profileCriteria);
                                List<string> CombinedDeviceList = new List<string>();
                                foreach (var item in LocationDeviceList)
                                {
                                    
                                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                                    {
                                        string query = "select device_id from location_master l left join device_master d on  d.location_id = l.location_id where l.location_id='" + item+ "'";
                                        log.Info("the  result is:" + query);
                                        OracleCommand command = new OracleCommand(query, connection);
                                        OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                                        DataTable dataTable = new DataTable();
                                        connection.Open();
                                        log.Info("Connection Opened Successfully");
                                        dataAdapter.Fill(dataTable);
                                        connection.Close();
                                        foreach (DataRow dr in dataTable.Rows)
                                        {
                                            CombinedDeviceList.Add(Convert.ToString(dr["Device_id"]));

                                            
                                        }
                                        connection.Close();
                                    }


                                    //CombineDevice = string.Join(",", deviceids);
                                    //CombinedDeviceList.Add(CombineDevice);

                                   

                                }

                                //string finalCombinedDevices = string.Join(",", CombinedDeviceList);


                                GroupDevice = CombinedDeviceList;


                            }
                            else
                            {
                                GroupDevice = new List<string>(profileCriteria);

                            }

                            

                        }
                    }
                    GetSchedule.DirSelect = GetSchedule.PopulateDirectory();
                    var directItem = GetSchedule.DirSelect.Find(p => p.Value == schedules.DirName.ToString());
                    if (directItem != null)
                    {
                        directItem.Selected = true;
                        schedules.DirID = directItem.Value;
                        schedules.DirName = directItem.Text;
                    }
                    //GetSchedule.LocationSelect = GetSchedule.PopulateLocation();
                    GetSchedule.DeviceSelect = GetSchedule.PopulateDevices();
                    if (schedules.DeviceName != null)
                    {
                        var deviceItem = GetSchedule.DeviceSelect.Find(p => p.Value == schedules.DeviceName.ToString());
                        if (deviceItem != null)
                        {
                            deviceItem.Selected = true;
                            schedules.DeviceIP = deviceItem.Text;
                            schedules.DeviceID = deviceItem.Value;
                        }
                    }
                    if (schedules.ProfileType == "GROUP" && GroupDevice != null)
                    {

                        foreach (var deviceitem in GroupDevice)
                        {

                            using (OracleConnection connection = new OracleConnection(ConnectionString))
                            {
                                string locations = "select count(*) from schedule_master";
                                log.Info("the  Scheduleinsert result is:" + locations);
                                using (OracleCommand commands = new OracleCommand(locations, connection))
                                {
                                    connection.Open();
                                    int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
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

                                if (schedules.Schedule == "SCHEDULE")
                                {
                                    log.Info("the selected schedule type is :" + schedules.Schedule);
                                    schedules.Group = schedules.GroupCode;
                                    schedules.ProfileType = schedules.ProfileType;
                                    schedules.ScheduleCode = GetSAuto.SID;
                                    schedules.ScheduleType = schedules.Schedule;
                                    schedules.RepCode = schedules.RepID;
                                    schedules.Flag = "B";
                                    schedules.DestinationPath = schedules.DirName;
                                    schedules.DirCode = schedules.DirID;
                                    schedules.SetupFile = schedules.SetupFile;
                                    schedules.ScheduleDate = schedules.ScheduleDate;
                                    schedules.ScheduleTime = schedules.ScheduleTime;
                                    schedules.DeployDate = "-";
                                    schedules.DeployTime = "-";
                                    schedules.ObjID = deviceitem;
                                    //if (schedules.Profile == "DEVICE")
                                    //{
                                    //    schedules.ObjID = GetSAuto.OID;
                                    //}
                                    //if (schedules.Profile == "LOCATION")
                                    //{
                                    //    schedules.ObjID = GetSAuto.OID;
                                    //}

                                    note = GetSchedule.ScheduleInsert(schedules);
                                    if (note >= 1)
                                    {
                                        TempData["InsertSuccess"] = "Schedule Inserted Successfully";
                                        log.Info("Schedule Inserted Successfully");
                                    }
                                    else
                                    {
                                        TempData["InsertFail"] = "Schedule Inserted Failed";
                                        log.Info("Schedule Inserted Failed");
                                    }
                                }
                                if (schedules.Schedule == "IMMEDIATE")
                                {
                                    log.Info("the selected schedule type is :" + schedules.Schedule);
                                    schedules.Group = schedules.GroupCode;
                                    schedules.ProfileType = schedules.ProfileType;
                                    schedules.ScheduleType = schedules.Schedule;
                                    schedules.ScheduleCode = GetSAuto.SID;
                                    schedules.RepCode = schedules.RepID;
                                    schedules.RepName = schedules.RepName;
                                    schedules.Flag = "B";
                                    schedules.DestinationPath = schedules.DirName;
                                    schedules.DirCode = schedules.DirID;
                                    schedules.SetupFile = schedules.SetupFile;
                                    schedules.ScheduleDate = "-";
                                    schedules.ScheduleTime = "-";
                                    schedules.DeployDate = DateTime.Now.ToString("dd-MM-yyyy");
                                    DateTime currentTime = DateTime.Now;
                                    DateTime time = currentTime.AddMinutes(3);
                                    schedules.DeployTime = time.ToString("hh:mm:ss");
                                    schedules.ObjID = deviceitem;
                                    //if (schedules.Profile == "DEVICE")
                                    //{
                                    //    schedules.ObjID = GetSAuto.OID;
                                    //}
                                    //if (schedules.Profile == "LOCATION")
                                    //{
                                    //    schedules.ObjID = GetSAuto.OID;
                                    //}

                                    note = GetSchedule.ScheduleInsert(schedules);

                                    if (note >= 1)
                                    {
                                        TempData["InsertSuccess"] = "Schedule Inserted Successfully";
                                        log.Info("Schedule Inserted Successfully");
                                    }
                                    else
                                    {
                                        TempData["InsertFail"] = "Schedule Inserted Failed";
                                        log.Info("Schedule Inserted Failed");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (OracleConnection connection = new OracleConnection(ConnectionString))
                        {
                            string locations = "select count(*) from schedule_master";
                            log.Info("the  Scheduleinsert result is:" + locations);
                            using (OracleCommand commands = new OracleCommand(locations, connection))
                            {
                                connection.Open();
                                int count = Convert.ToInt16(commands.ExecuteScalar()) + 1;
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

                            if (schedules.Schedule == "SCHEDULE")
                            {
                                log.Info("the selected schedule type is :" + schedules.Schedule);
                                schedules.Group = schedules.GroupCode;
                                schedules.ProfileType = schedules.ProfileType;
                                schedules.ScheduleCode = GetSAuto.SID;
                                schedules.ScheduleType = schedules.Schedule;
                                schedules.RepCode = schedules.RepID;
                                schedules.Flag = "B";
                                schedules.DestinationPath = schedules.DirName;
                                schedules.DirCode = schedules.DirID;
                                schedules.SetupFile = schedules.SetupFile;
                                schedules.ScheduleDate = schedules.ScheduleDate;
                                schedules.ScheduleTime = schedules.ScheduleTime;
                                schedules.DeployDate = "-";
                                schedules.DeployTime = "-";
                                schedules.ObjID = schedules.DeviceID;
                                //if (schedules.Profile == "DEVICE")
                                //{
                                //    schedules.ObjID = GetSAuto.OID;
                                //}
                                //if (schedules.Profile == "LOCATION")
                                //{
                                //    schedules.ObjID = GetSAuto.OID;
                                //}

                                note = GetSchedule.ScheduleInsert(schedules);
                                if (note >= 1)
                                {
                                    TempData["InsertSuccess"] = "Schedule Inserted Successfully";
                                    log.Info("Schedule Inserted Successfully");
                                }
                                else
                                {
                                    TempData["InsertFail"] = "Schedule Inserted Failed";
                                    log.Info("Schedule Inserted Failed");
                                }
                            }
                            if (schedules.Schedule == "IMMEDIATE")
                            {
                                log.Info("the selected schedule type is :" + schedules.Schedule);
                                schedules.Group = schedules.GroupCode;
                                schedules.ProfileType = schedules.ProfileType;
                                schedules.ScheduleType = schedules.Schedule;
                                schedules.ScheduleCode = GetSAuto.SID;
                                schedules.RepCode = schedules.RepID;
                                schedules.RepName = schedules.RepName;
                                schedules.Flag = "B";
                                schedules.DestinationPath = schedules.DirName;
                                schedules.DirCode = schedules.DirID;
                                schedules.SetupFile = schedules.SetupFile;
                                schedules.ScheduleDate = "-";
                                schedules.ScheduleTime = "-";
                                schedules.DeployDate = DateTime.Now.ToString("dd-MM-yyyy");
                                DateTime currentTime = DateTime.Now;
                                DateTime time = currentTime.AddMinutes(3);
                                schedules.DeployTime = time.ToString("hh:mm:ss");
                                schedules.ObjID = schedules.DeviceID;
                                //if (schedules.Profile == "DEVICE")
                                //{
                                //    schedules.ObjID = GetSAuto.OID;
                                //}
                                //if (schedules.Profile == "LOCATION")
                                //{
                                //    schedules.ObjID = GetSAuto.OID;
                                //}

                                note = GetSchedule.ScheduleInsert(schedules);

                                if (note >= 1)
                                {
                                    TempData["InsertSuccess"] = "Schedule Inserted Successfully";
                                    log.Info("Schedule Inserted Successfully");
                                }
                                else
                                {
                                    TempData["InsertFail"] = "Schedule Inserted Failed";
                                    log.Info("Schedule Inserted Failed");
                                }
                            }
                        }
                    }
                }

                else
                {
                    return RedirectToAction("DashBoard");

                }
                
            }

            catch (Exception ex)
            {

            }
            return RedirectToAction("ScheduleList");
        }

        public ActionResult ScheduleUpdate(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = active-nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = nav-link";
            ViewBag.MenuDownload = "class = nav-link";
            try
            {
                if (Session["UserID"] != null)
                {
                    ViewData["Repos"] = GetSchedule.PopulateRepository();
                    ViewBag.Repos = GetSchedule.PopulateRepository();
                    ViewData["Direct"] = GetSchedule.PopulateDirectory();
                    ViewBag.Direct = GetSchedule.PopulateDirectory();
                    ViewData["Device"] = GetSchedule.PopulateDevices();
                    ViewBag.Device = GetSchedule.PopulateDevices();
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetSchedule.ScheduleList().Find(umodel => umodel.ScheduleCode == id));
        }

        [HttpPost]

        public ActionResult ScheduleUpdate(ScheduleMaster schedule, string id)
        {
            try
            {

                if (Session["UserID"] != null)
                {
                    schedule.Flag = "B";
                    GetSchedule.RepSelect = GetSchedule.PopulateRepository();
                    ViewData["Repos"] = GetSchedule.PopulateRepository();
                    var itemRepos = GetSchedule.RepSelect.Find(p => p.Value == schedule.RepCode.ToString());
                    if (itemRepos != null)
                    {
                        itemRepos.Selected = true;
                        schedule.RepCode = itemRepos.Value;
                        schedule.RepName = itemRepos.Text;
                    }
                    schedule.SetupFile = schedule.RepName;
                    GetSchedule.DirSelect = GetSchedule.PopulateDirectory();
                    ViewData["Direct"] = GetSchedule.PopulateDirectory();
                    var itemDirecs = GetSchedule.DirSelect.Find(p => p.Value == schedule.DirCode.ToString());
                    if (itemDirecs != null)
                    {
                        itemDirecs.Selected = true;
                        schedule.DirCode = itemDirecs.Value;
                        schedule.DirName = itemDirecs.Text;
                    }
                    GetSchedule.DeviceSelect = GetSchedule.PopulateDevices();
                    ViewData["Device"] = GetSchedule.PopulateDevices();
                    var itemDevices = GetSchedule.DeviceSelect.Find(p => p.Value == schedule.ObjID.ToString());
                    if (itemDevices != null)
                    {
                        itemDevices.Selected = true;
                        GetSchedule.DeviceIP = itemDevices.Text;
                        GetSchedule.DeviceID = itemDevices.Value;
                    }
                    if (schedule.ScheduleType == "IMMEDIATE")
                    {
                        schedule.ScheduleType = "IMMEDIATE";
                        log.Info("Schedule type is:" + schedule.ScheduleType);
                        schedule.DeployDate = DateTime.Now.ToString("dd-MM-yyyy");
                        log.Info("Schedule deploydate is:" + schedule.DeployDate);
                        DateTime currentTime = DateTime.Now;
                        DateTime time = currentTime.AddMinutes(3);
                        schedule.DeployTime = time.ToString("hh:mm:ss");
                        log.Info("Schedule deploytime is:" + schedule.DeployTime);
                    }
                    else
                    {
                        schedule.ScheduleType = "SCHEDULE";
                        log.Info("Scheduletype is:" + schedule.ScheduleType);
                    }
                    schedule.ObjID = GetSchedule.DeviceID;
                    int note = 0;
                    note = GetSchedule.UpdateSchedule(schedule, id);
                    if (note >= 1)
                    {
                        TempData["UpdateSuccess"] = "Schedule Updated Successfully";
                        log.Info(" Schedule Updated successfully");
                    }
                    else
                    {
                        TempData["UpdateFail"] = "Schedule Updated Failed";
                        log.Info(" Schedule Updated Failed");
                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("ScheduleList");
        }

        public ActionResult ScheduleDelete(string id)
        {
            try
            {

                if (Session["UserID"] != null)
                {
                    GetSchedule.DeleteSchedule(id);
                    log.Info("Schedule delete successfully");
                    return RedirectToAction("ScheduleList");
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult Restart(string id)
        {
            try
            {
                GetSchedule.ScheduleRestart(id);
                log.Info("Restart successfully");
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("DashBoard");
        }
        public ActionResult NotFound()
        { 
            try
            {
                ViewBag.MenuHome = "class = nav-link";
                ViewBag.MenuLocation = "class = nav-link";
                ViewBag.MenuDevice = "class = nav-link";
                ViewBag.MenuRepository = "class = nav-link";
                ViewBag.MenuSchedule = "class = nav-link";
                ViewBag.MenuTransmission = "class = nav-link";
                ViewBag.MenuReport = "class = nav-link";
                ViewBag.MenuUser = "class = nav-link";
                ViewBag.MenuGroup = "class = nav-link";
                ViewBag.MenuDownload = "class = nav-link";
            }
            catch
            {

            }
            return View();
        }
      

        public ActionResult GroupList()
        {



            try
            {
                if (Session["UserID"] != null)
                {

                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = nav-link";
                    ViewBag.MenuUser = "class =nav-link";
                    ViewBag.MenuGroup = "class = active-nav-link";
                    ViewBag.MenuDownload = "class = nav-link";

                    ViewBag.GroupSuccess = TempData["GroupInsertSuccess"];
                    ViewBag.UpdateSuccess = TempData["UpdateSuccess"];


                    ViewBag.DeleteSuccess = TempData["deleteSuccess"];
                    ViewBag.DeleteFail = TempData["deleteFail"];


                 


                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            return View(GetGroup.GroupList());

        }

        public ActionResult GroupDirect()
        {
            return RedirectToAction("GroupInsert");
        }

        GroupMaster GetGroup = new GroupMaster();

       
        [HttpGet]
        public ActionResult  GroupState(string selectedValue)
        {
            var groupCountrySelect = selectedValue;

            if (groupCountrySelect != null)
            {

                GetGroup.GroupStateSelect = GetGroup.PopulateState(groupCountrySelect);
            }

            return Json(GetGroup.GroupStateSelect, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GroupCity(string selectedValue)
        {
            var groupStateSelect = selectedValue;

            if (groupStateSelect != null)
            {

                GetGroup.GroupCitySelect = GetGroup.PopulateCity(groupStateSelect);
            }

            return Json(GetGroup.GroupCitySelect, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GroupLocation(string selectedValue)
        {
            CityValue = selectedValue;
            //var groupCitySelect = selectedValue;

            if (CityValue != null)
            {

                GetGroup.GroupLocationSelect = GetGroup.PopulateLocation(CityValue);
            }


            return Json(GetGroup.GroupLocationSelect, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GroupInsert()
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = active-nav-link";
            ViewBag.MenuDownload = "class = nav-link";

            try
            {
                if (Session["UserID"] != null)
                {

                    GetGroup.GroupSelect = GetGroup.PopulateDevices();
                    GetGroup.GroupCountrySelect = GetAutoID.PopulateCountry();
                    GetGroup.GroupStateSelect = new List<SelectListItem>();
                    GetGroup.GroupCitySelect = new List<SelectListItem>();
                    GetGroup.GroupLocationSelect = new List<SelectListItem>();



                 
                    ViewBag.GroupFail = TempData["GroupInsertFail"];


                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetGroup);
        }


        [HttpPost]

        public ActionResult GroupInsert(GroupMaster group)
        {
            int note = 0;

            string combineditems = string.Empty;
            string branch_code = string.Empty;
            try
            {
                if (Session["UserID"] != null)
                {
                   
                    

                    string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {
                        string country = "select count(*) from profile_master where profile_code  like 'PRF%'";
                        log.Info("the  insert query result is:" + country);
                        using (OracleCommand commands = new OracleCommand(country, connection))
                        {
                            connection.Open();
                            log.Info("Connection opened successfully");
                            int count = Convert.ToInt16(commands.ExecuteScalar());
                            log.Info("the  query result is:" + count);
                            if (count == 0)
                            {
                                GetGroup.GroupCode = "PRF001";
                            }
                            else
                            {
                                string query = "SELECT * FROM(SELECT profile_code FROM profile_master ORDER BY profile_code DESC) WHERE ROWNUM = 1";

                                OracleCommand command = new OracleCommand(query, connection);

                                //connection.Open();
                                log.Info("Connection Opened Successfully");

                                string lastadata = command.ExecuteScalar().ToString();
                                int lastUserId = Int32.Parse(lastadata.Substring(lastadata.Length - 3)) + 1;



                                if (lastUserId < 10)
                                {
                                    GetGroup.GroupCode = "PRF" + "00" + lastUserId;
                                }
                                else if (lastUserId > 9 && lastUserId < 100)
                                {
                                    GetGroup.GroupCode = "PRF" + "0" + lastUserId;
                                }
                                else
                                {
                                    GetGroup.GroupCode = "PRF" + lastUserId;
                                }
                            }
                        }

                        GetGroup.GroupSelect = GetGroup.PopulateDevices();
                        var groupSelect = group.GroupProfileSelection;


                        if (groupSelect != "L")
                        {
                           

                            foreach (var selectedvalue in group.GroupDeviceID)
                            {
                                var groupDevice = GetGroup.GroupSelect.Find(p => p.Value == selectedvalue);
                                

                                if (groupDevice != null)
                                {
                                    groupDevice.Selected = true;

                                    //group.GroupDeviceIP = new List<string> { groupDevice.Text };
                                    //group.GroupDeviceID = new List<string> { groupDevice.Value };
                                    GroupSelectedItems.Add(groupDevice.Value);



                                }
                                branch_code = "D";
                                var profileKey = "DEVICE";
                                combineditems = profileKey  + "*" + string.Join("#", GroupSelectedItems);
                            }
                        }
                        else
                        {
                            GetGroup.GroupLocationSelect = GetGroup.PopulateLocation(CityValue);
                            foreach (var selectedvalue in group.GroupLocation)
                            {
                                var groupLocation = GetGroup.GroupLocationSelect.Find(p => p.Value == selectedvalue);

                                if (groupLocation != null)
                                {
                                    groupLocation.Selected = true;

                                  
                                    GroupSelectedItems.Add(groupLocation.Value);

                                 }
                                branch_code = "L";
                                var profileKey = "LOCATION";
                                combineditems = profileKey + "*" + string.Join("#", GroupSelectedItems);
                            }

                        }


                        group.GroupProfileSelection = branch_code;
                        group.GroupCriteria = combineditems;

                        group.GroupCode = GetGroup.GroupCode;
                        group.GroupOwner = Session["UserName"].ToString();
                        group.GroupCreatedDatTime= DateTime.Now.ToString();

                        GetGroup.GroupType = group.GroupType;
                      

                        note = GetGroup.GroupInsert(group);


                    }


                    if (note > 0)
                    {
                        log.Info("Group Created Successfully");
                        TempData["GroupInsertSuccess"] = "Group Created Successfully";
                        return RedirectToAction("GroupList");
                    }


                    else
                    {
                        //TempData["CountryInsertFail"] = "Country Inserted Failed";
                        log.Info("Group Inserted Failed");
                        TempData["GroupInsertFail"] = "GroupCode or GroupName already exists";
                        //MessageBox.Show("Insert Country already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return RedirectToAction("GroupInsert");

                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("GroupList");

        }

        public ActionResult GroupUpdate(string id)
        {
            ViewBag.MenuHome = "class = nav-link";
            ViewBag.MenuLocation = "class = nav-link";
            ViewBag.MenuDevice = "class = nav-link";
            ViewBag.MenuRepository = "class = nav-link";
            ViewBag.MenuSchedule = "class = nav-link";
            ViewBag.MenuTransmission = "class = nav-link";
            ViewBag.MenuReport = "class = nav-link";
            ViewBag.MenuUser = "class = nav-link";
            ViewBag.MenuGroup = "class = active-nav-link";
            ViewBag.MenuDownload = "class = nav-link";


           
            try
            {
                if (Session["UserID"] != null)
                {
                   
                        var profileSession = GetGroup.GroupList().Find(P => P.GroupCode == id);
                        string groupProfile = profileSession.GroupCriteria;
                        if (groupProfile != null)
                        {
                            string[] profile = groupProfile.Split('*');
                            string profileSelect = profile[0];
                            string[] profileCriteria = profile[1].Split('#');

                            List<string> profileList = new List<string>(profileCriteria);
                            if (profileSelect == "DEVICE")
                            {
                            ViewBag.GroupProfile = "D";
                            ViewBag.DeviceSelectedListvalue = profileList;
                                 ViewBag.DeviceList = GetGroup.PopulateDevices();
                             }
                            else
                            {
                            ViewBag.GroupProfile = "L";
                            ViewBag.LocationSelectedListvalue = profileList;
                                ViewBag.CountryList = GetAutoID.PopulateCountry();

                            string ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

                            foreach (var selectedvalue in profileList)
                            {

                                using (OracleConnection connection = new OracleConnection(ConnectionString))
                                {
                                    string locations = "select a.location_id,a.location_name,b.city_id,b.city_name,c.state_id,c.state_name," +
                                        "d.country_id,d.country_name from location_master a left join city_master b on a.city_id = b.city_id " +
                                        "left join state_master c on b.state_id = c.state_id left join country_master d on c.country_id = d.country_id" +
                                        " where location_id = '" + selectedvalue + "'";
                                    log.Info("the  query result is:" + locations);
                                    using (OracleCommand command = new OracleCommand(locations, connection))
                                    {
                                        OracleDataAdapter dataAdapter = new OracleDataAdapter(command.CommandText, connection);
                                        System.Data.DataTable dataTable = new System.Data.DataTable();
                                        dataAdapter.Fill(dataTable);
                                        int RowCount = dataTable.Rows.Count;

                                        GetGroup.GroupCountry = ViewBag.CountrySelectedListvalue = dataTable.Rows[0]["country_id"].ToString();
                                        GetGroup.GroupState = ViewBag.StateSelectedListvalue = dataTable.Rows[0]["state_id"].ToString();
                                        GetGroup.GroupCity = ViewBag.CitySelectedListvalue = dataTable.Rows[0]["city_id"].ToString();


                                    }
                                }
                            }


                            ViewBag.StateList = GetGroup.PopulateState(GetGroup.GroupCountry);
                            ViewBag.CityList = GetGroup.PopulateCity(GetGroup.GroupState);
                            ViewBag.LocationList = GetGroup.PopulateLocation(GetGroup.GroupCity);

                        }


                        ViewBag.GroupUpdateFail = TempData["GroupUpdateFail"];
                    }
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }
            }
            catch (Exception ex)
            {

            }
            return View(GetGroup.GroupList().Find(P => P.GroupCode == id));
        }

        [HttpPost]

        public ActionResult GroupUpdate(GroupMaster group, string id)
        {
            int note = 0;
            string combineditems = string.Empty;
            
            try
            {
                if (Session["UserID"] != null)
                {



                    var groupSelect = group.GroupProfileSelection;

                    if (groupSelect != "L")
                    {
                       
                        GetGroup.GroupSelect = GetGroup.PopulateDevices();
                        if (group.GroupDeviceID.Count > 0)
                        {
                            foreach (var selectedvalue in group.GroupDeviceID)
                            {
                                var groupDevice = GetGroup.GroupSelect.Find(p => p.Value == selectedvalue);


                                if (groupDevice != null)
                                {
                                    groupDevice.Selected = true;

                                    //group.GroupDeviceIP = new List<string> { groupDevice.Text };
                                    //group.GroupDeviceID = new List<string> { groupDevice.Value };
                                    GroupSelectedItems.Add(groupDevice.Value);



                                }
                                var profileKey = "DEVICE";
                                combineditems = profileKey + "*" + string.Join("#", GroupSelectedItems);
                            }
                        }
                    }
                    else
                    {
                        GetGroup.GroupLocationSelect = GetGroup.PopulateLocation(CityValue);
                        foreach (var selectedvalue in group.GroupLocation)
                        {
                            var groupLocation = GetGroup.GroupLocationSelect.Find(p => p.Value == selectedvalue);

                            if (groupLocation != null)
                            {
                                groupLocation.Selected = true;


                                GroupSelectedItems.Add(groupLocation.Value);

                            }
                            var profileKey = "LOCATION";
                            combineditems = profileKey + "*" + string.Join("#", GroupSelectedItems);
                        }

                    }



                    group.GroupCriteria = combineditems;

                    group.GroupCode = GetGroup.GroupCode;
                    group.GroupOwner = Session["UserName"].ToString();
                    group.GroupCreatedDatTime = DateTime.Now.ToString();

                    GetGroup.GroupType = group.GroupType;


                    note = GetGroup.GroupUpdate(group, id);

                  
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }

            if (note >= 1)
            {
                TempData["UpdateSuccess"] = "Group Updated Successfully";
                log.Info("Group Updated Successfully");
                return RedirectToAction("GroupList");

            }
            else
            {
                TempData["GroupUpdateFail"] = "Group  update failed or name is already exists";
                log.Info("Group Updated failed");
                //MessageBox.Show("Device update ip and port already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return RedirectToAction("GroupUpdate");
            }


        }

        public ActionResult GroupDelete(string id)
        {
            int note = 0;
            try
            {

                if (Session["UserID"] != null)
                {
                    note = GetGroup.DeleteGroup(id);
                    log.Info("Group delete successfully");
                   
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }

            }
            catch (Exception ex)
            {

            }
            if (note > 0)
            {

                log.Info("Group deleted sucessfully");
                TempData["deleteSuccess"] = "Group deleted Successfully";
                return RedirectToAction("GroupList");
            }
            else
            {

                log.Info("Group deleted Failed");
                TempData["deletefail"] = "Group Delete Failed,This is Used in Schedule";
                return RedirectToAction("GroupList");
            }
            
        }



        [HttpGet]
        public ActionResult Report()
         {
            try
            {
                if (Session["UserID"] != null)
                {


                    ViewBag.MenuHome = "class = nav-link";
                    ViewBag.MenuLocation = "class = nav-link";
                    ViewBag.MenuDevice = "class = nav-link";
                    ViewBag.MenuRepository = "class = nav-link";
                    ViewBag.MenuSchedule = "class = nav-link";
                    ViewBag.MenuTransmission = "class = nav-link";
                    ViewBag.MenuReport = "class = active-nav-link";
                    ViewBag.MenuUser = "class = nav-link";
                    ViewBag.MenuGroup = "class = nav-link";
                    ViewBag.MenuDownload = "class = nav-link";



                    ViewBag.DisplayFlag = "N";
                    reportmaster.BankNameSelect = reportmaster.PopulateBankName();
                    ReportViewer reportViewer = new ReportViewer();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(900);
                    reportViewer.Height = Unit.Percentage(900);
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Report1.rdlc";

                    DataSet1 ds = new DataSet1();



                    ReportDataSource rds = new ReportDataSource("DataSet1", ds.Tables[0]);
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(rds);
                    ViewBag.ReportViewer = reportViewer;
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch
            {

            }
            return View(reportmaster);
        }

        [HttpPost]
        public ActionResult Report(ReportMaster reportmaster)
        {
            try
            {
                ViewBag.MenuHome = "class = nav-link";
                ViewBag.MenuLocation = "class = nav-link";
                ViewBag.MenuDevice = "class = nav-link";
                ViewBag.MenuRepository = "class = nav-link";
                ViewBag.MenuSchedule = "class = nav-link";
                ViewBag.MenuTransmission = "class = nav-link";
                ViewBag.MenuReport = "class = active-nav-link";
                ViewBag.MenuUser = "class = nav-link";
                ViewBag.MenuGroup = "class = nav-link";
                ViewBag.MenuDownload = "class = nav-link";




                string selceteditem = reportmaster.Report;
                
                    DataSet1 ds = new DataSet1();
                    ViewBag.DisplayFlag = "Y";
                    
                    string fd = reportmaster.FromDate;
                    string td = reportmaster.ToDate;
                    ReportViewer reportViewer = new ReportViewer();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.Width = Unit.Percentage(2000);
                    reportViewer.Height = Unit.Percentage(900);
                if (selceteditem != "D")
                {
                    reportmaster.BankNameSelect = reportmaster.PopulateBankName();
                    var orgItem = reportmaster.BankNameSelect.Find(p => p.Value == reportmaster.BankName.ToString());
                    if (orgItem != null)
                    {
                        orgItem.Selected = true;
                        reportmaster.BankName = orgItem.Value;
                        reportmaster.BankName = orgItem.Text;
                    }
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Report1.rdlc";
                    string title = string.Empty;
                    if (selceteditem == "T")
                    {

                        title = "EJFile Status Report";

                    }
                    else if (selceteditem == "F")
                    {

                        title = "EJFile Failed Report";

                    }
                    else 
                    {

                        title = "EJFile Reschedule Report";

                    }
                    
                    reportmaster.GetFileterRecord(ds, fd, td, title,selceteditem, reportmaster.BankName);


                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        ds.DataTable1.Rows.Add("", "", "", "", "", "", "", "", title, fd, td, "No Data Found!");

                    }
                    ReportDataSource rds = new ReportDataSource("DataSet1", ds.Tables[0]);
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(rds);
                    ViewBag.ReportViewer = reportViewer;
                }
                else
                {
                   
                    string title1 = "Device Transmission Record";
                    reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Report2.rdlc";
                    reportmaster.GetTransmissionRecord(ds,title1);
                    if (ds.Tables[1].Rows.Count == 0)
                    {
                        ds.DataTable2.Rows.Add("", "", "", "", "", "", "", title1,"No Data Found!");

                    }
                    ReportDataSource rds1 = new ReportDataSource("DataSet1", ds.Tables[1]);
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(rds1);
                    ViewBag.ReportViewer = reportViewer;
                }
                    
                

            }
            catch
            {

            }
            return View(reportmaster);
        }
    }
}