using System;
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
            int i,y;
            Employees Emp = db.Employees.FirstOrDefault(x => x.FirstName == FixedShift.Name);
            for (i = 1; FSrespo.DayOfWeek(i) != FixedShift.Day; i++) ;
            y = FixedShift.Morning == true ? 0 : FixedShift.Afternoon == true ? 2 : 3;

            if (db.FinalShift.Count()==28 || db.FinalShift.Count()==0) { FSrespo.OfDayHandler(true, 0, 0);}

            try
            {
                if (Emp != null && ModelState.IsValid)
                {
                    FixedShift.EmployID = Emp.ID;
                    flag = true;
                    FixedShift.OfDayType = FSrespo.OrderOfDayTypeHandler(i,y);
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
                    return View(FixedShift);
                }
                ModelState.AddModelError("Name", "Employee's ID was not found or matched");
                return View(FixedShift);
            }
            return View(FixedShift);
        }

        //public ActionResult FClose()
        //{
        //    FSrespo.PrevShiftsRotation();
        //    HsDelete.PrevWeeksDeletion();
        //    HsDelete.FshiftDeletion();

        //    List<string> FirstLetter = new List<string>(new string[] { "M", "A", "N" });
        //    List<FinalShift> FShift = new List<FinalShift>();
        //    int i = 1, x;
        //    string NameofShift = null;
        //    ShiftPref Stype = new ShiftPref();
        //    List<int> NightFlag = new List<int>(new int[FSrespo.ListOfEmployees().Count]);
        //    List<int> NoOfShifts = new List<int>(new int[FSrespo.ListOfEmployees().Count]);

        //    foreach (var prop in Stype.GetType().GetProperties())
        //    {
        //        if (prop.PropertyType == typeof(Nullable<bool>))
        //        {
        //            NameofShift = prop.Name;
        //            for (i = 1; i < Convert.ToInt16(NameofShift.Substring(NameofShift.Length - 1, 1)); i++) ;
        //            foreach (var Letter in FirstLetter)
        //            {
        //                if (Letter == "M" && prop.Name.Substring(0, 1) == Letter)
        //                {
        //                    if (FSrespo.Exist(i, Letter))
        //                    {
        //                        continue;
        //                    }

        //                    for (x = 0; x < 2; x++)
        //                    {
        //                        if (x == 0 && FSrespo.OneMornExist(i, Letter))
        //                        {
        //                            continue;
        //                        }

        //                        FinalShift BeforeChecking = (FSrespo.NewCheckerP(i, Letter));

        //                        if (BeforeChecking != null)
        //                        {
        //                            FSrespo.SavingToDB(BeforeChecking);
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            BeforeChecking = new FinalShift();
        //                            FSrespo.MorningReset(BeforeChecking, i);
        //                            continue;
        //                        }
        //                    }
        //                }
        //                else if (Letter != "M" && prop.Name.Substring(0, 1) == Letter)
        //                {
        //                    if (FSrespo.Exist(i, Letter))
        //                    {
        //                        continue;
        //                    }

        //                    FinalShift BeforeChecking = (FSrespo.NewCheckerP(i, Letter));

        //                    if (BeforeChecking != null)
        //                    {
        //                        FSrespo.SavingToDB(BeforeChecking);
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        BeforeChecking = new FinalShift
        //                        {
        //                            Day = FSrespo.DayOfWeek(i)
        //                        };
        //                        BeforeChecking = FSrespo.ResetFeilds(BeforeChecking);
        //                        switch (Letter)
        //                        {
        //                            case "A":
        //                                BeforeChecking.Afternoon = true;
        //                                break;
        //                            case "N":
        //                                BeforeChecking.Night = true;
        //                                break;
        //                        }
        //                        FSrespo.SavingToDB(BeforeChecking);
        //                        continue;
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        public ActionResult Index()
        {
            int ad = 0;

            var pweeks = db.PrevWeeks.Select(k=>k.EmployID).ToList();
            var fweeks = db.FinalShift.Select(k => k.EmployID).ToList();
            if(pweeks.SequenceEqual(fweeks))
            {
                ad = 1;
            }

            ViewBag.admin = ad;

            var nextshifts = db.FinalShift.OrderByDescending(x => x.OfDayType).OrderBy(x => x.OfDayType).ToList();
            return View(nextshifts);
        }

        public ActionResult FShiftsForEmployees()
        {
            var nextshifts = db.FinalShift.OrderByDescending(x => x.OfDayType).OrderBy(x => x.OfDayType).ToList();
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
            return RedirectToAction("Index");
        }

        public ActionResult DeleteFshifts()
        {
            HsDelete.SpecialFixedFshiftDeletion();
            return RedirectToAction("Fixed");
        }

        public ActionResult Edit(long? id)
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
            finalShift.Employees = db.Employees.ToList();
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
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(finalShift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(finalShift);
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
