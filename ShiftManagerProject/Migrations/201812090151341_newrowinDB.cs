namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newrowinDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrevWeeks", "OfDayType", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrevWeeks", "OfDayType");
        }
    }
}
