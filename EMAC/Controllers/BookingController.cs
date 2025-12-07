using Microsoft.AspNetCore.Mvc;
using EMAC.Data;
using EMAC.Models;
using EMAC.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EMAC.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public BookingController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetAvailableSlots(string serviceType, string location, DateTime date)
        {
            if (date < DateTime.Today) return Json(new { success = false, message = "التاريخ غير صالح" });

            var potentialTechs = _context.Technicians
                .AsEnumerable()
                .Where(t => t.IsAvailable(location, serviceType, date))
                .ToList();

            if (!potentialTechs.Any())
            {
                return Json(new { success = true, slots = new List<string>(), message = "لا يوجد فني متاح في منطقتك حالياً لهذه الخدمة." });
            }

            var allSlots = new List<string> { "08:00 ص", "09:00 ص", "10:00 ص", "11:00 ص", "01:00 م", "02:00 م", "03:00 م", "04:00 م", "05:00 م" };
            var availableSlots = new List<string>();

            foreach (var slot in allSlots)
            {
                bool isAnyTechFree = potentialTechs.Any(tech =>
                    !_context.ServiceRequests.Any(r =>
                        r.TechnicianId == tech.Id &&
                        r.AppointmentDate.Date == date.Date &&
                        r.TimeSlot == slot
                    )
                );

                if (isAnyTechFree) availableSlots.Add(slot);
            }

            if (!availableSlots.Any())
            {
                return Json(new { success = true, slots = new List<string>(), message = "جميع المواعيد محجوزة لهذا اليوم." });
            }

            return Json(new { success = true, slots = availableSlots });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBooking([FromBody] ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = $"بيانات غير مكتملة:\n{errors}" });
            }

            try
            {
                var suitableTechs = _context.Technicians
                    .AsEnumerable()
                    .Where(t => t.IsAvailable(request.Location, request.ServiceType, request.AppointmentDate))
                    .ToList();

                Technician selectedTech = null;

                foreach (var tech in suitableTechs)
                {
                    bool isBusy = _context.ServiceRequests.Any(r =>
                        r.TechnicianId == tech.Id &&
                        r.AppointmentDate.Date == request.AppointmentDate.Date &&
                        r.TimeSlot == request.TimeSlot);

                    if (!isBusy)
                    {
                        selectedTech = tech;
                        break;
                    }
                }

                if (selectedTech == null)
                {
                    return Json(new { success = false, message = "عذراً، الموعد المختار لم يعد متاحاً." });
                }

                request.TechnicianId = selectedTech.Id;
                request.GenerateIdentifiers();
                request.Status = "Confirmed";
                request.CreatedAt = DateTime.Now;

                _context.ServiceRequests.Add(request);
                await _context.SaveChangesAsync();



                string adminNumber = "966564133825"; 

                string message = $"⚠️ *طلب صيانة جديد (تأكيد العميل)*\n" +
                                 $"رقم الطلب: {request.RequestNumber}\n" +
                                 $"العميل: {request.CustomerName}\n" +
                                 $"الخدمة: {request.ServiceType}\n" +
                                 $"الموعد: {request.AppointmentDate.ToShortDateString()} | {request.TimeSlot}\n" +
                                 $"الموقع: {request.Location}\n" +
                                 $"الفني المعين: {selectedTech.FullName}\n" +
                                 $"الوصف: {request.ProblemDescription}\n\n" +
                                 $"يرجى تأكيد الاستلام.";

                string whatsappUrl = $"https://wa.me/{adminNumber}?text={System.Net.WebUtility.UrlEncode(message)}";

                return Json(new
                {
                    success = true,
                    requestNumber = request.RequestNumber,
                    deviceCode = request.DeviceCode,
                    whatsappUrl = whatsappUrl, 
                    message = "تم تسجيل الحجز. سيتم توجيهك للواتساب لإرسال التفاصيل للإدارة."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ: " + ex.Message });
            }
        }
    }
}