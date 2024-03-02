using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BussinessObject
{
    public partial class LumosDBContext : DbContext
    {
        public LumosDBContext()
        {
        }

        public LumosDBContext(DbContextOptions<LumosDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<AdminConfiguration> AdminConfigurations { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<BookingDetail> BookingDetails { get; set; } = null!;
        public virtual DbSet<BookingLog> BookingLogs { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<FavoritePartner> FavoritePartners { get; set; } = null!;
        public virtual DbSet<HistoryLog> HistoryLogs { get; set; } = null!;
        public virtual DbSet<MedicalReport> MedicalReports { get; set; } = null!;
        public virtual DbSet<Partner> Partners { get; set; } = null!;
        public virtual DbSet<PartnerService> PartnerServices { get; set; } = null!;
        public virtual DbSet<PartnerType> PartnerTypes { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<ServiceBooking> ServiceBookings { get; set; } = null!;
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public virtual DbSet<ServiceDetail> ServiceDetails { get; set; } = null!;
        public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionStrings());
            }
        }

        private string GetConnectionStrings()
        {
            IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
            return config["ConnectionStrings:DB"];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressId).ValueGeneratedOnAdd();

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .HasColumnName("Address");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Address_Customer");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.AdminId).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.ImgUrl).IsUnicode(true);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(true);

                entity.Property(e => e.RefreshToken).IsUnicode(true);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<AdminConfiguration>(entity =>
            {
                entity.HasKey(e => e.AdminConfigId)
                    .HasName("PK__AdminCon__706D42E3CD05B80F");

                entity.ToTable("AdminConfiguration");

                entity.Property(e => e.AdminConfigId).ValueGeneratedOnAdd();

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdminConfigurations)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK_AdminConfiguration_Admin");

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.AdminConfigurations)
                    .HasForeignKey(d => d.ConfigId)
                    .HasConstraintName("FK_AdminConfiguration_SystemConfiguration");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).ValueGeneratedOnAdd();

                entity.Property(e => e.Address).HasMaxLength(100);
                entity.Property(e => e.Rating).HasColumnType("decimal(3,1");

                entity.Property(e => e.BookingDate).HasColumnType("date");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
                entity.Property(e => e.PaymentLinkId)
                    .HasMaxLength(50)
                    .IsUnicode(true);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FeedbackImage)
                    .HasMaxLength(1024)
                    .IsUnicode(true);

                entity.Property(e => e.FeedbackLumos).HasMaxLength(200);

                entity.Property(e => e.FeedbackPartner).HasMaxLength(200);
                entity.Property(e => e.bookingTime).HasColumnType("int").IsRequired();
                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Booking_PaymentMethod");
            });

            modelBuilder.Entity<BookingDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK__BookingD__135C316D8882B929");

                entity.ToTable("BookingDetail");

                entity.Property(e => e.DetailId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("text");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_BookingDetail_Booking");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_BookingDetail_MedicalReport");
            });

            modelBuilder.Entity<BookingLog>(entity =>
            {
                entity.ToTable("BookingLog");

                entity.Property(e => e.BookingLogId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("text");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingLogs)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_BookingLog_Booking");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.Fullname).HasMaxLength(100);

                entity.Property(e => e.ImgUrl).IsUnicode(true);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(true);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(true);

                entity.Property(e => e.RefreshToken).IsUnicode(true);

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<FavoritePartner>(entity =>
            {
                entity.HasKey(e => e.FavoriteId)
                    .HasName("PK__Favorite__CE74FAD5DADCCCB8");

                entity.ToTable("FavoritePartner");

                entity.Property(e => e.FavoriteId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).HasMaxLength(100);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.FavoritePartners)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_FavoritePartner_Customer");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.FavoritePartners)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_FavoritePartner_Partner");
            });

            modelBuilder.Entity<HistoryLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK__HistoryL__5E5486487A9BEB7C");

                entity.ToTable("HistoryLog");

                entity.Property(e => e.LogId).ValueGeneratedOnAdd();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Message).HasColumnType("text");
            });

            modelBuilder.Entity<MedicalReport>(entity =>
            {
                entity.HasKey(e => e.ReportId)
                    .HasName("PK__MedicalR__D5BD48056690407F");

                entity.ToTable("MedicalReport");

                entity.Property(e => e.ReportId).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Fullname).HasMaxLength(20);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("text");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(true);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.MedicalReports)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_MedicalReport_Customer");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("Partner");

                entity.Property(e => e.PartnerId).ValueGeneratedOnAdd();

                entity.Property(e => e.Address).HasMaxLength(255).IsRequired();

                entity.Property(e => e.BusinessLicenseNumber).IsUnicode(true).IsRequired();

                entity.Property(e => e.Code).HasMaxLength(50).IsUnicode(true).IsRequired();
                entity.Property(e => e.Rating).HasColumnType("decimal(3,1").IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();

                entity.Property(e => e.Email).HasMaxLength(50).IsUnicode(true).IsRequired();

                entity.Property(e => e.ImgUrl).IsUnicode(true);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.PartnerName).HasMaxLength(100).IsRequired();

                entity.Property(e => e.Password).HasMaxLength(100).IsUnicode(false).IsRequired();

                entity.Property(e => e.Phone)
                    .HasMaxLength(13)
                    .IsUnicode(true)
                    .IsRequired();

                entity.Property(e => e.RefreshToken).IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Partners)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Partner_PartnerType");
            });

            modelBuilder.Entity<PartnerService>(entity =>
            {
                entity.HasKey(e => e.ServiceId)
                    .HasName("PK__PartnerS__C51BB00A0405CF5F");

                entity.ToTable("PartnerService");

                entity.Property(e => e.ServiceId).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(300)
                    .IsUnicode(true);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.Rating)
                .HasColumnType("decimal(3,1)")
                .HasPrecision(3, 1);

                entity.Property(e => e.Price).HasColumnType("int").IsRequired();


                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.PartnerServices)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_PartnerService_Partner");
            });

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__PartnerT__516F03B5C3CF9D1C");

                entity.ToTable("PartnerType");

                entity.Property(e => e.TypeId).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                    .HasName("PK__PaymentM__9B556A38D51AF085");

                entity.ToTable("PaymentMethod");

                entity.Property(e => e.PaymentId).ValueGeneratedOnAdd();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.Property(e => e.ScheduleId).ValueGeneratedOnAdd();

                entity.Property(e => e.From).HasColumnType("time(0)");
                entity.Property(e => e.To).HasColumnType("time(0)");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedBy)
                        .HasMaxLength(50)
                        .IsUnicode(true);

                entity.Property(e => e.WorkShift).HasColumnType("int").IsRequired();

                entity.Property(e => e.DayOfWeek).HasColumnType("int").IsRequired();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasMaxLength(100)
                    .IsUnicode(true);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_Schedule_Partner")
                    .IsRequired();
            });

            modelBuilder.Entity<ServiceBooking>(entity =>
            {
                entity.ToTable("ServiceBooking");

                entity.Property(e => e.ServiceBookingId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Detail)
                    .WithMany(p => p.ServiceBookings)
                    .HasForeignKey(d => d.DetailId)
                    .HasConstraintName("FK_ServiceBooking_BookingDetail");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceBookings)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_ServiceBooking_PartnerService");
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__ServiceC__19093A0B211A2729");

                entity.ToTable("ServiceCategory");

                entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<ServiceDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK__ServiceD__135C316D3FBC6307");

                entity.ToTable("ServiceDetail");

                entity.Property(e => e.DetailId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ServiceDetails)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_ServiceDetail_ServiceCategory");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceDetails)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_ServiceDetail_PartnerService");
            });

            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.HasKey(e => e.ConfigId)
                    .HasName("PK__SystemCo__C3BC335C7A0CB385");

                entity.ToTable("SystemConfiguration");

                entity.Property(e => e.ConfigId).ValueGeneratedOnAdd();

                entity.Property(e => e.Config).HasColumnType("text");

                entity.Property(e => e.Field).HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
