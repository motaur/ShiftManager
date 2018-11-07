using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ShiftManagerProject.Models;

namespace ShiftManagerProject.DAL
{
    public class ShiftManagerInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ShiftManagerContext>
    {
        protected override void Seed(ShiftManagerContext context)
        {
            var Manager = new List<Employees>
            {
            new Employees{ID=12345678,FirstName="Maurice",LastName="Saadon",Email="Maurice.s@lsports.eu",Pass="123456",Admin=true},
            };

            Manager.ForEach(s => context.Employees.Add(s));
            context.SaveChanges();
        }
    }
}