-- ============================================
-- V011_Add_ProductVariant_Notes.sql
-- Adds optional notes to product variants
-- ============================================

BEGIN TRANSACTION;

ALTER TABLE ProductVariant
ADD Notes NVARCHAR(500) NULL;

PRINT 'Added column: ProductVariant.Notes';

INSERT INTO SchemaVersion (Version) VALUES (11);

COMMIT;
