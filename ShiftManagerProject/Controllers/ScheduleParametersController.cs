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
    public class ScheduleParametersController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        private HistoryDeletionHandler HsDelete = new HistoryDeletionHandler();

        public ActionResult Index()
        {
            if (db.ScheduleParameters.Where(x => x.Day == null).Any())
            {
                ViewBag.WeekP = 1;
            }
            return View(db.ScheduleParameters.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            return View(scheduleParameters);
        }


        public ActionResult DayCreate()
        {
            return View();
        }

        public ActionResult WeekCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ScheduleParameters scheduleParameters)
        {
            int TotalShifts = 0;
            int idnum = 0;
            if (db.ShiftsPerWeek.Any())
            {
                idnum = db.ShiftsPerWeek.FirstOrDefault().ID;
            }

            if (scheduleParameters.Day == null)
            {
                if (idnum == 0)
                {
                    ShiftsPerWeek SPW = new ShiftsPerWeek
                    {
                        NumOfShifts = (scheduleParameters.Morning + scheduleParameters.Afternoon + scheduleParameters.Night) * 7
                    };
                    db.ShiftsPerWeek.Add(SPW);
                    db.SaveChanges();
                }
                else
                {
                    ShiftsPerWeek EditOldShift = db.ShiftsPerWeek.Find(idnum);
                    EditOldShift.NumOfShifts = (scheduleParameters.Morning + scheduleParameters.Afternoon + scheduleParameters.Night) * 7;
                    db.Entry(EditOldShift).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                var Mshift = db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault();
                TotalShifts += scheduleParameters.DMorning - Mshift;
                var Ashift = db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault();
                TotalShifts += scheduleParameters.DAfternoon - Ashift;
                var Nshift = db.ScheduleParameters.Select(x => x.Night).FirstOrDefault();
                TotalShifts += scheduleParameters.DNight - Nshift;

                TotalShifts += db.ShiftsPerWeek.Select(y => y.NumOfShifts).FirstOrDefault();
                ShiftsPerWeek SperW = db.ShiftsPerWeek.Find(idnum);
                SperW.NumOfShifts = TotalShifts;

                db.Entry(SperW).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                db.ScheduleParameters.Add(scheduleParameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(scheduleParameters);
        }

        public ActionResult EditWeek(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            HsDelete.SpecialFixedFshiftDeletion();
            return View(scheduleParameters);
        }

        public ActionResult EditDay(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            HsDelete.SpecialFixedFshiftDeletion();
            return View(scheduleParameters);
        }

        public ActionResult EditMax(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            return View(scheduleParameters);
        }

        [HttpGet, ActionName("Edit")]
        public ActionResult EditChoice(int? id, bool? WhichEdit)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }

            if (WhichEdit == true)
            {
                return RedirectToAction("EditMax", new { ID = id });
            }

            if (scheduleParameters.Day == null)
            {
                return RedirectToAction("EditWeek", new { ID = id });
            }
            else
            {
                return RedirectToAction("EditDay", new { ID = id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ScheduleParameters scheduleParameters, bool? WhichEdit)
        {
            if(WhichEdit==true)
            {
                scheduleParameters.Morning = db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault();
                scheduleParameters.Afternoon = db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault();
                scheduleParameters.Night = db.ScheduleParameters.Select(x => x.Night).FirstOrDefault();
            }

            if (scheduleParameters.Day == null && WhichEdit == null)
            {
                scheduleParameters.MaxMorning = db.ScheduleParameters.Select(x => x.MaxMorning).FirstOrDefault();
                scheduleParameters.MaxAfternoon = db.ScheduleParameters.Select(x => x.MaxAfternoon).FirstOrDefault();
                scheduleParameters.MaxNight = db.ScheduleParameters.Select(x => x.MaxNight).FirstOrDefault();

                if (scheduleParameters.Morning != db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += (scheduleParameters.Morning - db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault()) * 7;

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.Afternoon != db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += (scheduleParameters.Afternoon - db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault()) * 7;

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.Night != db.ScheduleParameters.Select(x => x.Night).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += (scheduleParameters.Night - db.ScheduleParameters.Select(x => x.Night).FirstOrDefault()) * 7;

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if (scheduleParameters.Day != null && WhichEdit == null)
            {
                if (scheduleParameters.DMorning != db.ScheduleParameters.Where(p => p.Day == scheduleParameters.Day).Select(y => y.DMorning).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += scheduleParameters.DMorning - db.ScheduleParameters.Where(p => p.Day == scheduleParameters.Day).Select(y => y.DMorning).FirstOrDefault();

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.DAfternoon != db.ScheduleParameters.Where(p => p.Day == scheduleParameters.Day).Select(y => y.DAfternoon).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += scheduleParameters.DAfternoon - db.ScheduleParameters.Where(p => p.Day == scheduleParameters.Day).Select(y => y.DAfternoon).FirstOrDefault();

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (scheduleParameters.DNight != db.ScheduleParameters.Where(p => p.Day == scheduleParameters.Day).Select(y => y.DNight).FirstOrDefault())
                {
                    ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();
                    SperW.NumOfShifts += scheduleParameters.DNight - db.ScheduleParameters.Where(p => p.Day == scheduleParameters.Day).Select(y => y.DNight).FirstOrDefault();

                    db.Entry(SperW).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(scheduleParameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(scheduleParameters);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            if (scheduleParameters == null)
            {
                return HttpNotFound();
            }
            return View(scheduleParameters);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduleParameters scheduleParameters = db.ScheduleParameters.Find(id);
            ShiftsPerWeek SperW = db.ShiftsPerWeek.FirstOrDefault();

            if (scheduleParameters.Day == null)
            {
                SperW.NumOfShifts -= scheduleParameters.Morning * 7;
                SperW.NumOfShifts -= scheduleParameters.Afternoon * 7;
                SperW.NumOfShifts -= scheduleParameters.Night * 7;

                db.Entry(SperW).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                SperW.NumOfShifts += db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault() - scheduleParameters.DMorning;
                SperW.NumOfShifts += db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault() - scheduleParameters.DAfternoon;
                SperW.NumOfShifts += db.ScheduleParameters.Select(x => x.Night).FirstOrDefault() - scheduleParameters.DNight;

                db.Entry(SperW).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.ScheduleParameters.Remove(scheduleParameters);
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
