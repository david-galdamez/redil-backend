using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace redil_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_student_redil_RedilId",
                table: "student_redil");

            migrationBuilder.DropIndex(
                name: "IX_classes_RedilId",
                table: "classes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_redil_RedilId_JoinedAt",
                table: "student_redil",
                columns: new[] { "RedilId", "JoinedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_rediles_Name",
                table: "rediles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_classes_RedilId_ClassDate",
                table: "classes",
                columns: new[] { "RedilId", "ClassDate" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_student_redil_RedilId_JoinedAt",
                table: "student_redil");

            migrationBuilder.DropIndex(
                name: "IX_rediles_Name",
                table: "rediles");

            migrationBuilder.DropIndex(
                name: "IX_classes_RedilId_ClassDate",
                table: "classes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "users",
                type: "boolean",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_redil_RedilId",
                table: "student_redil",
                column: "RedilId");

            migrationBuilder.CreateIndex(
                name: "IX_classes_RedilId",
                table: "classes",
                column: "RedilId");
        }
    }
}
