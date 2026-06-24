using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rental_orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    customer = table.Column<string>(type: "varchar(90)", maxLength: 90, nullable: false),
                    vehicles_id = table.Column<int>(type: "int", nullable: false),
                    plate = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    address_street = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    address_city = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    address_postal_code = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    amount = table.Column<double>(type: "double", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_rental_orders", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rental_orders");
        }
    }
}
