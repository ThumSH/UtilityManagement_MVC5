using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UtilityManagement_MVC5.Models;

namespace UtilityManagement_MVC5.Controllers
{
    public class ReadingsController : Controller
    {
        private UMS_DBEntities db = new UMS_DBEntities();

        // ==========================================
        // 1. LIST READINGS (Index)
        // ==========================================
        public ActionResult Index()
        {
            var readings = db.Readings.Include(r => r.Meter);
            return View(readings.ToList());
        }

        // ==========================================
        // 3. CREATE NEW READING (With Persistence Fix)
        // ==========================================
        [OutputCache(NoStore = true, Duration = 0)] // Prevents browser caching of the form
        public ActionResult Create()
        {
            ViewBag.MeterID = new SelectList(db.Meters, "MeterID", "MeterSerialNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Create([Bind(Include = "ReadingID,MeterID,ReadingDate,PreviousReading,CurrentReading")] Reading reading)
        {
            if (ModelState.IsValid)
            {
                // Logic Check: Prevent negative usage
                if (reading.CurrentReading < reading.PreviousReading)
                {
                    ModelState.AddModelError("CurrentReading", "Error: Current Reading cannot be lower than Previous Reading.");
                    ViewBag.MeterID = new SelectList(db.Meters, "MeterID", "MeterSerialNumber", reading.MeterID);
                    return View(reading);
                }

                try
                {
                    db.Readings.Add(reading);
                    db.SaveChanges();

                    // FIX: Redirect to the GET action to clear the model state and show a fresh form
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    // Catch SQL errors (e.g., constraints/concurrency issues)
                    string errorMessage = ex.InnerException?.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", "Database Error: " + errorMessage);
                }
            }

            ViewBag.MeterID = new SelectList(db.Meters, "MeterID", "MeterSerialNumber", reading.MeterID);
            return View(reading);
        }

        // ==========================================
        // 4. EDIT READING (Persistence Fix Applied)
        // ==========================================
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Reading reading = db.Readings.Find(id);
            if (reading == null) return HttpNotFound();
            ViewBag.MeterID = new SelectList(db.Meters, "MeterID", "MeterSerialNumber", reading.MeterID);
            return View(reading);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReadingID,MeterID,ReadingDate,PreviousReading,CurrentReading")] Reading reading)
        {
            if (ModelState.IsValid)
            {
                if (reading.CurrentReading < reading.PreviousReading)
                {
                    ModelState.AddModelError("CurrentReading", "Error: Current Reading cannot be lower than Previous Reading.");
                    ViewBag.MeterID = new SelectList(db.Meters, "MeterID", "MeterSerialNumber", reading.MeterID);
                    return View(reading);
                }

                db.Entry(reading).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index"); // This is a correct redirect
            }
            ViewBag.MeterID = new SelectList(db.Meters, "MeterID", "MeterSerialNumber", reading.MeterID);
            return View(reading);
        }

        // ... (Remaining methods: Details, Delete, DeleteConfirmed, Dispose are unchanged) ...
    }
}