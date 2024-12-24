using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using HospitalManagement.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class RegisterController : BaseController
    {

        // GET: Register
        [HttpPost, ValidateInput(false)]

        public JsonResult pick(DateTime startTime, bool isRevisit, string symptoms, int roomId, string rooms)
        {
            var user = CookiesManage.GetUser();

            if (user == null)
            {
                return Json(new { status = false, mess = "Vui lòng đăng nhập" });
            }

            if (startTime.TimeOfDay < new TimeSpan(6, 30, 0)
                || startTime.TimeOfDay > new TimeSpan(23, 59, 0))
            {
                return Json(new
                {
                    status = false,
                    mess = "Chỉ được phép đặt lịch khám trong giờ làm việc từ 6h30 sáng đến 5h30 chiều !"
                });
            }

            if (startTime.DayOfWeek == DayOfWeek.Saturday)
            {
                if (startTime.TimeOfDay < new TimeSpan(6, 30, 0) || startTime.TimeOfDay > new TimeSpan(11, 30, 0))
                {
                    return Json(new
                    {
                        status = false,
                        mess = "Chỉ được đặt lịch khám ngày thứ 7 từ 6h30 đến 11h30 sáng !"
                    });
                }
            }

            if (startTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return Json(new
                {
                    status = false,
                    mess = "Ngày chủ nhật phòng khám không làm việc ! Vui lòng đặt lịch vào thời gian làm việc trong tuần từ thứ 2 -> thứ 7 !"
                });
            }

            Dictionary<string, List<string>> lstSymtompsFaculty = new Dictionary<string, List<string>>();
            List<string> lstSymptoms = new List<string>();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext(lstSymtompsFaculty, lstSymptoms)))
            {
                int requestNumber = 1;
                // Get Faculty base on Symptoms
                string faculty = "";
                foreach (var entry in lstSymtompsFaculty)
                {
                    if (entry.Value.Contains(symptoms))
                    {
                        faculty = entry.Key;
                    }
                }
                Faculty requestFaculty = workScope.Faculties.GetAll().Where(x => x.Name == faculty).FirstOrDefault();

                // Validate Room ID
                Room room = workScope.Rooms.GetAll().FirstOrDefault(r => r.Id == roomId);
                if (room == null)
                {
                    return Json(new { status = false, mess = "Phòng khám không hợp lệ !" });
                }

                // Giới hạn tối đa số lượt khám cho mỗi phòng
                const int maxCapacityPerRoom = 10;

                // Lấy danh sách bệnh nhân đã đăng ký trong ngày
                var patientRegisterAtDate = workScope.PatientRegisters
                    .Query(x => x.StartTime.HasValue
                        && x.StartTime.Value.Year == startTime.Year
                        && x.StartTime.Value.Month == startTime.Month
                        && x.StartTime.Value.Day == startTime.Day)
                    .ToList();

                // Kiểm tra số lượng bệnh nhân trong phòng
                if (patientRegisterAtDate.Where(x => x.RoomId == room.Id).Count() >= maxCapacityPerRoom)
                {
                    return Json(new { status = false, mess = "Phòng khám đã hết số lượng bệnh nhân trong ngày !" });
                }

                if (patientRegisterAtDate.Where(x => x.RoomId == room.Id).Any())
                {
                    requestNumber = patientRegisterAtDate
                        .Where(x => x.RoomId == room.Id)
                        .Max(x => x.Number) + 1;
                }
                else
                {
                    // Nếu chưa có bệnh nhân nào đăng ký thì số thứ tự là 1
                    requestNumber = 1;
                }

                var register = new PatientRegister
                {
                    RoomId = room.Id,
                    PatientId = user.PatientId.GetValueOrDefault(),
                    StartTime = startTime,
                    Number = requestNumber,
                    //IsRevisit = isRevisit
                };

                workScope.PatientRegisters.Add(register);
                workScope.Complete();

                string message = string.Format("Bạn đã lấy được số thứ tự <b>{0}</b> <br/> Khám tại phòng <b>{1}</b> ( <b>{2}</b> ) <br/> Thời gian: {3}",
                    register.Number,
                    room.Description,  // Using room's description for the message
                    rooms,
                    register.StartTime.Value.ToString("dd/MM/yyyy HH:mm"));

                if (isRevisit)
                {
                    message += ",<br/> Hình thức: Tái khám.";
                }

                return Json(new
                {
                    status = true,
                    mess = message,
                    nextNumber = requestNumber + 1
                });
            }
        }


        private static readonly Dictionary<string, List<string>> mapSymptompsFaculty = new Dictionary<string, List<string>>
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

        // Returns symptoms for the faculty

        public JsonResult GetAllSymptoms()
        {
            var allSymptoms = new List<string>();

            foreach (var faculty in mapSymptompsFaculty)
            {
                allSymptoms.AddRange(faculty.Value);
            }

            return Json(new { status = true, symptoms = allSymptoms.Distinct() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFacultyBySymptom(string symptom)
        {
            foreach (var faculty in mapSymptompsFaculty)
            {
                if (faculty.Value.Contains(symptom))
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        // Lấy thông tin faculty
                        var facultyInfo = workScope.Faculties
                            .GetAll()
                            .FirstOrDefault(f => f.Name == faculty.Key);

                        if (facultyInfo != null)
                        {
                            // Lấy danh sách phòng của khoa
                            var rooms = workScope.Rooms
                                .GetAll()
                                .Where(r => r.FacultyId == facultyInfo.Id)
                                .Select(r => new {
                                    Id = r.Id,
                                    Description = r.Description
                                })
                                .ToList();

                            return Json(new
                            {
                                status = true,
                                faculty = faculty.Key,
                                rooms = rooms
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return Json(new
            {
                status = false,
                message = "Không tìm thấy khoa phù hợp"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Del(int id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.PatientRegisters.Get(id);
                    if (elm != null)
                    {
                        //del
                        workScope.PatientRegisters.Remove(elm);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
        public JsonResult GetPatientFaculty()
        {
            var user = CookiesManage.GetUser();
            var faculties = new List<object>();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                Guid patientId = (Guid)workScope.Accounts.GetAll().FirstOrDefault(a => a.Id == user.Id).PatientId;
                var records = workScope.PatientRecords.GetAll().Where(x => x.PatientId == patientId).ToList();

                foreach (var record in records)
                {
                    var detail = workScope.DetailRecords.GetAll().FirstOrDefault(x => x.RecordId == record.RecordId);
                    if (detail != null)
                    {
                        var faculty = workScope.Faculties.Get(detail.FacultyId);
                        var doctor = workScope.Doctors.Get(detail.DoctorId);
                        if (faculty != null && doctor != null && !faculties.Any(f => f.Equals(faculty.Id)))
                        {
                            faculties.Add(new
                            {
                                FacultyId = faculty.Id,
                                FacultyName = faculty.Name,
                                DoctorId = doctor.Id,
                                DoctorName = doctor.Name
                            });
                        }
                    }
                }
            }

            return Json(new { status = true, faculties = faculties }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoomByDoctor(Guid doctorId)
        {
            var room = new object();
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                room = workScope.Rooms.GetAll().FirstOrDefault(x => x.DoctorId == doctorId);
            }
            return Json(new { status = true, roomPatient = room }, JsonRequestBehavior.AllowGet);
        }

    }
}
