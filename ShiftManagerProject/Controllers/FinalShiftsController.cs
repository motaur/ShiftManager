﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;
using System.Net.Mail;

namespace ShiftManagerProject.Controllers
{
    public class FinalShiftsController : Controller
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        private FshiftRepository FSrespo = new FshiftRepository();
        private HistoryDeletionHandler HsDelete = new HistoryDeletionHandler();

        public ActionResult ListOfShifts()
        {
            int closed = 0;
            if (!db.FinalShift.Any())
            {
                closed = 1;
            }
            ViewBag.close = closed;
            return View(db.ShiftPref.ToList());
        }

        public ActionResult Fixed()
        {
            HsDelete.FshiftDeletion();
            FinalShift finalShift = new FinalShift
            {
                Employees = db.Employees.ToList()
            };
            return View(finalShift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fixed(FinalShift FixedShift)
        {
            bool flag = false;
            int i, y;
            Employees Emp = db.Employees.FirstOrDefault(x => x.FirstName == FixedShift.Name);
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            for (i = 1; FSrespo.DayOfWeek(i) != FixedShift.Day; i++) ;
            y = FixedShift.Morning == true ? 0 : FixedShift.Afternoon == true ? 2 : 3;

            DayOfWeek day = new System.DayOfWeek();
            string dday = FSrespo.DayOfWeek(i);

            for (int d = 0; d < 7; d++)
            {
                if (day.ToString() != dday)
                {
                    day = (DayOfWeek)((d + 1) % 7);
                }
                else { break; }
            }

            FixedShift.Dates = FSrespo.NextWeeksDates(day);

            if (db.FinalShift.Count() == totalshifts || db.FinalShift.Count() 
                == 0) { FSrespo.OfDayHandler(true, 0, 0); }

            try
            {
                if (Emp != null && ModelState.IsValid)
                {
                    FixedShift.EmployID = Emp.ID;
                    flag = true;
                    FixedShift.OfDayType = FSrespo.OrderOfDayTypeHandler(i, y);
                    db.FinalShift.Add(FixedShift);
                    db.SaveChanges();
                    return RedirectToAction("Fixed");
                }
            }
            catch
            {
                if (flag)
                {
                    ModelState.AddModelError("Name", "Error Saving this shift");
                    FixedShift.Employees = db.Employees.ToList();
                    return View(FixedShift);
                }
                ModelState.AddModelError("Name", "Employee's ID was not found or matched");
                FixedShift.Employees = db.Employees.ToList();
                return View(FixedShift);
            }
            return View(FixedShift);
        }

        public ActionResult Index()
        {
            int ad = 0;
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var pweeks = db.PrevWeeks.OrderByDescending(x => x.ID).Take(totalshifts).OrderBy(w => w.OfDayType).Select(k => k.EmployID).ToList();
            var fweeks = db.FinalShift.OrderBy(q => q.OfDayType).Select(k => k.EmployID).ToList();
            if (pweeks.SequenceEqual(fweeks))
            {
                ad = 1;
            }

            ViewBag.admin = ad;

            ViewBag.shifts = db.ScheduleParameters.Select(x => x.Morning + x.Afternoon + x.Night).FirstOrDefault();
            ViewBag.morning = db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault();
            ViewBag.afternoon = db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault();
            ViewBag.night = db.ScheduleParameters.Select(x => x.Night).FirstOrDefault();

            var nextshifts = db.FinalShift.OrderBy(x => x.OfDayType).ToList();

            return View(nextshifts);
        }

        public ActionResult FShiftsForEmployees()
        {
            int i = 0;
            var nextshifts = db.FinalShift.ToList();
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var StartDateForSecondWeek = db.PrevWeeks.ToList().OrderByDescending(x => x.Dates.Date).Take(totalshifts).Select(y => y.Dates.Date).OrderBy(y=>y.Date).FirstOrDefault();

            if (DateTime.Now > StartDateForSecondWeek)
            {
                ViewBag.ShiftUpdate = ++i;
            }
            else
            {
                nextshifts = nextshifts.OrderBy(x => x.OfDayType).ToList();
            }
            ViewBag.ShiftUpdate = i;

            ViewBag.shifts = db.ScheduleParameters.Select(x => x.Morning + x.Afternoon + x.Night).FirstOrDefault();
            ViewBag.morning = db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault();
            ViewBag.afternoon = db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault();
            ViewBag.night = db.ScheduleParameters.Select(x => x.Night).FirstOrDefault();

            return View(nextshifts);
        }

        public ActionResult NewClose()
        {
            HsDelete.PrevWeeksDeletion();
            HsDelete.SpecialFixedFshiftDeletion();

            FSrespo.LeastShiftPref();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FinalShift finalShift)
        {
            if (ModelState.IsValid)
            {
                db.FinalShift.Add(finalShift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Send()
        {
            FSrespo.PrevShiftsRotation();
            HsDelete.PrevWeeksDeletion();
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();

            DateTime nextSunday = DateTime.Now.AddDays(1);
            while (nextSunday.DayOfWeek != DayOfWeek.Sunday)
            { nextSunday = nextSunday.AddDays(1); }
            var NextWeek = Convert.ToDateTime(nextSunday).ToString("dd/MM/yyyy");
            var shifts = db.PrevWeeks.ToList().OrderByDescending(k => k.ID).Take(totalshifts).OrderBy(x => x.OfDayType);
            var empList = new List<Employees>();

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nocshiftmaster@gmail.com", "buefifa19")
            };

            foreach (var emp in db.Employees.ToList())
            {
                if (shifts.Where(x => x.EmployID == emp.ID).Any())
                {
                    empList.Add(emp);
                }
            }

            foreach (var employee in empList)
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("nocshiftmaster@gmail.com")
                };
                mailMessage.To.Add(new MailAddress(employee.Email));
                mailMessage.Subject = "Your work schedule has been updated!";
                string body = "<table>" + "<h3>" + "Your work schedule has been updated for week " + NextWeek + "</h3>";
                foreach (var shift in shifts.Where(x => x.EmployID == employee.ID).OrderBy(d => d.OfDayType))
                {
                    body += "<tr>";
                    body += "<th>" + shift.Day + " " + "</th>";
                    body += "<td>" + (shift.Morning == true ? " Morning" : shift.Afternoon == true ? " Afternoon" : " Night") + "</td>";
                    body += "</tr>";
                }
                body += "</table>";
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                smtp.Send(mailMessage);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(long? id, bool? confirmed)
        {
            int x;
            List<Employees> EmployeeList = new List<Employees>();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalShift finalShift = db.FinalShift.Find(id);
            if (finalShift == null)
            {
                return HttpNotFound();
            }

            if (confirmed == true)
            {
                finalShift.Employees = db.Employees.ToList();
                return View(finalShift);
            }

            for (x = 0; FSrespo.DayOfWeek(x) != finalShift.Day; x++) ;
            string y = finalShift.Morning == true ? "M" : finalShift.Afternoon == true ? "A" : "N";

            if (confirmed == false)
            {
                foreach (var eight in FSrespo.AvailableEightEightEmployees(x, y, FSrespo.AvailableEmployees(x, y)).ToList())
                {
                    foreach (var worker in db.Employees.ToList())
                    {
                        if (eight == worker.FirstName)
                        {
                            EmployeeList.Add(worker);
                        }
                    }
                }

                finalShift.Employees = EmployeeList.ToList();
                return View(finalShift);
            }

            foreach (var avail in FSrespo.AvailableEmployees(x, y))
            {
                foreach (var worker in db.Employees.ToList())
                {
                    if (avail == worker.FirstName)
                    {
                        if (!FSrespo.PreviousShifts(x, y, worker))
                        {
                            if (!(FSrespo.FutureShifts(x, y, worker)))
                            {
                                EmployeeList.Add(worker);
                            }
                        }
                    }
                }
            }

            finalShift.Employees = EmployeeList.ToList();
            return View(finalShift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FinalShift finalShift)
        {
            foreach (var worker in db.Employees)
            {
                if (finalShift.Name == worker.FirstName)
                {
                    finalShift.EmployID = worker.ID;
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(finalShift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            finalShift.Employees = db.Employees.ToList();
            return View(finalShift);
        }

        public ActionResult SaveToRemake()
        {
            HsDelete.RemakeDeletion();
            FSrespo.SaveToRemakeTBL();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SavedSchedule()
        {
            int ad = 0;
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var sweeks = db.Remake.Take(totalshifts).OrderBy(w => w.OfDayType).Select(k => k.EmployID).ToList();
            var fweeks = db.FinalShift.OrderBy(q => q.OfDayType).Select(k => k.EmployID).ToList();
            if (sweeks.SequenceEqual(fweeks))
            {
                ad = 1;
            }

            ViewBag.admin = ad;

            ViewBag.msg = db.Remake.OrderBy(g=>g.OfDayType).Select(g => g.OfDayType).Count() == totalshifts ? 1 : 0;
            ViewBag.shifts = db.ScheduleParameters.Select(x => x.Morning + x.Afternoon + x.Night).FirstOrDefault();
            ViewBag.morning = db.ScheduleParameters.Select(x => x.Morning).FirstOrDefault();
            ViewBag.afternoon = db.ScheduleParameters.Select(x => x.Afternoon).FirstOrDefault();
            ViewBag.night = db.ScheduleParameters.Select(x => x.Night).FirstOrDefault();
            var nextshifts = db.Remake.OrderBy(x => x.OfDayType).ToList();
            return View(nextshifts);
        }

        public ActionResult SaveTheSchedule(bool id)
        {
            if (id)
            {
                HsDelete.SpecialFixedFshiftDeletion();
                FSrespo.SaveRemakeToFinalTBL();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalShift finalShift = db.FinalShift.Find(id);
            if (finalShift == null)
            {
                return HttpNotFound();
            }
            return View(finalShift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FinalShift finalShift = db.FinalShift.Find(id);
            db.FinalShift.Remove(finalShift);
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
