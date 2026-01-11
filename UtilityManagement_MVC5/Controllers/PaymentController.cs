using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using UtilityManagement_MVC5.Models;

namespace UtilityManagement_MVC5.Controllers
{
    public class PaymentController : Controller
    {
        // Connection String for your SQL Express Database
        private string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=UMS_DB;Integrated Security=True;TrustServerCertificate=True";

        // ==========================================
        // 1. SEARCH BILLS (The "Find & Issue" Feature)
        // ==========================================
        public ActionResult Search(string query)
        {
            var results = new List<BillSearchViewModel>();

            // Only run the search if the user typed something
            if (!string.IsNullOrEmpty(query))
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Query to find UNPAID bills matching Name or Phone
                    string sql = @"SELECT b.BillID, c.FullName, u.TypeName, b.TotalAmount, b.DueDate, b.IsPaid
                                   FROM Bills b
                                   JOIN Readings r ON b.ReadingID = r.ReadingID
                                   JOIN Meters m ON r.MeterID = m.MeterID
                                   JOIN Customers c ON m.CustomerID = c.CustomerID
                                   JOIN UtilityTypes u ON m.UtilityID = u.UtilityID
                                   WHERE (c.FullName LIKE @q OR c.PhoneNumber LIKE @q) 
                                   AND b.IsPaid = 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@q", "%" + query + "%");
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                results.Add(new BillSearchViewModel
                                {
                                    BillID = (int)r["BillID"],
                                    CustomerName = r["FullName"].ToString(),
                                    Utility = r["TypeName"].ToString(),
                                    Amount = (decimal)r["TotalAmount"],
                                    DueDate = Convert.ToDateTime(r["DueDate"]),
                                    IsPaid = (bool)r["IsPaid"]
                                });
                            }
                        }
                    }
                }
            }
            // Keep the search text in the box
            ViewBag.CurrentQuery = query;
            return View(results);
        }

        // ==========================================
        // 2. PAYMENT PORTAL (The "Cashier" Feature)
        // ==========================================

        // GET: Show Payment Page (Auto-fill Bill ID if provided)
        public ActionResult Create(int? billId)
        {
            var model = new PaymentViewModel();
            if (billId.HasValue)
            {
                model.BillID = billId.Value;
            }
            return View(model);
        }

        // POST: Process the Payment
        [HttpPost]
        public ActionResult Create(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        // Insert Payment (Trigger will auto-update Bill status)
                        string query = "INSERT INTO Payments (BillID, AmountPaid, PaymentMethod, PaymentDate) VALUES (@bid, @amt, @mth, GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@bid", model.BillID);
                            cmd.Parameters.AddWithValue("@amt", model.AmountPaid);
                            cmd.Parameters.AddWithValue("@mth", model.PaymentMethod);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Message = "Payment Successful! The system has updated the balance.";
                    ModelState.Clear(); // Clear form for next customer
                    return RedirectToAction("Create");
                }
                catch (SqlException ex)
                {
                    // --- SMART ERROR HANDLING ---
                    // If the error contains "FOREIGN KEY", it means Bill ID doesn't exist
                    if (ex.Message.Contains("FOREIGN KEY"))
                    {
                        ViewBag.Error = "❌ Invalid Bill ID: The Bill ID '" + model.BillID + "' does not exist in the system.";
                    }
                    else
                    {
                        ViewBag.Error = "Database Error: " + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An unexpected error occurred: " + ex.Message;
                }
            }
            return View(model);
        }
    }
}