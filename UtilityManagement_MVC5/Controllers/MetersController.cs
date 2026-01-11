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
    public class MetersController : Controller
    {
        private UMS_DBEntities db = new UMS_DBEntities();

        // GET: Meters
        public ActionResult Index()
        {
            var meters = db.Meters.Include(m => m.Customer).Include(m => m.UtilityType);
            return View(meters.ToList());
        }

        // GET: Meters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meter meter = db.Meters.Find(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            return View(meter);
        }

        // GET: Meters/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName");
            ViewBag.UtilityID = new SelectList(db.UtilityTypes, "UtilityID", "TypeName");
            return View();
        }

        // POST: Meters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MeterID,CustomerID,UtilityID,MeterSerialNumber,InstallationDate")] Meter meter)
        {
            if (ModelState.IsValid)
            {
                db.Meters.Add(meter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", meter.CustomerID);
            ViewBag.UtilityID = new SelectList(db.UtilityTypes, "UtilityID", "TypeName", meter.UtilityID);
            return View(meter);
        }

        // GET: Meters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meter meter = db.Meters.Find(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", meter.CustomerID);
            ViewBag.UtilityID = new SelectList(db.UtilityTypes, "UtilityID", "TypeName", meter.UtilityID);
            return View(meter);
        }

        // POST: Meters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MeterID,CustomerID,UtilityID,MeterSerialNumber,InstallationDate")] Meter meter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", meter.CustomerID);
            ViewBag.UtilityID = new SelectList(db.UtilityTypes, "UtilityID", "TypeName", meter.UtilityID);
            return View(meter);
        }

        // GET: Meters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meter meter = db.Meters.Find(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            return View(meter);
        }

        // POST: Meters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Meter meter = db.Meters.Find(id);
            db.Meters.Remove(meter);
            db.SaveChanges();
            return RedirectToAction("Index");
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
