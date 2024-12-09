using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using HospitalManagement.Areas.Admin.Authorization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace HospitalManagement.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Admin)]
    public class PatientRegisterController : BaseController
    {
        private const string KeyElement = "Bốc số";

        // GET: Admin/Event
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var patients = workScope.Patients.GetAll().ToList();
                ViewBag.Patients = new SelectList(patients, "Id", "FullName");

                var rooms = workScope.Rooms.GetAll().ToList();
                ViewBag.Rooms = new SelectList(rooms, "Id", "Description");

                var listData = workScope.PatientRegisters.GetAll()
                    .OrderByDescending(x => x.StartTime)
                    .ToList();

                // Nhóm số lượng theo ngày
                var groupByDate = listData.GroupBy(x => x.StartTime.Value.Date)
                    .Select(x => new
                    {
                        Date = x.Key,
                        Registers = x.ToList(),
                    });
                // Duyệt theo ngày
                foreach (var objDate in groupByDate)
                {
                    // Duyệt qua danh sách đăng ký
                    foreach (var reg in objDate.Registers)
                    {
                        // Lấy tổng số đăng ký trong ngày theo phòng hiện tại
                        var totalReg = objDate.Registers
                            .Where(x => x.RoomId == reg.RoomId)
                            .Count();
                        // Lấy phòng
                        var room = rooms.Where(x => x.Id == reg.RoomId)
                            .FirstOrDefault();
                        // Tính tỷ lệ đăng ký ở phòng hiện tại
                        var p = totalReg / room.Capacity;

                        // Lấy các phòng khác cùng khoa
                        var roomsSimilarInFaculty = rooms
                            .Where(x => x.FacultyId == room.FacultyId && x.Id != room.Id)
                            .ToList();

                        // Duyệt qua danh sách các phòng cùng khoa
                        foreach (var roomSimilar in roomsSimilarInFaculty)
                        {
                            // Tính tổng số đăng ký
                            var totalRegInSimilar = objDate
                                .Registers.Where(x => x.RoomId == roomSimilar.Id)
                                .Count();
                            // Tính tỷ lệ đăng ký
                            var pSimilar = totalRegInSimilar / roomSimilar.Capacity;
                            // Nếu tỷ lệ đăng ký nhỏ hơn thì cho phép chuyển qua
                            if (pSimilar < p)
                            {
                                reg.ChangeRoomId = roomSimilar.Id;
                                reg.ChangeRoomName = roomSimilar.Description;
                                break;
                            } 
                        } 
                    } 
                }

                // Lấy ra danh sách 

                listData = groupByDate
                    .SelectMany(x => x.Registers)
                    .OrderByDescending(x => x.StartTime)
                    .ToList();

                return View(listData);
            }
        }

        [HttpPost]
        public JsonResult GetJson(int id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var PatientRegister = workScope.PatientRegisters.FirstOrDefault(x => x.Id == id);

                return PatientRegister == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            PatientRegister.Id,
                            PatientRegister.PatientId,
                            PatientRegister.RoomId,
                            PatientRegister.StartTime,
                            PatientRegister.Number
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ChangeRoom(PatientRegister input, bool isEdit)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.PatientRegisters.Get(input.Id);

                    if (elm != null) //update
                    {
                        var room = workScope.Rooms.FirstOrDefault(x => x.Id == input.RoomId);

                        if (room == null)
                            return Json(new { status = false, mess = "Không tìm thấy phòng cần chuyển" });

                        int requestNumber = -1;

                        // Lấy số lượng đã đặt

                        var patientRegisterAtDate = workScope.PatientRegisters
                            .Query(x => x.StartTime.HasValue
                                && x.StartTime.Value.Year == elm.StartTime.Value.Year
                                && x.StartTime.Value.Month == elm.StartTime.Value.Month
                                && x.StartTime.Value.Day == elm.StartTime.Value.Day)
                            .ToList();

                        // Nếu số lượng ít hơn số lượng tối đa
                        if (patientRegisterAtDate.Where(x => x.RoomId == room.Id)
                            .Count() < room.Capacity)
                        {
                            if (patientRegisterAtDate
                                .Where(x => x.RoomId == room.Id).Any())
                            {
                                requestNumber = patientRegisterAtDate
                                    .Where(x => x.RoomId == room.Id)
                                    .Max(x => x.Number) + 1;
                            }
                            else
                                requestNumber = 1;

                        }
                        else // Ngược lại tìm các phòng cùng khoa
                        {
                            var roomsInFaculty = workScope.Rooms
                                .Query(x => x.FacultyId == room.FacultyId 
                                && x.Id != elm.RoomId)
                                .ToList();
                            // Duyệt qua danh sách các phòng cùng khoa
                            foreach (Room r in roomsInFaculty)
                            {
                                // Lấy số lượng đã đăng ký
                                if (patientRegisterAtDate.Where(x => x.RoomId == r.Id)
                                    .Count() < r.Capacity)
                                {
                                    if (patientRegisterAtDate
                                        .Where(x => x.RoomId == r.Id).Any())
                                    {
                                        requestNumber = patientRegisterAtDate
                                        .Where(x => x.RoomId == r.Id)
                                        .Max(x => x.Number) + 1;
                                    }
                                    else
                                        requestNumber = 1;

                                    room = r;
                                    break;
                                }
                            }
                        }

                        if (requestNumber == -1)
                        {
                            return Json(new { status = false, mess = "Hết số lượng số đăng ký ở các phòng khám trong ngày ! Vui lòng đặt vào ngày khác !" });
                        }

                        elm.RoomId = room.Id;
                        elm.Number = requestNumber;

                        workScope.PatientRegisters.Put(elm, elm.Id);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhập thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(PatientRegister input, bool isEdit)
        {
            try
            {
                if (isEdit)
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.PatientRegisters.Get(input.Id);

                        if (elm != null) //update
                        {
                            var room = workScope.Rooms.FirstOrDefault(x => x.Id == input.RoomId);

                            if (room == null)
                                return Json(new { status = false, mess = "Không tìm thấy phòng cần đặt " });

                            int requestNumber = -1;

                            // Lấy số lượng đã đặt

                            var patientRegisterAtDate = workScope.PatientRegisters
                                .Query(x => x.StartTime.HasValue
                                    && x.StartTime.Value.Year == input.StartTime.Value.Year
                                    && x.StartTime.Value.Month == input.StartTime.Value.Month
                                    && x.StartTime.Value.Day == input.StartTime.Value.Day)
                                .ToList();

                            // Nếu số lượng ít hơn số lượng tối đa
                            if (patientRegisterAtDate.Where(x => x.RoomId == room.Id).Count() < room.Capacity)
                            {
                                if (patientRegisterAtDate
                                    .Where(x => x.RoomId == room.Id).Any())
                                {
                                    requestNumber = patientRegisterAtDate
                                        .Where(x => x.RoomId == room.Id)
                                        .Max(x => x.Number) + 1;
                                }
                                else
                                    requestNumber = 1;

                            }
                            else // Ngược lại tìm các phòng cùng khoa
                            {
                                var roomsInFaculty = workScope.Rooms
                                    .Query(x => x.FacultyId == room.FacultyId
                                    && x.Id != elm.RoomId);

                                // Duyệt qua danh sách các phòng cùng khoa
                                foreach (Room r in roomsInFaculty)
                                {
                                    // Lấy số lượng đã đăng ký
                                    if (patientRegisterAtDate.Where(x => x.RoomId == r.Id).Count() < r.Capacity)
                                    {
                                        if (patientRegisterAtDate
                                            .Where(x => x.RoomId == r.Id).Any())
                                        {
                                            requestNumber = patientRegisterAtDate
                                            .Where(x => x.RoomId == r.Id)
                                            .Max(x => x.Number) + 1;
                                        }
                                        else
                                            requestNumber = 1;

                                        room = r;
                                        break;
                                    }
                                }
                            }

                            if (requestNumber == -1)
                            {
                                return Json(new { status = false, mess = "Hết số lượng số đăng ký ở các phòng khám trong ngày ! Vui lòng đặt vào ngày khác !" });
                            }

                            input.RoomId = room.Id;
                            input.Number = requestNumber;

                            elm = input;

                            workScope.PatientRegisters.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        input.Id = 0;
                        workScope.PatientRegisters.Add(input);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
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
                        return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
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