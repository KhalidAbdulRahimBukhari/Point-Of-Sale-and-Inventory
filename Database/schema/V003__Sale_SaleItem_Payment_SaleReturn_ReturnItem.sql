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

    ItemDiscountAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
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
    Change DECIMAL(12,2) NOT NULL,
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
