using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations
{
    public partial class AdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9aae0b6d-d50c-4d0a-9t80-2a6873e3845d", "7b9168f2-1e33-4168-8df9-eba28b9637e0", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5673b8cf-12de-48s6-92ad-fae4a77932ad", 0, "52eb92c0-92df-4c86-8ad8-e74916a6b3f8", "augaldecr@gmail.com", false, false, null, "augaldecr@gmail.com", "augaldecr@gmail.com", "AQAAAAEAACcQAAAAEBJ5CuULuofoc4SD7Twh5I8HMVID4WxDBp8einV7H8RFMHMvv5HexU3UTKsEiDbRDg==", null, false, "157e19eb-1063-427e-8969-c84ae719f475", false, "augaldecr@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "5673b8cf-12de-48s6-92ad-fae4a77932ad" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9aae0b6d-d50c-4d0a-9t80-2a6873e3845d");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5673b8cf-12de-48s6-92ad-fae4a77932ad");
        }
    }
}
