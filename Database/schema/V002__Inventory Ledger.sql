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
-- Seed movement types (V1)
-------------------------------------------------
INSERT INTO MovementType (Code, Description) VALUES
('SALE',        'Stock decrease due to sale'),
('RETURN',      'Stock increase due to return'),
('PURCHASE',    'Stock increase due to purchase'),
('ADJUSTMENT',  'Manual stock adjustment');

-------------------------------------------------
-- Schema Version
-------------------------------------------------
INSERT INTO SchemaVersion (Version) VALUES (2);

COMMIT;
