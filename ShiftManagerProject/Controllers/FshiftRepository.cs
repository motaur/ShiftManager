using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class FshiftRepository
    {
        private ShiftManagerContext db = new ShiftManagerContext();
        static Random rnd = new Random();

        public FinalShift CheckPref(int x, string y)
        {
            List<Employees> Workers = new List<Employees>();
            var LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(2);
            var PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(2);
            var flag = 0;
            var a = 0;
            string shiftofday = null;

            foreach (var employ in db.Employees)
            {
                flag = 0;
                ShiftPref pref = new ShiftPref();
                pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);
                if (pref == null)
                {
                    continue;
                }

                if (x == 1 && y == "M")
                {
                    foreach (var LastShift in LastDay)
                    {
                        if (employ.ID == LastShift.EmployID)
                        {
                            flag++;
                        }
                    }
                }
                else if (x == 1 && y == "A")
                {
                    LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(1);
                    foreach (var lastday in LastDay)
                    {
                        if (employ.ID == lastday.EmployID)
                        {
                            flag++;
                        }
                    }

                    foreach (var PrevShifts in PrevShift)
                    {
                        if (employ.ID == PrevShifts.EmployID)
                        {
                            flag++;
                        }
                    }
                }
                else if (y == "N" || (y == "A" && x != 1))
                {
                    PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(3);
                    foreach (var PrevShifts in PrevShift)
                    {
                        if (employ.ID == PrevShifts.EmployID)
                        {
                            flag++;
                        }
                    }

                    PrevShift = db.FinalShift.Where(k => k.Afternoon == true);
                    foreach (var AfternoonShift in PrevShift)
                    {
                        if (employ.ID == AfternoonShift.EmployID)
                        {
                            a++;
                        }
                        if (a == 2)
                        {
                            flag++;
                        }
                    }

                    if (y == "N" && x == 1)
                    {
                        foreach (var LastShift in LastDay)
                        {
                            if (employ.ID == LastShift.EmployID && LastShift.Night == true)
                            {
                                flag++;
                            }
                        }
                    }
                }
                else if (y == "M" && x != 1)
                {
                    PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(2);
                    foreach (var PrevShifts in PrevShift)
                    {
                        if (employ.ID == PrevShifts.EmployID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag == 0)
                {
                    foreach (var shift in pref.GetType().GetProperties())
                    {
                        if (shift.Name.EndsWith(Convert.ToString(x)) && shift.Name.StartsWith(y))
                        {
                            shiftofday = shift.Name;
                            shiftofday = shiftofday.Remove(shiftofday.Length - 1);

                            var val = (Boolean)shift.GetValue(pref);
                            if (val)
                            {
                                Workers.Add(employ);
                                break;
                            }
                            break;
                        }
                    }
                }
            }
            FinalShift Fshift = new FinalShift();

            if (!Workers.Any())
            {
                foreach (var employ in db.Employees)
                {
                    ShiftPref prefshift = new ShiftPref();
                    prefshift = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);
                    if (prefshift == null)
                    {
                        continue;
                    }

                    foreach (var shift in prefshift.GetType().GetProperties())
                    {
                        if (shift.Name.EndsWith(Convert.ToString(x)) && shift.Name.StartsWith(y))
                        {
                            shiftofday = shift.Name;
                            shiftofday = shiftofday.Remove(shiftofday.Length - 1);

                            var val = (Boolean)shift.GetValue(prefshift);
                            if (val)
                            {
                                Workers.Add(employ);
                                break;
                            }
                            break;
                        }
                    }
                }
            }
            else if (Workers.Any())
            {
                int r = rnd.Next(Workers.Count);
                Fshift = new FinalShift
                {
                    EmployID = Workers[r].ID,
                    Name = Workers[r].FirstName
                };
            }


            var Ftype = typeof(FinalShift);

            foreach (var shift in Fshift.GetType().GetProperties())
            {
                if (shift.Name == "Day")
                {
                    Ftype.GetProperty(shift.Name).SetValue(Fshift, DayOfWeek(x));
                }
                if (shiftofday == shift.Name)
                {
                    Ftype.GetProperty(shift.Name).SetValue(Fshift, true);
                }
            }
            return (Fshift);
        }

        public string DayOfWeek(int d)
        {
            List<string> dayofweek = new List<string>(new string[] { "", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
            return (dayofweek[d]);
        }

        public bool AfternoonChecker(long ID)
        {
            var PrevShifts = db.FinalShift.Where(k => k.Afternoon == true);
            var p = 0;

            foreach (var AfternoonShift in PrevShifts)
            {
                if (ID == AfternoonShift.EmployID)
                {
                    p++;
                }
            }
            if (p == 2)
            {
                return true;
            }

            return false;
        }

        public List<long> ListOfEmployees()
        {
            List<long> EmployIDs = new List<long>();
            foreach (var emp in db.Employees)
            {
                EmployIDs.Add(emp.ID);
            }
            return EmployIDs;
        }

        public void EightEightChecker()
        {
            var Cshifts = (db.FinalShift.OrderByDescending(k => k.ID).Take(28).OrderBy(k => k.ID)).ToList();
            int flagC = 0;

            foreach (var employ in ListOfEmployees())
            {
                flagC = 0;
                for (int p = 0, q = 0; p < Cshifts.Count(); p++)
                {
                    if (Cshifts[p].Day != "Sunday")
                    {
                        if (Cshifts[p].Morning == true)
                        {
                            q = q == 2 ? 0 : q;
                            q++;

                            if (q != 2)
                            {
                                if (Cshifts[p - 2].EmployID == Cshifts[p].EmployID && Cshifts[p].EmployID == employ)
                                {
                                    flagC++;
                                }
                            }
                            else
                            {
                                if (Cshifts[p - 3].EmployID == Cshifts[p].EmployID && Cshifts[p].EmployID == employ)
                                {
                                    flagC++;
                                }
                            }
                        }
                        else if (Cshifts[p].Afternoon == true)
                        {
                            if (Cshifts[p - 3].EmployID == Cshifts[p].EmployID && Cshifts[p].EmployID == employ)
                            {
                                flagC++;
                            }
                        }

                    }
                    if (flagC == 2)
                    {
                        flagC = 0;
                        FinalShift ID = db.FinalShift.Find(Cshifts[p].ID);
                        Cshifts[p].EmployID = 0;
                        Cshifts[p].Name = null;
                        var result = db.FinalShift.SingleOrDefault(b => b.ID == ID.ID);
                        db.Entry(result).CurrentValues.SetValues(Cshifts[p]);
                        db.SaveChanges();
                    }
                }
            }
        }

        public void ReviewAndPlace(List<int> NoOfShifts, List<FinalShift> FShift, List<int> NightFlag)
        {
            int y = 0;
            var fshifts = (db.FinalShift.OrderByDescending(k => k.ID).Take(28).OrderBy(k => k.ID)).ToList();
            FinalShift LastMorn = new FinalShift();

            for (int i = 0, j = 0, flag = 0; i < ListOfEmployees().Count; i++)
            {
                ShiftPref employ = new ShiftPref
                {
                    EmployID = ListOfEmployees()[i]
                };
                ShiftPref pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.EmployID);
                if (pref == null)
                {
                    continue;
                }

                if (NoOfShifts[i] < pref.NoOfShifts)
                {
                    j = 0;
                    flag = 0;
                    foreach (var shift in fshifts)
                    {
                        if (j++ % 4 == 0)
                        {
                            flag++;
                        }
                        if (shift.Morning == true && shift.EmployID != 0)
                        {
                            LastMorn = shift;
                        }
                        if (shift.EmployID == 0)
                        {
                            y = 0;
                            FinalShift BeforeChecking = new FinalShift();
                            if (shift.Morning == true)
                            {
                                recheck: BeforeChecking = (CheckPref(flag, "M"));
                                if (BeforeChecking.EmployID != ListOfEmployees()[i])
                                {
                                    y++;
                                    if (y <= 2)
                                    {
                                        goto recheck;
                                    }
                                    continue;
                                }
                                else
                                {
                                    foreach (var Dshift in FShift)
                                    {
                                        if (shift.Day == Dshift.Day)
                                        {
                                            if (Dshift.EmployID == BeforeChecking.EmployID)
                                            {
                                                y = 2;
                                                break;
                                            }
                                        }
                                        else if (DayOfWeek(flag - 1) == Dshift.Day && Dshift.Night == true)
                                        {
                                            if (Dshift.EmployID == BeforeChecking.EmployID)
                                            {
                                                y = 2;
                                                break;
                                            }
                                        }
                                    }
                                    if (y == 2)
                                    {
                                        continue;
                                    }
                                }
                                if (LastMorn.Day == shift.Day && BeforeChecking.EmployID == LastMorn.EmployID)
                                {
                                    goto recheck;
                                }
                            }
                            else if (shift.Afternoon == true)
                            {
                                recheck: BeforeChecking = (CheckPref(flag, "A"));
                                if (BeforeChecking.EmployID != ListOfEmployees()[i])
                                {
                                    y++;
                                    if (y <= 2)
                                    {
                                        goto recheck;
                                    }
                                    continue;
                                }
                                else
                                {
                                    if (AfternoonChecker(ListOfEmployees()[i]))
                                    {
                                        continue;
                                    }

                                    foreach (var Dshift in FShift)
                                    {
                                        if (shift.Day == Dshift.Day && Dshift.Afternoon != true)
                                        {
                                            if (Dshift.EmployID == BeforeChecking.EmployID)
                                            {
                                                y = 2;
                                                break;
                                            }
                                        }
                                    }
                                    if (y == 2)
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                recheck: BeforeChecking = (CheckPref(flag, "N"));
                                if (BeforeChecking.EmployID != ListOfEmployees()[i])
                                {
                                    y++;
                                    if (y <= 2)
                                    {
                                        goto recheck;
                                    }
                                    continue;
                                }
                                else
                                {
                                    if (NightFlag[i] >= 2)
                                    {
                                        continue;
                                    }
                                    NightFlag[i] += 1;

                                    foreach (var Dshift in FShift)
                                    {
                                        if (shift.Day == Dshift.Day && Dshift.Night != true)
                                        {
                                            if (Dshift.EmployID == BeforeChecking.EmployID)
                                            {
                                                y = 2;
                                                break;
                                            }
                                        }
                                        else if (flag != 7 && DayOfWeek(flag + 1) == Dshift.Day && Dshift.Morning == true)
                                        {
                                            if (Dshift.EmployID == BeforeChecking.EmployID)
                                            {
                                                y = 2;
                                                break;
                                            }
                                        }
                                        if (Dshift.EmployID == BeforeChecking.EmployID && Dshift.Night == true)
                                        {
                                            y++;
                                        }
                                    }
                                    if (y >= 2)
                                    {

                                        continue;
                                    }
                                }
                            }

                            if (BeforeChecking.EmployID != 0)
                            {
                                if (BeforeChecking.Morning == true)
                                {
                                    LastMorn = BeforeChecking;
                                }
                                BeforeChecking.ID = shift.ID;
                                NoOfShifts[i] += 1;
                                FShift.Add(BeforeChecking);
                                var result = db.FinalShift.SingleOrDefault(b => b.ID == shift.ID);

                                if (result != null)
                                {
                                    db.Entry(result).CurrentValues.SetValues(BeforeChecking);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }

                }
            }
        }

        public void NightChecker()
        {
            int flagC = 0;
            var fshifts = (db.FinalShift.OrderByDescending(k => k.ID).Take(28).OrderBy(k => k.ID)).ToList();

            foreach (var employ in ListOfEmployees())
            {
                List<FinalShift> NightShifts = new List<FinalShift>();
                flagC = 0;
                foreach (var shift in fshifts)
                {
                    if (shift.Night == true && shift.EmployID == employ)
                    {
                        flagC++;
                        NightShifts.Add(shift);
                    }
                    if (flagC > 2)
                    {
                        FinalShift ID = db.FinalShift.Find(shift.ID);
                        shift.EmployID = 0;
                        shift.Name = null;
                        var result = db.FinalShift.SingleOrDefault(b => b.ID == ID.ID);
                        db.Entry(result).CurrentValues.SetValues(shift);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}