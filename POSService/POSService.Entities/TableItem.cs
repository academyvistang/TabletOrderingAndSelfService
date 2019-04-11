using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSService.Entities
{
    public class TableItem
    {

        public bool PaidInFull { get; set; } 

        public bool Takeaway { get; set; }

        public int StaffId { get; set; }

        public int StockItemId { get; set; }
        public string StockItemName { get; set; }

        public string IndividualNote { get; set; }

        public string ItemNote { get; set; }

        public string CategoryName { get; set; }


        public string ProductDepartment { get; set; }


        public string StockItemPicture { get; set; }


        public string TableName { get; set; } 

        public decimal StockItemPrice { get; set; }

        public int Id { get; set; }
        public int TableId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public System.DateTime DateSold { get; set; }
        public int Cashier { get; set; }
        public Nullable<int> GuestOrderItemId { get; set; }
        public bool Fulfilled { get; set; }
        public bool Collected { get; set; }
        public Nullable<System.DateTime> CompletedTime { get; set; }
        public Nullable<System.DateTime> CollectedTime { get; set; }
        public bool Completed { get; set; }
        public bool StoreFulfilled { get; set; }
        public Nullable<System.DateTime> StoreFulfilledTime { get; set; }
        public bool SentToPOS { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool SentToPrinter { get; set; }
        public string GuestGuid { get; set; }
    }
}
