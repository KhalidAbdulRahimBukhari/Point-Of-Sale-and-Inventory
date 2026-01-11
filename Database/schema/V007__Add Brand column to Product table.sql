-- V007: Added Brand column to Product table
BEGIN TRANSACTION;

PRINT 'V007: Adding Brand column to Product table...';

ALTER TABLE Product
ADD Brand NVARCHAR(100) NULL;

INSERT INTO SchemaVersion (Version) VALUES (7);
COMMIT;