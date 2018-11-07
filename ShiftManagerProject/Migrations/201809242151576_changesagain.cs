namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changesagain : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinalShift", "Morning", c => c.Boolean());
            AlterColumn("dbo.FinalShift", "Afternoon", c => c.Boolean());
            AlterColumn("dbo.FinalShift", "Night", c => c.Boolean());
            AlterColumn("dbo.FinalShift", "EmployID", c => c.Long(nullable: true));
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false, identity: false));
            AlterColumn("dbo.Saturday", "ID", c => c.Long(nullable: false, identity: true));
        }

        public override void Down()
        {
            AlterColumn("dbo.FinalShift", "Night", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FinalShift", "Afternoon", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FinalShift", "Morning", c => c.Boolean(nullable: false));
        }
    }
}
