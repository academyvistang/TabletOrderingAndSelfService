using BarMateTabletOrdering.Helpers;
using BarMateTabletOrdering.Models;
using Microsoft.PointOfService;
using POSService;
using POSService.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BarMateTabletOrdering.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        //InsertTable

        //SendToPosPrinter(lst, bt.TableAlias, true, guestOrderId, guestOrderNote);
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            //Log the error!!
            //_Logger.Error(filterContext.Exception);

            //Redirect or return a view, but not both.
            //filterContext.Result = RedirectToAction("Index", "ErrorHandler");
            // OR 
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/ErrorHandler/Index.cshtml"
            };
        }

        protected IEnumerable<TableItem> ProcessWaiterOrderOutput(int tableId, int cashierId, string note, int placeId)
        {
            IEnumerable<TableItem> enumerable;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("ProcessWaiterOrderNewWithPrinterPara2", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@CashierId", cashierId);
                    command.Parameters.AddWithValue("@TableId", tableId);
                    command.Parameters.AddWithValue("@Note", note);
                    command.Parameters.AddWithValue("@PlaceId", placeId);
                    SqlDataReader reader = command.ExecuteReader();
                    enumerable = Enumerable.ToArray<TableItem>(this.ReadTableItemsPara(reader));
                }
            }
            return enumerable;
        }

        protected int ProcessWaiterOrderReady(int tableId, int cashierId, string note, int placeId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("ProcessWaiterOrderReady", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@CashierId", cashierId);
                    command.Parameters.AddWithValue("@TableId", tableId);
                    command.Parameters.AddWithValue("@Note", note);
                    command.Parameters.AddWithValue("@PlaceId", placeId);
                    command.ExecuteNonQuery();
                }
            }
            return 0;
        }

 

        protected void SendErrorToPrinter(string printerName, string error, string orderNo, string tableName)
        {
            string userData = (base.HttpContext.User.Identity as FormsIdentity).Ticket.UserData;
            DateTime? nullable = new DateTime?(DateTime.Now);
            this.PrintLineItemRawKitchen(printerName, error);
            string itemCode = this.TruncateAt(nullable.Value.ToShortTimeString().PadRight(0x15), 0x15) + this.TruncateAt(nullable.Value.ToShortDateString().PadLeft(10), 10);
            this.PrintLineItemRawKitchen(printerName, itemCode);
            string str3 = this.TruncateAt("ORDER NO.".PadRight(0x15), 0x15) + this.TruncateAt(orderNo.PadLeft(10), 10);
            this.PrintLineItemRawKitchen(printerName, str3);
            string str4 = this.TruncateAt(tableName.PadRight(0x15), 0x15) + this.TruncateAt(userData.PadLeft(10), 10);
            this.PrintLineItemRawKitchen(printerName, str4);
            this.PrintTextLineRaw(printerName, string.Empty);
            this.PrintTextLineRaw(printerName, string.Empty);
            RawPrinterHelper.FullCut(printerName);
        }

        protected int ReProcessWaiterOrderReady(int tableId, int cashierId, string note, int placeId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("ProcessWaiterOrderReadyReProcess", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    command.Parameters.AddWithValue("@CashierId", cashierId);
                    command.Parameters.AddWithValue("@TableId", tableId);
                    command.Parameters.AddWithValue("@Note", note);
                    command.Parameters.AddWithValue("@PlaceId", placeId);
                    command.ExecuteNonQuery();
                }
            }
            return 0;
        }

        protected int SendToPosPrinter(string printerName, int personId, List<StockItem> lst, string tableName, bool pleasePrint, int? guestOrderId, DateTime? timeOfOrder, string guestOrderNote = "", string orderNo = "")
        {
            ArrayList list = new ArrayList();
            ArrayList list1 = new ArrayList();
            ArrayList list2 = new ArrayList();
            string userData = (base.HttpContext.User.Identity as FormsIdentity).Ticket.UserData;
            decimal num = 0M;
            string str2 = this.TruncateAt(timeOfOrder.Value.ToShortTimeString().PadRight(0x15), 0x15) + this.TruncateAt(timeOfOrder.Value.ToShortDateString().PadLeft(10), 10);
            list.Add(str2);
            list.Add(this.TruncateAt(tableName.PadRight(0x15), 0x15) + this.TruncateAt(userData.PadLeft(10), 10));
            list.Add(this.TruncateAt("ORDER NO.".PadRight(0x15), 0x15) + this.TruncateAt(orderNo.PadLeft(10), 10));
            if (!string.IsNullOrEmpty(guestOrderNote))
            {
                list.Add(guestOrderNote);
            }
            list.Add("============");
            foreach (PrintStockItemModel model in Enumerable.ToList<PrintStockItemModel>(Enumerable.Select<IGrouping<long, StockItem>, PrintStockItemModel>((IEnumerable<IGrouping<long, StockItem>>)(from x in lst group x by x.Id), delegate (IGrouping<long, StockItem> x) {
                PrintStockItemModel model1 = new PrintStockItemModel();
                PrintStockItemModel model2 = new PrintStockItemModel();
                model2.Quantity = Enumerable.Sum<StockItem>((IEnumerable<StockItem>)x, y => y.Quantity);
                PrintStockItemModel local2 = model2;
                local2.Description = Enumerable.FirstOrDefault<StockItem>((IEnumerable<StockItem>)x).Description;
                local2.UnitPrice = Enumerable.FirstOrDefault<StockItem>((IEnumerable<StockItem>)x).UnitPrice;
                return local2;
            })))
            {
                decimal num4 = model.UnitPrice * model.Quantity;
                num += num4;
                if (!string.IsNullOrEmpty(model.Description))
                {
                    int length = model.Description.Length;
                    if (length < 0x21)
                    {
                        list.Add(this.TruncateAt(model.Description.PadRight(length), length) + this.TruncateAt(model.Quantity.ToString().PadLeft(3), 3));
                        continue;
                    }
                    list.Add(this.TruncateAt(model.Description.PadRight(0x22), 0x22) + this.TruncateAt(model.Quantity.ToString().PadLeft(3), 3));
                }
            }
            int num2 = 0;
            double result = 0.0;
            double.TryParse(num.ToString(), out result);
            int hashCode = DateTime.Now.ToString().GetHashCode();
            hashCode.ToString("x");

            if (printerName.StartsWith("Star"))
            {
                return 0;
            }
            else
            {
                string printer = printerName;
                try
                {
                    if (list[0] != list[1])
                    {
                        Hashtable hashtable = new Hashtable();

                        IEnumerator enumerator2 = list.GetEnumerator();
                        
                            while (true)
                            {
                                if (!enumerator2.MoveNext())
                                {
                                    break;
                                }
                                object current = enumerator2.Current;
                                try
                                {
                                    hashtable.Add(current.ToString(), current.ToString());
                                    this.PrintLineItemRawKitchen(printer, current.ToString());
                                    num2++;
                                    continue;
                                }
                                catch
                                {
                                    hashCode = 0;
                                }

                                return hashCode;
                            }
                        
                        this.PrintTextLineRaw(printer, string.Empty);
                        this.PrintTextLineRaw(printer, string.Empty);
                        RawPrinterHelper.FullCut(printer);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                }
                //goto TR_0000;
            }
            //return hashCode;
            return (num2 - 5);
        }

        /*Id    INT,
  ItemId INT,
  Qty   INT,
  TableId      INT,
  StockItemName VARCHAR(500),
  StockItemId INT,
  ProductDepartment VARCHAR(500)*/

        public IEnumerable<TableItem> ReadTableItemsPara(SqlDataReader reader)
        {

            while (reader.Read())
            {
                yield return new TableItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TableId = reader.IsDBNull(reader.GetOrdinal("TableId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TableId")),
                    ItemId = reader.IsDBNull(reader.GetOrdinal("ItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ItemId")),
                    StockItemId = reader.IsDBNull(reader.GetOrdinal("StockItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StockItemId")),
                    StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")),
                    Qty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),


                    //Cashier = reader.IsDBNull(reader.GetOrdinal("Cashier")) ? 0 : reader.GetInt32(reader.GetOrdinal("Cashier")),
                    //Qty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                    //DateSold = reader.IsDBNull(reader.GetOrdinal("DateSold")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateSold")),
                    //Collected = reader.IsDBNull(reader.GetOrdinal("Collected")) ? false : reader.GetBoolean(reader.GetOrdinal("Collected")),
                    //Fulfilled = reader.IsDBNull(reader.GetOrdinal("Fulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("Fulfilled")),
                    //Completed = reader.IsDBNull(reader.GetOrdinal("Completed")) ? false : reader.GetBoolean(reader.GetOrdinal("Completed")),
                    //StoreFulfilled = reader.IsDBNull(reader.GetOrdinal("StoreFulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("StoreFulfilled")),
                    //IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    //SentToPOS = reader.IsDBNull(reader.GetOrdinal("SentToPOS")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPOS")),
                    //SentToPrinter = reader.IsDBNull(reader.GetOrdinal("SentToPrinter")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPrinter")),
                    //Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note")),
                    //StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")),
                    //StockItemPrice = reader.IsDBNull(reader.GetOrdinal("StockItemPrice")) ? decimal.Zero : reader.GetDecimal(reader.GetOrdinal("StockItemPrice")),
                    //StockItemPicture = reader.IsDBNull(reader.GetOrdinal("StockItemPicture")) ? "" : reader.GetString(reader.GetOrdinal("StockItemPicture")),
                    ProductDepartment = reader.IsDBNull(reader.GetOrdinal("ProductDepartment")) ? "" : reader.GetString(reader.GetOrdinal("ProductDepartment"))
                };
            };
        }

        public IEnumerable<StockItem> GetStockItemsPOSNoAsync()
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetStockItemsPOS", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadStockItems(reader).ToArray();

                }
            }
        }



        public async Task<IEnumerable<StockItem>> GetStockItemsPOS()
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetStockItemsPOS", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadStockItems(reader).ToArray();

                }
            }
        }



        private IEnumerable<StockItem> ReadStockItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new StockItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                    UnitPrice = reader.IsDBNull(reader.GetOrdinal("UnitPrice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                    StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")),
                    IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "" : reader.GetString(reader.GetOrdinal("CategoryName")),
                    ProductDepartmentId = reader.IsDBNull(reader.GetOrdinal("ProductDepartmentId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProductDepartmentId")),
                    DistributionPointId = reader.IsDBNull(reader.GetOrdinal("DistributionPointId")) ? 0 : reader.GetInt32(reader.GetOrdinal("DistributionPointId")),
                    Remaining = reader.IsDBNull(reader.GetOrdinal("Remaining")) ? 0 : reader.GetInt32(reader.GetOrdinal("Remaining"))

                };
            };
        }

        
        public async Task<IEnumerable<BarTable>> GetMyTables(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTablesWithTotal", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadBarTableItems(reader).ToArray();

                }
            }

        }
        
        public async Task<IEnumerable<BarTable>> GetMyTablesWhatever(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTablesWhateverTotal", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadBarTableItemsTotal(reader).ToArray();

                }
            }

        }
        public async Task<IEnumerable<BarTable>> GetBarTableByTableId(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetBarTableByTableId", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadBarTableItems(reader).ToArray();

                }
            }
        }

        public IEnumerable<BarTable> GetBarTableByIdNoAsync(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetBarTableById", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadBarTableItems(reader).ToArray();

                }
            }

        }

        public async Task<IEnumerable<BarTable>> GetBarTableById(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetBarTableById", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadBarTableItems(reader).ToArray();

                }
            }

        }

        public async Task<IEnumerable<BarTable>> GetMyTablesBase(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTablesWithTotal", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadBarTableItemsTotal(reader).ToArray();

                }
            }

        }

        /*TableItem.[Id]
      ,TableItem.[TableId]
      ,TableItem.[ItemId]
      ,TableItem.[Qty]
      ,TableItem.[DateSold]
      ,TableItem.[Cashier]
      ,TableItem.[GuestOrderItemId]
      ,TableItem.[Fulfilled]
      ,TableItem.[Collected]
      ,TableItem.[CompletedTime]
      ,TableItem.[CollectedTime]
      ,TableItem.[Completed]
      ,TableItem.[StoreFulfilled]
      ,TableItem.[StoreFulfilledTime]
      ,TableItem.[SentToPOS]
      ,TableItem.[Note]
      ,TableItem.[IsActive]
      ,TableItem.[SentToPrinter]
	  ,SI.Id AS StockItemId
	  ,SI.StockItemName AS StockItemName
	  ,SI.UnitPrice AS StockItemPrice
	  ,SI.PicturePath AS StockItemPicture
	  ,SI.StarBuy
	  ,pd.[Description] AS ProductDepartment*/

        /* BarTable.[Id]
      ,BarTable.[TableName]
      ,BarTable.[TableAlias]
      ,BarTable.[IsActive]
      ,BarTable.[GuestId]
      ,BarTable.[TableId]
      ,BarTable.[StaffId]
      ,BarTable.[CreatedDate]
      ,BarTable.[Printed]
      ,BarTable.[GuestGuid]
	  ,Sum(ti.Qty * si.UnitPrice) AS TableTotal*/

        private IEnumerable<BarTable> ReadBarTableItemsTotal(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new BarTable
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TableName = reader.IsDBNull(reader.GetOrdinal("TableName")) ? "" : reader.GetString(reader.GetOrdinal("TableName")),
                    TableAlias = reader.IsDBNull(reader.GetOrdinal("TableAlias")) ? "" : reader.GetString(reader.GetOrdinal("TableAlias")),
                    IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    GuestId = reader.IsDBNull(reader.GetOrdinal("GuestId")) ? 0 : reader.GetInt32(reader.GetOrdinal("GuestId")),
                    TableId = reader.IsDBNull(reader.GetOrdinal("TableId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TableId")),
                    StaffId = reader.IsDBNull(reader.GetOrdinal("StaffId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StaffId")),
                    CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    Printed = reader.IsDBNull(reader.GetOrdinal("Printed")) ? false : reader.GetBoolean(reader.GetOrdinal("Printed")),
                    GuestGuid = reader.IsDBNull(reader.GetOrdinal("GuestGuid")) ? "" : reader.GetString(reader.GetOrdinal("GuestGuid")),
                    TableTotal = reader.IsDBNull(reader.GetOrdinal("TableTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TableTotal"))
                };
            };
        }
        private IEnumerable<BarTable> ReadBarTableItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new BarTable
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TableName = reader.IsDBNull(reader.GetOrdinal("TableName")) ? "" : reader.GetString(reader.GetOrdinal("TableName")),
                    TableAlias = reader.IsDBNull(reader.GetOrdinal("TableAlias")) ? "" : reader.GetString(reader.GetOrdinal("TableAlias")),
                    IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    GuestId = reader.IsDBNull(reader.GetOrdinal("GuestId")) ? 0 : reader.GetInt32(reader.GetOrdinal("GuestId")),
                    TableId = reader.IsDBNull(reader.GetOrdinal("TableId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TableId")),
                    StaffId = reader.IsDBNull(reader.GetOrdinal("StaffId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StaffId")),
                    CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    Printed = reader.IsDBNull(reader.GetOrdinal("Printed")) ? false : reader.GetBoolean(reader.GetOrdinal("Printed")),
                    GuestGuid = reader.IsDBNull(reader.GetOrdinal("GuestGuid")) ? "" : reader.GetString(reader.GetOrdinal("GuestGuid")),
                    GuestNumber = reader.IsDBNull(reader.GetOrdinal("GuestNumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("GuestNumber"))
                };
            };
        }

        
        public async Task<IEnumerable<TableItem>> GetKitchenItemsByRole(string role)
        {

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetKitchenItemsByRole", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Role", role);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItemsRole(reader).ToArray();

                }
            }

        }

        public async Task<IEnumerable<TableItem>> GetKitchenItemsByRoleId(int roleId)
        {

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetKitchenItemsByRoleId", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItemsRole(reader).ToArray();

                }
            }

        }


        public async Task<IEnumerable<TableItem>> GetMyTableItemsByTableId(int tableId)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTableItemsByTableId", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@TableId", tableId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItems(reader).ToArray();

                }
            }

        }


        public async Task<IEnumerable<TableItem>> GetMyTableItems(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTableItems", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItems(reader).ToArray();

                }
            }
        }

        public IEnumerable<TableItem> GetMyTableItemsByTableIdAllNoAsync(int tableId)
        {

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTableItemsByTableIdAll", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@TableId", tableId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadTableItemsPD(reader).ToArray();

                }
            }

        }

        public async Task<IEnumerable<TableItem>> GetMyTableItemsByTableIdAll(int tableId)
        {

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTableItemsByTableIdAll", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@TableId", tableId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItemsPD(reader).ToArray();

                }
            }

        }

        

        public async Task<IEnumerable<TableItem>> GetTableItemsByTableTel(string telephone)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetTableItemsByTableTel", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Telephone", telephone);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItems(reader).ToArray();

                }
            }

        }

        public async Task<IEnumerable<TableItem>> GetTableItemsByTableId(int tableId)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetMyTableItemsByTableId", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@TableId", tableId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadTableItems(reader).ToArray();

                }
            }

        }
        public async Task<IEnumerable<SoldItem>> GetSoldItems(DateTime startDate, DateTime endDate)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetSoldItemsAudit", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    SqlDataReader reader = null;

                    try
                    {
                        reader = await cmd.ExecuteReaderAsync();
                    }
                    catch(Exception)
                    {
                       
                    }

                    return ReadSoldItemsItems(reader).ToArray();
                }
            }

        }

        private IEnumerable<SoldItem> ReadSoldItemsItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new SoldItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TotalPrice = reader.IsDBNull(reader.GetOrdinal("TotalPrice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                    ReceiptNumber = reader.IsDBNull(reader.GetOrdinal("RecieptNumber")) ? "" : reader.GetString(reader.GetOrdinal("RecieptNumber")),
                    StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")), 
                    PaymentMethodName = reader.IsDBNull(reader.GetOrdinal("PaymentMethodName")) ? "" : reader.GetString(reader.GetOrdinal("PaymentMethodName")), 
                    Qty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                    DateSold = reader.IsDBNull(reader.GetOrdinal("DateSold")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateSold"))
                };
            };
        }

        public async Task<IEnumerable<Payment>> GetPayments(DateTime startDate, DateTime endDate)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetPaymentDetails", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    var reader = await cmd.ExecuteReaderAsync();

                    return ReadPaymentItems(reader).ToArray();
                }
            }

        }

        private IEnumerable<Payment> ReadPaymentItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new Payment
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    PaymentMethodId = reader.GetInt32(reader.GetOrdinal("PaymentMethodId")),
                    ReceiptNumber = reader.IsDBNull(reader.GetOrdinal("ReceiptNumber")) ? "" : reader.GetString(reader.GetOrdinal("ReceiptNumber")),
                    Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Total")),
                    TotalCash = reader.IsDBNull(reader.GetOrdinal("TotalCash")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalCash")),
                    TotalCredit = reader.IsDBNull(reader.GetOrdinal("TotalCredit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalCredit")),
                    TotalPOS = reader.IsDBNull(reader.GetOrdinal("TotalPOS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalPOS")),
                    TotalTransfer = reader.IsDBNull(reader.GetOrdinal("TotalTransfer")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalTransfer")),
                    PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
                };
            };
        }

        public async Task<IEnumerable<DailyAccount>> GetLastAudit()
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetlastAudit", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    return ReadAuditItems(reader).ToArray();
                }
            }

        }

        private IEnumerable<DailyAccount> ReadAuditItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new DailyAccount
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    StaffId = reader.IsDBNull(reader.GetOrdinal("StaffId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StaffId")),
                    Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Total")),
                    TotalCash = reader.IsDBNull(reader.GetOrdinal("TotalCash")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalCash")),
                    TotalCredit = reader.IsDBNull(reader.GetOrdinal("TotalCredit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalCredit")),
                    TotalPos = reader.IsDBNull(reader.GetOrdinal("TotalPos")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalPos")),
                    TotalTransfer = reader.IsDBNull(reader.GetOrdinal("TotalTransfer")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalTransfer")),

                    ClosureStartDate = reader.IsDBNull(reader.GetOrdinal("ClosureStartDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("ClosureStartDate")),
                    ClosureEndDate = reader.IsDBNull(reader.GetOrdinal("ClosureEndDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("ClosureEndDate"))
                };
            };
        }

        //public async Task<IEnumerable<TableItem>> GetTableItems(int placeId)
        //{
        //    using (SqlConnection myConnection = new SqlConnection(GetConnectionStringCabbash()))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("GetTableItems", myConnection))
        //        {
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //            await myConnection.OpenAsync();

        //            cmd.Parameters.AddWithValue("@PlaceId", placeId);

        //            var reader = await cmd.ExecuteReaderAsync();

        //            return ReadTableItems(reader).ToArray();
        //        }
        //    }

        //}

        /*
      ,TableItem.[Fulfilled]
      ,TableItem.[Collected]
      ,TableItem.[CompletedTime]
      ,TableItem.[CollectedTime]
      ,TableItem.[Completed]
      ,TableItem.[StoreFulfilled]
      ,TableItem.[StoreFulfilledTime]
      ,TableItem.[SentToPOS]
      ,TableItem.[Note]
      ,TableItem.[IsActive]
      ,TableItem.[SentToPrinter]
	  ,SI.Id AS StockItemId
	  ,SI.StockItemName AS StockItemName
	  ,SI.UnitPrice AS StockItemPrice
	  ,SI.PicturePath AS StockItemPicture
	  ,pd.[Description] AS ProductDepartment*/

        public IEnumerable<TableItem> ReadTableItemsPD(SqlDataReader reader)
        {

            while (reader.Read())
            {
                yield return new TableItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TableId = reader.IsDBNull(reader.GetOrdinal("TableId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TableId")),
                    ItemId = reader.IsDBNull(reader.GetOrdinal("ItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ItemId")),
                    StockItemId = reader.IsDBNull(reader.GetOrdinal("StockItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StockItemId")),
                    Cashier = reader.IsDBNull(reader.GetOrdinal("Cashier")) ? 0 : reader.GetInt32(reader.GetOrdinal("Cashier")),
                    Qty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                    DateSold = reader.IsDBNull(reader.GetOrdinal("DateSold")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateSold")),
                    Collected = reader.IsDBNull(reader.GetOrdinal("Collected")) ? false : reader.GetBoolean(reader.GetOrdinal("Collected")),
                    Fulfilled = reader.IsDBNull(reader.GetOrdinal("Fulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("Fulfilled")),
                    Completed = reader.IsDBNull(reader.GetOrdinal("Completed")) ? false : reader.GetBoolean(reader.GetOrdinal("Completed")),
                    StoreFulfilled = reader.IsDBNull(reader.GetOrdinal("StoreFulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("StoreFulfilled")),
                    IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    SentToPOS = reader.IsDBNull(reader.GetOrdinal("SentToPOS")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPOS")),
                    SentToPrinter = reader.IsDBNull(reader.GetOrdinal("SentToPrinter")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPrinter")),
                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note")),
                    StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")),
                    StockItemPrice = reader.IsDBNull(reader.GetOrdinal("StockItemPrice")) ? decimal.Zero : reader.GetDecimal(reader.GetOrdinal("StockItemPrice")),
                    StockItemPicture = reader.IsDBNull(reader.GetOrdinal("StockItemPicture")) ? "" : reader.GetString(reader.GetOrdinal("StockItemPicture")),
                    ProductDepartment = reader.IsDBNull(reader.GetOrdinal("ProductDepartment")) ? "" : reader.GetString(reader.GetOrdinal("ProductDepartment"))
                };
            };
        }

        public IEnumerable<TableItem> ReadTableItemsRole(SqlDataReader reader)
        {

            while (reader.Read())
            {
                yield return new TableItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TableId = reader.IsDBNull(reader.GetOrdinal("TableId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TableId")),
                    ItemId = reader.IsDBNull(reader.GetOrdinal("ItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ItemId")),
                    StockItemId = reader.IsDBNull(reader.GetOrdinal("StockItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StockItemId")),
                    Qty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                    DateSold = reader.IsDBNull(reader.GetOrdinal("DateSold")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateSold")),
                    Cashier = reader.IsDBNull(reader.GetOrdinal("Cashier")) ? 0 : reader.GetInt32(reader.GetOrdinal("Cashier")),
                    Collected = reader.IsDBNull(reader.GetOrdinal("Collected")) ? false : reader.GetBoolean(reader.GetOrdinal("Collected")),
                    CompletedTime = reader.IsDBNull(reader.GetOrdinal("CompletedTime")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CompletedTime")),
                    CollectedTime = reader.IsDBNull(reader.GetOrdinal("CollectedTime")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CollectedTime")),
                    Fulfilled = reader.IsDBNull(reader.GetOrdinal("Fulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("Fulfilled")),
                    Completed = reader.IsDBNull(reader.GetOrdinal("Completed")) ? false : reader.GetBoolean(reader.GetOrdinal("Completed")),
                    StoreFulfilled = reader.IsDBNull(reader.GetOrdinal("StoreFulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("StoreFulfilled")),
                    IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    SentToPOS = reader.IsDBNull(reader.GetOrdinal("SentToPOS")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPOS")),
                    SentToPrinter = reader.IsDBNull(reader.GetOrdinal("SentToPrinter")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPrinter")),
                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note")),
                    StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")),
                    StockItemPrice = reader.IsDBNull(reader.GetOrdinal("StockItemPrice")) ? decimal.Zero : reader.GetDecimal(reader.GetOrdinal("StockItemPrice")),
                    TableName = reader.IsDBNull(reader.GetOrdinal("TableName")) ? "" : reader.GetString(reader.GetOrdinal("TableName")),
                    Takeaway = reader.IsDBNull(reader.GetOrdinal("Takeaway")) ? false : reader.GetBoolean(reader.GetOrdinal("Takeaway")),
                    StaffId = reader.IsDBNull(reader.GetOrdinal("StaffId")) ? 0 : reader.GetInt32(reader.GetOrdinal("StaffId")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "" : reader.GetString(reader.GetOrdinal("CategoryName")),
                    ItemNote = reader.IsDBNull(reader.GetOrdinal("ItemNote")) ? "" : reader.GetString(reader.GetOrdinal("ItemNote"))
                };
            };
        }

        

        public IEnumerable<TableItem> ReadTableItems(SqlDataReader reader)
        {

            while (reader.Read())
            {
                yield return new TableItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TableId = reader.IsDBNull(reader.GetOrdinal("TableId")) ? 0 : reader.GetInt32(reader.GetOrdinal("TableId")),
                    ItemId = reader.IsDBNull(reader.GetOrdinal("ItemId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ItemId")),
                    Qty = reader.IsDBNull(reader.GetOrdinal("Qty")) ? 0 : reader.GetInt32(reader.GetOrdinal("Qty")),
                    DateSold = reader.IsDBNull(reader.GetOrdinal("DateSold")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DateSold")),
                    Collected = reader.IsDBNull(reader.GetOrdinal("Collected")) ? false : reader.GetBoolean(reader.GetOrdinal("Collected")),
                    Fulfilled = reader.IsDBNull(reader.GetOrdinal("Fulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("Fulfilled")),
                    Completed = reader.IsDBNull(reader.GetOrdinal("Completed")) ? false : reader.GetBoolean(reader.GetOrdinal("Completed")),
                    StoreFulfilled = reader.IsDBNull(reader.GetOrdinal("StoreFulfilled")) ? false : reader.GetBoolean(reader.GetOrdinal("StoreFulfilled")),
                    IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    SentToPOS = reader.IsDBNull(reader.GetOrdinal("SentToPOS")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPOS")),
                    SentToPrinter = reader.IsDBNull(reader.GetOrdinal("SentToPrinter")) ? false : reader.GetBoolean(reader.GetOrdinal("SentToPrinter")),
                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note")),
                    StockItemName = reader.IsDBNull(reader.GetOrdinal("StockItemName")) ? "" : reader.GetString(reader.GetOrdinal("StockItemName")),
                    StockItemPrice = reader.IsDBNull(reader.GetOrdinal("StockItemPrice")) ? decimal.Zero : reader.GetDecimal(reader.GetOrdinal("StockItemPrice"))


                    //GuestGuid = reader.IsDBNull(reader.GetOrdinal("GuestGuid")) ? "" : reader.GetString(reader.GetOrdinal("GuestGuid"))
                };
            };
        }

        protected void SendToPosPrinterVoids(string printerName, List<POSService.Entities.StockItem> lst, string tableName, DateTime? timeOfOrder)
        {
            ArrayList ar = new ArrayList();
            ArrayList arSD = new ArrayList();
            ArrayList arVat = new ArrayList();


            FormsIdentity formsIdentity = HttpContext.User.Identity as FormsIdentity;
            FormsAuthenticationTicket ticket = formsIdentity.Ticket;
            string userData = ticket.UserData;


            var totalAmount = decimal.Zero;



            var receipt = TruncateAt("VOID BY ".PadRight(10), 10) + TruncateAt(userData.PadLeft(5), 5);

            ar.Add(receipt);

            receipt = TruncateAt(tableName.PadRight(32), 32);

            ar.Add(receipt);

            string strTableTime = TruncateAt(timeOfOrder.Value.ToShortTimeString().PadRight(21), 21) + TruncateAt(timeOfOrder.Value.ToShortDateString().PadLeft(10), 10);

            var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.Sum(y => y.Quantity), Description = x.FirstOrDefault().Description, UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

            ar.Add(strTableTime);

            ar.Add("===================================");

            foreach (var si in groupList)
            {
                var amount = si.UnitPrice * si.Quantity;
                totalAmount += amount;

                var toPrint = si.Description;

                if (!string.IsNullOrEmpty(toPrint))
                {
                    var len = si.Description.Length;

                    if (len < 33)
                    {
                        string str = TruncateAt(si.Description.PadRight(len), len) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                        ar.Add(str);
                    }
                    else
                    {
                        string str = TruncateAt(si.Description.PadRight(34), 34) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                        ar.Add(str);
                    }

                }
            }



            double dTotal = 0;

            double.TryParse(totalAmount.ToString(), out dTotal);

            var printer = printerName;

            var initialList = lst;

            if (!printer.StartsWith("Star"))
            {

                printer = printerName;

                try
                {


                    foreach (var item in ar)
                    {
                        PrintLineItemRawKitchen(printer, item.ToString());
                    }

                    PrintTextLineRaw(printer, string.Empty);

                    PrintTextLineRaw(printer, String.Empty);
                    RawPrinterHelper.FullCut(printer);

                }
                catch (Exception)
                {
                    //throw ex;
                }
                finally
                {

                }

            }

        }


        private void PrintReceipt(List<POSService.Entities.StockItem> lst, double total, int tax, int discount)
        {
            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                try
                {
                    var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                    splitDetails = shopDetails.Split('@');

                    if (splitDetails.Length != 4)
                    {
                        splitDetails = null;
                    }

                }
                catch (Exception)
                {

                }

                if (splitDetails != null)
                {
                    PrintReceiptHeader(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName);
                }
                else
                {
                    PrintReceiptHeader(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName);
                }

                foreach (var item in lst)
                {
                    //var total = item.UnitPrice * item.Quantity;
                    PrintLineItem(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));

                }

                //PrintLineItem(printer, "Item 1", 10, 99.99);
                //PrintLineItem(printer, "Item 2", 101, 0.00);
                //PrintLineItem(printer, "Item 3", 9, 0.1);
                //PrintLineItem(printer, "Item 4", 1000, 1);

                PrintReceiptFooter(printer, total, tax, discount, "THANK YOU FOR YOUR PATRONAGE.");
            }
            finally
            {
                DisconnectFromPrinter(printer);

            }
        }

        private void DisconnectFromPrinter(PosPrinter printer)
        {
            try
            {
                printer.Release();
                printer.Close();

            }
            catch
            {

            }

        }

        private void ConnectToPrinter(PosPrinter printer)
        {
            try
            {
                printer.Open();
                printer.Claim(10000);
                printer.DeviceEnabled = true;
            }
            catch
            {

            }
        }

        private PosPrinter GetReceiptPrinter()
        {
            //PosExplorer explorer = new PosExplorer();
            //return explorer.GetDevices(DeviceType.PosPrinter, DeviceCompatibilities.OposAndCompatibilityLevel1);

            PosExplorer posExplorer = null;

            try
            {

                posExplorer = new PosExplorer();
            }
            catch (Exception)
            {

                //posExplorer = new PosExplorer(this);
            }

            //var ppp = posExplorer.GetDevices(DeviceType.PosPrinter, DeviceCompatibilities.OposAndCompatibilityLevel1);
            // var pp = posExplorer.GetDevices();
            // DeviceInfo receiptPrinterDevice = posExplorer.GetDevice("EPSON TM-T20II Receipt", "EPSON TM-T20II Receipt"); //May need to change this if you don't use a logicial name or
            //use a different one.
            DeviceInfo receiptPrinterDevice = posExplorer.GetDevice(DeviceType.PosPrinter, "POS1Printer"); //May need to change this if you don't use a logicial name or//my_device
            //DeviceInfo receiptPrinterDevice = posExplorer.GetDevice(DeviceType.PosPrinter, "Microsoft PosPrinter Simulator"); //May need to change this if you don't use a logicial name or//my_device

            //DeviceInfo receiptPrinterDevice1 = posExplorer.GetDevice(DeviceType.LineDisplay, "my_device"); //May need to change this if you don't use a logicial name or//my_device
            // receiptPrinterDevice.

            return (PosPrinter)posExplorer.CreateInstance(receiptPrinterDevice);
        }


        private void PrintReceiptFooter(PosPrinter printer, double subTotal, double tax, double discount, string footerText, string gueestOrderNote = "")
        {
            string offSetString = new string(' ', printer.RecLineChars / 2);

            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
            PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL  {0}", subTotal.ToString("#0.00")));
            PrintTextLine(printer, offSetString + String.Format("TAX        {0}", tax.ToString("#0.00")));
            PrintTextLine(printer, offSetString + String.Format("DISCOUNT   {0}", discount.ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, offSetString + String.Format("TOTAL      {0}", (subTotal - (tax + discount)).ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, String.Empty);

            if (!string.IsNullOrEmpty(gueestOrderNote))
            {
                PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + gueestOrderNote);
            }



            PrintTextLine(printer, String.Empty);


            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);

            //Added in these blank lines because RecLinesToCut seems to be wrong on my printer and
            //these extra blank lines ensure the cut is after the footer ends.
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);

            //Print 'advance and cut' escape command.
            PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'0', (byte)'0', (byte)'P', (byte)'f', (byte)'P' }));
        }

        private void PrintLineItem(PosPrinter printer, string strTableDetails)
        {

            PrintText(printer, TruncateAt(strTableDetails.PadRight(21), 21));
            PrintText(printer, TruncateAt(DateTime.Now.ToShortTimeString().PadLeft(9), 9));
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
        }

        private void PrintLineItem(PosPrinter printer, string itemCode, int quantity, double unitPrice)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintText(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintText(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }


        private void PrintReceiptHeaderNoCompany(PosPrinter printer, string table, string taxNumber, DateTime dateTime, string cashierName)
        {

            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Format("TABLE : {0}", table));

            PrintTextLine(printer, String.Empty);
            PrintText(printer, "Item             ");
            PrintText(printer, "Qty  ");
            PrintText(printer, "Unit Price ");
            PrintTextLine(printer, "Total      ");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

        }

        private void PrintReceiptHeader(PosPrinter printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime, string cashierName)
        {
            PrintTextLine(printer, companyName);
            PrintTextLine(printer, addressLine1);
            PrintTextLine(printer, addressLine2);
            PrintTextLine(printer, taxNumber);
            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Empty);
            PrintText(printer, "Item             ");
            PrintText(printer, "Qty  ");
            PrintText(printer, "Unit Price ");
            PrintTextLine(printer, "Total      ");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

        }

        private void PrintText(PosPrinter printer, string text)
        {
            if (text.Length <= printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text); //Print text
            else if (text.Length > printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, TruncateAt(text, printer.RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest.
        }

        private void PrintTextLine(PosPrinter printer, string text)
        {
            if (text.Length < printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text + Environment.NewLine); //Print text, then a new line character.
            else if (text.Length > printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, TruncateAt(text, printer.RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest, no new line character (printer will probably auto-feed for us)
            else if (text.Length == printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text + Environment.NewLine); //Print text, no new line character, printer will probably auto-feed for us.
        }

        private string TruncateAt(string text, int maxWidth)
        {
            string retVal = text;
            if (text.Length > maxWidth)
                retVal = text.Substring(0, maxWidth);

            return retVal;
        }

        private void PrintReceiptRaw(List<POSService.Entities.StockItem> lst, double total, double tax, double discount, string receiptNumber, bool? addRestaurantGuestExtraTax, string guestTableNumber, bool printOnly, int paymentMethodId, string footerText, bool showHeader = true, string guestOrderNote = "")
        {
            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            try
            {
                var grpList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Description = x.FirstOrDefault().Description, Quantity = x.Sum(z => z.Quantity), UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                try
                {
                    var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                    splitDetails = shopDetails.Split('@');

                    if (splitDetails.Length != 4)
                    {
                        splitDetails = null;
                    }

                }
                catch (Exception)
                {
                    //throw ex;
                }

                PrintReceiptHeaderRawKitchen(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName, receiptNumber, guestTableNumber);


                foreach (var item in grpList)
                {
                    //PrintLineItemRawKitchen(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));
                }

                

                //PrintReceiptFooterRaw(printer, total, tax, discount, "none", footerText, printOnly, paymentMethodId, guestOrderNote);

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {

            }
        }

        private void PrintReceiptFooterRaw(string printer, double subTotal, double tax, double discount, string anyTaxDetails, string footerText, bool printOnly, int paymentMethodId, string guestOrderNote = "")
        {
            int RecLineChars = 42;

            string offSetString = new string(' ', ((RecLineChars / 2) - 4));

            var sc = 0;


            PrintTextLineRaw(printer, new string('-', (RecLineChars / 3) * 2));
            PrintTextLineRaw(printer, offSetString + String.Format("SUB-TOTAL  {0}", subTotal.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("TAX        {0}", tax.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("DISCOUNT   {0}", discount.ToString("#0.00")));

            if (sc > decimal.Zero)
            {
                PrintTextLineRaw(printer, offSetString + String.Format("SERVICE CHRG   {0}", sc.ToString("#0.00")));
            }

            var finalTotal = ((subTotal + tax) - (discount)).ToString("#0.00");

            if (sc > decimal.Zero)
            {
                finalTotal = finalTotal + sc;
            }

            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));
            PrintTextLineRaw(printer, offSetString + String.Format("TOTAL      {0}", finalTotal));
            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));

            if (!string.IsNullOrEmpty(guestOrderNote))
            {
                PrintTextLineRaw(printer, offSetString + String.Format("NOTE -     {0}", guestOrderNote));
            }

            if (!printOnly)
            {
                if (paymentMethodId == 5)
                {
                    PrintTextLineRaw(printer, offSetString + String.Format("COMPLIMENTARY -     {0}", "CLOSED"));
                }
                else
                {
                    PrintTextLineRaw(printer, offSetString + String.Format("CASHED ------     {0}", "CLOSED"));
                }

                PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));
            }

            PrintTextLineRaw(printer, String.Empty);

            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            var eCentre = Convert.ToChar(27) + Convert.ToChar(97) + "1";
            //PrintTextLineRaw(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
            offSetString = new string(' ', RecLineChars / 4);
            PrintTextLineRaw(printer, offSetString + footerText);

            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, String.Empty);

            if (!string.IsNullOrEmpty(anyTaxDetails))
            {
                PrintTextLineRaw(printer, offSetString + anyTaxDetails);
            }


            //Added in these blank lines because RecLinesToCut seems to be wrong on my printer and
            //these extra blank lines ensure the cut is after the footer ends.

            byte[] DrawerOpen5 = { 0xA };

            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);

            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED


            //Print 'advance and cut' escape command.
            //PrintTextLineRaw(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'0', (byte)'0', (byte)'P', (byte)'f', (byte)'P' }));
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            RawPrinterHelper.FullCut(printer);
            RawPrinterHelper.OpenCashDrawer1(printer);
        }


        private void PrintLineItemRaw(string printer, string itemCode, int quantity, double unitPrice)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintLineItemRawKitchen(string printer, string itemCode)
        {
            var lineItem = itemCode + Environment.NewLine;
            //PrintTextRaw(printer, TruncateAt(itemCode.PadRight(20), 20));
            PrintTextRaw(printer, lineItem);
            //PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            //PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintTextLineRaw(string printer, string text)
        {

            string eNmlText = Convert.ToChar(27) + "!" + Convert.ToChar(0);
            text = eNmlText + text;
            int RecLineChars = 42;
            if (text.Length < RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, text + Environment.NewLine); //Print text //Print text, then a new line character.
            else if (text.Length > RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, TruncateAt(text, RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest, no new line character (printer will probably auto-feed for us)
            else if (text.Length == RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, text + Environment.NewLine); //Print text, no new line character, printer will probably auto-feed for us.
        }

        private void PrintReceiptHeaderRawKitchen(string printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime,
        string cashierName, string receiptNumber, string guestTableNumber)
        {
            int RecLineChars = 42;
            PrintTextLineRaw(printer, companyName);
            PrintTextLineRaw(printer, addressLine1);
            PrintTextLineRaw(printer, addressLine2);
            PrintTextLineRaw(printer, taxNumber);
            PrintTextLineRaw(printer, new string('-', RecLineChars));
            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, String.Format("DATE : {0}", dateTime.ToString()));
            PrintTextLineRaw(printer, String.Format("TABLE : {0}", guestTableNumber));

            PrintTextLineRaw(printer, String.Empty);
        }


        private void PrintReceiptHeaderRaw(string printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime,
         string cashierName, string receiptNumber, string guestTableNumber)
        {
            int RecLineChars = 42;
            PrintTextLineRaw(printer, companyName);
            PrintTextLineRaw(printer, addressLine1);
            PrintTextLineRaw(printer, addressLine2);
            PrintTextLineRaw(printer, taxNumber);
            PrintTextLineRaw(printer, new string('-', RecLineChars));
            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, String.Format("DATE : {0}", dateTime.ToString()));
            PrintTextLineRaw(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLineRaw(printer, String.Format("RECEIPT NO. : {0}", receiptNumber));
            PrintTextLineRaw(printer, String.Format("TABLE : {0}", guestTableNumber));

            PrintTextLineRaw(printer, String.Empty);
            PrintTextRaw(printer, "Item             ");
            PrintTextRaw(printer, "Qty  ");
            PrintTextRaw(printer, "Unit Price ");
            PrintTextRaw(printer, "Total      ");
            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, new string('=', RecLineChars));
            PrintTextLineRaw(printer, String.Empty);

        }

        private void PrintTextRaw(string printer, string text)
        {

            int RecLineChars = 42;
            string eNmlText = Convert.ToChar(27) + "!" + Convert.ToChar(0);
            text = eNmlText + text;

            if (text.Length <= RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, text); //Print text
            else if (text.Length > RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, TruncateAt(text, RecLineChars));//Print exactly as many characters as the printer allows, truncating the rest.
        }



        private void PrintLineItem(PosPrinter printer, string itemCode, int quantity)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(31), 31));
            PrintText(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            //PrintText(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            //PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        protected void SendToPosPrinter(string printerName, List<POSService.Entities.StockItem> lst, string tableName, bool pleasePrint, int? guestOrderId, DateTime? timeOfOrder, string guestOrderNote = "", string orderNo = "")
        {
            ArrayList ar = new ArrayList();
            ArrayList arSD = new ArrayList();
            ArrayList arVat = new ArrayList();


            FormsIdentity formsIdentity = HttpContext.User.Identity as FormsIdentity;
            FormsAuthenticationTicket ticket = formsIdentity.Ticket;
            string userData = ticket.UserData;


            var totalAmount = decimal.Zero;

            //string strTableTime = MyPadright(tableName, 5) + MyPadright(DateTime.Now.ToShortTimeString(), 5);
            string strTableTime = TruncateAt(timeOfOrder.Value.ToShortTimeString().PadRight(21), 21) + TruncateAt(timeOfOrder.Value.ToShortDateString().PadLeft(10), 10);

            var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.Sum(y => y.Quantity), Description = x.FirstOrDefault().Description, UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

            //var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.ToList().Count, Description = x.FirstOrDefault().Description, UnitPrice  = x.FirstOrDefault().UnitPrice}).ToList();

            //ar.Add("");

            ar.Add(strTableTime);

            //ar.Add("");

            string strWaiter = TruncateAt(tableName.PadRight(21), 21) + TruncateAt(userData.PadLeft(10), 10);

            ar.Add(strWaiter);

            //ar.Add("");

            string receipt = TruncateAt("ORDER NO.".PadRight(21), 21) + TruncateAt(orderNo.PadLeft(10), 10);

            ar.Add(receipt);

            //ar.Add("");

            if (!string.IsNullOrEmpty(guestOrderNote))
                ar.Add(guestOrderNote);

            ar.Add("============");

            foreach (var si in groupList)
            {
                var amount = si.UnitPrice * si.Quantity;
                totalAmount += amount;

                var toPrint = si.Description;

                if(!string.IsNullOrEmpty(toPrint))
                {
                    var len = si.Description.Length;

                    if(len < 33)
                    {
                        string str = TruncateAt(si.Description.PadRight(len), len) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                        ar.Add(str);
                    }
                    else
                    {
                        string str = TruncateAt(si.Description.PadRight(34), 34) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                        ar.Add(str);
                    }
                    
                }
                
            }

            //var isFullPos = IsFullPos();

            double dTotal = 0;

            double.TryParse(totalAmount.ToString(), out dTotal);

            //PRINT PRESENT BILLL
            var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

            var printer = printerName;

            var initialList = lst;

            if (!printer.StartsWith("Star"))
            {

                printer = printerName;

                try
                {


                    foreach (var item in ar)
                    {
                        PrintLineItemRawKitchen(printer, item.ToString());
                    }

                    PrintTextLineRaw(printer, string.Empty);

                    PrintTextLineRaw(printer, String.Empty);
                    RawPrinterHelper.FullCut(printer);

                }
                catch (Exception)
                {
                    //throw ex;
                }
                finally
                {

                }

            }

        }



        protected void SendToPosPrinter(List<POSService.Entities.StockItem> lst, string tableName, bool pleasePrint, int? guestOrderId, DateTime? timeOfOrder, string guestOrderNote = "")
        {
            ArrayList ar = new ArrayList();
            ArrayList arSD = new ArrayList();
            ArrayList arVat = new ArrayList();

            var totalAmount = decimal.Zero;

            //string strTableTime = MyPadright(tableName, 5) + MyPadright(DateTime.Now.ToShortTimeString(), 5);
            string strTableTime = TruncateAt(tableName.PadRight(21), 21) + TruncateAt(timeOfOrder.Value.ToShortTimeString().PadLeft(10), 10);

            var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.Sum(y => y.Quantity), Description = x.FirstOrDefault().Description, UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

            //var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.ToList().Count, Description = x.FirstOrDefault().Description, UnitPrice  = x.FirstOrDefault().UnitPrice}).ToList();

            ar.Add("");

            ar.Add(strTableTime);

            ar.Add("");

            if (!string.IsNullOrEmpty(guestOrderNote))
                ar.Add(guestOrderNote);

            ar.Add("============");

            foreach (var si in groupList)
            {
                var amount = si.UnitPrice * si.Quantity;
                totalAmount += amount;
                string str = TruncateAt(si.Description.PadRight(34), 34) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                ar.Add(str);
            }

            //var isFullPos = IsFullPos();

            double dTotal = 0;

            double.TryParse(totalAmount.ToString(), out dTotal);

            //PRINT PRESENT BILLL
            var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            var initialList = lst;

            if (!printer.StartsWith("Star"))
            {

                printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

                try
                {
                   

                    foreach (var item in ar)
                    {
                        PrintLineItemRawKitchen(printer, item.ToString());
                    }

                    PrintTextLineRaw(printer, string.Empty);

                    PrintTextLineRaw(printer, String.Empty);
                    RawPrinterHelper.FullCut(printer);

                }
                catch (Exception)
                {
                    //throw ex;
                }
                finally
                {

                }

            }
            
        }


        public void RecordManagersDelete(int itemId, int qty, int personId, string tableName, int waiterId, decimal unitPrice)
        {
           
            var totalPrice = decimal.Zero;
            totalPrice = qty * unitPrice;

           


            var sqlString = @"INSERT INTO [dbo].[SOLDITEMS]
           ([ItemId]
           ,[Qty]
           ,[TotalPrice]
           ,[DateSold]
		   ,[PersonId]
           ,[IsActive]
           ,[TableName]
           ,[WaiterId]
		   )
           VALUES
           (@ItemId
           ,@Qty
           ,@TotalPrice
           ,@DateSold
		   ,@PersonId
           ,@IsActive
           ,@TableName
           ,@WaiterId
		   )";

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, myConnection))
                {
                    myConnection.Open();
                    SqlParameter custId1 = cmd.Parameters.AddWithValue("@ItemId", itemId);
                    SqlParameter custId2 = cmd.Parameters.AddWithValue("@Qty", qty);
                    SqlParameter custId3 = cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    SqlParameter custId5 = cmd.Parameters.AddWithValue("@DateSold", DateTime.Now);
                    SqlParameter custId6 = cmd.Parameters.AddWithValue("@PersonId", personId);
                    SqlParameter custId8 = cmd.Parameters.AddWithValue("@IsActive", true);
                    SqlParameter custId9 = cmd.Parameters.AddWithValue("@TableName", tableName);//WaiterId
                    SqlParameter custId10 = cmd.Parameters.AddWithValue("@WaiterId", waiterId);//
                    cmd.ExecuteNonQuery();
                }
            }
        }


        protected static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        protected int DeleteCashierEmptyTables(int cashierId)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteCashierEmptyTables", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@CashierId", cashierId);

                    cmd.ExecuteNonQuery();
                }
            }

            return modified;
        }

        protected int DeleteCashierTableStockItem(int id, int tableId, int type)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteTableStockItem", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.Parameters.AddWithValue("@TableId", tableId);

                    cmd.Parameters.AddWithValue("@Type", type);

                    cmd.ExecuteScalar();
                }
            }

            return modified;
        }

        protected int DeleteCashierTableItem(int id)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ReduceOrDeleteTableStockItem", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();
                    
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteScalar();
                }
            }

            return modified;
        }

        
        public async Task<int> DeleteTableItem(int id)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteTableItem", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@Id", id);

                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;
        }

        private IEnumerable<CabbashUser> ReadPersonItemsFull(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new CabbashUser
                {
                    PersonID = reader.IsDBNull(reader.GetOrdinal("PersonID")) ? 0 : reader.GetInt32(reader.GetOrdinal("PersonID")),
                    FullMember = reader.IsDBNull(reader.GetOrdinal("FullMember")) ? false : reader.GetBoolean(reader.GetOrdinal("FullMember")),
                    PersonTypeId = reader.IsDBNull(reader.GetOrdinal("PersonTypeId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PersonTypeId")),
                    Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? "" : reader.GetString(reader.GetOrdinal("Username")),
                    PersonTypeName = reader.IsDBNull(reader.GetOrdinal("PersonTypeName")) ? "" : reader.GetString(reader.GetOrdinal("PersonTypeName")),
                    MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? "" : reader.GetString(reader.GetOrdinal("MiddleName"))
                };
            };
        }

        private IEnumerable<CabbashUser> ReadPersonItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new CabbashUser
                {
                    PersonID = reader.IsDBNull(reader.GetOrdinal("PersonID")) ? 0 : reader.GetInt32(reader.GetOrdinal("PersonID")),
                    PersonTypeId = reader.IsDBNull(reader.GetOrdinal("PersonTypeId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PersonTypeId")),
                    Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? "" : reader.GetString(reader.GetOrdinal("Username")),
                    PersonTypeName = reader.IsDBNull(reader.GetOrdinal("PersonTypeName")) ? "" : reader.GetString(reader.GetOrdinal("PersonTypeName")),
                    MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? "" : reader.GetString(reader.GetOrdinal("MiddleName"))
                };
            };
        }

        public IEnumerable<CabbashUser> GetUserById(int personId)
        {

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserByPersonId", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@PersonId", personId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadPersonItems(reader).ToArray();

                }
            }

        }

        private IEnumerable<CabbashUser> ReadPersonItemsUsed(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new CabbashUser
                {
                    Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? "" : reader.GetString(reader.GetOrdinal("Username")),
                    MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? "" : reader.GetString(reader.GetOrdinal("MiddleName"))
                };
            };
        }

        public IEnumerable<CabbashUser> GetUsedTables()
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetUsedTables", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadPersonItemsUsed(reader).ToArray();

                }
            }

        }


        

        public IEnumerable<CabbashUser> GetPersonsByCode(string code)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserByCode", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Code", code);

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadPersonItemsFull(reader).ToArray();

                }
            }

        }



        public void ResetUserLoginALL(string cmdSring)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                myConnection.Open();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand(cmdSring, myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.ExecuteNonQuery();


                            oTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

           
        }

        public void SignUserIn(string code)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                myConnection.Open();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("SignUserIn", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@Code", code);


                            cmd.ExecuteNonQuery();


                            oTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }


        }

        public void SignOutUser(int personId)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                myConnection.Open();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("SignUserOut", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@PersonID", personId);


                            cmd.ExecuteNonQuery();


                            oTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }


        }

        public async Task<int> MoveItemBase(int leftSideTableId, int rightSideTableId, int id, int personId)
        {
          

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("MoveTableItem", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@LeftSideTableId", leftSideTableId);

                            cmd.Parameters.AddWithValue("@RightSideTableId", rightSideTableId);

                            cmd.Parameters.AddWithValue("@Id", id);

                            cmd.Parameters.AddWithValue("@PersonId", personId);


                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;
        }


        
        public async Task<int> UpdateTableItemCollectedById(int id)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateTableItemById", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@Id", id);

                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;
        }

        public async Task<int> UpdateTableWithTelephone(string telephone, int id)
        {
            

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateTableWithTelephone", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@Id", id);

                            cmd.Parameters.AddWithValue("@Telephone", telephone);

                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;

        }

        public async Task<int> UpdateTableItemNote(int tableItemId, string note, string personName, bool completed = false)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("InsertOrUpdateTableItemNote", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@TableItemId", tableItemId);
                            cmd.Parameters.AddWithValue("@Note", note);
                            cmd.Parameters.AddWithValue("@PersonName", personName);
                            cmd.Parameters.AddWithValue("@Completed", completed);


                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;
        }

        public async Task<int> UpdateTableItemById(int id)
        {


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateTableItemById", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@Id", id);

                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;
        }
        public async Task<int> UpdateTableItemQtyById(int id, int qty)
        {
            

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateTableItemQtyById", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@Id", id);

                            cmd.Parameters.AddWithValue("@Qty", qty);

                            await cmd.ExecuteNonQueryAsync();


                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return 1;
        }

        public async Task<List<TableItem>> InsertBulkNotes(IEnumerable<TableItem> settings)
        {
            var nowNow = DateTime.Now;

            List<TableItem> KitchenNotes = new List<TableItem>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("InsertOrUpdateTableItemNote", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {
                            foreach (var tableItem in settings)
                            {
                                cmd.Parameters.Clear();

                                cmd.Parameters.AddWithValue("@TableItemId", tableItem.Id);

                                cmd.Parameters.AddWithValue("@Note", tableItem.IndividualNote);

                                cmd.Parameters.AddWithValue("@PersonName", "WAITER");

                                cmd.Parameters.AddWithValue("@Completed", false);

                                await cmd.ExecuteScalarAsync();
                            }

                            oTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            oTransaction.Rollback();
                        }
                    }
                }
            }

            return KitchenNotes;
        }

        public async Task<List<TableItem>> InsertBulkItems(IEnumerable<TableItem> settings)
        {
            if(!settings.Any())
            {
                return new List<TableItem>();
            }

            var nowNow = DateTime.Now;

            List<TableItem> KitchenNotes = new List<TableItem>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                await myConnection.OpenAsync();

                using (SqlTransaction oTransaction = myConnection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand("InsertTableItem", myConnection))
                    {
                        cmd.Transaction = oTransaction;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        try
                        {
                            foreach (var tableItem in settings)
                            {
                                cmd.Parameters.Clear();

                                cmd.Parameters.AddWithValue("@TableId", tableItem.TableId);

                                cmd.Parameters.AddWithValue("@ItemId", tableItem.ItemId);

                                cmd.Parameters.AddWithValue("@Qty", tableItem.Qty);

                                cmd.Parameters.AddWithValue("@DateSold", nowNow);

                                cmd.Parameters.AddWithValue("@Cashier", tableItem.Cashier);

                                cmd.Parameters.AddWithValue("@Fulfilled", false);

                                cmd.Parameters.AddWithValue("@Collected", false);

                                cmd.Parameters.AddWithValue("@Completed", false);

                                cmd.Parameters.AddWithValue("@StoreFulfilled", false);

                                cmd.Parameters.AddWithValue("@SentToPOS", false);

                                cmd.Parameters.AddWithValue("@Note", tableItem.Note);

                                cmd.Parameters.AddWithValue("@IsActive", false);

                                cmd.Parameters.AddWithValue("@SentToPrinter", false);

                                var obj = await cmd.ExecuteScalarAsync();

                                //Grab ids for print to kitchen table items that were just created

                                int newId = 0;

                                int.TryParse(obj.ToString(), out newId);

                                if(newId != 0)
                                {
                                    KitchenNotes.Add(new TableItem { Id = newId, IndividualNote = tableItem.IndividualNote });
                                }
                               
                            }

                            oTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            oTransaction.Rollback();
                            KitchenNotes = new List<TableItem>();
                        }
                    }
                }
            }

            return KitchenNotes;
        }
       


        protected int ProcessWaiterOrder(int tableId, int cashierId, string note, int placeId)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ProcessWaiterOrder", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@CashierId", cashierId);
                    cmd.Parameters.AddWithValue("@TableId", tableId);
                    cmd.Parameters.AddWithValue("@Note", note);
                    cmd.Parameters.AddWithValue("@PlaceId", placeId);

                    cmd.ExecuteNonQuery();
                }
            }

            return modified;
        }


        protected int InsertTableItemNoAwait(TableItem tableItem)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertTableItem", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Cashier", tableItem.Cashier);

                    cmd.Parameters.AddWithValue("@Collected", false);

                    cmd.Parameters.AddWithValue("@Completed", false);

                    cmd.Parameters.AddWithValue("@SentToPrinter", false);

                    cmd.Parameters.AddWithValue("@Note", tableItem.Note);

                    cmd.Parameters.AddWithValue("@DateSold", DateTime.Now);

                    cmd.Parameters.AddWithValue("@Fulfilled", false);

                    cmd.Parameters.AddWithValue("@IsActive", false);

                    cmd.Parameters.AddWithValue("@ItemId", tableItem.ItemId);

                    cmd.Parameters.AddWithValue("@Qty", tableItem.Qty);

                    cmd.Parameters.AddWithValue("@SentToPOS", false);

                    cmd.Parameters.AddWithValue("@StoreFulfilled", false);

                    cmd.Parameters.AddWithValue("@TableId", tableItem.TableId);

                    var obj = cmd.ExecuteScalar();

                    int.TryParse(obj.ToString(), out modified);

                }
            }


            return modified;
        }

        //
        protected async Task<int> InsertTableItem(TableItem tableItem)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertTableItem", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@Cashier", tableItem.Cashier);

                    cmd.Parameters.AddWithValue("@Collected", false);

                    cmd.Parameters.AddWithValue("@Completed", false);

                    cmd.Parameters.AddWithValue("@SentToPrinter", false);

                    cmd.Parameters.AddWithValue("@Note", "NONE");

                    cmd.Parameters.AddWithValue("@DateSold", DateTime.Now);

                    cmd.Parameters.AddWithValue("@Fulfilled", false);

                    cmd.Parameters.AddWithValue("@IsActive", false);

                    cmd.Parameters.AddWithValue("@ItemId", tableItem.ItemId);

                    cmd.Parameters.AddWithValue("@Qty", tableItem.Qty);

                    cmd.Parameters.AddWithValue("@SentToPOS", false);

                    cmd.Parameters.AddWithValue("@StoreFulfilled", false);

                    cmd.Parameters.AddWithValue("@TableId", tableItem.TableId);

                    try
                    {
                        var obj = await cmd.ExecuteScalarAsync();
                        int.TryParse(obj.ToString(), out modified);
                    }
                    catch(Exception)
                    {

                    }
                }
            }

           
            return modified;
        }

        protected async Task<int> InsertCustomerOrder(TableItem tableItem, int tableItemId)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertCustomerOrder", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();
                    
                    cmd.Parameters.AddWithValue("@ItemId", tableItem.ItemId);
                    cmd.Parameters.AddWithValue("@Quantity", tableItem.Qty);
                    cmd.Parameters.AddWithValue("@PurchaseDate", tableItem.DateSold);
                    cmd.Parameters.AddWithValue("@UniqueId", tableItem.TableId.ToString());
                    cmd.Parameters.AddWithValue("@UserId", tableItem.TableId.ToString());
                    cmd.Parameters.AddWithValue("@PlaceId", 1);
                    cmd.Parameters.AddWithValue("@GuestId", tableItem.TableId);
                    cmd.Parameters.AddWithValue("@TableItemId", modified);
                    cmd.Parameters.AddWithValue("@TableId", tableItem.TableId);
                    cmd.Parameters.AddWithValue("@TableName", "TABLE " + tableItem.TableId.ToString());

                    await cmd.ExecuteScalarAsync();
                }
            }

            return modified;
        }

        protected async Task<int> InsertBarTable(BarTable barTable)
        {
            int modified = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("InsertTableWithTakeAway", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    await myConnection.OpenAsync();

                    cmd.Parameters.AddWithValue("@GuestId", barTable.GuestId);

                    cmd.Parameters.AddWithValue("@GuestGuid", barTable.GuestGuid);

                    cmd.Parameters.AddWithValue("@IsActive", true);

                    cmd.Parameters.AddWithValue("@StaffId", barTable.StaffId);

                    cmd.Parameters.AddWithValue("@TableAlias", barTable.TableAlias);

                    cmd.Parameters.AddWithValue("@TableId", barTable.TableId);

                    cmd.Parameters.AddWithValue("@TableName", barTable.TableName);

                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    cmd.Parameters.AddWithValue("@Printed", false);

                    cmd.Parameters.AddWithValue("@GuestNumber", barTable.GuestNumber);

                    cmd.Parameters.AddWithValue("@Takeaway", barTable.Takeaway);

                    cmd.Parameters.AddWithValue("@Telephone", barTable.Telephone);


                    object sId = await cmd.ExecuteScalarAsync();

                    int.TryParse(sId.ToString(), out modified);
                }
            }

            return modified;
        }


        protected IEnumerable<Transaction> GetParticularTransactionSingle(int transactionId)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetTranscationById", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Id", transactionId);//@TransactionDate


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Transaction
                            {
                                TableId = reader.GetInt32(reader.GetOrdinal("TableId")),
                                kitchenNote = reader.GetString(reader.GetOrdinal("kitchenNote")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount"))
                            };
                        };
                    }
                }
            }
        }
    }
}