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
        static int DaySerial = 0;
        static int[,] Mat = new int[5,7];

        public int OfDayHandler(bool whattodo, int d, int s)
        {
            int num = 0;
            if(whattodo)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0, x = 1; x < 8 && i == 0; j++, x++)
                    {
                        Mat[i,j] = x;
                    }

                    for (int j = 0, x = i-1; j < 7 && i != 0; j++, x += 4)
                    {
                        Mat[i,j] = x;
                    }
                }
            }
            else
            {
                if(s==0)
                {
                    num = Mat[s + 1, d - 1] != 99 ? Mat[s + 1, d - 1] : Mat[s + 2, d - 1];
                    if (Mat[s + 1, d - 1] == num)
                    {
                        Mat[s + 1, d - 1] = 99;
                    }
                    else
                    {
                        Mat[s + 2, d - 1] = 99;
                    }
                }
                else
                {
                    num = Mat[s + 1, d - 1];
                    Mat[s + 1, d - 1] = 99;
                }
            }
          
            return num;
        }


        public FinalShift CheckPref(int x, string y)
        {
            List<Employees> Workers = new List<Employees>();
            var LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(2);
            var PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(2);
            var flag = 0;
            string shiftofday = null;

            foreach (var employ in db.Employees)
            {
                var a = 0;
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
                else if (y == "N" && x == 1)
                {
                    foreach (var LastShift in LastDay)
                    {
                        if (employ.ID == LastShift.EmployID && LastShift.Night == true)
                        {
                            flag++;
                        }
                    }
                    PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(3);
                    foreach (var PrevShifts in PrevShift)
                    {
                        if (employ.ID == PrevShifts.EmployID)
                        {
                            flag++;
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
                else if ((y == "N" || y == "A") && x != 1)
                {
                    PrevShift = db.FinalShift.OrderByDescending(p => p.ID).Take(3);
                    foreach (var PrevShifts in PrevShift)
                    {
                        if (employ.ID == PrevShifts.EmployID)
                        {
                            flag++;
                        }
                    }

                    if (y == "A" && x > 2)
                    {
                        PrevShift = db.FinalShift.Where(k => k.Afternoon == true).OrderByDescending(p => p.ID).Take(x - 1);
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
                    }
                    if (y == "N" && x > 2)
                    {
                        PrevShift = db.FinalShift.Where(k => k.Night == true).OrderByDescending(p => p.ID).Take(x - 1);
                        foreach (var NightShift in PrevShift)
                        {
                            if (employ.ID == NightShift.EmployID)
                            {
                                a++;
                            }
                            if (a == 2)
                            {
                                flag++;
                            }
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
            var PrevShifts = db.FinalShift.Where(k => k.Afternoon == true).OrderByDescending(k => k.ID).Take(7);
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

        //public void ReviewAndPlace(List<int> NoOfShifts, List<FinalShift> FShift, List<int> NightFlag)
        //{
        //    int y = 0;
        //    var fshifts = (db.FinalShift.OrderByDescending(k => k.ID).Take(28).OrderBy(k => k.ID)).ToList();
        //    FinalShift LastMorn = new FinalShift();

        //    for (int i = 0, j = 0, flag = 0; i < ListOfEmployees().Count; i++)
        //    {
        //        ShiftPref employ = new ShiftPref
        //        {
        //            EmployID = ListOfEmployees()[i]
        //        };
        //        ShiftPref pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.EmployID);
        //        if (pref == null)
        //        {
        //            continue;
        //        }

        //        if (NoOfShifts[i] < pref.NoOfShifts)
        //        {
        //            j = 0;
        //            flag = 0;
        //            foreach (var shift in fshifts)
        //            {
        //                if (j++ % 4 == 0)
        //                {
        //                    flag++;
        //                }
        //                if (shift.Morning == true && shift.EmployID != 0)
        //                {
        //                    LastMorn = shift;
        //                }
        //                if (shift.EmployID == 0)
        //                {
        //                    y = 0;
        //                    FinalShift BeforeChecking = new FinalShift();
        //                    if (shift.Morning == true)
        //                    {
        //                        recheck: BeforeChecking = (CheckPref(flag, "M"));
        //                        if (BeforeChecking.EmployID != ListOfEmployees()[i])
        //                        {
        //                            y++;
        //                            if (y <= 2)
        //                            {
        //                                goto recheck;
        //                            }
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            foreach (var Dshift in FShift)
        //                            {
        //                                if (shift.Day == Dshift.Day)
        //                                {
        //                                    if (Dshift.EmployID == BeforeChecking.EmployID)
        //                                    {
        //                                        y = 2;
        //                                        break;
        //                                    }
        //                                }
        //                                else if (DayOfWeek(flag - 1) == Dshift.Day && Dshift.Night == true)
        //                                {
        //                                    if (Dshift.EmployID == BeforeChecking.EmployID)
        //                                    {
        //                                        y = 2;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                            if (y == 2)
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        if (LastMorn.Day == shift.Day && BeforeChecking.EmployID == LastMorn.EmployID)
        //                        {
        //                            goto recheck;
        //                        }
        //                    }
        //                    else if (shift.Afternoon == true)
        //                    {
        //                        recheck: BeforeChecking = (CheckPref(flag, "A"));
        //                        if (BeforeChecking.EmployID != ListOfEmployees()[i])
        //                        {
        //                            y++;
        //                            if (y <= 2)
        //                            {
        //                                goto recheck;
        //                            }
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            if (AfternoonChecker(ListOfEmployees()[i]))
        //                            {
        //                                continue;
        //                            }

        //                            foreach (var Dshift in FShift)
        //                            {
        //                                if (shift.Day == Dshift.Day && Dshift.Afternoon != true)
        //                                {
        //                                    if (Dshift.EmployID == BeforeChecking.EmployID)
        //                                    {
        //                                        y = 2;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                            if (y == 2)
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        recheck: BeforeChecking = (CheckPref(flag, "N"));
        //                        if (BeforeChecking.EmployID != ListOfEmployees()[i])
        //                        {
        //                            y++;
        //                            if (y <= 2)
        //                            {
        //                                goto recheck;
        //                            }
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            if (NightFlag[i] >= 2)
        //                            {
        //                                continue;
        //                            }
        //                            NightFlag[i] += 1;

        //                            foreach (var Dshift in FShift)
        //                            {
        //                                if (shift.Day == Dshift.Day && Dshift.Night != true)
        //                                {
        //                                    if (Dshift.EmployID == BeforeChecking.EmployID)
        //                                    {
        //                                        y = 2;
        //                                        break;
        //                                    }
        //                                }
        //                                else if (flag != 7 && DayOfWeek(flag + 1) == Dshift.Day && Dshift.Morning == true)
        //                                {
        //                                    if (Dshift.EmployID == BeforeChecking.EmployID)
        //                                    {
        //                                        y = 2;
        //                                        break;
        //                                    }
        //                                }
        //                                if (Dshift.EmployID == BeforeChecking.EmployID && Dshift.Night == true)
        //                                {
        //                                    y++;
        //                                }
        //                            }
        //                            if (y >= 2)
        //                            {

        //                                continue;
        //                            }
        //                        }
        //                    }

        //                    if (BeforeChecking.EmployID != 0)
        //                    {
        //                        if (BeforeChecking.Morning == true)
        //                        {
        //                            LastMorn = BeforeChecking;
        //                        }
        //                        BeforeChecking.ID = shift.ID;
        //                        NoOfShifts[i] += 1;
        //                        FShift.Add(BeforeChecking);
        //                        var result = db.FinalShift.SingleOrDefault(b => b.ID == shift.ID);

        //                        if (result != null)
        //                        {
        //                            db.Entry(result).CurrentValues.SetValues(BeforeChecking);
        //                            db.SaveChanges();
        //                        }
        //                    }

        //                }
        //            }

        //        }
        //    }
        //}

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

        public void MorningReset(FinalShift BeforeChecking, int i)
        {
            BeforeChecking.Day = DayOfWeek(i);
            BeforeChecking.Morning = true;
            BeforeChecking.EmployID = 0;
            BeforeChecking.Name = null;
            SavingToDB(BeforeChecking);
        }

        public void SavingToDB(FinalShift BeforeChecking)
        {
            BeforeChecking.OfDayType = DaySerial++;
            DaySerial = DaySerial > 27 ? 0 : DaySerial;
            db.FinalShift.Add(BeforeChecking);
            db.SaveChanges();
        }

        public FinalShift ResetFeilds(FinalShift BeforeChecking)
        {
            BeforeChecking.EmployID = 0;
            BeforeChecking.Name = null;
            return BeforeChecking;
        }

        public void PrevShiftsRotation()
        {
            PrevWeeks pweek = new PrevWeeks();
            var fshift = db.FinalShift.ToList();
            foreach (var Fshift in fshift)
            {
                pweek.Day = Fshift.Day;
                pweek.EmployID = Fshift.EmployID;
                pweek.Name = Fshift.Name;
                pweek.OfDayType = Fshift.OfDayType;

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
        }

        public bool FutureShifts(int x, string y, Employees employee)
        {
            string dayofweek = DayOfWeek(x);
            string Fdayofweek = x != 7 ? DayOfWeek(x + 1) : DayOfWeek(x);
            int flag = 0;
            if (y == "M")
            {
                var futureShifts = db.FinalShift.Where(k => k.Day == dayofweek);
                foreach (var shift in futureShifts)
                {
                    if (shift.EmployID == employee.ID)
                    {
                        flag++;
                    }
                }
                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "A")
            {
                if (x != 7)
                {
                    var futureShifts = db.FinalShift.Where(k => (k.Day == dayofweek && k.Night == true) || (k.Day == Fdayofweek && k.Morning == true));

                    foreach (var shift in futureShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var futureShifts = db.FinalShift.Where(k => k.Day == dayofweek && k.Night == true);

                    foreach (var shift in futureShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "N")
            {
                if (x != 7)
                {
                    var futureShifts = db.FinalShift.Where(k => k.Day == Fdayofweek && (k.Morning == true || k.Afternoon == true));

                    foreach (var shift in futureShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PreviousShifts(int x, string y, Employees employee)
        {
            string dayofweek = DayOfWeek(x);
            string lastdayofweek = DayOfWeek(x - 1);
            int flag = 0;
            if (y == "M")
            {
                if (x == 1)
                {
                    var LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(2);
                    foreach (var LastShift in LastDay)
                    {
                        if (LastShift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var PrevShifts = db.FinalShift.Where(k => (k.Day == lastdayofweek && (k.Afternoon == true || k.Night == true)) || (k.Day == dayofweek && k.Morning == true));
                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "A")
            {
                if (x == 1)
                {
                    var LastDay = db.PrevWeeks.OrderByDescending(p => p.ID).Take(1);
                    foreach (var lastday in LastDay)
                    {
                        if (lastday.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }

                    var PrevShifts = db.FinalShift.Where(k => k.Day == dayofweek && k.Morning == true);

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var PrevShifts = db.FinalShift.Where(k => (k.Day == lastdayofweek && k.Night == true) || (k.Day == dayofweek && k.Morning == true));

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }

            if (y == "N")
            {
                if (x == 1)
                {
                    var PrevShifts = db.FinalShift.Where(k => (k.Day == dayofweek && (k.Morning == true || k.Afternoon == true)) || (k.Day == lastdayofweek && k.Night == true));

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }
                else
                {
                    var PrevShifts = db.FinalShift.Where(k => k.Day == dayofweek && (k.Morning == true || k.Afternoon == true));

                    foreach (var shift in PrevShifts)
                    {
                        if (shift.EmployID == employee.ID)
                        {
                            flag++;
                        }
                    }
                }

                if (flag != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Exist(int x, string y)
        {
            int flag = 0;
            string dayofweek = DayOfWeek(x);
            List<FinalShift> futureShifts = db.FinalShift.Where(k => k.Day == dayofweek).ToList();
            if (futureShifts.Any())
            {
                switch (y)
                {
                    case "M":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Morning == true)
                            {
                                AssignDayType(shift);
                                flag++;
                            }
                        }
                        if (flag == 2)
                        {
                            return true;
                        }
                        break;
                    case "A":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Afternoon == true)
                            {
                                AssignDayType(shift);
                                return true;
                            }
                        }
                        break;
                    case "N":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Night == true)
                            {
                                AssignDayType(shift);
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        public bool OneMornExist(int x, string y)
        {
            string dayofweek = DayOfWeek(x);
            var futureShifts = db.FinalShift.Where(k => k.Day == dayofweek);
            if (futureShifts.Any())
            {
                switch (y)
                {
                    case "M":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Morning == true)
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        public bool HasTwoAfternoons(long ID, int x)
        {
            if (x > 2)
            {
                int a = 0;
                var PrevShift = db.FinalShift.Where(k => k.Afternoon == true).OrderByDescending(p => p.ID).Take(x - 1);
                foreach (var AfternoonShift in PrevShift)
                {
                    if (ID == AfternoonShift.EmployID)
                    {
                        a++;
                    }
                    if (a == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool HasTwoNights(long ID, int x)
        {
                int a = 0;
                var PrevShift = db.FinalShift.Where(k => k.Night == true).OrderByDescending(p => p.OfDayType);
                foreach (var NightShift in PrevShift)
                {
                    if (ID == NightShift.EmployID)
                    {
                        a++;
                    }
                    if (a == 1)
                    {
                        return true;
                    }
                }
            return false;
        }

        public bool CanAddShift(long ID)
        {
            var EmployShifts = db.FinalShift.Where(k => k.EmployID == ID);
            ShiftPref pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == ID);

            if (EmployShifts.Count() < pref.NoOfShifts)
            {
                return true;
            }
            return false;
        }

        public FinalShift NewCheckerP(int x, string y)
        {

            List<Employees> Workers = new List<Employees>();
            int flag = 0;
            FinalShift Fshift = new FinalShift();
            int r;

            foreach (var employ in db.Employees)
            {
                string shiftofday = null;
                ShiftPref pref = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);

                if (pref == null)
                {
                    continue;
                }

                if (!PreviousShifts(x, y, employ))
                {
                    if (!FutureShifts(x, y, employ))
                    {
                        foreach (var shift in pref.GetType().GetProperties())
                        {
                            if (shift.Name.EndsWith(Convert.ToString(x)) && shift.Name.StartsWith(y))
                            {
                                shiftofday = shift.Name;
                                shiftofday = shiftofday.Remove(shiftofday.Length - 1);

                                var val = (Boolean)shift.GetValue(pref);
                                if (val && CanAddShift(employ.ID))
                                {
                                    Workers.Add(employ);
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            Redo: if (Workers.Any())
            {
                r = rnd.Next(Workers.Count);
                Fshift = new FinalShift
                {
                    EmployID = Workers[r].ID,
                    Name = Workers[r].FirstName,
                    Day = DayOfWeek(x)
                };
            }
            else
            {
                return null;
            }

            switch (y)
            {
                case "M":
                    Fshift.Morning = true;
                    break;
                case "A":
                    if (HasTwoAfternoons(Fshift.EmployID, x))
                    {
                        if (flag == 3)
                        {
                            return null;
                        }
                        Workers.Remove(Workers[r]);
                        flag++;
                        goto Redo;
                    }
                    Fshift.Afternoon = true;
                    break;
                case "N":
                    if (HasTwoNights(Fshift.EmployID, x))
                    {
                        if (flag == 3)
                        {
                            return null;
                        }
                        Workers.Remove(Workers[r]);
                        flag++;
                        goto Redo;
                    }
                    Fshift.Night = true;
                    break;
            }

            return Fshift;

        }

        public void AssignDayType(FinalShift Fshift)
        {
            var result = db.FinalShift.SingleOrDefault(b => b.ID == Fshift.ID);
            Fshift.OfDayType = DaySerial++;
            DaySerial = DaySerial > 27 ? 0 : DaySerial;

            if (result != null)
            {
                db.Entry(result).CurrentValues.SetValues(Fshift);
                db.SaveChanges();
            }
            else
            {
                new Exception("There is no such record to modify");
            }
        }

        public void LeastShiftPref()
        {
            ShiftPref prefshifts = new ShiftPref();
            List<Employees> employees = db.Employees.ToList();

            var val = false;
            FinalShift NewFinalS = new FinalShift();
            List<string> FirstLetter = new List<string>(new string[] { "M", "M", "A", "N" });
            int[] counter = new int[4], TrueCounter = new int[4];
            int i, j = 0, max = 0, TrueMax=0;

            OfDayHandler(true, 0, 0);

            for (i = 1; i < 8; i++)
            {
                foreach (var employ in employees)
                {
                    prefshifts = db.ShiftPref.SingleOrDefault(ShiftPref => ShiftPref.EmployID == employ.ID);
                    if (prefshifts == null)
                    {
                        continue;
                    }

                    foreach (var shifts in prefshifts.GetType().GetProperties())
                    {            
                        if (shifts.Name.EndsWith(Convert.ToString(i)))
                        {
                            if (NewExistForNewCheckerP(i, shifts.Name.Substring(0, 1)))
                            {  continue; }

                            val = (Boolean)shifts.GetValue(prefshifts);
                        
                            switch (shifts.Name.Substring(0, 1))
                            {
                                case "M":
                                    if (!val)
                                    {
                                        counter[0] += 1;   
                                    }
                                    else
                                    {
                                        TrueCounter[0] += 1;
                                    }
                                    break;
                                case "A":
                                    if (!val)
                                    {
                                        counter[2] += 1;
                                    }
                                    else
                                    {
                                        TrueCounter[2] += 1;
                                    }
                                    break;
                                case "N":
                                    if (!val)
                                    {
                                        counter[3] += 1;
                                    }
                                    else
                                    {
                                        TrueCounter[3] += 1;
                                    }
                                    break;
                            }
                        }
                    }
                }

                max = counter.ToList().IndexOf(counter.Max());
                TrueMax = TrueCounter.ToList().IndexOf(TrueCounter.Max());

                if (counter.Max() > 0)
                {
                    NewFinalS = NewCheckerP(i, FirstLetter[max]);
                    if (NewFinalS == null)
                    {
                        NewFinalS = new FinalShift
                        {
                            EmployID = 0,
                            Name = null,
                            Day = DayOfWeek(i)
                        };
                        switch (FirstLetter[max])
                        {
                            case "M":
                                NewFinalS.Morning = true;
                                break;
                            case "A":
                                NewFinalS.Afternoon = true;
                                break;
                            case "N":
                                NewFinalS.Night = true;
                                break;
                        }
                    }
                    NewFinalS.OfDayType = OrderOfDayTypeHandler(i, max);
                }
                else if (TrueCounter.Max() > 0 && counter.Max() == 0)
                {
                    NewFinalS = NewCheckerP(i, FirstLetter[TrueMax]);
                    if (NewFinalS == null)
                    {
                        NewFinalS = new FinalShift
                        {
                            EmployID = 0,
                            Name = null,
                            Day = DayOfWeek(i)
                        };
                        switch (FirstLetter[TrueMax])
                        {
                            case "M":
                                NewFinalS.Morning = true;
                                break;
                            case "A":
                                NewFinalS.Afternoon = true;
                                break;
                            case "N":
                                NewFinalS.Night = true;
                                break;
                        }
                    }
                    NewFinalS.OfDayType = OrderOfDayTypeHandler(i, TrueMax);
                }           

                    db.FinalShift.Add(NewFinalS);
                    db.SaveChanges();
                
                    counter[0] = counter[1] = counter[2] = counter[3]= 0;
                    TrueCounter[0] = TrueCounter[1] = TrueCounter[2] = TrueCounter[3] = 0;
                
                j++;

                i = j + 1 < 29 ? i : 9; 
                i = i + 1 == 8 ? 0 : i;
             
            }
        }

        public int OrderOfDayTypeHandler(int d, int s)
        {
            return OfDayHandler(false,d,s);
        }

        public bool NewExistForNewCheckerP(int x, string y)
        {
            int flag = 0;
            string dayofweek = DayOfWeek(x);
            List<FinalShift> futureShifts = db.FinalShift.Where(k => k.Day == dayofweek).ToList();
            if (futureShifts.Any())
            {
                switch (y)
                {
                    case "M":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Morning == true)
                            {
                                flag++;
                            }
                        }
                        if (flag == 2)
                        {
                            return true;
                        }
                        break;
                    case "A":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Afternoon == true)
                            {
                                return true;
                            }
                        }
                        break;
                    case "N":
                        foreach (var shift in futureShifts)
                        {
                            if (shift.Night == true)
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }
    }
}