namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DayIDs : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Saturday", "ID1");
            DropColumn("dbo.Saturday", "ID3");
            DropPrimaryKey("dbo.Saturday");
            AlterColumn("dbo.Saturday", "ID2", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Saturday", "ID2");
            DropColumn("dbo.Saturday", "ID7");
            DropColumn("dbo.Saturday", "ID6");
            DropColumn("dbo.Saturday", "ID5");
            DropColumn("dbo.Saturday", "ID4");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Saturday", "ID4", c => c.Int(identity: true));
            AddColumn("dbo.Saturday", "ID5", c => c.Int(identity: true));
            AddColumn("dbo.Saturday", "ID6", c => c.Int(identity: true));
            AddColumn("dbo.Saturday", "ID7", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Saturday");
            AlterColumn("dbo.Saturday", "ID1", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID2", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID3", c => c.Int(identity: true));
            AddPrimaryKey("dbo.Saturday", "ID7");
            RenameColumn(table: "dbo.Saturday", name: "ID3", newName: "ID9");
            RenameColumn(table: "dbo.Saturday", name: "ID1", newName: "ID8");
            AddColumn("dbo.Saturday", "ID3", c => c.Int(identity: true));
            AddColumn("dbo.Saturday", "ID1", c => c.Int(identity: true));
        }
    }
}
