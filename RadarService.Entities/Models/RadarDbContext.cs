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

    public virtual DbSet<Command> Commands { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceCommand> DeviceCommands { get; set; }

    public virtual DbSet<DeviceLog> DeviceLogs { get; set; }

    public virtual DbSet<DeviceScheduler> DeviceSchedulers { get; set; }

    public virtual DbSet<FormParameter> FormParameters { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<ResponseCondition> ResponseConditions { get; set; }

    public virtual DbSet<Scheduler> Schedulers { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<StepRequest> StepRequests { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Command>(entity =>
        {
            entity.ToTable("Command");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("Device");

            entity.Property(e => e.BaseAddress).HasMaxLength(50);
            entity.Property(e => e.LastUpdateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<DeviceCommand>(entity =>
        {
            entity.ToTable("DeviceCommand");

            entity.HasOne(d => d.Command).WithMany(p => p.DeviceCommands)
                .HasForeignKey(d => d.CommandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceCommand_Command");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceCommands)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceCommand_Device");
        });

        modelBuilder.Entity<DeviceLog>(entity =>
        {
            entity.ToTable("DeviceLog");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LogDateTime).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceLogs)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceLog_Device");
        });

        modelBuilder.Entity<DeviceScheduler>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.SchedulerId, e.DeviceId });

            entity.ToTable("DeviceScheduler");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

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

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Request");

            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Url).HasMaxLength(50);
        });

        modelBuilder.Entity<ResponseCondition>(entity =>
        {
            entity.ToTable("ResponseCondition");

            entity.Property(e => e.CommandName).HasMaxLength(100);
            entity.Property(e => e.Condition).HasMaxLength(100);
            entity.Property(e => e.Result).HasMaxLength(100);

            entity.HasOne(d => d.Request).WithMany(p => p.ResponseConditions)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResponseCondition_Request");
        });

        modelBuilder.Entity<Scheduler>(entity =>
        {
            entity.ToTable("Scheduler");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.ToTable("Step");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<StepRequest>(entity =>
        {
            entity.ToTable("StepRequest");

            entity.HasOne(d => d.Command).WithMany(p => p.StepRequests)
                .HasForeignKey(d => d.CommandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StepRequest_Command");

            entity.HasOne(d => d.Request).WithMany(p => p.StepRequests)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StepRequest_Request");

            entity.HasOne(d => d.Step).WithMany(p => p.StepRequests)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StepRequest_Step");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
