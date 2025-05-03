using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotParkAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableByScheduleToParkingLot2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableBySchedule",
                table: "Parking_Lots",
                newName: "available_by_schedule");

            migrationBuilder.AlterColumn<bool>(
                name: "available_by_schedule",
                table: "Parking_Lots",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "available_by_schedule",
                table: "Parking_Lots",
                newName: "AvailableBySchedule");

            migrationBuilder.AlterColumn<bool>(
                name: "AvailableBySchedule",
                table: "Parking_Lots",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }
    }
}
