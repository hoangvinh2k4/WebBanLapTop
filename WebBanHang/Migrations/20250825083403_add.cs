//using System;
//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace WebBanHang.Migrations
//{
//    /// <inheritdoc />
//    public partial class add : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "Brands",
//                columns: table => new
//                {
//                    BrandID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    NameBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Brands", x => x.BrandID);
//                });

//            migrationBuilder.CreateTable(
//                name: "Categories",
//                columns: table => new
//                {
//                    CategoryID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    ParentID = table.Column<int>(type: "int", nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
//                });

//            migrationBuilder.CreateTable(
//                name: "ProductImages",
//                columns: table => new
//                {
//                    ImageID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    ProductID = table.Column<int>(type: "int", nullable: false),
//                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    IsMain = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ProductImages", x => x.ImageID);
//                });

//            migrationBuilder.CreateTable(
//                name: "Products",
//                columns: table => new
//                {
//                    ProductID = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    CategoryID = table.Column<int>(type: "int", nullable: false),
//                    NameProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    Stock = table.Column<int>(type: "int", nullable: false),
//                    BrandID = table.Column<int>(type: "int", nullable: false),
//                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Products", x => x.ProductID);
//                    table.ForeignKey(
//                        name: "FK_Products_Brands_BrandID",
//                        column: x => x.BrandID,
//                        principalTable: "Brands",
//                        principalColumn: "BrandID",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateIndex(
//                name: "IX_Products_BrandID",
//                table: "Products",
//                column: "BrandID");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "Categories");

//            migrationBuilder.DropTable(
//                name: "ProductImages");

//            migrationBuilder.DropTable(
//                name: "Products");

//            migrationBuilder.DropTable(
//                name: "Brands");
//        }
//    }
//}
