using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotParkAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToParkingLot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "available_by_schedule",
                table: "Parking_Lots",
                newName: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Parking_Lots",
                newName: "available_by_schedule");
        }
    }
}
