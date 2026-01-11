using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Required for EF
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UtilityManagement_MVC5.Models;

namespace UtilityManagement_MVC5.Controllers
{
    public class CustomersController : Controller
    {
        // Connect to the Database using Entity Framework
        private UMS_DBEntities db = new UMS_DBEntities();

        // ==========================================
        // 1. LIST CUSTOMERS (Index)
        // ==========================================
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // ==========================================
        // 2. SHOW DETAILS
        // ==========================================
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // ==========================================
        // 3. CREATE NEW CUSTOMER
        // ==========================================
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,FullName,Email,PhoneNumber,AddressLine,RegisteredDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Set default date if missing
                if (customer.RegisteredDate == null || customer.RegisteredDate == DateTime.MinValue)
                {
                    customer.RegisteredDate = DateTime.Now;
                }

                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // ==========================================
        // 4. EDIT CUSTOMER
        // ==========================================
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,FullName,Email,PhoneNumber,AddressLine,RegisteredDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // ==========================================
        // 5. DELETE CUSTOMER (The Fix is Here!)
        // ==========================================

        // GET: Show Confirmation Page
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Actually Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            try
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                // THIS CATCHES THE FOREIGN KEY ERROR
                ViewBag.Error = "⚠️ Cannot delete this customer! They have active meters, bills, or complaints linked to their account.";

                // Return the same view so they see the error
                return View("Delete", customer);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An unexpected error occurred: " + ex.Message;
                return View("Delete", customer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}