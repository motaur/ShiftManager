
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftManagerProject.Models
{
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required(ErrorMessage = "First Name Required")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Required")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("User")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DisplayName("Password")]
        public string Pass { get; set; }

        [Required(ErrorMessage = "Admin Permissions Required")]
        [DisplayName("Admin Permissions")]
        public Nullable<bool> Admin { get; set; }

    }

    public class Sunday : Monday
    {
        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning1 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon1 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night1 { get; set; }
    }
    public class Monday : Tuesday
    {
        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning2 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon2 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night2 { get; set; }
    }
    public class Tuesday : Wednesday
    {
        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning3 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon3 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night3 { get; set; }
    }
    public class Wednesday : Thursday
    {
        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning4 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon4 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night4 { get; set; }
    }
    public class Thursday : Friday
    {
        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning5 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon5 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night5 { get; set; }
    }
    public class Friday : Saturday
    {
        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning6 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon6 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night6 { get; set; }
    }
    public class Saturday
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning7 { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon7 { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night7 { get; set; }
    }

    public class ShiftPref : Sunday
    {
        [Required]
        public long EmployID { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        [Required]
        [DisplayName("Number of Shifts")]
        public int NoOfShifts { get; set; }

    }

    public class FinalShift 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }

        public long EmployID { get; set; }

        public string Name { get; set; }

        [Required]
        public string Day { get; set; }

        [DisplayName("Morning")]
        public Nullable<bool> Morning { get; set; }

        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon { get; set; }
 
        [DisplayName("Night")]
        public Nullable<bool> Night { get; set; }

        public int OfDayType { get; set; }

        [Required]
        public DateTime Dates { get; set; }

        public IEnumerable<Employees> Employees { get; set; }
    }

    public class PrevWeeks
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long ID { get; set; }

        public long EmployID { get; set; }

        public string Name { get; set; }

        [Required]
        public string Day { get; set; }

        [Required]
        [DisplayName("Morning")]
        public Nullable<bool> Morning { get; set; }
        [Required]
        [DisplayName("Afternoon")]
        public Nullable<bool> Afternoon { get; set; }
        [Required]
        [DisplayName("Night")]
        public Nullable<bool> Night { get; set; }

        public int OfDayType { get; set; }

        [Required]
        public DateTime Dates { get; set; }
    }
}