
using System;
using System.Collections.Generic;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace TranzmitUI.Models
{
    public class RAutoID
    {


        public string RID { get; set; }
        public string DID { get; set; }
    }
    public class RepositoryMaster
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RepositoryMaster));

        [Required(ErrorMessage = "Please fill the detail")]
        public HttpPostedFileBase files { get; set; }

        //public string files { get; set; }
        public byte[] ImageData { get; set; }
        public string RepCode { get; set; }


        public string RepType { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]

        public string RepPath { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string RepName { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string RepVersion { get; set; }
        public List<SelectListItem> FileSelect { get; set; }
        public List<SelectListItem> PathSelect { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string File { get; set; }
        public string DirCode { get; set; }
        public string DateTime { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string DestinationPath { get; set; }

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public List<SelectListItem> PopulatePath()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                items.Add(new SelectListItem() { Text = "C:\\Repository\\SendFiles\\", Value = "1" });

            }
            catch (Exception ex)
            {

            }
            return items;
        }

        public List<SelectListItem> PopulateFiles()
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
                        query = " SELECT repname from repository_master where obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = " SELECT repname from repository_master";
                    }
                    log.Info("the populatefile query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for Repository master in populatefiles");
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["repname"].ToString()
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

        public List<RepositoryMaster> RepositoryList()
        {
            List<TranzmitUI.Models.RepositoryMaster> repos = new List<Models.RepositoryMaster>();
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];

            string query = string.Empty;
            try
            {

                OracleConnection connection = new OracleConnection(connectionstring);
                if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                {
                    query = "select repcode,reptype,reppath,repname,rep_version from repository_master where obj_id = '" + UserOraganization + "'";
                }
                else
                {
                    query = "select repcode,reptype,reppath,repname,rep_version from repository_master";
                }
                log.Info("the repositorylist query result is:" + query);
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Repository master in Repositorylist");
                dataAdapter.Fill(dataTable);
                connection.Close();
                foreach (DataRow dr in dataTable.Rows)
                {
                    repos.Add(new Models.RepositoryMaster
                    {
                        RepCode = Convert.ToString(dr["repcode"]),
                        RepType = Convert.ToString(dr["reptype"]),
                        RepPath = Convert.ToString(dr["reppath"]),
                        RepName = Convert.ToString(dr["repname"]),
                        RepVersion = Convert.ToString(dr["rep_version"]),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return repos;
        }

        public bool dependancyeCheck(string id)
        {
            string dpcheck = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from schedule_master where repcode = '" + id + "' ";
            log.Info("the  query result is:" + query);
            OracleCommand command = new OracleCommand(query, connection);
            connection.Open();
            log.Info("Connection Opened Successfully for Respository master in depandancy check");

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

        public bool ExitsvalueCheck(string repname, string id)
        {
            string evalue = string.Empty;

            OracleConnection connection = new OracleConnection(connectionstring);
            string query = "select count(*)  from repository_master where   repname='" + repname + "'";
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

        //public int UploadRepositoryFile(HttpPostedFileBase[] files,)
        //{
        //    int returnValue = 0;
        //    try
        //    {

        //        if (files != null && files.Length > 0)
        //        {
        //            foreach (HttpPostedFileBase file in files)
        //            {
        //                if (file != null && file.ContentLength > 0)
        //                {

        //                    using (OracleConnection connection = new OracleConnection(connectionstring))
        //                    {
        //                        connection.Open();
        //                        log.Info("Connection Opened Successfully");
        //                        string query = "insert into repository_master(repcode,reppath,repname,rep_version,obj_id)values('" + reposInsert.RepCode + "','" + reposInsert.RepPath + "'," +
        //                            "'" + reposInsert.RepName + "','" + reposInsert.RepVersion + "','" + UserOraganization + "')";
        //                        log.Info("the repositoryinsert query result is:" + query);
        //                        using (OracleCommand command = new OracleCommand(query, connection))
        //                        {
        //                            count = command.ExecuteNonQuery();
        //                            log.Info("Command executed successfully");
        //                        }
        //                        connection.Close();
        //                    }



        //                    // Specify the folder path where you want to save the file
        //                    string folderPath = ("C:\\Repository\\SendFiles1\\");

        //                    // Ensure the folder exists, create it if it doesn't
        //                    if (!Directory.Exists(folderPath))
        //                    {
        //                        Directory.CreateDirectory(folderPath);
        //                    }

        //                    // Save the file to the server
        //                    string fileName = Path.GetFileName(file.FileName);
        //                    string filePath = Path.Combine(folderPath, fileName);
        //                    file.SaveAs(filePath);

        //                    // Store the repository path in your database or storage
        //                    string repositoryPath = "/UploadedFiles/" + fileName;
        //                    // Here you would typically save the repositoryPath to your database

        //                    log.Info("File uploaded successfully");
        //                }


        //            }
        //        }

        //        else
        //        {

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return returnValue;
        //}

        public int RepositoryInsert(RepositoryMaster reposInsert)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];


            try
            {
                if (ExitsvalueCheck(reposInsert.RepName, reposInsert.RepCode))
                {
                    using (OracleConnection connection = new OracleConnection(connectionstring))
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Repository master in Repositoryinsert");
                        string query = "insert into repository_master(repcode,reppath,repname,rep_version,obj_id,repimagecontent)values('" + reposInsert.RepCode + "','" + reposInsert.RepPath + "'," +
                            "'" + reposInsert.RepName + "','" + reposInsert.RepVersion + "','" + UserOraganization + "',:ImageData)";

                     

                            log.Info("the repositoryinsert query result is:" + query);
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            command.Parameters.Add(":ImageData", OracleDbType.Blob).Value = reposInsert.ImageData;
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

        public int UpdateRepository(RepositoryMaster update, string id)
        {
            int count = 0;
            var UserOraganization = System.Web.HttpContext.Current.Session["UserOraganization"];
            string query = string.Empty;

            try
            {
                OracleConnection connection = new OracleConnection(connectionstring);
                {
                    connection.Open();
                    log.Info("Connection Opened Successfully for Repository master in UpdateRepository");
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "update repository_master set repname='" + update.RepName + "',rep_version='" + update.RepVersion + "',reppath='" + update.RepPath + "'" +
                       " where repcode='" + id + "' and obj_id = '" + UserOraganization + "'";
                    }
                    else
                    {
                        query = "update repository_master set repname='" + update.RepName + "',rep_version='" + update.RepVersion + "',reppath='" + update.RepPath + "'" +
                     " where repcode='" + id + "'";
                    }
                    log.Info("the updaterespository query result is:" + query);
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

        public int DeleteRepository(string id)

        {
            int count = 0;
            try
            {
                if (dependancyeCheck(id))
                {
                    OracleConnection connection = new OracleConnection(connectionstring);
                    {
                        connection.Open();
                        log.Info("Connection Opened Successfully for Repository master in DeleteRepository");
                        string query = "Delete  from repository_master where repcode='" + id + "'";
                        log.Info("the deleterepository query result is:" + query);
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

    public class FileProcessor
    {
        private FileStream _fileStream;
        public void OpenFile(string filePath) 
        { 
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read); 
        }

    }
}
       