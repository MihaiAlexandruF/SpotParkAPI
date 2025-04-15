using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotParkAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__B9BE370F334D2E48", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Parking_Lots",
                columns: table => new
                {
                    parking_lot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    owner_id = table.Column<int>(type: "int", nullable: false),
                    address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    price_per_hour = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Parking___7C960F5B597B5EDB", x => x.parking_lot_id);
                    table.ForeignKey(
                        name: "FK__Parking_L__owner__3E52440B",
                        column: x => x.owner_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Availability_Schedule",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parking_lot_id = table.Column<int>(type: "int", nullable: false),
                    availability_type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "weekly"),
                    day_of_week = table.Column<string>(type: "varchar(9)", unicode: false, maxLength: 9, nullable: true),
                    open_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    close_time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Availabi__C46A8A6FE68A3B18", x => x.schedule_id);
                    table.ForeignKey(
                        name: "FK__Availabil__parki__44FF419A",
                        column: x => x.parking_lot_id,
                        principalTable: "Parking_Lots",
                        principalColumn: "parking_lot_id");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    reservation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    driver_id = table.Column<int>(type: "int", nullable: false),
                    parking_lot_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    total_cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "active"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservat__31384C295A699A72", x => x.reservation_id);
                    table.ForeignKey(
                        name: "FK__Reservati__drive__4AB81AF0",
                        column: x => x.driver_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Reservati__parki__4BAC3F29",
                        column: x => x.parking_lot_id,
                        principalTable: "Parking_Lots",
                        principalColumn: "parking_lot_id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reservation_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payment_method = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    payment_status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "pending"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__ED1FC9EAFF800CB2", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__Payments__reserv__52593CB8",
                        column: x => x.reservation_id,
                        principalTable: "Reservations",
                        principalColumn: "reservation_id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_availability_schedule",
                table: "Availability_Schedule",
                columns: new[] { "parking_lot_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "idx_parking_lots_owner_id",
                table: "Parking_Lots",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "idx_payments_reservation_id",
                table: "Payments",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "idx_reservations_driver_id",
                table: "Reservations",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "idx_reservations_parking_lot_id",
                table: "Reservations",
                column: "parking_lot_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_username",
                table: "Users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Users__AB6E6164D8C51D1C",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Users__F3DBC57232E6832F",
                table: "Users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Availability_Schedule");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Parking_Lots");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
