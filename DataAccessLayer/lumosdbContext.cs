using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BussinessObject
{
    public partial class lumosdbContext : DbContext
    {
        public lumosdbContext()
        {
        }

        public lumosdbContext(DbContextOptions<lumosdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<AdminConfiguration> AdminConfigurations { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<FavoritePartner> FavoritePartners { get; set; }
        public virtual DbSet<HistoryLog> HistoryLogs { get; set; }
        public virtual DbSet<MedicalReport> MedicalReports { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerService> PartnerServices { get; set; }
        public virtual DbSet<PartnerType> PartnerTypes { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ServiceBooking> ServiceBookings { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<ServiceDetail> ServiceDetails { get; set; }
        public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; }

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
            return config["ConnectionStrings:DefaultConnection"];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressId).ValueGeneratedNever();

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .HasColumnName("Address");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_Address_ReportId");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.AdminId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AdminConfiguration>(entity =>
            {
                entity.HasKey(e => e.AdminConfigId)
                    .HasName("PK__AdminCon__706D42E3A7D00A16");

                entity.ToTable("AdminConfiguration");

                entity.Property(e => e.AdminConfigId).ValueGeneratedNever();

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdminConfigurations)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK_AdminConfiguration_Admin");

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.AdminConfigurations)
                    .HasForeignKey(d => d.ConfigId)
                    .HasConstraintName("FK_AdminConfiguration_ConfigId");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).ValueGeneratedNever();

                entity.Property(e => e.BookingDate).HasColumnType("date");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FeedbackImage).HasMaxLength(255);

                entity.Property(e => e.FeedbackLumos).HasMaxLength(200);

                entity.Property(e => e.FeedbackPartner).HasMaxLength(200);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Booking_PaymentMethod");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_Booking_ReportId");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname).HasMaxLength(100);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FavoritePartner>(entity =>
            {
                entity.HasKey(e => e.FavoriteId)
                    .HasName("PK__Favorite__CE74FAD547AAF3B5");

                entity.ToTable("FavoritePartner");

                entity.Property(e => e.FavoriteId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).HasMaxLength(100);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.FavoritePartners)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_FavoritePartner_CustomerId");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.FavoritePartners)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_FavoritePartner_Partner");
            });

            modelBuilder.Entity<HistoryLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK__HistoryL__5E5486486B061BED");

                entity.ToTable("HistoryLog");

                entity.Property(e => e.LogId).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Message).HasColumnType("text");
            });

            modelBuilder.Entity<MedicalReport>(entity =>
            {
                entity.HasKey(e => e.ReportId)
                    .HasName("PK__MedicalR__D5BD4805031A3938");

                entity.ToTable("MedicalReport");

                entity.Property(e => e.ReportId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname).HasMaxLength(20);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("text");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.MedicalReports)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_MedicalReport_Customer");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("Partner");

                entity.Property(e => e.PartnerId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DisplayName).HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.PartnerName).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PractiscingCertificate).HasMaxLength(20);

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PartnerService>(entity =>
            {
                entity.HasKey(e => e.ServiceId)
                    .HasName("PK__PartnerS__C51BB00A54FA255E");

                entity.ToTable("PartnerService");

                entity.Property(e => e.ServiceId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.PartnerServices)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_PartnerService_BookingId");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.PartnerServices)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_PartnerService_Partner");
            });

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.ToTable("PartnerType");

                entity.Property(e => e.PartnertypeId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.PartnerTypes)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_PartnerType_Partner");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                    .HasName("PK__PaymentM__9B556A38D5F383D8");

                entity.ToTable("PaymentMethod");

                entity.Property(e => e.PaymentId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.Property(e => e.ScheduleId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("text");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_Schedule_PartnerId");
            });

            modelBuilder.Entity<ServiceBooking>(entity =>
            {
                entity.ToTable("ServiceBooking");

                entity.Property(e => e.ServiceBookingId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PartnerService)
                    .WithMany(p => p.ServiceBookings)
                    .HasForeignKey(d => d.PartnerServiceId)
                    .HasConstraintName("FK_ServiceBooking_PartnerServiceId");
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__ServiceC__19093A0B9F5A22DE");

                entity.ToTable("ServiceCategory");

                entity.Property(e => e.CategoryId).ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ImgUrl)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK__ServiceD__135C316D46D4E55F");

                entity.ToTable("ServiceDetail");

                entity.Property(e => e.DetailId).ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ServiceDetails)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_ServiceDetail_CategoryId");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceDetails)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_ServiceDetail_ServiceId");
            });

            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.HasKey(e => e.ConfigId)
                    .HasName("PK__SystemCo__C3BC335CD3C5DF24");

                entity.ToTable("SystemConfiguration");

                entity.Property(e => e.ConfigId).ValueGeneratedNever();

                entity.Property(e => e.Config).HasColumnType("text");

                entity.Property(e => e.Field).HasColumnType("text");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
