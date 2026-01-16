BEGIN TRANSACTION;

-------------------------------------------------
-- Country
-------------------------------------------------
INSERT INTO Country (CountryName) VALUES
('Germany'),
('Saudi Arabia'),
('France'),
('Italy'),
('Spain'),
('USA'),
('UK'),
('Turkey'),
('UAE'),
('Egypt');

-------------------------------------------------
-- Shop
-------------------------------------------------
INSERT INTO Shop (CountryID, Name, CurrencyCode, City, District)
VALUES (1, 'Demo Shop Berlin', 'EUR', 'Berlin', 'Mitte');

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
VALUES ('Admin User', 'admin@demo.com', 1, '+491234567');

-------------------------------------------------
-- Users
-------------------------------------------------
INSERT INTO Users (PersonID, ShopID, RoleID, UserName, Password)
VALUES (1, 1, 1, 'admin', 'hashed_password');

-------------------------------------------------
-- Category
-------------------------------------------------
INSERT INTO Category (ShopID, Name) VALUES
(1, 'T-Shirts'),
(1, 'Jeans'),
(1, 'Drinks'),
(1, 'Sweets');

-------------------------------------------------
-- Product
-------------------------------------------------
INSERT INTO Product (ShopID, CategoryID, Name, Brand)
VALUES
(1, 1, 'Basic T-Shirt', 'Generic'),
(1, 2, 'Blue Jeans', 'DenimCo'),
(1, 3, 'Cola Drink', 'ColaBrand'),
(1, 4, 'Chocolate Bar', 'SweetCo');

-------------------------------------------------
-- ProductVariant
-------------------------------------------------
INSERT INTO ProductVariant
(ShopID, ProductID, SKU, Barcode, Size, Color, SellingPrice, CostPrice, CurrentStock)
VALUES
(1, 1, 'TS-S-BLK', '1000001', 'S', 'Black', 15, 7, 0),
(1, 1, 'TS-M-WHT', '1000002', 'M', 'White', 15, 7, 0),
(1, 2, 'JE-32-BLU', '2000001', '32', 'Blue', 45, 25, 0),
(1, 3, 'DR-500',    '3000001', '500ml', NULL, 2.5, 1, 0),
(1, 4, 'CH-STD',    '4000001', NULL, NULL, 1.5, 0.7, 0);

-------------------------------------------------
-- MovementType
-------------------------------------------------
INSERT INTO MovementType (Code, Description) VALUES
('PURCHASE', 'Stock added'),
('SALE', 'Sold to customer'),
('RETURN', 'Returned item'),
('ADJUSTMENT', 'Manual correction');

-------------------------------------------------
-- StockMovement (initial stock)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, UnitCost, CreatedByUserID, CreatedByUsername)
VALUES
(1, 1, 1, 50, 7, 1, 'admin'),
(1, 2, 1, 40, 7, 1, 'admin'),
(1, 3, 1, 30, 25, 1, 'admin'),
(1, 4, 1, 100, 1, 1, 'admin'),
(1, 5, 1, 200, 0.7, 1, 'admin');

-------------------------------------------------
-- Sale
-------------------------------------------------
INSERT INTO Sale
(ShopID, UserID, InvoiceNumber, SubTotal, GrandTotal, CurrencyCode)
VALUES
(1, 1, 'INV-001', 32.5, 32.5, 'EUR');

-------------------------------------------------
-- SaleItem
-------------------------------------------------
INSERT INTO SaleItem
(SaleID, ShopID, VariantID, Quantity,
 ProductNameAtSale, UnitPriceAtSale, UnitCostAtSale,
 TotalCostAtSale, TotalPriceAtSale, CurrencyCode)
VALUES
(1, 1, 1, 1, 'Basic T-Shirt', 15, 7, 7, 15, 'EUR'),
(1, 1, 4, 1, 'Cola Drink', 2.5, 1, 1, 2.5, 'EUR'),
(1, 1, 5, 1, 'Chocolate Bar', 1.5, 0.7, 0.7, 1.5, 'EUR');

-------------------------------------------------
-- StockMovement (sales effect)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, UnitCost, ReferenceID, CreatedByUserID, CreatedByUsername)
VALUES
(1, 1, 2, -1, 7, 1, 1, 'admin'),
(1, 4, 2, -1, 1, 1, 1, 'admin'),
(1, 5, 2, -1, 0.7, 1, 1, 'admin');

-------------------------------------------------
-- Payment
-------------------------------------------------
INSERT INTO Payment
(SaleID, Amount, CurrencyCode, PaymentMethod, Status)
VALUES
(1, 32.5, 'EUR', 'CASH', 'PAID');

-------------------------------------------------
-- SaleReturn
-------------------------------------------------
INSERT INTO SaleReturn (SaleID, ShopID, UserID, Reason)
VALUES (1, 1, 1, 'Customer changed mind');

-------------------------------------------------
-- ReturnItem
-------------------------------------------------
INSERT INTO ReturnItem
(SaleReturnID, SaleItemID, QuantityReturned, UnitPriceAtReturn)
VALUES (1, 1, 1, 15);

-------------------------------------------------
-- StockMovement (return)
-------------------------------------------------
INSERT INTO StockMovement
(ShopID, VariantID, MovementTypeID, QuantityChange, UnitCost, ReferenceID, CreatedByUserID, CreatedByUsername)
VALUES
(1, 1, 3, 1, 7, 1, 1, 'admin');

-------------------------------------------------
-- ExpenseCategory
-------------------------------------------------
INSERT INTO ExpenseCategory (Name) VALUES
('Salary'),
('Food'),
('Transportation');

-------------------------------------------------
-- Expense
-------------------------------------------------
INSERT INTO Expense
(ShopID, ExpenseCategoryID, Amount, CurrencyCode, ExpenseDate, CreatedByUserID)
VALUES
(1, 1, 1200, 'EUR', GETDATE(), 1),
(1, 2, 150, 'EUR', GETDATE(), 1);

COMMIT;
