using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirBot.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ibb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.InsertData(
                table:"roles",
                columns: new[] { "Id", "Name", "NormalizedName" , "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "User", "USER", Guid.NewGuid().ToString() } 
            
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM roles");
        }
    }
}
