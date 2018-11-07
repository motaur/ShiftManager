namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nametoshiftpref : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Saturday", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Saturday", "Name");
        }
    }
}
