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
