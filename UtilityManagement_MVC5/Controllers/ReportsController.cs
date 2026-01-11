using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Configuration;
using UtilityManagement_MVC5.Models;

namespace UtilityManagement_MVC5.Controllers
{
    public class ReportsController : Controller
    {
        private string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=UMS_DB;Integrated Security=True;TrustServerCertificate=True";

        // 1. Revenue Report
        public ActionResult Index()
        {
            var list = new List<RevenueReportViewModel>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM v_MonthlyRevenue ORDER BY BillYear DESC, BillMonth DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new RevenueReportViewModel
                            {
                                UtilityType = r["UtilityType"].ToString(),
                                Year = Convert.ToInt32(r["BillYear"]),
                                Month = Convert.ToInt32(r["BillMonth"]),
                                TotalRevenue = Convert.ToDecimal(r["TotalRevenue"]),
                                TotalBills = Convert.ToInt32(r["TotalBillsGenerated"])
                            });
                        }
                    }
                }
            }
            return View(list);
        }

        // 2. Top Consumers
        public ActionResult TopConsumers()
        {
            var list = new List<TopConsumerViewModel>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM v_TopConsumers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new TopConsumerViewModel
                            {
                                CustomerName = r["FullName"].ToString(),
                                Utility = r["Utility"].ToString(),
                                TotalUsage = Convert.ToDecimal(r["TotalUsage"]),
                                TotalSpent = Convert.ToDecimal(r["TotalSpent"])
                            });
                        }
                    }
                }
            }
            return View(list);
        }

        // 3. Defaulters
        public ActionResult Defaulters()
        {
            var list = new List<DefaulterViewModel>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM v_UnpaidBillsReport ORDER BY DueDate ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new DefaulterViewModel
                            {
                                FullName = r["FullName"].ToString(),
                                PhoneNumber = r["PhoneNumber"].ToString(),
                                UtilityType = r["Utility"].ToString(),
                                TotalAmount = Convert.ToDecimal(r["TotalAmount"]),
                                DueDate = Convert.ToDateTime(r["DueDate"]),
                                DaysOverdue = r["DaysOverdue"] != DBNull.Value ? Convert.ToInt32(r["DaysOverdue"]) : 0
                            });
                        }
                    }
                }
            }
            return View(list);
        }
    }
}