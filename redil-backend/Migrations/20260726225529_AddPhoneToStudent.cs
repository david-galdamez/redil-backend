using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace redil_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar columna Phone como nullable (estudiantes existentes quedan con null)
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // Reemplazar índice único de Email por Phone
            migrationBuilder.DropIndex(
                name: "IX_students_Email",
                table: "students");

            migrationBuilder.CreateIndex(
                name: "IX_students_Phone",
                table: "students",
                column: "Phone",
                unique: true);

            // Hacer Email nullable (ya no es el identificador principal)
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_students_Phone",
                table: "students");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "students");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_Email",
                table: "students",
                column: "Email",
                unique: true);
        }
    }
}
