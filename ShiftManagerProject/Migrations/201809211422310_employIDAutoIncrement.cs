namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employIDAutoIncrement : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false, identity: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false, identity: true));
        }
    }
}
