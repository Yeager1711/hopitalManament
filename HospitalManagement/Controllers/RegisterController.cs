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
        public JsonResult pick(DateTime startTime, bool isRevisit) 
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

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                int requestNumber = 1;
                Room room = null;

                // Giới hạn tối đa số lượt khám cho mỗi phòng
                const int maxCapacityPerRoom = 10;

                // Lấy danh sách tất cả các phòng
                var roomsInFaculties = workScope.Rooms.GetAll().ToList();

                //var roomsWithFacultyName = workScope.Rooms
                //.Include(r => r.Faculty)
                //.Select(r => new
                //{
                //    RoomId = r.Id,
                //    RoomDescription = r.Description,
                //    FacultyName = r.Faculty.Name
                //})
                //.ToList();

                // Lấy danh sách bệnh nhân đã đăng ký trong ngày
                var patientRegisterAtDate = workScope.PatientRegisters
                    .Query(x => x.StartTime.HasValue
                        && x.StartTime.Value.Year == startTime.Year
                        && x.StartTime.Value.Month == startTime.Month
                        && x.StartTime.Value.Day == startTime.Day)
                    .ToList();

                // Kiểm tra từng phòng
                foreach (var r in roomsInFaculties)
                {
                    if (patientRegisterAtDate.Where(x => x.RoomId == r.Id).Count() < maxCapacityPerRoom)
                    {
                        room = r;

                        if (patientRegisterAtDate.Where(x => x.RoomId == r.Id).Any())
                        {
                            requestNumber = patientRegisterAtDate
                                .Where(x => x.RoomId == r.Id)
                                .Max(x => x.Number) + 1;
                        }
                        else
                        {
                            // Nếu chưa có bệnh nhân nào đăng ký thì số thứ tự là 1
                            requestNumber = 1;
                        }

                        break;
                    }
                }

                if (requestNumber == -1)
                {
                    return Json(new { status = false, mess = "Hết số lượng số đăng ký ở các phòng khám trong ngày ! Vui lòng đặt vào ngày khác !" });
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

                string message = string.Format("Bạn đã lấy được số thứ tự <b>{0}</b>, khám tại phòng <b>{1}</b>, thời gian {2}",
                    register.Number,
                    room.Description,
                    register.StartTime.Value.ToString("dd/MM/yyyy HH:mm"));

                if (isRevisit)  
                {
                    message += ", Hình thức: Tái khám";
                }

                return Json(new
                {
                    status = true,
                    mess = message,
                    nextNumber = requestNumber + 1
                });
            }
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
    }
}
