namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employid : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "ID", c => c.Long(nullable: false, identity: false));
        }
        
        public override void Down()
        {
        }
    }
}
