using System;
using System.Collections.Generic;

namespace Doc.ECM.Extension.SyncExample.Models
{
    internal class MyBusinessInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerTaxId { get; set; }
        public int ExternalId { get; set; }

        public List<MyBusinessInvoiceLine> InvoiceLines { get; set; }
    }
}
