namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinalShift",
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
            
        }
        
        public override void Down()
        {

        }
    }
}
