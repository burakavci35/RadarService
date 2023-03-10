// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RadarService.Entities.Models;

#nullable disable

namespace RadarService.Entities.Migrations
{
    [DbContext(typeof(RadarDbContext))]
    [Migration("20230312164426_MyFirstMigration")]
    partial class MyFirstMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RadarService.Entities.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BaseAddress")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Device", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.DeviceLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LogDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceLog", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.DeviceRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_DeviceCommand");

                    b.HasIndex("DeviceId");

                    b.HasIndex("RequestId");

                    b.ToTable("DeviceRequest", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.DeviceScheduler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("SchedulerId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_DeviceScheduler_1");

                    b.HasIndex("DeviceId");

                    b.HasIndex("SchedulerId");

                    b.ToTable("DeviceScheduler", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.FormParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("FormParameter", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Location", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("Response")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Request", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.ResponseCondition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("RequestName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("ResponseCondition", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.Scheduler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("EndTime")
                        .HasColumnType("bigint");

                    b.Property<long>("StartTime")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Scheduler", (string)null);
                });

            modelBuilder.Entity("RadarService.Entities.Models.Device", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Location", "Location")
                        .WithMany("Devices")
                        .HasForeignKey("LocationId")
                        .IsRequired()
                        .HasConstraintName("FK_Device_Location");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("RadarService.Entities.Models.DeviceLog", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Device", "Device")
                        .WithMany("DeviceLogs")
                        .HasForeignKey("DeviceId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceLog_Device");

                    b.Navigation("Device");
                });

            modelBuilder.Entity("RadarService.Entities.Models.DeviceRequest", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Device", "Device")
                        .WithMany("DeviceRequests")
                        .HasForeignKey("DeviceId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceCommand_Device");

                    b.HasOne("RadarService.Entities.Models.Request", "Request")
                        .WithMany("DeviceRequests")
                        .HasForeignKey("RequestId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceRequest_Request");

                    b.Navigation("Device");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("RadarService.Entities.Models.DeviceScheduler", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Device", "Device")
                        .WithMany("DeviceSchedulers")
                        .HasForeignKey("DeviceId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceScheduler_Device");

                    b.HasOne("RadarService.Entities.Models.Scheduler", "Scheduler")
                        .WithMany("DeviceSchedulers")
                        .HasForeignKey("SchedulerId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceScheduler_Scheduler");

                    b.Navigation("Device");

                    b.Navigation("Scheduler");
                });

            modelBuilder.Entity("RadarService.Entities.Models.FormParameter", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Request", "Request")
                        .WithMany("FormParameters")
                        .HasForeignKey("RequestId")
                        .IsRequired()
                        .HasConstraintName("FK_FormParameter_Request");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("RadarService.Entities.Models.Request", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Request", "Parent")
                        .WithMany("InverseParent")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Request_Request");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("RadarService.Entities.Models.ResponseCondition", b =>
                {
                    b.HasOne("RadarService.Entities.Models.Request", "Request")
                        .WithMany("ResponseConditions")
                        .HasForeignKey("RequestId")
                        .IsRequired()
                        .HasConstraintName("FK_ResponseCondition_Request");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("RadarService.Entities.Models.Device", b =>
                {
                    b.Navigation("DeviceLogs");

                    b.Navigation("DeviceRequests");

                    b.Navigation("DeviceSchedulers");
                });

            modelBuilder.Entity("RadarService.Entities.Models.Location", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("RadarService.Entities.Models.Request", b =>
                {
                    b.Navigation("DeviceRequests");

                    b.Navigation("FormParameters");

                    b.Navigation("InverseParent");

                    b.Navigation("ResponseConditions");
                });

            modelBuilder.Entity("RadarService.Entities.Models.Scheduler", b =>
                {
                    b.Navigation("DeviceSchedulers");
                });
#pragma warning restore 612, 618
        }
    }
}
