namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idKey : DbMigration
    {
        public override void Up()
        {

        }
        
        public override void Down()
        {
            AddColumn("dbo.Saturday", "ID2", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Saturday");
            AlterColumn("dbo.Saturday", "ID", c => c.Long(identity: true));
            AlterColumn("dbo.Saturday", "ID", c => c.Long());
            AlterColumn("dbo.Saturday", "ID", c => c.Long());
            AddPrimaryKey("dbo.Saturday", "ID2");
            RenameColumn(table: "dbo.Saturday", name: "ID", newName: "ID3");
            RenameColumn(table: "dbo.Saturday", name: "ID", newName: "ID1");
            AddColumn("dbo.Saturday", "ID", c => c.Long());
            AddColumn("dbo.Saturday", "ID", c => c.Long());
        }
    }
}
