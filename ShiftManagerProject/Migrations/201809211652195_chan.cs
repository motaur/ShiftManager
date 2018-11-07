namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chan : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Saturday", "ID8");
            DropColumn("dbo.Saturday", "ID9");
            DropPrimaryKey("dbo.Saturday");
                        
        }
        
        public override void Down()
        {
        }
    }
}
