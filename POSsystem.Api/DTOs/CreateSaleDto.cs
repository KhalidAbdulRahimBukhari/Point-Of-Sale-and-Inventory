namespace POSsystem.Api.DTOs
{
    public class CreateSaleDto
    {
        public List<CreateSaleItemDto> Items { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMethod { get; set; }

        public decimal SaleDiscount { get; set; }

        public string? Notes { get; set; }
    }

    public class CreateSaleItemDto
    {
        public string Barcode { get; set; }   // primary
        public string SKU { get; set; }        // fallback
        public int Quantity { get; set; }
    }
}
