using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTpcStrategy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "wallets",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "wallets",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "wallets",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "EntityVersion",
                table: "wallets",
                newName: "entity_version");
            
            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                table: "wallets",
                newName: "deleted_at_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "wallets",
                newName: "created_at_utc");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "transactions",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "transactions",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "EntityVersion",
                table: "transactions",
                newName: "entity_version");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                table: "transactions",
                newName: "deleted_at_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "transactions",
                newName: "created_at_utc");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "wallets",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "wallets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "wallets",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "wallets",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "entity_version",
                table: "wallets",
                newName: "EntityVersion");

            migrationBuilder.RenameColumn(
                name: "deleted_at_utc",
                table: "wallets",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "wallets",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "transactions",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "transactions",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "entity_version",
                table: "transactions",
                newName: "EntityVersion");

            migrationBuilder.RenameColumn(
                name: "deleted_at_utc",
                table: "transactions",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "transactions",
                newName: "CreatedAtUtc");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "wallets",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "transactions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
