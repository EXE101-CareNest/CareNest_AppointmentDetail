using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNest_AppointmentDetail.Infrastructure.CareNest_AppointmentDetail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_denormalized_service_fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceDetailName",
                table: "AppointmentDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "AppointmentDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "AppointmentDetails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceDetailName",
                table: "AppointmentDetails");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "AppointmentDetails");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "AppointmentDetails");
        }
    }
}
