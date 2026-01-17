namespace POSsystem.Api.DTOs
{
    public class CreateSaleReturnDto
    {
        public string InvoiceNumber { get; set; }
        public List<ReturnItemDto> Items { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class ReturnItemDto
    {
        public int SaleItemId { get; set; }
        public int QuantityToReturn { get; set; }
    }

}
