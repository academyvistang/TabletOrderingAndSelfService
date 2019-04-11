using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarMateTabletOrdering.Models
{
   
    public class PaymentsSoldItems
    {
        public Payment Payment { get; set; }

        public List<SoldItem> SoldItems { get; set; }
    }
    public class SoldItemGB
    {
        public List<SoldItem> Items { get; set; }
        public string PaymentMethodName { get; set; }
    }

    public class SoldItem
    {

        public int Id { get; set; }

        public int Qty { get; set; }
        public decimal TotalPrice { get; set; }



        public string ReceiptNumber { get; set; }
        public string Name { get; set; }
        public DateTime DateSold { get;  set; }
        public string StockItemName { get;  set; }
        public string PaymentMethodName { get;  set; }
    }
        public class Payment
    {
       
        public int Id { get; set; }

        public string ReceiptNumber { get; set; }


        public decimal Total { get; set; }

        public decimal TotalCash { get; set; }
        public decimal TotalPOS { get; set; }
        public decimal TotalTransfer { get; set; }
        public decimal TotalCredit { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentMethodId { get; set; }
    }
    public class DailyAccount
    {
        public DateTime ClosureStartDate { get; set; }
        public DateTime ClosureEndDate { get; set; }
        public int Id { get; set; }

        public int StaffId { get; set; }


        public decimal Total { get; set; }

        public decimal TotalCash { get; set; }
        public decimal TotalPos { get; set; }
        public decimal TotalTransfer { get; set; }
        public decimal TotalCredit { get; set; }
    }
}