using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarMateTabletOrdering.Models
{
    public class Transaction
    {
        public string kitchenNote { get; set; }

        public decimal Amount { get; set; }
        public int Id { get; set; }
       
        public int TableId { get;  set; }
        public DateTime TransactionDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}