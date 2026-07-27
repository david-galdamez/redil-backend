using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace redil_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNumCourseToRedil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumCourse",
                table: "rediles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumCourse",
                table: "rediles");
        }
    }
}
