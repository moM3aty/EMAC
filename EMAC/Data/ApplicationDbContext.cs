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

        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<WorkshopTicket> WorkshopTickets { get; set; }
        public DbSet<InitialReport> InitialReports { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<Technician>().HasData(
                new Technician { Id = 1, FullName = "أحمد علي", Specialty = "ac_maint", CoveredRegions = "hassa,dammam" },
                new Technician { Id = 2, FullName = "محمد سامي", Specialty = "fridge_repair", CoveredRegions = "riyadh,hassa" },
                new Technician { Id = 3, FullName = "فريق المشاريع (أ)", Specialty = "central_maint", CoveredRegions = "all" },
                new Technician { Id = 4, FullName = "فني عام (للتجربة)", Specialty = "General", CoveredRegions = "hassa,dammam,riyadh,other,all" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "مكيفات", ImageUrl = "ac_cat.png" },
                new Category { Id = 2, Name = "أجهزة مطبخ", ImageUrl = "kitchen_cat.png" },
                new Category { Id = 3, Name = "غسالات ومجففات", ImageUrl = "wash_cat.png" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 4, Name = "مكيفات سبليت", ParentId = 1 },
                new Category { Id = 5, Name = "مكيفات شباك", ParentId = 1 },
                new Category { Id = 6, Name = "مكيفات دولابي", ParentId = 1 },
                new Category { Id = 7, Name = "ثلاجات", ParentId = 2 },
                new Category { Id = 8, Name = "أفران", ParentId = 2 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "مكيف جري سبليت 18000 وحدة - بارد",
                    Description = "مكيف عالي الكفاءة، توزيع هواء رباعي، موفر للطاقة.",
                    Price = 2100,
                    OldPrice = 2450,
                    Brand = "Gree",
                    SizeOrCapacity = "18000 BTU",
                    CategoryId = 4,
                    ImageUrl = "gree_split.png",
                    IsFeatured = true
                },
                new Product
                {
                    Id = 2,
                    Name = "مكيف إل جي شباك 24000 وحدة",
                    Description = "كمبروسر قوي، تبريد سريع، ضمان 7 سنوات.",
                    Price = 1850,
                    OldPrice = 2000,
                    Brand = "LG",
                    SizeOrCapacity = "24000 BTU",
                    CategoryId = 5,
                    ImageUrl = "lg_window.png"
                },
                new Product
                {
                    Id = 3,
                    Name = "حلة ضغط كهربائية رويال 10 لتر",
                    Description = "12 برنامج للطهي، وعاء غير لاصق، مؤقت ذكي.",
                    Price = 229,
                    OldPrice = 299,
                    Brand = "Royal",
                    SizeOrCapacity = "10 Liter",
                    CategoryId = 8, 
                    ImageUrl = "royal_pot.png",
                    IsFeatured = true
                }
            );
        }
    }
}