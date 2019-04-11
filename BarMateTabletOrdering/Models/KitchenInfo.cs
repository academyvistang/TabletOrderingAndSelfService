using POSService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarMateTabletOrdering.Models
{
    
    public class TableItemGroup
    {
        public string PrinterName { get; set; }

        public List<TableItem> TableItems { get; set; }
    }

    public class KitchenInfo
    {
        public int KitchenID { get; set; }

        public string Notes { get; set; }
    }

    public class LatestGroupByQty
    {
        public decimal StockItemPrice { get; set; } 

        public int Qty { get; set; }

        public int TableItemId { get; set; } 

        public int StockItemId { get; set; }

        public string ItemNote { get; set; }

        public bool DepartmentReceived { get; set; }

        public string ItemNotes { get; set; }

        public List<TableItem> RealItemList { get; set; }
        public string StockItemName { get; set; }

        public string CategoryName { get; set; } 


        public string Note { get; set; }

        public bool Takeaway { get; set; }

        public bool PaidInFull { get; set; }


        public DateTime DateSold { get; set; }
    }

    public class LatestGroupByModel
    {
        public bool DepartmentRecieved { get; set; }
        public DateTime Datesold { get; set; }

        public List<TableItem> Items { get; set; }

        public string DatesoldStr { get; set; }

        public string ValueIds { get; set; }

        public string ItemNotes { get; set; }
    }

    public class KitchenModel
    {
        public BarTable BarTab { get; set; }

        public List<TableItem> List { get; set; }

        public string ValueIds { get; set; }

        public IEnumerable<KitchenModelGroupByDateSold> DateSoldList { get; set; }

        public IEnumerable<KitchenModel> NewList { get; set; }

        public string Note { get; set; }
        public List<TableItem> RealItems { get; set; }
        public bool Takeaway { get; set; }
        public bool PaidInFull { get; set; }
    }

    public class KitchenModelGroupByDateSold
    {
        public BarTable BarTab { get; set; }

        public List<TableItem> List { get; set; }

        public string ValueIds { get; set; }

        public DateTime ExactDateSold { get; set; }
    }
}