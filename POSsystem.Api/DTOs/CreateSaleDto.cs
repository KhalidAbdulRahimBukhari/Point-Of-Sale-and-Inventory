namespace POSsystem.Api.DTOs
{
    public class SaleDraftDto
    {
        public int ShopId { get; set; }
        public string CurrencyCode { get; set; } = "USD";

        public CashierDto Cashier { get; set; } = null!;

        public List<SaleItemDraftDto> Items { get; set; } = [];

        public SaleTotalsDto Totals { get; set; } = null!;

        public PaymentDraftDto? Payment { get; set; }

        public string CreatedAt { get; set; } = "";
    }

    public class CashierDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
    }


    public class SaleItemDraftDto
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }

        public string Name { get; set; } = "";
        public string? Size { get; set; }
        public string? Color { get; set; }

        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }

        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
    }

    public class SaleTotalsDto
    {
        public decimal SubTotal { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }


    public class PaymentDraftDto
    {
        public string Method { get; set; } = "";
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public string? Notes { get; set; }
    }



}
