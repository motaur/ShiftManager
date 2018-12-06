namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dayof : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinalShift", "OfDayType", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinalShift", "OfDayType");
        }
    }
}
