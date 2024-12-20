using BELibrary.Core.Utils;

namespace BELibrary.Migrations
{
    using BELibrary.DbContext;
    using BELibrary.Entity;
    using BELibrary.Utils;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<HospitalManagementDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(HospitalManagementDbContext context)
        {
            // Creating default admin account
            var passwordFactory = VariableExtensions.DefautlPassword + VariableExtensions.KeyCrypto;
            var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

            context.Accounts.AddOrUpdate(c => c.UserName, new Account()
            {
                Id = Guid.NewGuid(),
                FullName = "Quản Trị",
                Phone = "0973 642 032",
                UserName = "admin",
                Password = passwordCrypto,
                Role = 1,
                Gender = true,
                LinkAvatar = ""
            });

            context.SaveChanges();

            #region Faculty Seeding
            var faculties = new List<Faculty>
            {
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Tim mạch Can thiệp"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Tim mạch tổng quát"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Nhịp tim học"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Hồi sức Tim mạch"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Phẫu thuật Tim - Lồng ngực mạch máu"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Nội Tiêu hóa"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Nội Thần kinh tổng quát"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Ngoại Thần kinh"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Nội tiết"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Bệnh lý mạch máu não"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Bệnh Nhiệt đới"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Cơ xương khớp"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Hô hấp"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Ngoại Niệu - Ghép thận"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Nội Thận - Miễn dịch ghép"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Cấp cứu Tổng hợp"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Hồi sức tích cực - Chống độc"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Gây mê - Hồi sức Ngoại"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Ngoại tổng quát"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Ngoại Chấn thương chỉnh hình"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Tai mũi họng"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Răng Hàm Mặt - Mắt"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Y học cổ truyền - Phục hồi chức năng"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Y học thể thao"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Khám bệnh"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Khám và Điều trị theo yêu cầu"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Xét nghiệm"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Chẩn đoán hình ảnh"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Giải phẫu bệnh"},
                new Faculty {Id = Guid.NewGuid(), Name = "Đơn vị Nội soi"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Dinh dưỡng"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Kiểm soát nhiễm khuẩn"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Dược"},
                new Faculty {Id = Guid.NewGuid(), Name = "Khoa Ung bướu và Y học hạt nhân"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Tổ chức Cán bộ"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Kế hoạch Tổng hợp"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Điều dưỡng"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Chỉ đạo tuyến"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Tài chính Kế toán"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Hành chính Quản trị"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Vật tư - Thiết bị y tế"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Công nghệ thông tin"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Quản lý chất lượng"},
                new Faculty {Id = Guid.NewGuid(), Name = "Phòng Công tác xã hội"},
                new Faculty {Id = Guid.NewGuid(), Name = "Nhà Thuốc"}
            };

            foreach (var faculty in faculties)
            {
                context.Faculties.AddOrUpdate(c => c.Name, faculty);
            }

            #endregion Faculty

            #region Medicine Seeding
            var medicines = new List<Medicine>
            {
                new Medicine {Id = Guid.NewGuid(), Name = "Natri chloride 0,45%", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Natri chloride 10%", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Aminosteril N-Hepa 8%", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Abobotulinum Toxin A", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Acarbose", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Acenocoumarol", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Acetazolamide", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Acetyl-Dl-Leucine", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Acetylcysteine", Description = ""},
                new Medicine {Id = Guid.NewGuid(), Name = "Amoxicillin/ Clavulanic Acid (tiêm)", Description = ""}
                // Add more medicines as needed
            };

            foreach (var medicine in medicines)
            {
                context.Medicines.AddOrUpdate(c => c.Name, medicine);
            }
            #endregion Medicine

            context.SaveChanges();
        }
    }
}
