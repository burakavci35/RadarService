using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RadarService.Entities.Models;

public partial class RadarDbContext : DbContext
{
    public RadarDbContext()
    {
    }

    public RadarDbContext(DbContextOptions<RadarDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceLog> DeviceLogs { get; set; }

    public virtual DbSet<DeviceRequest> DeviceRequests { get; set; }

    public virtual DbSet<DeviceScheduler> DeviceSchedulers { get; set; }

    public virtual DbSet<FormParameter> FormParameters { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<ResponseCondition> ResponseConditions { get; set; }

    public virtual DbSet<Scheduler> Schedulers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=localhost\\SQLExpress;Database=RadarDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("Device");

            entity.Property(e => e.BaseAddress).HasMaxLength(100);
            entity.Property(e => e.LastUpdateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Location).WithMany(p => p.Devices)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Device_Location");
        });

        modelBuilder.Entity<DeviceLog>(entity =>
        {
            entity.ToTable("DeviceLog");

            entity.Property(e => e.LogDateTime).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceLogs)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceLog_Device");
        });

        modelBuilder.Entity<DeviceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_DeviceCommand");

            entity.ToTable("DeviceRequest");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceRequests)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceCommand_Device");

            entity.HasOne(d => d.Request).WithMany(p => p.DeviceRequests)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceRequest_Request");
        });

        modelBuilder.Entity<DeviceScheduler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_DeviceScheduler_1");

            entity.ToTable("DeviceScheduler");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceSchedulers)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceScheduler_Device");

            entity.HasOne(d => d.Scheduler).WithMany(p => p.DeviceSchedulers)
                .HasForeignKey(d => d.SchedulerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceScheduler_Scheduler");
        });

        modelBuilder.Entity<FormParameter>(entity =>
        {
            entity.ToTable("FormParameter");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Value).HasMaxLength(255);

            entity.HasOne(d => d.Request).WithMany(p => p.FormParameters)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormParameter_Request");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Request");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(50);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Request_Request");
        });

        modelBuilder.Entity<ResponseCondition>(entity =>
        {
            entity.ToTable("ResponseCondition");

            entity.Property(e => e.RequestName).HasMaxLength(100);

            entity.HasOne(d => d.Request).WithMany(p => p.ResponseConditions)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponseCondition_Request");
        });

        modelBuilder.Entity<Scheduler>(entity =>
        {
            entity.ToTable("Scheduler");

            entity.Property(e => e.EndTime).HasConversion<long>();
            entity.Property(e => e.StartTime).HasConversion<long>();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
