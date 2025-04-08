namespace Doc.ECM.Extension.SyncExample.Models
{
    internal class MyBusinessInvoiceLine
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProductId { get; set; }
        public int InvoiceId { get; set; }
    }
}
