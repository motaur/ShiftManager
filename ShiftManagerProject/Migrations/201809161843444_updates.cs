namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Saturday", "EmployID");
            RenameColumn(table: "dbo.Saturday", name: "EmployID2", newName: "EmployID");
            DropColumn("dbo.Saturday", "EmployID1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Saturday", "EmployID1", c => c.Long());
            RenameColumn(table: "dbo.Saturday", name: "EmployID", newName: "EmployID2");
            AddColumn("dbo.Saturday", "EmployID", c => c.Long());
        }
    }
}
