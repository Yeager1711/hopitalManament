namespace BELibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Room_Properties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientRegister", "PickTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Room", "FacultyId", c => c.Guid());
            CreateIndex("dbo.Room", "FacultyId");
            AddForeignKey("dbo.Room", "FacultyId", "dbo.Faculty", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Room", "FacultyId", "dbo.Faculty");
            DropIndex("dbo.Room", new[] { "FacultyId" });
            DropColumn("dbo.Room", "FacultyId");
            DropColumn("dbo.PatientRegister", "PickTime");
        }
    }
}
