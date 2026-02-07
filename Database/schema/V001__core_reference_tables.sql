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
