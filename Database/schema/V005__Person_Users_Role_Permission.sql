BEGIN TRANSACTION;
-------------------------------------------------
-- Role
-------------------------------------------------
CREATE TABLE Role (
    RoleID INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL,

    Permissions INT NOT NULL, -- Bitmask (application-defined)

    CONSTRAINT UQ_Role_RoleName UNIQUE (RoleName)
);

-------------------------------------------------
-- Person
-------------------------------------------------
CREATE TABLE Person (
    PersonID INT IDENTITY PRIMARY KEY,

    FullName NVARCHAR(200) NOT NULL,
    NationalNumber NVARCHAR(50) NULL,
    Email NVARCHAR(200) NULL,
    DateOfBirth DATE NULL,

    CountryID INT NULL,
    Gender NVARCHAR(10) NULL,
    Phone NVARCHAR(20) NULL,
    ImagePath NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),


    CONSTRAINT FK_Person_Country
        FOREIGN KEY (CountryID) REFERENCES Country(CountryID)
);

-------------------------------------------------
-- Users
-------------------------------------------------
CREATE TABLE Users (
    UserID INT IDENTITY PRIMARY KEY,

    PersonID INT NOT NULL,
    ShopID INT NOT NULL,
    RoleID INT NOT NULL,

    UserName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(256) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT UQ_Users_UserName
        UNIQUE (UserName),

    CONSTRAINT FK_Users_Person
        FOREIGN KEY (PersonID) REFERENCES Person(PersonID),

    CONSTRAINT FK_Users_Role
        FOREIGN KEY (RoleID) REFERENCES Role(RoleID),

    CONSTRAINT FK_Users_Shop
        FOREIGN KEY (ShopID) REFERENCES Shop(ShopID)
);

-------------------------------------------------
-- Schema Version
-------------------------------------------------
INSERT INTO SchemaVersion (Version) VALUES (5);

COMMIT;
