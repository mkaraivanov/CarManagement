using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPreventiveMaintenanceSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentEngineHours",
                table: "Vehicles",
                type: "decimal(10, 2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EngineHoursLastUpdated",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineHoursAtService",
                table: "ServiceRecords",
                type: "decimal(10, 2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaintenanceTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsSystemTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DefaultIntervalMonths = table.Column<int>(type: "INTEGER", nullable: true),
                    DefaultIntervalKilometers = table.Column<int>(type: "INTEGER", nullable: true),
                    DefaultIntervalHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    UseCompoundRule = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VehicleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TaskName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IntervalMonths = table.Column<int>(type: "INTEGER", nullable: true),
                    IntervalKilometers = table.Column<int>(type: "INTEGER", nullable: true),
                    IntervalHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    UseCompoundRule = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastCompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastCompletedMileage = table.Column<int>(type: "INTEGER", nullable: true),
                    LastCompletedHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    LastServiceRecordId = table.Column<Guid>(type: "TEXT", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextDueMileage = table.Column<int>(type: "INTEGER", nullable: true),
                    NextDueHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    ReminderDaysBefore = table.Column<int>(type: "INTEGER", nullable: false),
                    ReminderKilometersBefore = table.Column<int>(type: "INTEGER", nullable: false),
                    ReminderHoursBefore = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceSchedules_MaintenanceTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "MaintenanceTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaintenanceSchedules_ServiceRecords_LastServiceRecordId",
                        column: x => x.LastServiceRecordId,
                        principalTable: "ServiceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaintenanceSchedules_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaintenanceScheduleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DismissedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_MaintenanceSchedules_MaintenanceScheduleId",
                        column: x => x.MaintenanceScheduleId,
                        principalTable: "MaintenanceSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reminders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReminderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ActionUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Reminders_ReminderId",
                        column: x => x.ReminderId,
                        principalTable: "Reminders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MaintenanceTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultIntervalHours", "DefaultIntervalKilometers", "DefaultIntervalMonths", "Description", "IsSystemTemplate", "Name", "UpdatedAt", "UseCompoundRule", "UserId" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Engine", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 10000, 6, "Regular oil and filter change", true, "Oil Change", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Engine", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 10000, 6, "Replace engine oil filter", true, "Oil Filter Replacement", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Engine", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 20000, 12, "Replace engine air filter", true, "Air Filter Replacement", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Engine", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 50000, 24, "Replace spark plugs", true, "Spark Plugs Replacement", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Engine", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 100000, 60, "Replace timing belt", true, "Timing Belt Replacement", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Tires", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 10000, 6, "Rotate tires for even wear", true, "Tire Rotation", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Tires", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 60000, 48, "Replace worn tires", true, "Tire Replacement", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Tires", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 20000, 12, "Check and adjust wheel alignment", true, "Wheel Alignment", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Tires", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 2000, 1, "Check and adjust tire pressure", true, "Tire Pressure Check", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Brakes", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 20000, 12, "Inspect brake pads and rotors", true, "Brake Pad Inspection", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Brakes", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 40000, 24, "Flush and replace brake fluid", true, "Brake Fluid Replacement", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Fluids", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 40000, 24, "Flush and replace engine coolant", true, "Coolant Flush", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Fluids", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 80000, 48, "Change transmission fluid", true, "Transmission Fluid Change", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Fluids", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, 40000, 24, "Check and replace power steering fluid", true, "Power Steering Fluid", null, true, null },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "Inspection", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 12, "Comprehensive vehicle safety inspection", true, "Annual Safety Inspection", null, false, null },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Inspection", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 24, "Emission test and certification", true, "Emission Test", null, false, null },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "Inspection", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 12, "Test battery and charging system", true, "Battery Check", null, false, null },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "Equipment", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 250m, null, null, "Oil change for heavy equipment based on engine hours", true, "Oil Change - Heavy Equipment", null, false, null },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "Equipment", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 500m, null, null, "Change hydraulic fluid for equipment", true, "Hydraulic Fluid Change", null, false, null },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "Equipment", new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 100m, null, null, "Replace air filter on heavy equipment", true, "Air Filter - Equipment", null, false, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_IsActive",
                table: "MaintenanceSchedules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_LastServiceRecordId",
                table: "MaintenanceSchedules",
                column: "LastServiceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_NextDueDate",
                table: "MaintenanceSchedules",
                column: "NextDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_TemplateId",
                table: "MaintenanceSchedules",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_VehicleId",
                table: "MaintenanceSchedules",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTemplates_Category",
                table: "MaintenanceTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTemplates_Name",
                table: "MaintenanceTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTemplates_UserId",
                table: "MaintenanceTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Channel",
                table: "Notifications",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReminderId",
                table: "Notifications",
                column: "ReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_MaintenanceScheduleId",
                table: "Reminders",
                column: "MaintenanceScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_ScheduledDate",
                table: "Reminders",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_Status",
                table: "Reminders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "MaintenanceSchedules");

            migrationBuilder.DropTable(
                name: "MaintenanceTemplates");

            migrationBuilder.DropColumn(
                name: "CurrentEngineHours",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EngineHoursLastUpdated",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EngineHoursAtService",
                table: "ServiceRecords");
        }
    }
}
