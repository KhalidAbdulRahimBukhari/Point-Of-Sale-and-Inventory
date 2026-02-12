using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSsystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Country__10D160BFF0F7C5C3", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategory",
                columns: table => new
                {
                    ExpenseCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ExpenseC__9C2C63D8C55CE0C5", x => x.ExpenseCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "MovementType",
                columns: table => new
                {
                    MovementTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Movement__74FB1FF1C1C03246", x => x.MovementTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE3A673F71C6", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "SchemaVersion",
                columns: table => new
                {
                    Version = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SchemaVe__0F540135AE079F72", x => x.Version);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    PersonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NationalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    CountryID = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Person__AA2FFB85553DD75E", x => x.PersonID);
                    table.ForeignKey(
                        name: "FK_Person_Country",
                        column: x => x.CountryID,
                        principalTable: "Country",
                        principalColumn: "CountryID");
                });

            migrationBuilder.CreateTable(
                name: "Shop",
                columns: table => new
                {
                    ShopID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shop__67C556291829E8FD", x => x.ShopID);
                    table.ForeignKey(
                        name: "FK_Shop_Country",
                        column: x => x.CountryID,
                        principalTable: "Country",
                        principalColumn: "CountryID");
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__19093A2B81E304FF", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_Category_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonID = table.Column<int>(type: "int", nullable: false),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC50845295", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_Person",
                        column: x => x.PersonID,
                        principalTable: "Person",
                        principalColumn: "PersonID");
                    table.ForeignKey(
                        name: "FK_Users_Role",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK_Users_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__B40CC6ED9330B356", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK_Product_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    ExpenseCategoryID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Expense__1445CFF3C4D3A9A4", x => x.ExpenseID);
                    table.ForeignKey(
                        name: "FK_Expense_Category",
                        column: x => x.ExpenseCategoryID,
                        principalTable: "ExpenseCategory",
                        principalColumn: "ExpenseCategoryID");
                    table.ForeignKey(
                        name: "FK_Expense_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                    table.ForeignKey(
                        name: "FK_Expense_User",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    SaleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SaleDiscount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "COMPLETED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sale__1EE3C41F27441767", x => x.SaleID);
                    table.ForeignKey(
                        name: "FK_Sale_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                    table.ForeignKey(
                        name: "FK_Sale_User",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ProductVariant",
                columns: table => new
                {
                    VariantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    TotalSoldQty = table.Column<int>(type: "int", nullable: false),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductV__0EA233E48AD8091F", x => x.VariantID);
                    table.ForeignKey(
                        name: "FK_ProductVariant_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK_ProductVariant_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Change = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A584F36F8E4", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payment_Sale",
                        column: x => x.SaleID,
                        principalTable: "Sale",
                        principalColumn: "SaleID");
                });

            migrationBuilder.CreateTable(
                name: "SaleReturn",
                columns: table => new
                {
                    SaleReturnID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleID = table.Column<int>(type: "int", nullable: false),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    TotalRefundAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PARTIAL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SaleRetu__2CB01EE26AE553A7", x => x.SaleReturnID);
                    table.ForeignKey(
                        name: "FK_SaleReturn_Sale",
                        column: x => x.SaleID,
                        principalTable: "Sale",
                        principalColumn: "SaleID");
                    table.ForeignKey(
                        name: "FK_SaleReturn_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                    table.ForeignKey(
                        name: "FK_SaleReturn_User",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "SaleItem",
                columns: table => new
                {
                    SaleItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleID = table.Column<int>(type: "int", nullable: false),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductNameAtSale = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VariantDescriptionAtSale = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitPriceAtSale = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    UnitCostAtSale = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalCostAtSale = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TaxRateAtSale = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TaxAmountAtSale = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalPriceAtSale = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SaleItem__C605946103DEFED9", x => x.SaleItemID);
                    table.ForeignKey(
                        name: "FK_SaleItem_Sale",
                        column: x => x.SaleID,
                        principalTable: "Sale",
                        principalColumn: "SaleID");
                    table.ForeignKey(
                        name: "FK_SaleItem_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                    table.ForeignKey(
                        name: "FK_SaleItem_Variant",
                        column: x => x.VariantID,
                        principalTable: "ProductVariant",
                        principalColumn: "VariantID");
                });

            migrationBuilder.CreateTable(
                name: "StockMovement",
                columns: table => new
                {
                    StockMovementID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    MovementTypeID = table.Column<int>(type: "int", nullable: false),
                    QuantityChange = table.Column<int>(type: "int", nullable: false),
                    ReferenceID = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockMov__E963E35C12FD3C2B", x => x.StockMovementID);
                    table.ForeignKey(
                        name: "FK_StockMovement_MovementType",
                        column: x => x.MovementTypeID,
                        principalTable: "MovementType",
                        principalColumn: "MovementTypeID");
                    table.ForeignKey(
                        name: "FK_StockMovement_Shop",
                        column: x => x.ShopID,
                        principalTable: "Shop",
                        principalColumn: "ShopID");
                    table.ForeignKey(
                        name: "FK_StockMovement_User",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StockMovement_Variant",
                        column: x => x.VariantID,
                        principalTable: "ProductVariant",
                        principalColumn: "VariantID");
                });

            migrationBuilder.CreateTable(
                name: "ReturnItem",
                columns: table => new
                {
                    ReturnItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleReturnID = table.Column<int>(type: "int", nullable: false),
                    SaleItemID = table.Column<int>(type: "int", nullable: false),
                    QuantityReturned = table.Column<int>(type: "int", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TaxRefundAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReturnIt__8D87CD1A1DC55DAF", x => x.ReturnItemID);
                    table.ForeignKey(
                        name: "FK_ReturnItem_SaleItem",
                        column: x => x.SaleItemID,
                        principalTable: "SaleItem",
                        principalColumn: "SaleItemID");
                    table.ForeignKey(
                        name: "FK_ReturnItem_SaleReturn",
                        column: x => x.SaleReturnID,
                        principalTable: "SaleReturn",
                        principalColumn: "SaleReturnID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Category_Shop_Name",
                table: "Category",
                columns: new[] { "ShopID", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expense_CreatedByUserID",
                table: "Expense",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_ExpenseCategoryID",
                table: "Expense",
                column: "ExpenseCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_Shop_Date",
                table: "Expense",
                columns: new[] { "ShopID", "ExpenseDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "UQ_ExpenseCategory_Name",
                table: "ExpenseCategory",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Sale",
                table: "Payment",
                column: "SaleID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_CountryID",
                table: "Person",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "UQ_Person_NationalNumber",
                table: "Person",
                column: "NationalNumber",
                unique: true,
                filter: "[NationalNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryID",
                table: "Product",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ShopID",
                table: "Product",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "UQ_ProductVariant_Product_Size_Color",
                table: "ProductVariant",
                columns: new[] { "ProductID", "Size", "Color" },
                unique: true,
                filter: "[Size] IS NOT NULL AND [Color] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_ProductVariant_Shop_Barcode",
                table: "ProductVariant",
                columns: new[] { "ShopID", "Barcode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ProductVariant_Shop_SKU",
                table: "ProductVariant",
                columns: new[] { "ShopID", "SKU" },
                unique: true,
                filter: "([SKU] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItem_SaleItemID",
                table: "ReturnItem",
                column: "SaleItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItem_SaleReturnID",
                table: "ReturnItem",
                column: "SaleReturnID");

            migrationBuilder.CreateIndex(
                name: "UQ_Role_RoleName",
                table: "Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sale_Shop_Date",
                table: "Sale",
                columns: new[] { "ShopID", "SaleDate" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_UserID",
                table: "Sale",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ_Sale_Shop_InvoiceNumber",
                table: "Sale",
                columns: new[] { "ShopID", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_Sale",
                table: "SaleItem",
                column: "SaleID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_ShopID",
                table: "SaleItem",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_VariantID",
                table: "SaleItem",
                column: "VariantID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_SaleID",
                table: "SaleReturn",
                column: "SaleID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_ShopID",
                table: "SaleReturn",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_UserID",
                table: "SaleReturn",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_CountryID",
                table: "Shop",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovement_CreatedByUserID",
                table: "StockMovement",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovement_MovementTypeID",
                table: "StockMovement",
                column: "MovementTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovement_ShopID",
                table: "StockMovement",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovement_Variant_Date",
                table: "StockMovement",
                columns: new[] { "VariantID", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonID",
                table: "Users",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ShopID",
                table: "Users",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "UQ_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "ReturnItem");

            migrationBuilder.DropTable(
                name: "SchemaVersion");

            migrationBuilder.DropTable(
                name: "StockMovement");

            migrationBuilder.DropTable(
                name: "ExpenseCategory");

            migrationBuilder.DropTable(
                name: "SaleItem");

            migrationBuilder.DropTable(
                name: "SaleReturn");

            migrationBuilder.DropTable(
                name: "MovementType");

            migrationBuilder.DropTable(
                name: "ProductVariant");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Shop");

            migrationBuilder.DropTable(
                name: "Country");
        }
    }
}
