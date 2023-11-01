using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Material.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseMigration_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseDetail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    purchaseId = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    qty = table.Column<float>(type: "real", nullable: false),
                    rate = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDetail", x => x.id);
                    table.ForeignKey(
                        name: "FK_PurchaseDetail_Purchases_purchaseId",
                        column: x => x.purchaseId,
                        principalTable: "Purchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetail_purchaseId",
                table: "PurchaseDetail",
                column: "purchaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseDetail");
        }
    }
}
