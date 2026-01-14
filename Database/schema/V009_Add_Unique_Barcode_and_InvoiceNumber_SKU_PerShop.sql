--V009_Add_Unique_Barcode_and_InvoiceNumber_SKU_PerShop.sql

BEGIN TRANSACTION;

-- Barcode must be unique per shop
ALTER TABLE ProductVariant
ADD CONSTRAINT UQ_ProductVariant_Shop_Barcode
    UNIQUE (ShopID, Barcode);

-- Invoice number must be unique per shop
ALTER TABLE Sale
ADD CONSTRAINT UQ_Sale_Shop_InvoiceNumber
    UNIQUE (ShopID, InvoiceNumber);

-- SKU unique per shop (nullable, filtered)
CREATE UNIQUE INDEX UX_ProductVariant_Shop_SKU
ON ProductVariant (ShopID, SKU)
WHERE SKU IS NOT NULL;

INSERT INTO SchemaVersion (Version) VALUES (9);

COMMIT;
