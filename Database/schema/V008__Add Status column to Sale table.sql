-- V008: Add Status column to Sale table
BEGIN TRANSACTION;

PRINT 'V008: Adding Status column to Sale table...';

ALTER TABLE Sale
ADD Status NVARCHAR(50) NOT NULL DEFAULT 'COMPLETED';

INSERT INTO SchemaVersion (Version) VALUES (8);
COMMIT;