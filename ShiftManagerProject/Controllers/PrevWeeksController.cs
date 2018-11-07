using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class PrevWeeksController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();

        public ActionResult Index()
        {
            int rec = 0;
            var count = db.PrevWeeks.ToList();
            if (count.Count()>56)
            {
                rec++;
                foreach (var shift in db.PrevWeeks.Take(28))
                {
                    db.PrevWeeks.Remove(shift);
                }
            }
            ViewBag.records = rec;

            return View(db.PrevWeeks.ToList());
        }

        public ActionResult LastWeek()
        {
            var count = db.PrevWeeks.ToList();
            if (count.Count() > 56)
            {
                foreach (var shift in db.PrevWeeks.Take(28))
                {
                    db.PrevWeeks.Remove(shift);
                }
            }
            db.SaveChanges();

            var countF = db.FinalShift.ToList();
            if (countF.Count() >=84)
            {
                foreach (var shift in db.FinalShift.Take(countF.Count() - 56))
                {
                    db.FinalShift.Remove(shift);
                }
            }
            db.SaveChanges();

            Response.AddHeader("Refresh", "1");
            var nextshifts = db.PrevWeeks.Take(28);
            return View(nextshifts.ToList());
        }

        public ActionResult DeleteConfirmed()
        {
            foreach (var shift in db.PrevWeeks.Take(28))
            {
                db.PrevWeeks.Remove(shift);
            }
            db.SaveChanges();

            if (db.FinalShift.Count() >= 84)
            {
                foreach (var shift in db.FinalShift.Take(db.FinalShift.Count() - 56))
                {
                    db.FinalShift.Remove(shift);
                }
            }
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
