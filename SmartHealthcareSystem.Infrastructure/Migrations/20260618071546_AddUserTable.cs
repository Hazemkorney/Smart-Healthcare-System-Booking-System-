using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHealthcareSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Receptionists",
                newName: "SaltHash");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Receptionists",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Receptionists",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Receptionists",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PassHash",
                table: "Receptionists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Receptionists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Admin");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Patient");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Doctor");

            migrationBuilder.CreateIndex(
                name: "IX_Receptionists_Email",
                table: "Receptionists",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receptionists_Email",
                table: "Receptionists");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Receptionists");

            migrationBuilder.DropColumn(
                name: "PassHash",
                table: "Receptionists");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Receptionists");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "SaltHash",
                table: "Receptionists",
                newName: "Password");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Receptionists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Receptionists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);
        }
    }
}
