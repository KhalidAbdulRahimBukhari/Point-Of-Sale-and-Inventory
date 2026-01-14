-- ============================================
-- V010_Core_Essential_Indexes.sql
-- PERFORMANCE ONLY — no constraints
-- ============================================

BEGIN TRANSACTION;

------------------------------------------------
-- 1. Sales by Shop & Date
------------------------------------------------
DROP INDEX IF EXISTS IX_Sale_Shop_Date ON Sale;
CREATE INDEX IX_Sale_Shop_Date
ON Sale (ShopID, SaleDate DESC)
INCLUDE (GrandTotal, Status, UserID);
PRINT 'Created: IX_Sale_Shop_Date';


------------------------------------------------
-- 2. SaleItems by Sale
------------------------------------------------
DROP INDEX IF EXISTS IX_SaleItem_Sale ON SaleItem;
CREATE INDEX IX_SaleItem_Sale
ON SaleItem (SaleID)
INCLUDE (VariantID, Quantity, UnitPriceAtSale, TotalPriceAtSale);
PRINT 'Created: IX_SaleItem_Sale';


------------------------------------------------
-- 3. Stock Movements by Variant & Time
------------------------------------------------
DROP INDEX IF EXISTS IX_StockMovement_Variant_Date ON StockMovement;
CREATE INDEX IX_StockMovement_Variant_Date
ON StockMovement (VariantID, CreatedAt DESC);
PRINT 'Created: IX_StockMovement_Variant_Date';


------------------------------------------------
-- 4. Payments by Sale
------------------------------------------------
DROP INDEX IF EXISTS IX_Payment_Sale ON Payment;
CREATE INDEX IX_Payment_Sale
ON Payment (SaleID)
INCLUDE (Amount, PaymentMethod, Status);
PRINT 'Created: IX_Payment_Sale';


------------------------------------------------
-- 5. Expenses by Shop & Date
------------------------------------------------
DROP INDEX IF EXISTS IX_Expense_Shop_Date ON Expense;
CREATE INDEX IX_Expense_Shop_Date
ON Expense (ShopID, ExpenseDate DESC)
INCLUDE (Amount, ExpenseCategoryID);
PRINT 'Created: IX_Expense_Shop_Date';


INSERT INTO SchemaVersion (Version) VALUES (10);

COMMIT;
PRINT 'V010 performance indexes applied successfully';
