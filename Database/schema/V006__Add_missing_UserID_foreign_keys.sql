-- V006: Add missing UserID foreign keys
BEGIN TRANSACTION;

PRINT 'V006: Adding UserID foreign key constraints...';

ALTER TABLE Sale
ADD CONSTRAINT FK_Sale_User
    FOREIGN KEY (UserID) REFERENCES Users(UserID);

ALTER TABLE SaleReturn
ADD CONSTRAINT FK_SaleReturn_User
    FOREIGN KEY (UserID) REFERENCES Users(UserID);

ALTER TABLE Expense
ADD CONSTRAINT FK_Expense_User
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID);

ALTER TABLE StockMovement
ADD CONSTRAINT FK_StockMovement_User
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID);

INSERT INTO SchemaVersion (Version) VALUES (6);
COMMIT;