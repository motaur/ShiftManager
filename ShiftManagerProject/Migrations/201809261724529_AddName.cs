namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinalShift", "Name", c => c.String());
        }
        
        public override void Down()
        {
        }
    }
}
