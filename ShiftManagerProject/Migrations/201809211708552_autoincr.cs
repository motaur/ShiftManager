namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class autoincr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
               "dbo.PrevWeeks",
               c => new
               {
                   ID = c.Long(nullable: false, identity: true),
                   EmployID = c.Long(nullable: false),
                   Day = c.String(nullable: false),
                   Morning = c.Boolean(nullable: true),
                   Afternoon = c.Boolean(nullable: true),
                   Night = c.Boolean(nullable: true),
               })
               .PrimaryKey(t => t.ID);

            AlterColumn("dbo.Saturday", "ID", c => c.Long(nullable: false, identity: true));
        }
        
        public override void Down()
        {
        }
    }
}
