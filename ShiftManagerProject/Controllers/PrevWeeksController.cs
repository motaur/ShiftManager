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
            int rec = 0;
            var count = db.PrevWeeks.ToList();
            if (count.Count()>=56)
            {
                rec++;
                HsDelete.PrevWeeksDeletion();
            }
            ViewBag.records = rec;

            var nextshifts = db.PrevWeeks.OrderByDescending(x => x.OfDayType).OrderBy(x => x.OfDayType).ToList();
            return View(nextshifts);
        }

        public ActionResult LastWeek()
        {
            HsDelete.PrevWeeksDeletion();
            HsDelete.FshiftDeletion();

            var context = ((IObjectContextAdapter)db).ObjectContext;
            var refreshableObjects = db.ChangeTracker.Entries().Select(c => c.Entity).ToList();
            context.Refresh(RefreshMode.StoreWins, refreshableObjects);
            var nextshifts = db.PrevWeeks.Take(28).OrderBy(x => x.OfDayType).ToList();
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
