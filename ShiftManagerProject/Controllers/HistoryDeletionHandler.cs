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
            var count = db.PrevWeeks.ToList();
            if (count.Count() >= 56)
            {
                foreach (var shift in db.PrevWeeks.Take(28))
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
            var countF = db.FinalShift.ToList();
            if (countF.Count() >= 56)
            {
                foreach (var shift in db.FinalShift.Take(28))
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
            var countF = db.FinalShift.ToList();
        
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

        public void PreferenceDeletion()
        {
            foreach (var shift in db.ShiftPref)
            {
                db.ShiftPref.Remove(shift);
            }
            try
            {
                db.SaveChanges();
            }
            catch
            {
                throw new ArgumentException("Unable to delete Preference History");
            }
        }
    }
}