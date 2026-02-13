using Microsoft.EntityFrameworkCore;

[Keyless]
public class InvoiceHeaderView
{
    public int SaleID { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public string CashierUserName { get; set; } = null!;

    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal SaleDiscount { get; set; }
    public decimal GrandTotal { get; set; }

    public decimal AmountPaid { get; set; }
    public decimal ChangeAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
}
