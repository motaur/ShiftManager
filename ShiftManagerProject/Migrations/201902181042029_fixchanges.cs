namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixchanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false, identity: false));
            AlterColumn("dbo.Saturday", "ID", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Saturday", "NoOfShifts", c => c.Long(nullable: false));
            AlterColumn("dbo.Saturday", "EmployID", c => c.Long(nullable: false));
            AlterColumn("dbo.PrevWeeks", "EmployID", c => c.Long(nullable: true));
            AlterColumn("dbo.PrevWeeks", "OfDayType", c => c.Long(nullable: false));
            AlterColumn("dbo.FinalShift", "OfDayType", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
        }
    }
}
