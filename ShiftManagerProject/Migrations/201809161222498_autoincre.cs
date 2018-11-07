namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class autoincre : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Saturday");
            AlterColumn("dbo.Saturday", "ID7", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Saturday", "ID6", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID5", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID4", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID3", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID2", c => c.Int(identity: true));
            AlterColumn("dbo.Saturday", "ID1", c => c.Int(identity: true));
            AddPrimaryKey("dbo.Saturday", "ID7");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Saturday");
            AlterColumn("dbo.Saturday", "ID1", c => c.Int());
            AlterColumn("dbo.Saturday", "ID2", c => c.Int());
            AlterColumn("dbo.Saturday", "ID3", c => c.Int());
            AlterColumn("dbo.Saturday", "ID4", c => c.Int());
            AlterColumn("dbo.Saturday", "ID5", c => c.Int());
            AlterColumn("dbo.Saturday", "ID6", c => c.Int());
            AlterColumn("dbo.Saturday", "ID7", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Saturday", "ID7");
        }
    }
}
