BEGIN TRANSACTION;

-------------------------------------------------
-- Country
-------------------------------------------------
INSERT INTO Country (CountryName) VALUES
('Germany'),
('France'),
('Italy'),
('Spain'),
('USA');

-------------------------------------------------
-- Shop
-------------------------------------------------
INSERT INTO Shop (CountryID, Name, CurrencyCode, City, District)
VALUES (1, 'UrbanWear Berlin', 'EUR', 'Berlin', 'Mitte');

-------------------------------------------------
-- Role
-------------------------------------------------
INSERT INTO Role (RoleName, Permissions) VALUES
('Admin', 255),
('Cashier', 15);

-------------------------------------------------
-- Person
-------------------------------------------------
INSERT INTO Person (FullName, Email, CountryID, Phone)
VALUES 
('Max Bauer', 'admin@urbanwear.de', 1, '+4911111111'),
('Anna Schmidt', 'cashier@urbanwear.de', 1, '+4922222222');

-------------------------------------------------
-- Users
-------------------------------------------------
INSERT INTO Users (PersonID, ShopID, RoleID, UserName, Password)
VALUES 
(1, 1, 1, 'admin', '0000'),
(2, 1, 2, 'anna', '0000');

-------------------------------------------------
-- Category
-------------------------------------------------
INSERT INTO Category (ShopID, Name) VALUES
(1, 'T-Shirts'),
(1, 'Jeans'),
(1, 'Jackets'),
(1, 'Shoes');

-------------------------------------------------
-- Product
-------------------------------------------------
INSERT INTO Product (ShopID, CategoryID, Name, Brand)
VALUES
(1, 1, 'Basic T-Shirt', 'UrbanWear'),
(1, 2, 'Slim Fit Jeans', 'UrbanWear'),
(1, 3, 'Winter Jacket', 'UrbanWear'),
(1, 4, 'Sneakers Classic', 'UrbanWear');

-------------------------------------------------
-- ProductVariant
-------------------------------------------------
INSERT INTO ProductVariant
(ShopID, ProductID, SKU, Barcode, Size, Color, SellingPrice, CostPrice, CurrentStock)
VALUES
(1, 1, 'TS-S-BLK', '1000001', 'S', 'Black', 19.99, 8.00, 0),
(1, 1, 'TS-M-WHT', '1000002', 'M', 'White', 19.99, 8.00, 0),
(1, 2, 'JE-32-BLU', '2000001', '32', 'Blue', 59.99, 30.00, 0),
(1, 3, 'JK-L-BLK',  '3000001', 'L', 'Black', 129.99, 70.00, 0),
(1, 4, 'SN-42-WHT', '4000001', '42', 'White', 89.99, 45.00, 0);

-------------------------------------------------
-- MovementType
-------------------------------------------------
INSERT INTO MovementType (Description) VALUES
('PURCHASE'),
('SALE'),
('RETURN'),
('ADJUSTMENT');

-------------------------------------------------
-- Initial Stock (Purchase)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 1, 1, 50, NULL, 'Initial stock', 1, 'admin'),
(1, 2, 1, 40, NULL, 'Initial stock', 1, 'admin'),
(1, 3, 1, 30, NULL, 'Initial stock', 1, 'admin'),
(1, 4, 1, 20, NULL, 'Initial stock', 1, 'admin'),
(1, 5, 1, 25, NULL, 'Initial stock', 1, 'admin');

-------------------------------------------------
-- Sale
-------------------------------------------------
INSERT INTO Sale
(ShopID, UserID, InvoiceNumber, SubTotal, SaleDiscount, TaxTotal, GrandTotal, CurrencyCode, Status)
VALUES
(1, 2, 'INV-001', 79.98, 0, 0, 79.98, 'EUR', 'COMPLETED');

-------------------------------------------------
-- SaleItem
-------------------------------------------------
INSERT INTO SaleItem
(SaleID, ShopID, VariantID, Quantity,
 ProductNameAtSale, VariantDescriptionAtSale,
 UnitPriceAtSale, UnitCostAtSale,
 TotalCostAtSale, TaxRateAtSale, TaxAmountAtSale,
 TotalPriceAtSale, CurrencyCode)
VALUES
(1, 1, 1, 2,
 'Basic T-Shirt', 'Size S - Black',
 19.99, 8.00,
 16.00, 0, 0,
 39.98, 'EUR'),

(1, 1, 3, 1,
 'Slim Fit Jeans', 'Size 32 - Blue',
 59.99, 30.00,
 30.00, 0, 0,
 59.99, 'EUR');

-------------------------------------------------
-- StockMovement (SALE effect)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 1, 2, -2, 1, NULL, 2, 'anna'),
(1, 3, 2, -1, 1, NULL, 2, 'anna');

-------------------------------------------------
-- Payment
-------------------------------------------------
INSERT INTO Payment
(SaleID, Amount, CurrencyCode, PaymentMethod, Status, Change)
VALUES
(1, 80.00, 'EUR', 'CASH', 'PAID', 0.02);

-------------------------------------------------
-- Return
-------------------------------------------------
INSERT INTO SaleReturn
(SaleID, ShopID, UserID, Reason, TotalRefundAmount, ReturnType)
VALUES
(1, 1, 2, 'Size too small', 19.99, 'PARTIAL');

-------------------------------------------------
-- ReturnItem
-------------------------------------------------
INSERT INTO ReturnItem
(SaleReturnID, SaleItemID, QuantityReturned, RefundAmount, TaxRefundAmount)
VALUES
(1, 1, 1, 19.99, 0);

-------------------------------------------------
-- StockMovement (RETURN effect)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 1, 3, 1, 1, NULL, 2, 'anna');

-------------------------------------------------
-- ExpenseCategory
-------------------------------------------------
INSERT INTO ExpenseCategory (Name) VALUES
('Salary'),
('Rent'),
('Utilities');

-------------------------------------------------
-- Expense
-------------------------------------------------
INSERT INTO Expense
(ShopID, ExpenseCategoryID, Amount, CurrencyCode, ExpenseDate, CreatedByUserID)
VALUES
(1, 1, 2500, 'EUR', GETDATE(), 1),
(1, 2, 1800, 'EUR', GETDATE(), 1);

COMMIT;


BEGIN TRANSACTION;

-------------------------------------------------
-- Additional Products
-------------------------------------------------
INSERT INTO Product (ShopID, CategoryID, Name, Brand)
VALUES
(1, 1, 'Oversized T-Shirt', 'UrbanWear'),
(1, 2, 'Regular Fit Jeans', 'UrbanWear'),
(1, 3, 'Leather Jacket', 'UrbanWear');

-------------------------------------------------
-- Additional Variants
-------------------------------------------------
INSERT INTO ProductVariant
(ShopID, ProductID, SKU, Barcode, Size, Color, SellingPrice, CostPrice, CurrentStock, LowStockThreshold)
VALUES
(1, 5, 'TS-OVR-M-GRN', '5000001', 'M', 'Green', 24.99, 10.00, 0, 10),
(1, 6, 'JE-REG-34-BLK', '6000001', '34', 'Black', 69.99, 35.00, 0, 5),
(1, 7, 'JK-LTH-M-BLK', '7000001', 'M', 'Black', 199.99, 110.00, 0, 3);

-------------------------------------------------
-- Initial Stock For New Variants
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 6, 1, 60, NULL, 'Initial stock', 1, 'admin'),
(1, 7, 1, 35, NULL, 'Initial stock', 1, 'admin'),
(1, 8, 1, 15, NULL, 'Initial stock', 1, 'admin');

-------------------------------------------------
-- SECOND SALE (Card Payment)
-------------------------------------------------
INSERT INTO Sale
(ShopID, UserID, InvoiceNumber, SubTotal, SaleDiscount, TaxTotal, GrandTotal, CurrencyCode, Status, SaleDate)
VALUES
(1, 2, 'INV-002', 224.98, 0, 0, 224.98, 'EUR', 'COMPLETED', DATEADD(DAY, -2, SYSDATETIME()));

INSERT INTO SaleItem
(SaleID, ShopID, VariantID, Quantity,
 ProductNameAtSale, VariantDescriptionAtSale,
 UnitPriceAtSale, UnitCostAtSale,
 TotalCostAtSale, TaxRateAtSale, TaxAmountAtSale,
 TotalPriceAtSale, CurrencyCode)
VALUES
(2, 1, 8, 1,
 'Leather Jacket', 'Size M - Black',
 199.99, 110.00,
 110.00, 0, 0,
 199.99, 'EUR'),

(2, 1, 6, 1,
 'Oversized T-Shirt', 'Size M - Green',
 24.99, 10.00,
 10.00, 0, 0,
 24.99, 'EUR');

INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 8, 2, -1, 2, NULL, 2, 'anna'),
(1, 6, 2, -1, 2, NULL, 2, 'anna');

INSERT INTO Payment
(SaleID, Amount, CurrencyCode, PaymentMethod, Status, Change)
VALUES
(2, 224.98, 'EUR', 'CARD', 'PAID', 0);

-------------------------------------------------
-- THIRD SALE (Discount Scenario)
-------------------------------------------------
INSERT INTO Sale
(ShopID, UserID, InvoiceNumber, SubTotal, SaleDiscount, TaxTotal, GrandTotal, CurrencyCode, Status, SaleDate)
VALUES
(1, 2, 'INV-003', 139.98, 20.00, 0, 119.98, 'EUR', 'COMPLETED', DATEADD(DAY, -1, SYSDATETIME()));

INSERT INTO SaleItem
(SaleID, ShopID, VariantID, Quantity,
 ProductNameAtSale, VariantDescriptionAtSale,
 UnitPriceAtSale, UnitCostAtSale,
 TotalCostAtSale, TaxRateAtSale, TaxAmountAtSale,
 TotalPriceAtSale, CurrencyCode)
VALUES
(3, 1, 3, 2,
 'Slim Fit Jeans', 'Size 32 - Blue',
 69.99, 30.00,
 60.00, 0, 0,
 139.98, 'EUR');

INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 3, 2, -2, 3, NULL, 2, 'anna');

INSERT INTO Payment
(SaleID, Amount, CurrencyCode, PaymentMethod, Status, Change)
VALUES
(3, 120.00, 'EUR', 'CASH', 'PAID', 0.02);

-------------------------------------------------
-- FULL RETURN (Leather Jacket)
-------------------------------------------------
INSERT INTO SaleReturn
(SaleID, ShopID, UserID, Reason, TotalRefundAmount, ReturnType)
VALUES
(2, 1, 2, 'Customer dissatisfied', 199.99, 'FULL');

INSERT INTO ReturnItem
(SaleReturnID, SaleItemID, QuantityReturned, RefundAmount, TaxRefundAmount)
VALUES
(2, 4, 1, 199.99, 0);

INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 8, 3, 1, 2, NULL, 2, 'anna');

-------------------------------------------------
-- Manual Stock Adjustment (Inventory Count Fix)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, ReferenceID, Reason, CreatedByUserID, CreatedByUsername)
VALUES
(1, 2, 4, -2, NULL, 'Damaged items removed after audit', 1, 'admin');

-------------------------------------------------
-- Additional Expenses
-------------------------------------------------
INSERT INTO Expense
(ShopID, ExpenseCategoryID, Amount, CurrencyCode, ExpenseDate, CreatedByUserID)
VALUES
(1, 3, 320, 'EUR', DATEADD(DAY, -3, GETDATE()), 1),
(1, 2, 1500, 'EUR', DATEADD(DAY, -5, GETDATE()), 1);

COMMIT;
