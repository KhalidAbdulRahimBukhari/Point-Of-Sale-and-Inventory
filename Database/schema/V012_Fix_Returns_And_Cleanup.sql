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
-- 3. MovementType: remove Code column
------------------------------------------------
ALTER TABLE MovementType
DROP CONSTRAINT UQ__Movement__A25C5AA7B65E68F6;

IF COL_LENGTH('MovementType', 'Code') IS NOT NULL
BEGIN
    ALTER TABLE MovementType
    DROP COLUMN Code;
END;


------------------------------------------------
-- 4. SaleItem: remove ItemDiscountAmount (V1 cleanup)
------------------------------------------------
ALTER TABLE SaleItem
DROP CONSTRAINT DF__SaleItem__ItemDi__6754599E;


IF COL_LENGTH('SaleItem', 'ItemDiscountAmount') IS NOT NULL
BEGIN
    ALTER TABLE SaleItem
    DROP COLUMN ItemDiscountAmount;
END;


------------------------------------------------
-- 5. StockMovement: remove UnitCost
------------------------------------------------
ALTER TABLE StockMovement
DROP CONSTRAINT CK_StockMovement_UnitCost;

IF COL_LENGTH('StockMovement', 'UnitCost') IS NOT NULL
BEGIN
    ALTER TABLE StockMovement
    DROP COLUMN UnitCost;
END;


------------------------------------------------
-- 6. Schema version bump
------------------------------------------------
INSERT INTO SchemaVersion (Version)
VALUES (12);

COMMIT;
