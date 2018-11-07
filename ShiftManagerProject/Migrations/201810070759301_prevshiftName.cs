namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prevshiftName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrevWeeks", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrevWeeks", "Name");
        }
    }
}
