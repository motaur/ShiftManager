using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ShiftManagerProject.Models;
using ShiftManagerProject.DAL;
using System.Web.SessionState;

namespace ShiftManagerProject.Controllers
{
    public class EmployeeRepository
    {
        private ShiftManagerContext db = new ShiftManagerContext();

        public Employees IsValid(string email, string password)
        {
            using (db = new ShiftManagerContext())
            {
                try
                {
                    var users = db.Employees.Where(u => u.Email == email && u.Pass == password);
                    return users.Single();
                }
                catch
                {
                    throw new ArgumentException("Wrong Credentials");
                }
            }
        }
        public bool UserCounter(Employees NewUser)
        {
            var users = db.Employees.Where(u => u.Email == NewUser.Email);
            int usersCounter = users.Count();

            if (usersCounter==0)
            {
                return false;
            }
            return true;
        }
    }
}