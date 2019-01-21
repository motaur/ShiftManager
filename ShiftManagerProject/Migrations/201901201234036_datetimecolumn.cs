namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetimecolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinalShift", "Dates", c => c.DateTime(nullable: false));
            AddColumn("dbo.PrevWeeks", "Dates", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrevWeeks", "Dates");
            DropColumn("dbo.FinalShift", "Dates");
        }
    }
}
