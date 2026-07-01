using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InvestmentTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NationalId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "string"),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentCycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParticipantId = table.Column<int>(type: "integer", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DurationMonths = table.Column<int>(type: "integer", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InterestType = table.Column<int>(type: "integer", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    MonthlyInterest = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalExpectedProfit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentCycles_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvestmentCycleId = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ScheduledAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    PaidDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_InvestmentCycles_InvestmentCycleId",
                        column: x => x.InvestmentCycleId,
                        principalTable: "InvestmentCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvestmentCycleId = table.Column<int>(type: "integer", nullable: false),
                    PaymentScheduleId = table.Column<int>(type: "integer", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReceivedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_InvestmentCycles_InvestmentCycleId",
                        column: x => x.InvestmentCycleId,
                        principalTable: "InvestmentCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentSchedules_PaymentScheduleId",
                        column: x => x.PaymentScheduleId,
                        principalTable: "PaymentSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "ApplicationUsers",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "FullName", "IsActive", "LastLoginDate", "ModifiedBy", "ModifiedDate", "Notes", "PasswordHash", "PhoneNumber", "Role", "Username" },
                values: new object[] { 1, "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@investmenttracker.com", "System Administrator", true, null, null, null, null, "$2a$11$vJYHhJvvQPl3/PxKGzHFW.hnJ8X8k8xF.8p0F8M/Z3h6ZrJM8Qq1y", null, 1, "admin" });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Category", "CreatedBy", "CreatedDate", "DataType", "Description", "DisplayOrder", "IsVisible", "Key", "ModifiedBy", "ModifiedDate", "Value" },
                values: new object[,]
                {
                    { 1, "General", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", "Company or application name", 1, true, "CompanyName", null, null, "Investment Tracker" },
                    { 2, "General", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", "Default currency code", 2, true, "Currency", null, null, "USD" },
                    { 3, "General", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", "Currency symbol for display", 3, true, "CurrencySymbol", null, null, "$" },
                    { 4, "Investment", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "decimal", "Default monthly interest rate percentage", 1, true, "DefaultInterestRate", null, null, "5.0" },
                    { 5, "Investment", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "int", "Default investment duration in months", 2, true, "DefaultDurationMonths", null, null, "12" },
                    { 6, "Notifications", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bool", "Enable email notifications for payments", 1, true, "EnableEmailNotifications", null, null, "false" },
                    { 7, "Reports", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", "Path to logo image for reports", 1, true, "ReportLogoPath", null, null, "/images/logo.png" },
                    { 8, "General", "System", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", "Default date format for display", 4, true, "DateFormat", null, null, "dd/MM/yyyy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_Email",
                table: "ApplicationUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_IsActive",
                table: "ApplicationUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_Username",
                table: "ApplicationUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentCycles_EndDate",
                table: "InvestmentCycles",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentCycles_ParticipantId",
                table: "InvestmentCycles",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentCycles_ParticipantId_Status",
                table: "InvestmentCycles",
                columns: new[] { "ParticipantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentCycles_StartDate",
                table: "InvestmentCycles",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentCycles_Status",
                table: "InvestmentCycles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Email",
                table: "Participants",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_IsActive",
                table: "Participants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_NationalId",
                table: "Participants",
                column: "NationalId",
                unique: true,
                filter: "\"NationalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Phone",
                table: "Participants",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvestmentCycleId",
                table: "Payments",
                column: "InvestmentCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvestmentCycleId_PaymentDate",
                table: "Payments",
                columns: new[] { "InvestmentCycleId", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentScheduleId",
                table: "Payments",
                column: "PaymentScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReferenceNumber",
                table: "Payments",
                column: "ReferenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_InvestmentCycleId",
                table: "PaymentSchedules",
                column: "InvestmentCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_InvestmentCycleId_ScheduledDate",
                table: "PaymentSchedules",
                columns: new[] { "InvestmentCycleId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_ScheduledDate",
                table: "PaymentSchedules",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_Status",
                table: "PaymentSchedules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Category",
                table: "Settings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Category_DisplayOrder",
                table: "Settings",
                columns: new[] { "Category", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                table: "Settings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "PaymentSchedules");

            migrationBuilder.DropTable(
                name: "InvestmentCycles");

            migrationBuilder.DropTable(
                name: "Participants");
        }
    }
}
