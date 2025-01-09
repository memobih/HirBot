using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ibb : Migration
    {
        /// <inheritdoc />
        /// 
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table:"roles",
                columns: new[] { "Id", "Name", "NormalizedName" , "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Admin", "ADMIN", Guid.NewGuid().ToString() } 
            
            );
            migrationBuilder.InsertData(
                table:"roles",
                columns: new[] { "Id", "Name", "NormalizedName" , "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "User", "USER", Guid.NewGuid().ToString() } 
            
            );
            migrationBuilder.InsertData(
                table:"roles",
                columns: new[] { "Id", "Name", "NormalizedName" , "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Company", "COMPANY", Guid.NewGuid().ToString() } 
            
            );
            migrationBuilder.InsertData(
                table:"roles",
                columns: new[] { "Id", "Name", "NormalizedName" , "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "SuperAdmin", "SUPERADMIN", Guid.NewGuid().ToString() } 
            
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM roles");
        }
    }
}
