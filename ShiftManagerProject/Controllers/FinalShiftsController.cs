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


        public ActionResult Index()
        {
            int ad = 0;
            if (!db.ShiftPref.Any())
            {
                ad = 1;
            }
            ViewBag.admin = ad;

            var nextshifts = db.FinalShift.OrderByDescending(x => x.ID).Take(28).OrderBy(x => x.ID);
            return View(nextshifts);
        }

        public ActionResult FShiftsForEmployees()
        {
            var nextshifts = db.FinalShift.OrderByDescending(x => x.ID).Take(28).OrderBy(x => x.ID);
            return View(nextshifts);
        }

        public ActionResult Close()
        {
            List<string> FirstLetter = new List<string>(new string[] { "M", "A", "N" });
            List<FinalShift> FShift = new List<FinalShift>();
            int i = 1, j = 0, flag = 0, x, MornChecker = 0, DoubleChecker = 0;
            string NameofShift = null;
            ShiftPref Stype = new ShiftPref();
            List<int> NightFlag = new List<int>(new int[FSrespo.ListOfEmployees().Count]);
            List<int> NoOfShifts = new List<int>(new int[FSrespo.ListOfEmployees().Count]);

            foreach (var prop in Stype.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(Nullable<bool>))
                {
                    NameofShift = prop.Name;
                    for (i = 1; i < Convert.ToInt16(NameofShift.Substring(NameofShift.Length - 1, 1)); i++) ;
                    foreach (var Letter in FirstLetter)
                    {
                        if (Letter == "M" && prop.Name.Substring(0, 1) == Letter)
                        {
                            for (x = 0; x < 2; x++)
                            {
                                MornChecker = 0;
                                FinalShift BeforeChecking = new FinalShift();
                                Recheck: BeforeChecking = (FSrespo.CheckPref(i, Letter));

                                if (MornChecker == 3)
                                {
                                    BeforeChecking.Day = FSrespo.DayOfWeek(i);
                                    BeforeChecking.Morning = true;
                                    BeforeChecking.EmployID = 0;
                                    BeforeChecking.Name = null;
                                    db.FinalShift.Add(BeforeChecking);
                                    db.SaveChanges();
                                    continue;
                                }

                                if (x == 1)
                                {
                                    if (FShift[FShift.Count - 1].EmployID == BeforeChecking.EmployID)
                                    {
                                        MornChecker++;
                                        goto Recheck;
                                    }
                                }

                                for (j = 0; j < FSrespo.ListOfEmployees().Count; j++)
                                {
                                    if (BeforeChecking.EmployID == FSrespo.ListOfEmployees()[j])
                                    {
                                        NoOfShifts[j] += 1;

                                        ShiftPref pref = db.ShiftPref.Single(ShiftPref => ShiftPref.EmployID == BeforeChecking.EmployID);
                                        if (NoOfShifts[j] > pref.NoOfShifts)
                                        {
                                            NoOfShifts[j] -= 1;
                                            MornChecker++;
                                            goto Recheck;
                                        }
                                        FShift.Add(BeforeChecking);
                                        db.FinalShift.Add(BeforeChecking);
                                        db.SaveChanges();
                                        break;
                                    }
                                    else if (BeforeChecking.EmployID == 0)
                                    {
                                        FShift.Add(BeforeChecking);
                                        db.FinalShift.Add(BeforeChecking);
                                        db.SaveChanges();
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (prop.Name.Substring(0, 1) == Letter)
                            {
                                
                                FinalShift BeforeChecking = new FinalShift();
                                BeforeChecking = (FSrespo.CheckPref(i, Letter));

                                DoubleChecker = 0;
                                flag = 0;

                                Checker1: for (j = 0; j < FSrespo.ListOfEmployees().Count; j++)
                                {
                                    if (BeforeChecking.EmployID == FSrespo.ListOfEmployees()[j])
                                    {
                                        if (DoubleChecker == 5)
                                        {
                                            BeforeChecking.EmployID = 0;
                                            BeforeChecking.Name = null;
                                            goto Double;
                                        }

                                        NoOfShifts[j] += 1;
                                        ShiftPref pref = db.ShiftPref.Single(ShiftPref => ShiftPref.EmployID == BeforeChecking.EmployID);

                                        if (NoOfShifts[j] > pref.NoOfShifts)
                                        {
                                            NoOfShifts[j] -= 1;
                                            DoubleChecker++;
                                            BeforeChecking = (FSrespo.CheckPref(i, Letter));
                                            goto Checker1;
                                        }

                                        if (BeforeChecking.Night == true)
                                        {
                                            NCheck: foreach (var Nshift in FShift)
                                            {
                                                if (NightFlag[j] >= 2)
                                                {
                                                    NoOfShifts[j] -= 1;
                                                    NightFlag[j] -= 1;
                                                    BeforeChecking.EmployID = 0;
                                                    BeforeChecking.Name = null;
                                                    goto Double;
                                                }
                                                if (flag == 3)
                                                {
                                                    NightFlag[j] += 1;
                                                    if(NightFlag[j] >=2)
                                                    {
                                                        NoOfShifts[j] -= 1;
                                                        NightFlag[j] -= 1;
                                                        BeforeChecking.EmployID = 0;
                                                        BeforeChecking.Name = null;
                                                        goto Double;
                                                    }
                                                    goto Finalize;
                                                }
                                                if (FSrespo.ListOfEmployees()[j] == Nshift.EmployID && Nshift.Night == true)
                                                {
                                                    BeforeChecking = (FSrespo.CheckPref(i, Letter));
                                                    if(BeforeChecking.EmployID == FSrespo.ListOfEmployees()[j])
                                                    {
                                                        BeforeChecking = FSrespo.CheckPref(i, Letter);
                                                    }
                                                    else
                                                    {
                                                        flag++;
                                                        NoOfShifts[j] -= 1;
                                                        goto Checker1;
                                                    }
                                                }
                                            }
                                            if (NightFlag[j] >= 2)
                                            {
                                                goto NCheck;
                                            }
                                            NightFlag[j] += 1;
                                        }

                                        Finalize: FShift.Add(BeforeChecking);
                                        db.FinalShift.Add(BeforeChecking);
                                        db.SaveChanges();
                                        break;
                                    }

                                    Double: if (BeforeChecking.EmployID == 0)
                                    {
                                        FShift.Add(BeforeChecking);
                                        db.FinalShift.Add(BeforeChecking);
                                        db.SaveChanges();
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }
            }

            FSrespo.ReviewAndPlace(NoOfShifts, FShift, NightFlag);
            FSrespo.NightChecker();

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
            foreach (var shift in db.ShiftPref)
            {
                db.ShiftPref.Remove(shift);
            }
            db.SaveChanges();
            return RedirectToAction("DeleteNSend");
        }

        public ActionResult DeleteNSend()
        {
            PrevWeeks pweek = new PrevWeeks();
            var fshift = db.FinalShift.ToList();
            foreach (var Fshift in fshift)
            {
                pweek.Day = Fshift.Day;
                pweek.EmployID = Fshift.EmployID;
                pweek.Name = Fshift.Name;
                if (Fshift.Morning == null)
                {
                    pweek.Morning = false;
                }
                else
                {
                    pweek.Morning = Fshift.Morning;
                }
                if (Fshift.Afternoon == null)
                {
                    pweek.Afternoon = false;
                }
                else
                {
                    pweek.Afternoon = Fshift.Afternoon;
                }
                if (Fshift.Night == null)
                {
                    pweek.Night = false;
                }
                else
                {
                    pweek.Night = Fshift.Night;
                }
                db.PrevWeeks.Add(pweek);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
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
