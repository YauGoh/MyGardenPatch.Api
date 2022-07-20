using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGardenPatch.LocalIdentity.Migrations
{
    public partial class SeedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("0c213b73-1c49-4383-a63b-b505e9d34644"), "90ae882c-a96c-427c-83ff-0515bc199860", "Api", "API" });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("60c30faa-e41b-427c-a08c-65e2eb39d990"), "e82d2414-ea67-491f-ad92-400889444d63", "Gardener", "GARDENER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0c213b73-1c49-4383-a63b-b505e9d34644"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("60c30faa-e41b-427c-a08c-65e2eb39d990"));
        }
    }
}
