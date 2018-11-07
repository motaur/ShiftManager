namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Employees");
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.Employees", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Employees");
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.Employees", "ID");
        }
    }
}
