using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POSService.Entities;

namespace BarMateTabletOrdering.Models
{
    public class PrintStockItemModel
    {
        public int Quantity { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime DateSold { get; set; }
    }

    public class CabbashViewModel
    {
        public int TableId { get; set; }

        public int SwapToTableId { get; set; }

        public int SwapTableId { get; set; }

        public int[] LeftTableIds { get; set; }

        public int[] RightTableIds { get; set; }




        //LoyaltyTelephone
        public string LoyaltyTelephone { get; set; }

        public List<string> CategoriesListString { get; set; }
        public int? CategoryId { get; set; }
        public List<string> DisplayImages { get; set; }
        public IOrderedEnumerable<StockItem> ProductsList { get; set; }
        public List<BarTable> TableNumbers { get; set; }
        public string TableAlias { get; set; }
        public List<BarTable> AllOpenTables { get; set; }
        public List<TableItem> ExistingList { get; set; }
        public bool CanTakePayment { get; set; }
        public bool CanCancelSale { get; set; }
        public int TotalItems { get; set; }
        public decimal Total { get; set; }
        public List<KitchenModel> Kitchenlist { get; set; }
        public int PlaceId { get;  set; }
        public string PicturePath { get; set; }
        public string NameOfPlace { get; set; }
        public List<TableItem> PurchasedList { get; set; }
        public string Description { get; set; }
        public string ItemName { get; set; }
        public string QRCodeImage { get; set; }
        public bool? InvalidCred { get; set; }
        public string TablesClashing { get; set; }
        public bool? TableClash { get; set; }
        public MultiSelectList MultiSelect { get; set; }
        public TableItem SingleItem { get; set; }
        public List<string> QuickSearchString { get; set; }
        public List<PaymentsSoldItems> AllPayments { get;  set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool WeHaveOpenTables { get; set; }
        public int SelectedTableId { get; set; }
        public List<BarTable> OccupiedTables { get; set; }
        public string OpenedTableName { get;  set; }
        public bool CanSendToDepartment { get; set; }
        public bool CanSendReprocess { get; set; }
        public bool CanDoProcess { get; set; }

    }
}