
--V010_Add_Initial_Indexes.sql


-- Sales
CREATE INDEX IX_Sale_Shop_Date ON Sale (ShopID, SaleDate);
CREATE INDEX IX_Sale_User ON Sale (UserID);

-- SaleItem
CREATE INDEX IX_SaleItem_Sale ON SaleItem (SaleID);
CREATE INDEX IX_SaleItem_Variant ON SaleItem (VariantID);

-- StockMovement
CREATE INDEX IX_StockMovement_Variant_Date 
ON StockMovement (VariantID, CreatedAt);

CREATE INDEX IX_StockMovement_Shop_Date
ON StockMovement (ShopID, CreatedAt);

-- Payment
CREATE INDEX IX_Payment_Sale ON Payment (SaleID);

-- Expense
CREATE INDEX IX_Expense_Shop_Date ON Expense (ShopID, ExpenseDate);
