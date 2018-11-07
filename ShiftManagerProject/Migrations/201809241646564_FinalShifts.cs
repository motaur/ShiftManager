namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalShifts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinalShift", "EmployID", c => c.Long(nullable: true));
        }
        
        public override void Down()
        {
        }
    }
}
