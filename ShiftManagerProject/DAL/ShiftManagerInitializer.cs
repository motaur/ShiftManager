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
           
        }
    }
}