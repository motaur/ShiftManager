namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changestofinalshift : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinalShift", "Morning", c => c.Boolean());
            AlterColumn("dbo.FinalShift", "Afternoon", c => c.Boolean());
            AlterColumn("dbo.FinalShift", "Night", c => c.Boolean());
    
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FinalShift", "Night", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FinalShift", "Afternoon", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FinalShift", "Morning", c => c.Boolean(nullable: false));
        }
    }
}
