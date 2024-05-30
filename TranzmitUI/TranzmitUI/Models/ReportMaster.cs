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
    public class ReportMaster
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReportMaster));

        [Required(ErrorMessage = "Please fill the detail")]
        public string FromDate { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string ToDate { get; set; }

        [Required(ErrorMessage = "Please fill the detail")]
        public string Report { get; set; }

        public string BankName { get; set; }
        public List<SelectListItem> BankNameSelect { get; set; }

        //public string TranzmitEJDate { get; set; }

        string connectionstring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

        public bool GetFileterRecord(DataSet1 ds, string fdate, string tdate, string title, string selcitem,string bname)
        {
            List<TranzmitUI.Models.DeviceTransmission> filestatus = new List<Models.DeviceTransmission>();
            var Orag = System.Web.HttpContext.Current.Session["UserOraganization"];
            try
            {

                FromDate = fdate;
                ToDate = tdate;
                Report = selcitem;
                BankName = bname;
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = string.Empty;

                if (Report == "T")
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "select bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(filereceive_date,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from ejfile_status where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and bankname='"+Orag+"'order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                    else if(BankName == "All")
                    {
                        query = "select bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(filereceive_date,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from ejfile_status where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                    else 
                    {
                        query = "select bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(filereceive_date,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from ejfile_status where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and bankname='" + BankName + "' order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                }
                else if (Report == "F")
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "select bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(filereceive_date,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from ejfile_status where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and (trans_status is null or trans_status != 'C') and bankname='"+Orag+"'order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                    else if (BankName == "All")
                    {
                        query = "select bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(filereceive_date,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from ejfile_status where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and (trans_status is null or trans_status != 'C')  order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                    else 
                    {
                        query = "select bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(filereceive_date,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from ejfile_status where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and (trans_status is null or trans_status != 'C') and bankname='" + BankName + "' order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }


                }
                else 
                {
                    if (System.Web.HttpContext.Current.Session["UserRole"].ToString().Equals("M"))
                    {
                        query = "select (select bankname from ej_link el where el.atm_id=rl.atm_id) as bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(reschedule_dt,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,remarks as ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from reschedule_log rl where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and bankname='"+Orag+"'order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                    else if (BankName == "All")
                    {
                        query = "select (select bankname from ej_link el where el.atm_id=rl.atm_id) as bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(reschedule_dt,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,remarks as ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from reschedule_log rl where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy')  order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }
                    else
                    {
                        query = "select (select bankname from ej_link el where el.atm_id=rl.atm_id) as bankname,atm_id,atm_ip,atm_port,TO_CHAR(ej_date,'dd/MM/yyyy') as ej_date,ej_filename,TO_CHAR(reschedule_dt,'dd/MM/yyyy HH24:MI:SS') as filereceive_date,remarks as ej_status,'" + title + "' as title,TO_CHAR(TO_DATE('" + FromDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as fromdate, TO_CHAR(TO_DATE('" + ToDate + "','dd/MM/yyyy'),'dd/MM/yyyy') as todate,'' as emptydata from reschedule_log rl where ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') and bankname='" + BankName + "' order by ej_date desc";
                        log.Info("the ejfile status  query result  is:" + query);
                    }

                }
               

                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                connection.Open();
                log.Info("Connection Opened Successfully for Report");
                dataAdapter.Fill(ds, "DataTable1");
                connection.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public List<SelectListItem> PopulateBankName()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionstring))
                {
                    string query = "SELECT TO_CHAR('All') AS bankname FROM DUAL UNION SELECT DISTINCT TO_CHAR(bankname) AS bankname FROM ej_link";
                    log.Info("the populate query result is:" + query);
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        log.Info("Connection Opened Successfully for report master in populate bankname ");
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
        public bool GetTransmissionRecord(DataSet1 ds, string title)
        {
            List<TranzmitUI.Models.DeviceTransmission> filestatus = new List<Models.DeviceTransmission>();
            try
            {

                string st = "Transfered";
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = "select dt.*,'" + title + "' as title,'' as emptydata,de.device_ip as device_ip,de.device_port as device_port,sm.schedule_type as type,sm.schedule_date as schedule_date,sm.schedule_time as schedule_time,dt.files_name as source,'" + st + "' as status from device_transmission dt LEFT JOIN device_master de ON dt.device_id = de.device_id LEFT JOIN schedule_master sm ON dt.schedule_code = sm.schedule_code";
                log.Info("the  query result  is:" + query);

                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                connection.Open();
                log.Info("Connection Opened Successfully for Report");
                dataAdapter.Fill(ds, "DataTable2");
                connection.Close();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool GetEJFileRecord(string fdate, string tdate)
        {
            List<TranzmitUI.Models.DeviceTransmission> filestatus = new List<Models.DeviceTransmission>();
            try
            {

                FromDate = fdate;
                ToDate = tdate;
               
                OracleConnection connection = new OracleConnection(connectionstring);
                string query = string.Empty;

               
                query = "select (select reschedule_status From reschedule_log re where es.atm_id = re.atm_id and es.ej_date = re.ej_date order by reschedule_dt desc fetch first 1 rows only) reschedule_status ,es.* from ejfile_status es  where es.ej_date BETWEEN  TO_DATE('" + FromDate + "','dd/MM/yyyy') AND  TO_DATE('" + ToDate + "','dd/MM/yyyy') order by filereceive_date desc";
                log.Info("the ejfile status  query result  is:" + query);
                
               
                OracleCommand command = new OracleCommand(query, connection);
                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                log.Info("Connection Opened Successfully for Report");
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


                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
    }
}