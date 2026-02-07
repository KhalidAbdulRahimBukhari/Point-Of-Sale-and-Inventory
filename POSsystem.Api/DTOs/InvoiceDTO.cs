namespace POSsystem.Api.DTOs
{
    public class InvoiceItemDto
    {
        public string Name { get; set; } = null!;
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }


    public class InvoiceDto
    {
        public string InvoiceNo { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Cashier { get; set; } = null!;

        public List<InvoiceItemDto> Items { get; set; } = new();

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public string PaymentMethod { get; set; } = "";
    }

}
