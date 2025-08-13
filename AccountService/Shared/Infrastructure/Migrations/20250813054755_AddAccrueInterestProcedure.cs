using AccountService.Shared.BackgroundJobs;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccrueInterestProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InterestAccrualDailyBackgroundJob.CreateOrReplaceAccrueInterestProcedureCommand);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
