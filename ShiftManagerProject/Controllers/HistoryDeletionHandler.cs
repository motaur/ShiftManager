using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShiftManagerProject.DAL;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.Controllers
{
    public class HistoryDeletionHandler
    {
        private ShiftManagerContext db = new ShiftManagerContext();

        public void PrevWeeksDeletion()
        {
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var count = db.PrevWeeks.ToList();
            if (count.Count() > 56)
            {
                foreach (var shift in db.PrevWeeks.Take(totalshifts))
                {
                    db.PrevWeeks.Remove(shift);
                }
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    throw new ArgumentException("Unable to delete Previous Week History");
                }
            }
        }

        public void FshiftDeletion()
        {
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            var countF = db.FinalShift.ToList();
            if (countF.Count() >= (totalshifts*2))
            {
                foreach (var shift in db.FinalShift.Take(totalshifts))
                {
                    db.FinalShift.Remove(shift);
                }
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    throw new ArgumentException("Unable to delete Final Shift History");
                }
            }
        }

        public void SpecialFixedFshiftDeletion()
        {
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            if (db.FinalShift.Count() >= totalshifts)
            {
                var countF = db.FinalShift.Take(totalshifts).ToList();

                foreach (var shift in countF)
                {
                    db.FinalShift.Remove(shift);
                }
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    throw new ArgumentException("Unable to delete Final Shift History");
                }
            }
        }

        //public void PreferenceDeletion()
        //{
        //    foreach (var shift in db.ShiftPref)
        //    {
        //        db.ShiftPref.Remove(shift);
        //    }
        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch
        //    {
        //        throw new ArgumentException("Unable to delete Preference History");
        //    }
        //}

        public void RemakeDeletion()
        {
            var totalshifts = db.ShiftsPerWeek.Select(o => o.NumOfShifts).FirstOrDefault();
            if (db.Remake.Count() >= totalshifts)
            {
                var RemakeList = db.Remake.ToList();
                foreach (var shift in RemakeList)
                {
                    db.Remake.Remove(shift);
                }
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    throw new ArgumentException("Unable to delete Saved Shift History");
                }

            }
        }
    }
}