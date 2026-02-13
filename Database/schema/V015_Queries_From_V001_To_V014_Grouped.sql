BEGIN TRANSACTION;

-------------------------------------------------
-- Country (Lookup)
-------------------------------------------------
CREATE TABLE Country (
    CountryID INT IDENTITY PRIMARY KEY,
    CountryName NVARCHAR(100) NOT NULL
);

-------------------------------------------------
-- Shop
-------------------------------------------------
CREATE TABLE Shop (
    ShopID INT IDENTITY PRIMARY KEY,
    CountryID INT NOT NULL,

    Name NVARCHAR(200) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL,

    City NVARCHAR(100) NOT NULL,
    District NVARCHAR(100) NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Shop_Country
        FOREIGN KEY (CountryID) REFERENCES Country(CountryID)
);

-------------------------------------------------
-- Category
-------------------------------------------------
CREATE TABLE Category (
    CategoryID INT IDENTITY PRIMARY KEY,
    ShopID INT NOT NULL,

    Name NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Category_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),
);

-------------------------------------------------
-- Product
-------------------------------------------------
CREATE TABLE Product (
    ProductID INT IDENTITY PRIMARY KEY,
    ShopID INT NOT NULL,
    CategoryID INT NOT NULL,

    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    ImagePath NVARCHAR(500) NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Product_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),

    CONSTRAINT FK_Product_Category
        FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

-------------------------------------------------
-- ProductVariant
-------------------------------------------------
CREATE TABLE ProductVariant (
    VariantID INT IDENTITY PRIMARY KEY,
    ShopID INT NOT NULL,
    ProductID INT NOT NULL,

    SKU NVARCHAR(50) NULL,
    Barcode NVARCHAR(50) NOT NULL,

    Size NVARCHAR(50) NULL,
    Color NVARCHAR(50) NULL,

    SellingPrice DECIMAL(12,2) NOT NULL,
    CostPrice DECIMAL(12,2) NOT NULL,

    CurrentStock INT NOT NULL DEFAULT 0,
    TotalSoldQty INT NOT NULL DEFAULT 0,
    LowStockThreshold INT NULL,

    ImagePath NVARCHAR(500) NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_ProductVariant_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),

    CONSTRAINT FK_ProductVariant_Product
        FOREIGN KEY (ProductID) REFERENCES Product(ProductID),


    CONSTRAINT CK_ProductVariant_Prices
        CHECK (SellingPrice >= 0 AND CostPrice >= 0)
);

-------------------------------------------------
-- Schema Versioning
-------------------------------------------------
CREATE TABLE SchemaVersion (
    Version INT PRIMARY KEY,
    AppliedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

INSERT INTO SchemaVersion (Version) VALUES (1);

COMMIT;
GO

BEGIN TRANSACTION;

-------------------------------------------------
-- MovementType
-------------------------------------------------
CREATE TABLE MovementType (
    MovementTypeID INT IDENTITY PRIMARY KEY,
    Description NVARCHAR(200) NULL
);

-------------------------------------------------
-- StockMovement (ledger = source of truth)
-------------------------------------------------
CREATE TABLE StockMovement (
    StockMovementID BIGINT IDENTITY PRIMARY KEY,

    ShopID INT NOT NULL,
    VariantID INT NOT NULL,
    MovementTypeID INT NOT NULL,

    QuantityChange INT NOT NULL,          -- +in / -out

    ReferenceID INT NULL,                 -- SaleID, ReturnID, etc.
    Reason NVARCHAR(300) NULL,            -- REQUIRED for adjustments (enforced in app)

    CreatedByUserID INT NOT NULL,
    CreatedByUsername NVARCHAR(100) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_StockMovement_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),

    CONSTRAINT FK_StockMovement_Variant
        FOREIGN KEY (VariantID) REFERENCES ProductVariant(VariantID),

    CONSTRAINT FK_StockMovement_MovementType
        FOREIGN KEY (MovementTypeID) REFERENCES MovementType(MovementTypeID),

    CONSTRAINT CK_StockMovement_Quantity
        CHECK (QuantityChange <> 0),
);



-------------------------------------------------
-- Schema Version
-------------------------------------------------
INSERT INTO SchemaVersion (Version) VALUES (2);

COMMIT;
GO

BEGIN TRANSACTION;
-------------------------------------------------
-- Sale
-------------------------------------------------

CREATE TABLE Sale (
    SaleID INT IDENTITY PRIMARY KEY,

    ShopID INT NOT NULL,
    UserID INT NOT NULL,

    SaleDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    InvoiceNumber NVARCHAR(50) NOT NULL,

    SubTotal DECIMAL(12,2) NOT NULL,
    SaleDiscount DECIMAL(12,2) NOT NULL DEFAULT 0,
    TaxTotal DECIMAL(12,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(12,2) NOT NULL,

    CurrencyCode CHAR(3) NOT NULL,

    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Sale_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID)
);



-------------------------------------------------
-- SaleItem
-------------------------------------------------
CREATE TABLE SaleItem (
    SaleItemID INT IDENTITY PRIMARY KEY,

    SaleID INT NOT NULL,
    ShopID INT NOT NULL,
    VariantID INT NOT NULL,

    Quantity INT NOT NULL,

    ProductNameAtSale NVARCHAR(200) NOT NULL,
    VariantDescriptionAtSale NVARCHAR(200) NULL,

    UnitPriceAtSale DECIMAL(12,2) NOT NULL,
    UnitCostAtSale DECIMAL(12,2) NOT NULL,
    TotalCostAtSale DECIMAL(12,2) NOT NULL,


    TaxRateAtSale DECIMAL(5,2) NOT NULL DEFAULT 0,
    TaxAmountAtSale DECIMAL(12,2) NOT NULL DEFAULT 0,

    TotalPriceAtSale DECIMAL(12,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_SaleItem_Sale
        FOREIGN KEY (SaleID) REFERENCES Sale(SaleID),

    CONSTRAINT FK_SaleItem_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),

    CONSTRAINT FK_SaleItem_Variant
        FOREIGN KEY (VariantID) REFERENCES ProductVariant(VariantID),

    CONSTRAINT CK_SaleItem_Quantity
        CHECK (Quantity > 0),

    CONSTRAINT CK_SaleItem_Costs
        CHECK (UnitCostAtSale >= 0 AND TotalCostAtSale >= 0)
);




-------------------------------------------------
-- Payment
-------------------------------------------------

CREATE TABLE Payment (
    PaymentID INT IDENTITY PRIMARY KEY,

    SaleID INT NOT NULL,

    Amount DECIMAL(12,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL,

    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    Status NVARCHAR(50) NOT NULL, -- PAID, FAILED, REFUNDED

    CONSTRAINT FK_Payment_Sale
        FOREIGN KEY (SaleID) REFERENCES Sale(SaleID)
);



-------------------------------------------------
-- SaleReturn (Renamed from Return to avoid reserved keyword)
-------------------------------------------------
CREATE TABLE SaleReturn (
    SaleReturnID INT IDENTITY PRIMARY KEY,
    SaleID INT NOT NULL,
    ShopID INT NOT NULL,
    UserID INT NOT NULL,
    ReturnDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Reason NVARCHAR(300) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_SaleReturn_Sale FOREIGN KEY (SaleID) REFERENCES Sale(SaleID),
    CONSTRAINT FK_SaleReturn_Shop FOREIGN KEY (ShopID) REFERENCES Shop(ShopID)
);



-------------------------------------------------
-- ReturnItem (Updated to reference SaleReturn)
-------------------------------------------------
CREATE TABLE ReturnItem (
    ReturnItemID INT IDENTITY PRIMARY KEY,
    SaleReturnID INT NOT NULL,
    SaleItemID INT NOT NULL,
    QuantityReturned INT NOT NULL,
    UnitPriceAtReturn DECIMAL(12,2) NOT NULL,
    TaxAmountAtReturn DECIMAL(12,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_ReturnItem_SaleReturn 
        FOREIGN KEY (SaleReturnID) REFERENCES SaleReturn(SaleReturnID),
    CONSTRAINT FK_ReturnItem_SaleItem 
        FOREIGN KEY (SaleItemID) REFERENCES SaleItem(SaleItemID),
    CONSTRAINT CK_ReturnItem_Quantity CHECK (QuantityReturned > 0)
);

-------------------------------------------------
-- Schema Version
-------------------------------------------------

INSERT INTO SchemaVersion (Version) VALUES (3);


COMMIT;
GO

BEGIN TRANSACTION;

-------------------------------------------------
-- ExpenseCategory
-------------------------------------------------
CREATE TABLE ExpenseCategory (
    ExpenseCategoryID INT IDENTITY PRIMARY KEY,

    Name NVARCHAR(200) NOT NULL,

);

-------------------------------------------------
-- Expense
-------------------------------------------------
CREATE TABLE Expense (
    ExpenseID INT IDENTITY PRIMARY KEY,

    ShopID INT NOT NULL,
    ExpenseCategoryID INT NOT NULL,

    Amount DECIMAL(12,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL,

    Description NVARCHAR(500) NULL,
    ExpenseDate DATE NOT NULL,

    CreatedByUserID INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Expense_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID),

    CONSTRAINT FK_Expense_Category
        FOREIGN KEY (ExpenseCategoryID) REFERENCES ExpenseCategory(ExpenseCategoryID),

    CONSTRAINT CK_Expense_Amount
        CHECK (Amount > 0)
);


-------------------------------------------------
-- Schema Version
-------------------------------------------------
INSERT INTO SchemaVersion (Version) VALUES (4);

COMMIT;
GO

BEGIN TRANSACTION;
-------------------------------------------------
-- Role
-------------------------------------------------
CREATE TABLE Role (
    RoleID INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL,

    Permissions INT NOT NULL, -- Bitmask (application-defined)

    CONSTRAINT UQ_Role_RoleName UNIQUE (RoleName)
);

-------------------------------------------------
-- Person
-------------------------------------------------
CREATE TABLE Person (
    PersonID INT IDENTITY PRIMARY KEY,

    FullName NVARCHAR(200) NOT NULL,
    NationalNumber NVARCHAR(50) NULL,
    Email NVARCHAR(200) NULL,
    DateOfBirth DATE NULL,

    CountryID INT NULL,
    Gender NVARCHAR(10) NULL,
    Phone NVARCHAR(20) NULL,
    ImagePath NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),


    CONSTRAINT FK_Person_Country
        FOREIGN KEY (CountryID) REFERENCES Country(CountryID)
);

-------------------------------------------------
-- Users
-------------------------------------------------
CREATE TABLE Users (
    UserID INT IDENTITY PRIMARY KEY,

    PersonID INT NOT NULL,
    ShopID INT NOT NULL,
    RoleID INT NOT NULL,

    UserName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(256) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT UQ_Users_UserName
        UNIQUE (UserName),

    CONSTRAINT FK_Users_Person
        FOREIGN KEY (PersonID) REFERENCES Person(PersonID),

    CONSTRAINT FK_Users_Role
        FOREIGN KEY (RoleID) REFERENCES Role(RoleID),

    CONSTRAINT FK_Users_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID)
);

-------------------------------------------------
-- Schema Version
-------------------------------------------------
INSERT INTO SchemaVersion (Version) VALUES (5);

COMMIT;
GO

-- V006: Add missing UserID foreign keys
BEGIN TRANSACTION;

PRINT 'V006: Adding UserID foreign key constraints...';

ALTER TABLE Sale
ADD CONSTRAINT FK_Sale_User
    FOREIGN KEY (UserID) REFERENCES Users(UserID);

ALTER TABLE SaleReturn
ADD CONSTRAINT FK_SaleReturn_User
    FOREIGN KEY (UserID) REFERENCES Users(UserID);

ALTER TABLE Expense
ADD CONSTRAINT FK_Expense_User
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID);

ALTER TABLE StockMovement
ADD CONSTRAINT FK_StockMovement_User
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID);

INSERT INTO SchemaVersion (Version) VALUES (6);
COMMIT;
GO

-- V007: Added Brand column to Product table
BEGIN TRANSACTION;

PRINT 'V007: Adding Brand column to Product table...';

ALTER TABLE Product
ADD Brand NVARCHAR(100) NULL;

INSERT INTO SchemaVersion (Version) VALUES (7);
COMMIT;
GO

-- V008: Add Status column to Sale table
BEGIN TRANSACTION;

PRINT 'V008: Adding Status column to Sale table...';

ALTER TABLE Sale
ADD Status NVARCHAR(50) NOT NULL DEFAULT 'COMPLETED';

INSERT INTO SchemaVersion (Version) VALUES (8);
COMMIT;
GO

--V009_Add_Unique_Barcode_and_InvoiceNumber_SKU_PerShop.sql
BEGIN TRANSACTION;


-- Invoice number must be unique per shop
ALTER TABLE Sale
ADD CONSTRAINT UQ_Sale_Shop_InvoiceNumber
    UNIQUE (ShopID, InvoiceNumber);

-- SKU unique per shop (nullable, filtered)
CREATE UNIQUE INDEX UX_ProductVariant_Shop_SKU
ON ProductVariant (ShopID, SKU)
WHERE SKU IS NOT NULL;

INSERT INTO SchemaVersion (Version) VALUES (9);

COMMIT;
GO

-- ============================================
-- V011_Add_ProductVariant_Notes.sql
-- Adds optional notes to product variants
-- ============================================

BEGIN TRANSACTION;

ALTER TABLE ProductVariant
ADD Notes NVARCHAR(500) NULL;

PRINT 'Added column: ProductVariant.Notes';

INSERT INTO SchemaVersion (Version) VALUES (11);

COMMIT;
GO

-- ============================================
-- V012_Fix_Returns_And_Cleanup.sql
-- ============================================

BEGIN TRANSACTION;

------------------------------------------------
-- 1. SaleReturn: add missing fields
------------------------------------------------
IF COL_LENGTH('SaleReturn', 'TotalRefundAmount') IS NULL
BEGIN
    ALTER TABLE SaleReturn
    ADD TotalRefundAmount DECIMAL(18,2) NOT NULL DEFAULT 0;
END;

IF COL_LENGTH('SaleReturn', 'ReturnType') IS NULL
BEGIN
    ALTER TABLE SaleReturn
    ADD ReturnType NVARCHAR(20) NOT NULL DEFAULT 'PARTIAL';
END;


------------------------------------------------
-- 2. ReturnItem: rename columns
------------------------------------------------
IF COL_LENGTH('ReturnItem', 'UnitPriceAtReturn') IS NOT NULL
BEGIN
    EXEC sp_rename 
        'ReturnItem.UnitPriceAtReturn',
        'RefundAmount',
        'COLUMN';
END;

IF COL_LENGTH('ReturnItem', 'TaxAmountAtReturn') IS NOT NULL
BEGIN
    EXEC sp_rename 
        'ReturnItem.TaxAmountAtReturn',
        'TaxRefundAmount',
        'COLUMN';
END;


------------------------------------------------
-- 6. Schema version bump
------------------------------------------------
INSERT INTO SchemaVersion (Version)
VALUES (12);

COMMIT;
GO

-- V013_Add_Change_To_Payment_Table.sql
BEGIN TRANSACTION;

ALTER TABLE Payment 
ADD Change DECIMAL(12,2) NOT NULL DEFAULT 0;

INSERT INTO SchemaVersion (Version) VALUES (13);

COMMIT;
GO

-- V014_InvoiceHeaderView_InvoiceItemView.sql

CREATE VIEW InvoiceHeaderView AS
SELECT
    s.SaleID,
    s.InvoiceNumber,
    s.CreatedAt,
    u.UserName AS CashierUserName,
    s.SubTotal,
    s.TaxTotal,
    s.SaleDiscount,
    s.GrandTotal,
    ISNULL(p.Amount, 0) AS AmountPaid,
    ISNULL(p.Change, 0) AS ChangeAmount,
    ISNULL(p.PaymentMethod, '') AS PaymentMethod
FROM Sale s
JOIN [Users] u ON u.UserID = s.UserID
LEFT JOIN Payment p ON p.SaleID = s.SaleID;
GO

CREATE VIEW InvoiceItemView AS
SELECT
    si.SaleID,
    si.ProductNameAtSale AS Name,
    pv.Size,
    si.UnitPriceAtSale AS Price,
    si.Quantity AS Qty
FROM SaleItem si
JOIN ProductVariant pv ON pv.VariantID = si.VariantID;
GO

INSERT INTO SchemaVersion (Version) VALUES (14);