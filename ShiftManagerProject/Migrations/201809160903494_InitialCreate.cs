namespace ShiftManagerProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Pass = c.String(nullable: false),
                        Admin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Saturday",
                c => new
                    {
                        ID7 = c.Int(nullable: false),
                        Morning7 = c.Boolean(nullable: false),
                        Afternoon7 = c.Boolean(nullable: false),
                        Night7 = c.Boolean(nullable: false),
                        ID6 = c.Int(),
                        Morning6 = c.Boolean(),
                        Afternoon6 = c.Boolean(),
                        Night6 = c.Boolean(),
                        ID5 = c.Int(),
                        Morning5 = c.Boolean(),
                        Afternoon5 = c.Boolean(),
                        Night5 = c.Boolean(),
                        ID4 = c.Int(),
                        Morning4 = c.Boolean(),
                        Afternoon4 = c.Boolean(),
                        Night4 = c.Boolean(),
                        ID3 = c.Int(),
                        Morning3 = c.Boolean(),
                        Afternoon3 = c.Boolean(),
                        Night3 = c.Boolean(),
                        ID2 = c.Int(),
                        Morning2 = c.Boolean(),
                        Afternoon2 = c.Boolean(),
                        Night2 = c.Boolean(),
                        ID1 = c.Int(),
                        Morning1 = c.Boolean(),
                        Afternoon1 = c.Boolean(),
                        Night1 = c.Boolean(),
                        ID = c.Long(),
                        EmployID = c.Long(),
                        ID8 = c.Long(),
                        EmployID1 = c.Long(),
                        ID9 = c.Long(identity: true),
                        EmployID2 = c.Long(),
                        NoOfShifts = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID7);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Saturday");
            DropTable("dbo.Employees");
        }
    }
}
