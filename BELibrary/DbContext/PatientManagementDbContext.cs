using System.Collections.Generic;

namespace BELibrary.DbContext
{
    using BELibrary.Entity;
    using System;
    using System.Data.Entity;

    public partial class HospitalManagementDbContext : DbContext
    {
        public HospitalManagementDbContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public HospitalManagementDbContext(Dictionary<string, List<string>> mapSymptompsFaculty, List<string> mapSymptons)
        {
            mapSymptompsFaculty = new Dictionary<string, List<string>>
            {
                { "Khoa Ngoại Chấn thương chỉnh hình", new List<string> { "Đau, sưng, biến dạng xương khớp.", "Gãy xương, trật khớp, bong gân.", "Giảm hoặc mất khả năng vận động chi." } },
                { "Khoa Ngoại Thần kinh", new List<string> { "Đau đầu dữ dội, buồn nôn, nôn.", "Tê liệt tay chân, mất cảm giác.", "Co giật, mất ý thức đột ngột." } },
                { "Khoa Bệnh Nhiệt đới", new List<string> { "Sốt cao, rét run.", "Phát ban, đau cơ, mệt mỏi.", "Tiêu chảy, nôn mửa, vàng da." } },
                { "Khoa Giải phẫu bệnh", new List<string> { "Khám chẩn đoán bệnh lý qua mẫu mô.", "Kết quả xét nghiệm tế bào và mô để phát hiện ung thư hoặc bệnh lý bất thường." } },
                { "Khoa Phẫu thuật Tim - Lồng ngực mạch máu", new List<string> { "Khó thở, đau ngực.", "Tim đập không đều, sưng chân.", "Ngất xỉu, tím tái." } },
                { "Khoa Nội tiết", new List<string> { "Khát nước, tiểu nhiều (bệnh tiểu đường).", "Tăng cân hoặc giảm cân bất thường.", "Run tay, đổ mồ hôi nhiều (rối loạn tuyến giáp)." } },
                { "Khoa Hồi sức Tim mạch", new List<string> { "Nhịp tim nhanh, chậm, không đều.", "Đau ngực, khó thở, phù nề.", "Ngất xỉu hoặc mất ý thức." } },
                { "Khoa Răng Hàm Mặt - Mắt", new List<string> { "Đau răng, sưng nướu, sâu răng.", "Mờ mắt, đau mắt đỏ, khô mắt.", "Lệch khớp cắn, đau quai hàm." } },
                { "Đơn vị Nội soi", new List<string> { "Đau bụng kéo dài, ợ nóng, khó tiêu.", "Nôn ra máu, đi ngoài phân đen.", "Khó nuốt hoặc đau họng kéo dài." } },
                { "Khoa Nội Thận - Miễn dịch ghép", new List<string> { "Phù chân, tiểu ít, tiểu ra máu.", "Đau lưng dưới, huyết áp cao.", "Triệu chứng nhiễm trùng sau ghép thận." } },
                { "Khoa Ngoại tổng quát", new List<string> { "Đau bụng dữ dội, sốt (viêm ruột thừa, viêm túi mật).", "U, cục bất thường ở bụng hoặc phần mềm.", "Khó tiêu, sụt cân không rõ nguyên nhân." } },
                { "Khoa Dinh dưỡng", new List<string> { "Sụt cân hoặc tăng cân nhanh chóng.", "Chán ăn, mệt mỏi kéo dài.", "Vấn đề tiêu hóa do chế độ ăn uống." } },
                { "Khoa Cơ xương khớp", new List<string> { "Đau nhức khớp, cứng khớp buổi sáng.", "Biến dạng khớp, hạn chế vận động.", "Đau lưng, đau cổ kéo dài." } },
                { "Khoa Bệnh lý mạch máu não", new List<string> { "Đau đầu dữ dội, mất thăng bằng.", "Mờ mắt, tê liệt nửa người.", "Nói khó, lơ mơ, hôn mê." } },
                { "Khoa Tai mũi họng", new List<string> { "Đau họng, khó nuốt, khản tiếng.", "Ngứa mũi, nghẹt mũi, chảy mũi.", "Đau tai, ù tai, chảy mủ tai." } },
                { "Khoa Hô hấp", new List<string> { "Khó thở, ho kéo dài, đau ngực.", "Ho ra máu, thở khò khè.", "Sốt kèm ho có đờm." } },
                { "Khoa Nội Thần kinh tổng quát", new List<string> { "Đau đầu, mất ngủ.", "Chóng mặt, mất trí nhớ.", "Tê bì tay chân, co giật." } },
               
            };

        }

        public List<string> GetSymptons()
        {
            var mapSymptons = new List<string>();
            mapSymptons.AddRange(new[] { "Đau, sưng, biến dạng xương khớp.", "Gãy xương, trật khớp, bong gân.", "Giảm hoặc mất khả năng vận động chi." });
            mapSymptons.AddRange(new[] { "Đau đầu dữ dội, buồn nôn, nôn.", "Tê liệt tay chân, mất cảm giác.", "Co giật, mất ý thức đột ngột." });
            mapSymptons.AddRange(new[] { "Sốt cao, rét run.", "Phát ban, đau cơ, mệt mỏi.", "Tiêu chảy, nôn mửa, vàng da." });
            mapSymptons.AddRange(new[] { "Khám chẩn đoán bệnh lý qua mẫu mô.", "Kết quả xét nghiệm tế bào và mô để phát hiện ung thư hoặc bệnh lý bất thường." });
            mapSymptons.AddRange(new[] { "Đau đầu, mất ngủ.", "Chóng mặt, mất trí nhớ.", "Tê bì tay chân, co giật." });
            mapSymptons.AddRange(new[] { "Đau bụng kéo dài, ợ nóng, khó tiêu.", "Nôn ra máu, đi ngoài phân đen.", "Khó nuốt hoặc đau họng kéo dài." });
            return mapSymptons;
        }

        public virtual DbSet<Account> Accounts { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<DetailPrescription> DetailPrescriptions { get; set; }

        public virtual DbSet<DetailRecord> DetailRecords { get; set; }

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<MedicalSupply> MedicalSupplies { get; set; }

        public virtual DbSet<Medicine> Medicines { get; set; }

        public virtual DbSet<Patient> Patients { get; set; }

        public virtual DbSet<Prescription> Prescriptions { get; set; }

        public virtual DbSet<Record> Records { get; set; }

        public virtual DbSet<Attachment> Attachments { get; set; }

        public virtual DbSet<AttachmentAssign> AttachmentAssigns { get; set; }

        public virtual DbSet<Faculty> Faculties { get; set; }

        public virtual DbSet<Doctor> Doctors { get; set; }

        public virtual DbSet<PatientRecord> PatientRecords { get; set; }

        public virtual DbSet<UserVerification> UserVerifications { get; set; }

        public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        public virtual DbSet<PatientDoctor> PatientDoctors { get; set; }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<PatientRegister> PatientRegisters { get; set; }

        //public DbSet<Symptons> Symptoms { get; set; }


        //Mới:  public DbSet<DetailsFacultySymptons> DetailsFacultySymptons { get; set; }
        //public DbSet<DetailsFacultySymptons> DetailsFacultySymptons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Items)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Medicine>()
                .HasMany(e => e.DetailPrescriptions)
                .WithRequired(e => e.Medicine)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.IndentificationCardId)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Patient>()
                .HasMany(e => e.MedicalSupplies)
                .WithRequired(e => e.Patient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Record>()
                .HasMany(e => e.DetailRecords)
                .WithRequired(e => e.Record)
                .WillCascadeOnDelete(false);
        }

        public dynamic GetSymptoms()
        {
            throw new NotImplementedException();
        }
    }
}