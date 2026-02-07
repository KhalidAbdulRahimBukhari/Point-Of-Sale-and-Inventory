create VIEW InvoiceHeaderView AS
SELECT
    s.SaleID,
    s.InvoiceNumber,
    s.CreatedAt,

    u.UserName AS CashierUserName,

    s.SubTotal,
    s.TaxTotal,
    s.SaleDiscount,
    s.GrandTotal,

    ISNULL(p.Amount, 0) AS AmountPaid,
    ISNULL(p.Change, 0) AS ChangeAmount,
    ISNULL(p.PaymentMethod, '') AS PaymentMethod
FROM Sale s
JOIN [Users] u ON u.UserID = s.UserID
JOIN Payment p ON p.SaleID = s.SaleID;


CREATE VIEW InvoiceItemView AS
SELECT
    si.SaleID,

    si.ProductNameAtSale AS Name,
    pv.Size,
    si.UnitPriceAtSale AS Price,
    si.Quantity AS Qty
FROM SaleItem si
JOIN ProductVariant pv ON pv.VariantID = si.VariantID;



