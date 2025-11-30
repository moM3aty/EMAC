using Microsoft.EntityFrameworkCore;
using EMAC.Models;
using System;

namespace EMAC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // الجداول الأساسية
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Technician> Technicians { get; set; }

        // جدول الخدمات (إدارة الخدمات)
        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        // جداول التواصل
        public DbSet<ContactMessage> ContactMessages { get; set; }

        // جداول الورشة والفواتير
        public DbSet<WorkshopTicket> WorkshopTickets { get; set; }
        public DbSet<InitialReport> InitialReports { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. بيانات الخدمات الافتراضية
            modelBuilder.Entity<ServiceCategory>().HasData(
                new ServiceCategory { Id = 1, Code = "ac_maint", ArabicName = "صيانة مكيفات سبليت/شباك", Description = "غسيل وتنظيف وإصلاح أعطال" },
                new ServiceCategory { Id = 2, Code = "central_maint", ArabicName = "صيانة تكييف مركزي", Description = "للمجمعات والفلل" },
                new ServiceCategory { Id = 3, Code = "pipe_extension", ArabicName = "تمديد مواسير نحاس", Description = "تأسيس وتمديد" },
                new ServiceCategory { Id = 4, Code = "fridge_repair", ArabicName = "صيانة ثلاجات ومجمدات", Description = "إصلاح منزلي وتجاري" },
                new ServiceCategory { Id = 5, Code = "wash_repair", ArabicName = "صيانة غسالات", Description = "أوتوماتيك وعادي" },
                new ServiceCategory { Id = 6, Code = "oven_repair", ArabicName = "صيانة أفران وميكروويف", Description = "كهرباء وغاز" },
                new ServiceCategory { Id = 7, Code = "fridge_room_install", ArabicName = "تركيب/صيانة غرف تبريد", Description = "مخازن التبريد" },
                new ServiceCategory { Id = 8, Code = "car_ac", ArabicName = "صيانة تكييف سيارات مبردة", Description = "شاحنات النقل المبرد" }
            );

            // 2. بيانات الفنيين (تم تحديثها لإضافة فني عام يغطي كل شيء)
            modelBuilder.Entity<Technician>().HasData(
                new Technician { Id = 1, FullName = "أحمد علي", Specialty = "ac_maint", CoveredRegions = "hassa,dammam" },
                new Technician { Id = 2, FullName = "محمد سامي", Specialty = "fridge_repair", CoveredRegions = "riyadh,hassa" },
                new Technician { Id = 3, FullName = "فريق المشاريع (أ)", Specialty = "central_maint", CoveredRegions = "all" },
                // --- إضافة جديدة: فني جوكر للتجربة ---
                new Technician { Id = 4, FullName = "فني عام (للتجربة)", Specialty = "General", CoveredRegions = "hassa,dammam,riyadh,other,all" }
            );

            // 3. بيانات تجريبية لطلبات الصيانة
            modelBuilder.Entity<ServiceRequest>().HasData(
                new ServiceRequest
                {
                    Id = 1,
                    CustomerName = "عبدالله محمد",
                    PhoneNumber = "0501234567",
                    ServiceType = "ac_maint",
                    Location = "hassa",
                    AppointmentDate = DateTime.Now.AddDays(-2),
                    TimeSlot = "09:00 ص",
                    ProblemDescription = "المكيف لا يبرد ويصدر صوت عالي",
                    RequestNumber = "REQ-20251120-1001",
                    DeviceCode = "DEV-AC-101",
                    Status = "InWorkshop",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new ServiceRequest
                {
                    Id = 2,
                    CustomerName = "شركة النقل السريع",
                    PhoneNumber = "0559876543",
                    ServiceType = "car_ac",
                    Location = "dammam",
                    AppointmentDate = DateTime.Now.AddDays(-1),
                    TimeSlot = "02:00 م",
                    ProblemDescription = "صيانة دورية لأسطول الشاحنات المبردة",
                    RequestNumber = "REQ-20251121-2005",
                    DeviceCode = "DEV-CA-205",
                    Status = "Confirmed",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new ServiceRequest
                {
                    Id = 3,
                    CustomerName = "سارة أحمد",
                    PhoneNumber = "0561112233",
                    ServiceType = "fridge_repair",
                    Location = "riyadh",
                    AppointmentDate = DateTime.Now.AddDays(-5),
                    TimeSlot = "10:00 ص",
                    ProblemDescription = "الثلاجة لا تعمل نهائياً",
                    RequestNumber = "REQ-20251115-3040",
                    DeviceCode = "DEV-FR-304",
                    Status = "Closed",
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            );

            // 4. بيانات تذاكر الورشة
            modelBuilder.Entity<WorkshopTicket>().HasData(
                new WorkshopTicket
                {
                    Id = 1,
                    ServiceRequestId = 1,
                    DeviceModel = "مكيف سبليت جنرال 24 وحدة",
                    SerialNumber = "SN-99887766",
                    DeviceCode = "WS-1122-5544",
                    PhysicalCondition = "خدوش بسيطة في الغطاء الخارجي",
                    Accessories = "ريموت كنترول",
                    ReceivedAt = DateTime.Now.AddDays(-2),
                    Status = "Inspected"
                },
                new WorkshopTicket
                {
                    Id = 2,
                    ServiceRequestId = 3,
                    DeviceModel = "ثلاجة سامسونج 20 قدم",
                    SerialNumber = "SN-11223344",
                    DeviceCode = "WS-1115-3322",
                    PhysicalCondition = "سليمة",
                    Accessories = "بدون",
                    ReceivedAt = DateTime.Now.AddDays(-5),
                    Status = "Closed"
                }
            );

            // 5. تقارير الفحص
            modelBuilder.Entity<InitialReport>().HasData(
                new InitialReport
                {
                    Id = 1,
                    WorkshopTicketId = 1,
                    FaultDescription = "تلف في الكومبريسور ونقص فريون",
                    RequiredSpareParts = "كومبريسور جديد، غاز فريون",
                    EstimatedCost = 850,
                    EstimatedTime = "3 أيام",
                    TechnicianNotes = "يحتاج تنظيف شامل للوحدة الداخلية",
                    CustomerApproval = false,
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new InitialReport
                {
                    Id = 2,
                    WorkshopTicketId = 2,
                    FaultDescription = "عطل في الثرموستات",
                    RequiredSpareParts = "ثرموستات أصلي",
                    EstimatedCost = 350,
                    EstimatedTime = "يوم واحد",
                    TechnicianNotes = "تم الاختبار بنجاح",
                    CustomerApproval = true,
                    CreatedAt = DateTime.Now.AddDays(-4)
                }
            );

            // 6. الفواتير
            modelBuilder.Entity<Invoice>().HasData(
                new Invoice
                {
                    Id = 1,
                    WorkshopTicketId = 2,
                    LaborCost = 150,
                    SparePartsCost = 200,
                    TaxAmount = 52.5m,
                    TotalAmount = 402.5m,
                    IsPaid = true,
                    PaymentMethod = "CreditCard",
                    IssuedAt = DateTime.Now.AddDays(-3)
                }
            );
        }
    }
}