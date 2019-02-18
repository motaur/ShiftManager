using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class PrevWeeksController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        private HistoryDeletionHandler HsDelete = new HistoryDeletionHandler();

        public ActionResult Index()
        {
            var Pcount = db.PrevWeeks.ToList();
            if (Pcount.Count()> 476)
            {
                HsDelete.PrevWeeksDeletion();
            }

            var nextshifts = db.PrevWeeks.OrderBy(r => DbFunctions.TruncateTime(r.Dates)).ThenBy(c => c.OfDayType).ToList();
            ViewBag.Employees = db.Employees.ToList();
            return View(nextshifts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            string Ename = form["Employees"].ToString();
            string Date = form["inputState"].ToString();
            var ReportShifts = db.PrevWeeks.ToList();

            if (Date != "Dates")
            {
                var today = DateTime.Today.AddDays(1).Date;
                var month = DateTime.Today;

                switch (Date)
                {
                    case "Month":
                        month = today.AddMonths(-1);
                        ReportShifts = ReportShifts.Where(y => y.Dates <= today && y.Dates >= month).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
                        break;
                    case "3 Months":
                        month = today.AddMonths(-3);
                        ReportShifts = ReportShifts.Where(y => y.Dates <= today && y.Dates >= month).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
                        break;
                    case "6 Months":
                        month = today.AddMonths(-6);
                        ReportShifts = ReportShifts.Where(y => y.Dates <= today && y.Dates >= month).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
                        break;
                    case "Year":
                        month = today.AddYears(-1);
                        ReportShifts = ReportShifts.Where(y => y.Dates <= today && y.Dates >= month).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
                        break;
                    case "All":
                        ReportShifts = ReportShifts.OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
                        break;
                }
            }

            if(Ename != "")
            {
                ReportShifts = ReportShifts.Where(x => x.Name == Ename).OrderBy(r => r.Dates.Date).ThenBy(c => c.OfDayType).ToList();
            }

            ViewBag.Employees = db.Employees.ToList();
            return View(ReportShifts);
        }

            public ActionResult LastWeek()
        {
            HsDelete.PrevWeeksDeletion();
            HsDelete.FshiftDeletion();

            var context = ((IObjectContextAdapter)db).ObjectContext;
            var refreshableObjects = db.ChangeTracker.Entries().Select(c => c.Entity).ToList();
            context.Refresh(RefreshMode.StoreWins, refreshableObjects);

            var nextshifts = db.PrevWeeks.ToList().Take(28).OrderBy(x => x.OfDayType);
            return View(nextshifts);
        }

        public ActionResult DeleteConfirmed()
        {
            HsDelete.PrevWeeksDeletion();
            HsDelete.FshiftDeletion();
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
