namespace BELibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Room_And_User_Register : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientRegister",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomId = c.Int(nullable: false),
                        PatientId = c.Guid(nullable: false),
                        Number = c.Int(nullable: false),
                        StartTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patient", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Room", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Capacity = c.Int(nullable: false),
                        DoctorId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctor", t => t.DoctorId)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientRegister", "RoomId", "dbo.Room");
            DropForeignKey("dbo.Room", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.PatientRegister", "PatientId", "dbo.Patient");
            DropIndex("dbo.Room", new[] { "DoctorId" });
            DropIndex("dbo.PatientRegister", new[] { "PatientId" });
            DropIndex("dbo.PatientRegister", new[] { "RoomId" });
            DropTable("dbo.Room");
            DropTable("dbo.PatientRegister");
        }
    }
}
