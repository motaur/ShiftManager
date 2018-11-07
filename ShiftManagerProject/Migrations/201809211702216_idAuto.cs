namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idAuto : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Saturday", "ID", c => c.Long(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Saturday", "ID", c => c.Long(nullable: false, identity: false));
        }
    }
}
