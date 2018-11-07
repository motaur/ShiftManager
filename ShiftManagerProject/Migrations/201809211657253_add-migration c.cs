namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigrationc : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Saturday", "ID2");
            AddPrimaryKey("dbo.Saturday", "ID");
        }
        
        public override void Down()
        {
        }
    }
}
