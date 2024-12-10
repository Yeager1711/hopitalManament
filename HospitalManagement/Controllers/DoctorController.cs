using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HospitalManagement.Handler;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace HospitalManagement.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Doctor
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search(string query, int? page, List<bool> genders, List<Guid> facultiesSelected)
        {
            ViewBag.Host = (Request.Url == null ? "" : Request.Url.Host);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            if (query == "")
            {
                query = null;
            }

            ViewBag.QueryData = query;
            var pageNumber = (page ?? 1);
            const int pageSize = 5;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctors = workScope.Doctors
                    .IncludeFilter(x => x.DoctorSchedules.Where(c => c.Status == 101))
                    .Where(x => !x.IsDelete)
                    .OrderBy(x => x.DoctorSchedules.Count)
                    .ToList();

                var faculties = workScope.Faculties.GetAll().ToList();
                ViewBag.Faculties = faculties;

                double elapsedMs = 0;

                var doctorsWithInfors = (
                    from mt in doctors
                    join f in faculties on mt.FacultyId equals f.Id

                    select mt).AsQueryable();

                if (!string.IsNullOrEmpty(query))
                    doctorsWithInfors = doctorsWithInfors
                                    .Where(x => x.Name.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Descriptions) && x.Descriptions.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Faculty.Name) && x.Faculty.Name.ToLower().Contains(query.Trim().ToLower())

                    );

                if (facultiesSelected != null && facultiesSelected.Count > 0)
                    doctorsWithInfors = doctorsWithInfors
                        .Where(x => facultiesSelected.Contains(x.Faculty.Id));
                
                if (genders != null && genders.Count > 0)
                    doctorsWithInfors = doctorsWithInfors
                        .Where(x => genders != null && genders.Contains(x.Gender));

                
                ViewBag.Total = doctorsWithInfors.Count();
                ViewBag.FacultiesSelected = facultiesSelected;
                ViewBag.Genders = genders;
                watch.Stop();

                elapsedMs = (double)watch.ElapsedMilliseconds / 1000;
                ViewBag.RequestTime = elapsedMs;
                return View(doctorsWithInfors.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Detail(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.Include(x => x.Faculty).FirstOrDefault(x => x.Id == id);
                if (doctor != null)
                {
                    return View(doctor);
                }
                else
                {
                    return RedirectToAction("Search");
                }
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == id && !x.IsDelete);

                return doctor == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công ",
                        data = new
                        {
                            doctor.Id,
                            doctor.Name,
                            doctor.Address,
                            doctor.Email,
                            doctor.Phone,
                            doctor.FacultyId,
                            doctor.Gender,
                            doctor.Avatar,
                            doctor.Descriptions
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Book(Guid doctorId, DateTime time)
        {
            var user = CookiesManage.GetUser();

            if (user == null)
            {
                return Json(new { status = false, mess = "Vui lòng đăng nhập" });
            }

            //if (time.TimeOfDay < new TimeSpan(6, 30, 0) || time.TimeOfDay > new TimeSpan(17, 30, 0))
            //{
            //    return Json(new
            //    {
            //        status = false,
            //        mess = "Chỉ được phép đặt lịch khám trong giờ làm việc từ 6h30 sáng đến 5h30 chiều !"
            //    });
            //} 

            if (time.DayOfWeek == DayOfWeek.Saturday)
            {
                if (time.TimeOfDay < new TimeSpan(6, 30, 0) || time.TimeOfDay > new TimeSpan(11, 30, 0))
                {
                    return Json(new
                    {
                        status = false,
                        mess = "Chỉ được đặt lịch khám ngày thứ 7 từ 6h30 đến 11h30 sáng !"
                    });
                }
            }

            if (time.DayOfWeek == DayOfWeek.Sunday)
            {
                return Json(new
                {
                    status = false,
                    mess = "Ngày chủ nhật phòng khám không làm việc ! Vui lòng đặt lịch vào thời gian làm việc trong tuần từ thứ 2 -> thứ 7 !"
                });
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var doctor = workScope.Doctors.FirstOrDefault(x => x.Id == doctorId && !x.IsDelete);
                
                if (doctor == null)
                    return Json(new { status = true, mess = "Cập nhập thành công " });

                // Kiểm tra trùng lịch khám
                // Lấy danh sách đăng ký khám của bác sỹ vào ngày được chọn
                
                var schedules = workScope.DoctorSchedules
                    .Find(x => x.DoctorId == doctorId
                      && time >= x.ScheduleBook && time <= EntityFunctions.AddMinutes(x.ScheduleBook, 30)
                      && x.Status == (int) BookingStatusKey.Pending) // Trạng thái là đang đợi
                    .ToList();

                // Thời gian được chọn nằm trong khoảng 30 phút khám của bác sỹ
                if (schedules.Any())
                {
                    var scheduleStartTime = schedules.First().ScheduleBook;
                    var scheduleEndTime = scheduleStartTime.AddMinutes(30);

                    return Json(new
                    {
                        status = false,
                        mess = string.Format("Thời gian đặt trùng với thời gian đặt lịch bệnh nhân khác từ {0} đến {1} ! Vui lòng chọn thời gian đặt khác !", scheduleStartTime.ToString("HH:mm"), scheduleEndTime.ToString("HH:mm"))
                    });
                } 

                workScope.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctor.Id,
                    PatientId = user.PatientId.GetValueOrDefault(),
                    ScheduleBook = time,
                    Status = BookingStatusKey.Pending
                });
                workScope.Complete();

                var patientDoctor = workScope.PatientDoctors.FirstOrDefault(x =>
                    x.DoctorId == doctor.Id && x.PatientId == user.PatientId);

                if (patientDoctor != null)
                    return Json(new { status = true, mess = "Cập nhập thành công " });

                workScope.PatientDoctors.Add(new PatientDoctor
                {
                    PatientId = user.PatientId.GetValueOrDefault(),
                    DoctorId = doctor.Id,
                    Status = 1
                });
                workScope.Complete();

                return Json(new { status = true, mess = "Cập nhập thành công " });
            }
        }
    }
}