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
