using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Challenge.CRM.Rommanel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rommanel");

            migrationBuilder.CreateTable(
                name: "customer_events",
                schema: "rommanel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_events_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "rommanel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    origin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    document_number = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    document_type = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    telephone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    address_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    federative_unit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    state_registration = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customer_events_aggregate_id",
                schema: "rommanel",
                table: "customer_events",
                column: "aggregate_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_events_aggregate_version",
                schema: "rommanel",
                table: "customer_events",
                columns: new[] { "aggregate_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_customers_document_number",
                schema: "rommanel",
                table: "customers",
                column: "document_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_customers_email",
                schema: "rommanel",
                table: "customers",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_events",
                schema: "rommanel");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "rommanel");
        }
    }
}
