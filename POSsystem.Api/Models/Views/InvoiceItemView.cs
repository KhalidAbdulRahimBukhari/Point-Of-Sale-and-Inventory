using Microsoft.EntityFrameworkCore;

[Keyless]
public class InvoiceItemView
{
    public int SaleID { get; set; }

    public string Name { get; set; } = null!;
    public string? Size { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
}
