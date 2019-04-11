using BarMateTabletOrdering.Helpers;
////using BarMateTabletOrdering.Hubs;
using BarMateTabletOrdering.Models;
using Microsoft.AspNet.SignalR;
using POSService;
using POSService.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BarMateTabletOrdering.Controllers
{
    [System.Web.Mvc.Authorize]
    public class PosController : BaseController
    {
        private int _distributionPointId = 2;
        private int _guestId = 0;
        private int _personId;

        public PosController()
        {
            _distributionPointId = GetDistributionPointId();
        }

        private int GetDistributionPointId()
        {

            int distributionPoint = 1;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["DistributionPointId"].ToString(), out distributionPoint);
            }
            catch
            {

            }

            return distributionPoint;
        }

        public int PersonId
        {
            get { return GetPersonId(); }
            set { _personId = value; }
        }

        private  int GetPersonId()
        {
            int personId = 1;
            var p = User.Identity.Name;
            int.TryParse(p, out personId);
            return personId;
        }

        [HttpPost]
        public async Task<ActionResult> PlaceOrderDoneByCollection(string valueIds, string printOrComplete)
        {
            if (string.IsNullOrEmpty(valueIds))
                return RedirectToAction("Index", "Home");

            var vals = valueIds.Split(',');


            foreach (var v in vals)
            {
                await UpdateTableItemCollectedById(int.Parse(v));
            }


            return RedirectToAction("Collections");
        }

        [HttpGet]
        public async Task<ActionResult> PlaceOrderDoneByCollectionSingle(int? id)
        {
            await UpdateTableItemCollectedById(id.Value);

            var role = System.Web.Security.Roles.GetRolesForUser().Single();

            var tisA = await GetKitchenItemsByRole("");

            var tis = tisA.Where(x => x.StaffId == PersonId).ToList();

            var kitchenlistAll = tis.ToList();

            var countAllInActive = kitchenlistAll.OrderBy(x => x.Id).LastOrDefault();

           
            var kitchenlist = kitchenlistAll.Where(x => x.DateSold > GetStartDateTime() && x.Fulfilled && !x.Collected).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.TableId)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = new BarTable { TableId = x.Key } }).ToList();

            return RedirectToAction("_CollectionPartialZero", new CabbashViewModel { TotalItems = kitchenlist.Count, Kitchenlist = kitchenlist });
        }


       

      
        private void PrintBillOnlyNewByTableItem(string printerName, List<TableItem> list, int? tableId, int personId, int? guestOrderId, string guestOrderNote = "")
        {
            var btA = GetBarTableByIdNoAsync(tableId.Value);

            BarTable bt = btA.FirstOrDefault();

            if(bt == null)
            {
                return;
            }

            //if(bt.Takeaway)
            //{
            //    printerName = printerName + "DELIVERY";
            //}

            //var tableList = list.Where(x => x.IsActive).ToList();

            var tableList = list.ToList();


            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            var thisProductListA = ProductsList();

            var thisProductList = thisProductListA.ToList();


            var orderNo = "";

            var orderTime = DateTime.Now;

            foreach (var ti in tableList)
            {
                var thisProduct = thisProductList.FirstOrDefault(x => x.Id == ti.ItemId);

                if(thisProduct != null)
                {
                    var price = thisProduct.UnitPrice;

                    var qty = ti.Qty;

                    totalBill += (price * qty);

                    var itemDescription = thisProduct.StockItemName;

                    orderTime = list.FirstOrDefault().DateSold;

                    orderNo = list.FirstOrDefault().Id.ToString();

                    lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

                }
            }

            SendToPosPrinter(printerName, lst, bt.TableAlias, true, guestOrderId, orderTime, guestOrderNote, orderNo);

         
        }

        private async void PrintBillOnlyNewByTableItem(List<TableItem> list, int? tableId, int personId, int? guestOrderId, string guestOrderNote = "")
        {
            var btA = await GetBarTableById(tableId.Value);

            BarTable bt = btA.FirstOrDefault();

            var tableList = list.Where(x => x.IsActive).ToList();

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            var thisProductListA = ProductsList();

            var thisProductList = thisProductListA.ToList();

            var orderTime = DateTime.Now;

            foreach (var ti in tableList)
            {
                var thisProduct = thisProductList.FirstOrDefault(x => x.Id == ti.ItemId);

                var price = thisProduct.UnitPrice;

                var qty = ti.Qty;

                totalBill += (price * qty);

                var itemDescription = thisProduct.StockItemName;

                orderTime = list.FirstOrDefault().DateSold;

                lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

            }

            SendToPosPrinter(lst, bt.TableAlias, true, guestOrderId, orderTime, guestOrderNote);
        }

        public ActionResult GetProductPic(string pictureId)
        {
            int? id = 1;

            CabbashViewModel vm = new CabbashViewModel();

            vm.PlaceId = id.Value;

            var actualFile = Path.Combine(Server.MapPath("~/Products/Small/"), pictureId);

            if (System.IO.File.Exists(actualFile))
            {
                vm.PicturePath = pictureId;
            }
            else
            {
                vm.PicturePath = "default.jpg";
            }

            return PartialView("_ProductImage", vm);
        }

        private DateTime GetStartDateTime()
        {
            DateTime now = DateTime.Now;
            DateTime returnDate = DateTime.Today;
            DateTime yesterDay = DateTime.Today.AddDays(-1);

            if (now.Hour >= 0 && now.Hour < 8 && now.Second > 0)
            {
                var yesterdaysStartDate = new DateTime(yesterDay.Year, yesterDay.Month, yesterDay.Day, 8, 1, 1);
                returnDate = yesterdaysStartDate;
            }
            else
            {
                var todaysStartDate = new DateTime(now.Year, now.Month, now.Day, 8, 1, 1);
                returnDate = todaysStartDate;
            }

            return returnDate;
        }


        

        public async Task<ActionResult> GetCollectionOrders()
        {

            var role = System.Web.Security.Roles.GetRolesForUser().Single();

            var tisA = await GetKitchenItemsByRole("");

            var tis = tisA.Where(x => x.StaffId == PersonId).ToList();

            var kitchenlistAll = tis.ToList();

            var countAllInActive = kitchenlistAll.OrderBy(x => x.Id).LastOrDefault();

            var listForPrinting = tis.Where(x => !x.SentToPrinter && x.DateSold > GetStartDateTime() && !x.Completed).OrderByDescending(x => x.DateSold).ToList();

            var kitchenlist = kitchenlistAll.Where(x => x.DateSold > GetStartDateTime() && x.Fulfilled && !x.Collected).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.TableId)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = new BarTable { TableId = x.Key } }).ToList();


            if(kitchenlist.Any())
            {
                return PartialView("_CollectionPartial", new CabbashViewModel { TotalItems = kitchenlist.Count, Kitchenlist = kitchenlist });
            }
            else
            {
                return PartialView("_EmptyPartial");
            }

        }

       

        
        public async Task<ActionResult> PostIndividualKitchenNote(int? tableId, int? tableItemId, string note)
        {
            try
            {
                await UpdateTableItemNote(tableItemId.Value, note, "WAITER");
            }
            catch
            {

            }

            return Json(new { tableItemId }, JsonRequestBehavior.AllowGet);

        }


        private async void SaveAsPrinted(string valueIds)
        {
            if (string.IsNullOrEmpty(valueIds))
                return;


            var vals = valueIds.Split(',');

            foreach (var v in vals)
            {
                int id = 0;

                int.TryParse(v, out id);

                await UpdateTableItemById(id);
            }

            return;
        }

        private void SendToOPOSPrinter(List<TableItem> list, string tablename, string cashierName, string pointName)
        {
            var note = string.Empty;

            if (list.Any() && !string.IsNullOrEmpty(list.FirstOrDefault().Note))
            {
                note = list.FirstOrDefault().Note;
            }

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            foreach (var ti in list)
            {
                lst.Add(new POSService.Entities.StockItem { Description = ti.StockItemName, Quantity = ti.Qty });
            }

            //PrintKitchenDocketRaw(lst, note, tablename, cashierName, pointName);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            //Log the error!!
            //_Logger.Error(filterContext.Exception);

            //Redirect or return a view, but not both.
            ////filterContext.Result = RedirectToAction("Index", "ErrorHandler");
            // OR 
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/ErrorHandler/Index.cshtml"
            };
        }

        public ActionResult DeleteEmptyTables()
        {
            CabbashViewModel vm = new CabbashViewModel();

            int? tableId = 0;

            DeleteCashierEmptyTables(PersonId);


            if (!tableId.HasValue)
            {
                tableId = 0;
                ViewBag.TableNum = 0;
                ViewBag.PinCode = "NONE";
            }

            if (tableId.Value > 0)
            {
                ViewBag.TableNum = tableId.Value;
                ViewBag.PinCode = "PIN : " + tableId.Value.ToString();
            }

            vm.TableId = tableId.Value;

            return View("Index", vm);
        }


        [HttpGet]
        public async Task<ActionResult> OpenTableByCashier(int? id, bool? isActive, string telephoneName)
        {
            BarTable bt = null;

            if (!isActive.HasValue)
            {
                isActive = false;
            }

            if (isActive.HasValue && !isActive.Value)
            {
                var btA = await GetBarTableByTableId(id.Value);
                bt = btA.OrderBy(x => x.GuestNumber).LastOrDefault();
            }
            else
            {
                var btA = await GetBarTableByTableId(id.Value);
                bt = btA.OrderBy(x => x.GuestNumber).LastOrDefault();
            }

           

            BarTable t = new BarTable { };

            CabbashViewModel vm = new CabbashViewModel();

            if (bt == null)
            {
               
                var now = DateTime.Now;
                t.CreatedDate = now;
                t.GuestGuid = Guid.NewGuid().ToString();
                t.GuestId = _guestId;
                t.IsActive = true;
                t.StaffId = PersonId;
                t.TableId = id.Value;
                t.TableAlias = "Table " + id.Value.ToString() + "_" + "GUEST 1";
                t.TableId = id.Value;
                t.TableName = "Table " + id.Value.ToString() + "_" + "GUEST 1";
                t.GuestNumber = 1;
                t.Telephone = "NONE";

                if (!string.IsNullOrEmpty(telephoneName))
                {
                    t.TableAlias = telephoneName;
                    t.TableName = telephoneName;
                    t.Telephone = telephoneName;
                    t.Takeaway = true;
                }
            }
            else
            {
                
                var now = DateTime.Now;
                t.CreatedDate = now;
                t.GuestGuid = Guid.NewGuid().ToString();
                t.GuestId = _guestId;
                t.IsActive = true;
                t.StaffId = PersonId;
                t.TableId = bt.TableId;
                t.TableAlias = "Table " + bt.TableId.ToString() + "_" + "GUEST " + (bt.GuestNumber + 1);
                t.TableId = bt.TableId;
                t.TableName = "Table " + bt.TableId.ToString() + "_" + "GUEST " + (bt.GuestNumber + 1);
                t.GuestNumber = bt.GuestNumber + 1;
                t.Telephone = "NONE";


                if (!string.IsNullOrEmpty(telephoneName))
                {
                    t.TableAlias = telephoneName + "_" + t.GuestNumber.ToString();
                    t.TableName = telephoneName  + "_" + t.GuestNumber.ToString();
                    t.Telephone = telephoneName;
                    t.Takeaway = true;
                }
            }

            int? tableId = 0;

            try
            {
                tableId = await InsertBarTable(t);
            }
            catch
            {

            }
            


            if (!tableId.HasValue)
            {
                tableId = 0;
                ViewBag.TableNum = 0;
                ViewBag.PinCode = "NONE";

            }

            if (tableId.Value > 0)
            {
                ViewBag.TableNum = tableId.Value;
                ViewBag.TableAlias = t.TableAlias;
                ViewBag.PinCode = "PIN : " + tableId.Value.ToString();
            }

            vm.TableId = tableId.Value;

            vm.ExistingList = new List<TableItem>();

            var loadPos = RenderRazorViewToString("_LoadPOS", vm);

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var tt in vm.ExistingList)
            {
                total += tt.Qty * tt.StockItemPrice;
                totalCount += tt.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);

            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = tableId.Value;

            var loadPosBottom = RenderRazorViewToString("_LoadPOSBottom", vm);

            return Json(new
            {
                ItemCount = totalCount,
                ReturnValue = true,
                success = true,
                InsertError = 0,
                LoadPosView = loadPos,
                LoadPosBottom = loadPosBottom,
                OpenedTableId = tableId.Value,
                OpenedTableName = t.TableName + " PIN: " + tableId.Value.ToString(),
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Authorize(Roles = "MIXER")]
        public ActionResult Mixer()
        {
            CabbashViewModel vm = new CabbashViewModel();
            return View(vm);
        }

        [System.Web.Mvc.Authorize(Roles = "SHISHA")]
        public ActionResult Shisha()
        {
            CabbashViewModel vm = new CabbashViewModel();
            return View(vm);
        }

        [System.Web.Mvc.Authorize(Roles = "KITCHEN")]
        public ActionResult KitchenOnline()
        {
            CabbashViewModel vm = new CabbashViewModel();
            return View(vm);
        }

        //[System.Web.Mvc.Authorize(Roles = "KITCHEN")]
        public ActionResult Kitchen()
        {
            CabbashViewModel vm = new CabbashViewModel();
            return View(vm);
        }
        //private Object thisLock = new Object();

    

        private int GetRoleId(string role)
        {
            if(!string.IsNullOrEmpty(role))
            {
                role = role.Trim().ToUpper();
            }

            if(role.StartsWith("KITCHEN"))
            {
                return 1;
            }
            else if (role.StartsWith("GRILL"))
            {
                return 2;
            }
            else if (role.StartsWith("SHISHA"))
            {
                return 3;
            }
            else if (role.StartsWith("COCKTAIL"))
            {
                return 4;
            }
            else if (role.StartsWith("OUTSIDEBAR"))
            {
                return 5;
            }

            return 0;
        }

        //[System.Web.Mvc.Authorize(Roles = "KITCHEN,WAITER")]
        public ActionResult Previous()
        {
            CabbashViewModel vm = new CabbashViewModel();
            return View(vm);
        }

        [System.Web.Mvc.Authorize(Roles = "WAITER,DELIVERY,MANAGER,CLUBWAITER")]
        public ActionResult Collections()
        {
            CabbashViewModel vm = new CabbashViewModel();
            return View(vm);
        }

        [System.Web.Mvc.Authorize(Roles = "WAITER,DELIVERY,MANAGER,CLUBWAITER")]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "tableId,id,tableAlias")]
        public ActionResult Index(int? tableId, int? id, string tableAlias)
        {
            CabbashViewModel vm = new CabbashViewModel();

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (tableId.HasValue)
            {
                BarTable table = Enumerable.FirstOrDefault<BarTable>(base.GetBarTableByIdNoAsync(tableId.Value));

                if (table != null)
                {
                    vm.OpenedTableName = table.TableName + " PIN: " + tableId.Value.ToString();
                    vm.TableAlias = table.TableAlias;
                }

                if (tableId.Value > 0)
                {
                    ViewBag.TableNum = tableId.Value;
                    ViewBag.TableAlias = tableAlias;
                    vm.TableAlias = tableAlias;
                    ViewBag.PinCode = "PIN : " + tableId.Value.ToString();
                }
            }
            else
            {
                tableId = 0;
                ViewBag.TableNum = 0;
                ViewBag.PinCode = "NONE";
                vm.OpenedTableName = "";
            }


            vm.TableId = tableId.Value;
           

            return View(vm);
        }



        public async Task<ActionResult> OpenTable()
        {
            int personId = PersonId;

            CabbashViewModel m = new CabbashViewModel { };

            var tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";

            try
            {
                tableNumbers = GetUsedTables().FirstOrDefault(x => x.PersonID == personId).MiddleName;
            }
            catch
            {
                tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";
            }

            if(string.IsNullOrEmpty(tableNumbers) || !tableNumbers.Contains(","))
            {
                return View("NoTables",m);
            }
            else
            {
                List<BarTable> tagIds = tableNumbers.Split(',').Select(x => new BarTable { TableId = int.Parse(x), Id = int.Parse(x), IsActive = false, StaffId = personId }).ToList();

                Stopwatch watch = new Stopwatch();

                watch.Start();

                var realTablesAsync = await GetMyTablesBase(personId);

                var realTables = realTablesAsync.GroupBy(x => x.TableId).Select(x => new BarTable { TableId = x.Key, Id = x.FirstOrDefault().Id, IsActive = x.FirstOrDefault().IsActive, StaffId = x.FirstOrDefault().StaffId }).ToList();

                watch.Stop();

                var e = watch.ElapsedMilliseconds;

                var ids = realTables.Select(x => x.TableId).ToList();

                var clean = tagIds.Where(x => !ids.Contains(x.TableId)).ToList();

                clean.AddRange(realTables);

                m.TableNumbers = clean;

                if(User.IsInRole("DELIVERY"))
                {
                    return View("OpenTableDelivery", m);
                }

                return View(m);
            }
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult GetProductsBlank()
        {
            CabbashViewModel vm = new CabbashViewModel();

            vm.CategoriesListString = new List<string>();
           
            var cv = RenderRazorViewToString("_CategoryView", vm);

            var pv = RenderRazorViewToString("_ProductsViewBlank", vm);

            return Json(new { PV = pv, CV = cv }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public async Task<ActionResult> GetMyTables(int? tableId)
        {
            CabbashViewModel vm = new CabbashViewModel();

            if (!tableId.HasValue)
            {
                tableId = 0;
            }

            int personId = PersonId;

            var tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";

            try
            {
                //tableNumbers = POSService.StockItemService.GetUserById(personId).FirstOrDefault().MiddleName;
                tableNumbers = GetUserById(personId).FirstOrDefault(x => x.PersonID == personId).MiddleName;
            }
            catch(Exception ex)
            {
                tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";
            }

            if (string.IsNullOrEmpty(tableNumbers) || !tableNumbers.Contains(","))
            {
                var wt1 = RenderRazorViewToString("_NoTables", vm);
                return Json(new { WT = wt1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<BarTable> tagIds = tableNumbers.Split(',').Select(x => new BarTable { TableId = int.Parse(x), Id = int.Parse(x), IsActive = false, StaffId = personId }).ToList();

                var realTablesA = await GetMyTablesBase(personId);

                var realTables = realTablesA.GroupBy(x => x.TableId).Select(x => new BarTable { TableId = x.Key, Id = x.FirstOrDefault().Id, IsActive = x.FirstOrDefault().IsActive, StaffId = x.FirstOrDefault().StaffId }).ToList();


                var ids = realTables.Select(x => x.TableId).ToList();

                var clean = tagIds.Where(x => !ids.Contains(x.TableId)).ToList();

                clean.AddRange(realTables);

                vm.TableNumbers = clean;
            }

            if (User.IsInRole("DELIVERY"))
            {
                var wtD = RenderRazorViewToString("_WaiterTableViewDelivery", vm);
                return Json(new { WT = wtD }, JsonRequestBehavior.AllowGet);
            }

            var wt = RenderRazorViewToString("_WaiterTableView", vm);

            return Json(new { WT = wt }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[OutputCache(Duration = int.MaxValue)]
        public ActionResult GetProducts()
        {
            var productsA = ProductsList();

           var products = productsA.ToList();

            var catList = products.OrderBy(x => x.CategoryName).Select(x => x.CategoryName).ToList();

            catList = catList.Distinct().ToList();

            //if (id.HasValue && id.Value > 0)
            //{
            //    products = products.Where(x => x.CategoryId == id.Value).ToList();
            //}

            CabbashViewModel vm = new CabbashViewModel();

            vm.CategoriesListString = catList;

            vm.CategoryId = 0;

            vm.ProductsList = products.OrderBy(x => x.StockItemName);

            var someName = vm.ProductsList.Select(x => x.StockItemName.ToUpper());

            string initials = someName.Where(s => !string.IsNullOrEmpty(s))
                          .Aggregate("", (xs, x) => xs + "," + x.First());

            List<string> uniques = initials.Split(',').Reverse().Distinct().Reverse().ToList();

            uniques.RemoveAt(0);

            vm.QuickSearchString = uniques;

            var cv = RenderRazorViewToString("_CategoryView", vm);

            var pv = RenderRazorViewToString("_ProductsView", vm);

            return Json(new { PV = pv, CV = cv }, JsonRequestBehavior.AllowGet);
        }

        public List<char> GetPrefix(IEnumerable<string> strings)
        {
            return strings.Aggregate(GetPrefix).ToList();
        }

        public string GetPrefix(string first, string second)
        {
            int prefixLength = 0;

            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] != second[i])
                    break;

                prefixLength++;
            }

            return first.Substring(0, prefixLength);
        }



        public async Task<ActionResult> ViewTablesManager()
        {

            CabbashViewModel m = new CabbashViewModel { };

            var openTablesA = await GetMyTablesBase(0);

            var openTables = openTablesA.OrderByDescending(x => x.CreatedDate).Where(x => x.TableId != 0).ToList();

            m.AllOpenTables = openTables;

            return View("ViewTablesManager", m);
        }

        
        public async Task<ActionResult> SplitTable()
        {

            CabbashViewModel m = new CabbashViewModel { };

            List<BarTable> openTables = null;

            if(User.IsInRole("Manager"))
            {
                var openTablesA = await GetMyTablesWhatever(0);
                openTables = openTablesA.OrderByDescending(x => x.TableTotal).ThenBy(x => x.TableName).ToList();
            }
            else
            {
                return RedirectToAction("LogOff", "Account");
            }

            var occupiedTables = openTables.Where(x => x.TableTotal > 0).ToList();

            openTables.Insert(0, new BarTable { Id = 0, TableName = "--PLS SELECT--" });

            occupiedTables.Insert(0, new BarTable { Id = 0, TableName = "--PLS SELECT--" });

            m.AllOpenTables = openTables;
            m.OccupiedTables = occupiedTables;

            return View(m);
        }


        public async Task<ActionResult> ViewTables()
        {

            CabbashViewModel m = new CabbashViewModel { };

            var openTablesA = await GetMyTablesWhatever(PersonId);

            var openTables = openTablesA.OrderByDescending(x => x.CreatedDate).Where(x => x.TableId != 0).ToList();

            m.AllOpenTables = openTables.OrderBy(x => x.TableId).ToList();

            return PartialView("_ViewTablesMine", m);
        }

      

        public async Task<ActionResult> LoadAllPos(int? id)
        {
            var vm = new CabbashViewModel();

            var exList = await GetMyTableItemsByTableIdAll(id.Value);

            vm.ExistingList = exList.ToList();

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);
            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = id.Value;

            return PartialView("_LoadAllPOS", vm);
        }

        [HttpGet]
        public async Task<ActionResult> LoadAllBottomPos(int? id, string tableName)
        {
            var vm = new CabbashViewModel();

            var exList = await GetMyTableItemsByTableIdAll(id.Value);

            vm.ExistingList = exList.ToList();

            var loadPos = RenderRazorViewToString("_LoadPOS", vm);

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }



            vm.CanSendToDepartment = false;
            vm.CanSendReprocess = false;
            vm.CanTakePayment = false;
            vm.CanCancelSale = true;

            vm.CanDoProcess = true;
            vm.CanTakePayment = false;
            vm.CanCancelSale = true;
            vm.CanTakePayment = (vm.ExistingList.Count > 0) && !Enumerable.Any<TableItem>(vm.ExistingList, x => !x.IsActive);
            vm.CanCancelSale = (vm.ExistingList.Count > 0) && !Enumerable.Any<TableItem>(vm.ExistingList, x => x.IsActive);
            int num4 = Enumerable.Count<TableItem>(vm.ExistingList, x => x.SentToPOS && !x.IsActive);
            int num5 = Enumerable.Count<TableItem>(vm.ExistingList, x => x.SentToPOS && x.IsActive && x.SentToPrinter);

            vm.CanSendToDepartment = (vm.ExistingList.Count > 0) && (num4 > 0);

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);

            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = id.Value;

            vm.CanDoProcess = vm.ExistingList.Any();

            vm.CanSendReprocess = (vm.ExistingList.Count > 0) && (num5 > 0);

            var loadPosBottom = RenderRazorViewToString("_LoadPOSBottom", vm);

            return Json(new
            {
                ItemCount = totalCount,
                ReturnValue = true,
                success = true,
                InsertError = 0,
                LoadPosView = loadPos,
                LoadPosBottom = loadPosBottom,
                OpenedTableId = id.Value,
                OpenedTableName = tableName + " PIN: " + id.Value.ToString(),
            }, JsonRequestBehavior.AllowGet);
        }



        public async Task<ActionResult> LoadBottomPos(int? id)
        {
            var vm = new CabbashViewModel();

            var exList = await GetMyTableItemsByTableIdAll(id.Value);

            vm.ExistingList = exList.ToList();

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);
            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = id.Value;

            return PartialView("_LoadPOSBottom", vm);
        }



        //var url = "@Url.Action("SwapTablesWaiter", "POS")";

        //    $.ajax({
        //    url: url,
        //        data: { leftSide: leftSide, rightSide: rightSide, selectedValues: selectedValues },

        [HttpPost]
        public async Task<ActionResult> SwapTablesWaiter(int? leftSide, int? rightSide, int[] selectedValues)
        {
            var btA = await GetBarTableById(leftSide.Value);
            BarTable bt = btA.LastOrDefault();

            var btB = await GetBarTableById(rightSide.Value);
            BarTable bt1 = btA.LastOrDefault();

            if(bt == null || bt1 == null)
            {
                return Json(new
                {
                    ErrorFound = 1
                }, JsonRequestBehavior.AllowGet);
            }

            var vm = new CabbashViewModel();

            var errorFound = 0;

            if (selectedValues != null && selectedValues.Any())
            {

                foreach (var sel in selectedValues)
                {
                    if (sel != 0)
                    {
                       await MoveItemBase(leftSide.Value, rightSide.Value, sel, PersonId);
                    }
                    else
                    {
                        errorFound = 1;
                        break;
                    }
                }
            }
            
            return Json(new
            {
                ErrorFound = errorFound
            }, JsonRequestBehavior.AllowGet);
        }

        private void MoveItem(int leftSide, int rightSide, int sel, int personId)
        {
             MoveItemBase(leftSide, rightSide, sel, personId);
        }

        [HttpGet]
        public async Task<ActionResult> GetTableItemsForSplit(int? tableId)
        {
            var vm = new CabbashViewModel();

            var exList = await GetMyTableItemsByTableIdAll(tableId.Value);

            vm.ExistingList = exList.ToList();

            vm.ExistingList.ForEach(x => x.StockItemName = GetStockItemNameName(x.StockItemName, x.Qty));

            vm.MultiSelect = new MultiSelectList(vm.ExistingList, "Id", "StockItemName");

            return PartialView("_ItemDropDown", vm);
        }

       

        [HttpGet]
        public async Task<ActionResult> GetTableItemsForSplitRight(int? tableId)
        {
            var vm = new CabbashViewModel();

            //var every = POSService.StockItemService.GetMyTableItems(PersonId).ToList();

            //vm.ExistingList = every.Where(x => x.TableId == tableId.Value).OrderByDescending(x => x.DateSold).ToList();

            //vm.ExistingList.ForEach(x => x.StockItemName = GetStockItemNameName(x.StockItemName,x.Qty));

            //vm.MultiSelect = new MultiSelectList(vm.ExistingList, "Id", "StockItemName");

            var exList = await GetMyTableItemsByTableIdAll(tableId.Value);

            vm.ExistingList = exList.ToList();

            vm.ExistingList.ForEach(x => x.StockItemName = GetStockItemNameName(x.StockItemName, x.Qty));

            vm.MultiSelect = new MultiSelectList(vm.ExistingList, "Id", "StockItemName");


            return PartialView("_ItemDropDownRight", vm);
        }

        private string GetStockItemNameName(string stockItemName, int qty)
        {
            return stockItemName + " (" + qty.ToString() + ")";
        }

        [HttpGet]
        public async Task<ActionResult> LoadPOS(int? id)
        {
            var vm = new CabbashViewModel();

            var ti = await GetMyTableItems(PersonId);

            vm.ExistingList = ti.ToList();

            return PartialView("_LoadPOS", vm);
        }

        public async Task<ActionResult> ReduceOrDeleteQuatityManager(int? tableId, int? id, int? qty, string password)
        {
            var btA = await GetBarTableById(tableId.Value);

            var bt = btA.LastOrDefault();

            var exList = await GetMyTableItemsByTableIdAll(tableId.Value);

            var stockItem = exList.FirstOrDefault(x => x.Id == id.Value);

            var tableName = "";

            if (bt != null && stockItem != null)
            {
                tableName = bt.TableName;

                if (qty.HasValue)
                {
                    var thisProductList = ProductsList();

                    using (TransactionScope scope = new TransactionScope())
                    {
                        RecordManagersDelete(stockItem.StockItemId, qty.Value, PersonId, tableName, stockItem.Cashier, stockItem.StockItemPrice);
                        DeleteCashierTableItem(stockItem.Id);
                        //DeleteCashierTableStockItem(id.Value, tableId.Value, qty.Value);
                        scope.Complete();

                        var printerName = "";

                        var actualStockItem = new StockItem { Id = stockItem.ItemId, Quantity = 1, Description = stockItem.StockItemName, StockItemName = stockItem.StockItemName, UnitPrice = stockItem.StockItemPrice };

                        if (actualStockItem != null)
                        {
                            printerName = stockItem.ProductDepartment;
                            SendToPosPrinterVoids(printerName, new List<StockItem> { actualStockItem }, tableName, DateTime.Now);
                        }

                    }
                }
            }



            var vm = new CabbashViewModel();

            var exList0 = await GetMyTableItemsByTableIdAll(tableId.Value);

            vm.ExistingList = exList0.ToList();

            var loadPos = RenderRazorViewToString("_LoadPOS", vm);

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);

            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = tableId.Value;

            var loadPosBottom = RenderRazorViewToString("_LoadPOSBottom", vm);

            return Json(new
            {
                ReturnValue = true,
                success = true,
                InsertError = 0,
                LoadPosView = loadPos,
                LoadPosBottom = loadPosBottom
            }, JsonRequestBehavior.AllowGet);


        }

      

        public async Task<ActionResult> ReduceOrDeleteQuatity(int? tableId, int? id, int? qty, int? type)
        {
            List<TableItem> items = new List<TableItem>();

            string tableCart = tableId.ToString();

            if (Session[tableCart] != null)
                items = (List<TableItem>)Session[tableCart];

            var itemIdString = id.ToString().Substring(1);

            if (!type.HasValue && !qty.HasValue)
            {
                var remove = items.FirstOrDefault(x => x.Id == id.Value);

                if(remove != null)
                {
                    items.Remove(remove);
                    await DeleteTableItem(id.Value);
                }
            }

            if (qty.HasValue && qty.Value >= 0 && items.Any())
            {
                items.Find(x => x.Id == id.Value).Qty = qty.Value;
                await UpdateTableItemQtyById(id.Value, qty.Value);
            }


            Session[tableCart] = items;

            var vm = new CabbashViewModel();

            vm.ExistingList = items.ToList();

            var loadPos = RenderRazorViewToString("_LoadPOS", vm);

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);

            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = tableId.Value;

            var loadPosBottom = RenderRazorViewToString("_LoadPOSBottom", vm);

            return Json(new
            {
                ReturnValue = true,
                success = true,
                InsertError = 0,
                LoadPosView = loadPos,
                LoadPosBottom = loadPosBottom
            }, JsonRequestBehavior.AllowGet);
        }

   

        private int GetPlaceId()
        {
            int placeId = 1;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["PlaceId"].ToString(), out placeId);
            }
            catch
            {

            }

            return placeId;
        }


        private bool GetPrintStatus(int id)
        {
            bool flag;
            int key = id;
            try
            {
                string str = this.PersonId.ToString() + "_hashtab";
                Hashtable hashtable = (Hashtable)base.Session[str];
                if (hashtable == null)
                {
                    hashtable = new Hashtable {
                {
                    key,
                    key
                }
            };
                    base.Session[str] = hashtable;
                    flag = true;
                }
                else
                {
                    try
                    {
                        hashtable.Add(key, key);
                        base.Session[str] = hashtable;
                        flag = true;
                    }
                    catch
                    {
                        flag = false;
                    }
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        private int PrintBillOnlyNewByTableItem(string printerName, List<TableItem> list, int? tableId, int personId, int? guestOrderId, BarTable bt, string guestOrderNote = "")
        {
            List<StockItem> lst = new List<StockItem>();
            string orderNo = "";
            DateTime now = DateTime.Now;
            foreach (TableItem item in Enumerable.ToList<TableItem>(list))
            {
                int qty = item.Qty;
                string stockItemName = item.StockItemName;
                orderNo = Enumerable.FirstOrDefault<TableItem>(list).Id.ToString();
                StockItem item1 = new StockItem();
                item1.Id = item.ItemId;
                item1.Quantity = qty;
                item1.UnitPrice = 0M;
                item1.Description = stockItemName;
                lst.Add(item1);
            }
            return base.SendToPosPrinter(printerName, personId, lst, bt.TableAlias, true, guestOrderId, new DateTime?(now), guestOrderNote, orderNo);
        }

   
        [HttpPost]
        public ActionResult ReProcessTheOrderByTableNumberOne(int? tableId, string kitchenNote, string guestTableNumber)
        {
            BarTable table = Enumerable.LastOrDefault<BarTable>(base.GetBarTableByIdNoAsync(tableId.Value));
            if (table == null)
            {
                string str3 = "Sorry, this table has been closed by the cashier!";
                return base.Json(new
                {
                    ErrorMessage = str3,
                    Success = 0
                }, JsonRequestBehavior.AllowGet);
            }
            int staffId = table.StaffId;
            if (string.IsNullOrEmpty(kitchenNote))
            {
                kitchenNote = "NONE";
            }
            int placeId = this.GetPlaceId();
            List<int> list1 = new List<int>();
            CabbashViewModel model = new CabbashViewModel();
            base.ReProcessWaiterOrderReady(tableId.Value, this.PersonId, kitchenNote, placeId);
            model.ExistingList = Enumerable.ToList<TableItem>(base.GetMyTableItemsByTableIdAllNoAsync(tableId.Value));
            Enumerable.ToList<TableItem>((IEnumerable<TableItem>)(from x in model.ExistingList
                                                                  where !x.IsActive
                                                                  select x)).ForEach(delegate (TableItem x) {
                                                                      x.SentToPOS = true;
                                                                  });
            string str = this.RenderRazorViewToString("_LoadPOS", model);
            decimal num2 = 0M;
            int num3 = 0;
            foreach (TableItem item in model.ExistingList)
            {
                num2 += item.Qty * item.StockItemPrice;
                num3 += item.Qty;
            }
            model.CanTakePayment = false;
            model.CanCancelSale = true;
            model.CanTakePayment = (model.ExistingList.Count > 0) && !Enumerable.Any<TableItem>(model.ExistingList, x => !x.IsActive);
            model.CanCancelSale = (model.ExistingList.Count > 0) && !Enumerable.Any<TableItem>(model.ExistingList, x => x.IsActive);
            int num4 = Enumerable.Count<TableItem>(model.ExistingList, x => x.SentToPOS && !x.IsActive);
            model.CanSendToDepartment = (model.ExistingList.Count > 0) && (num4 > 0);
            model.TotalItems = num3;
            model.Total = num2;
            model.TableId = tableId.Value;
            return base.Json(new
            {
                ReturnValue = true,
                Success = 1,
                InsertError = 0,
                LoadPosView = str,
                LoadPosBottom = this.RenderRazorViewToString("_LoadPOSBottom", model),
                OpenedTableName = table.TableName + " PIN: " + table.Id.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        //ProcessTheDepartment


        [HttpPost]
        public ActionResult ProcessTheDepartment(int? tableId, string kitchenNote)
        {
            BarTable bt = Enumerable.FirstOrDefault<BarTable>(base.GetBarTableByIdNoAsync(tableId.Value));

            if (bt == null)
            {
                string str = "Sorry, this table has been closed by the cashier!";
                return base.Json(new
                {
                    ErrorMessage = str,
                    Success = 0
                }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(kitchenNote))
            {
                kitchenNote = "NONE";
            }
            int placeId = this.GetPlaceId();
            List<int> list1 = new List<int>();
            CabbashViewModel model1 = new CabbashViewModel();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<TableItem> list = Enumerable.ToList<TableItem>(base.ProcessWaiterOrderOutput(tableId.Value, this.PersonId, kitchenNote, placeId));
                    int num2 = Enumerable.Count<IGrouping<int, TableItem>>((IEnumerable<IGrouping<int, TableItem>>)(from x in list group x by x.ItemId));
                    int num3 = 0;
                    if (Enumerable.Any<TableItem>(list))
                    {
                        int num4 = 0;
                        if (this.GetPrintStatus(Enumerable.FirstOrDefault<TableItem>(list).Id))
                        {
                            using (List<TableItemGroup>.Enumerator enumerator = Enumerable.ToList<TableItemGroup>(Enumerable.Select<IGrouping<string, TableItem>, 
                                TableItemGroup>((IEnumerable<IGrouping<string, TableItem>>)(from x in list group x by x.ProductDepartment), delegate (IGrouping<string, TableItem> x) 
                            {
                                TableItemGroup group1 = new TableItemGroup();
                                group1.PrinterName = x.Key;
                                group1.TableItems = Enumerable.ToList<TableItem>((IEnumerable<TableItem>)x);
                                return group1;
                            })).GetEnumerator())
                            {
                                while (true)
                                {
                                    if (!enumerator.MoveNext())
                                    {
                                        break;
                                    }
                                    TableItemGroup current = enumerator.Current;
                                    num4 = this.PrintBillOnlyNewByTableItem(current.PrinterName, current.TableItems, tableId, this.PersonId, 0, bt, kitchenNote);
                                    num3 += num4;
                                    if (num4 == 0)
                                    {
                                        base.SendErrorToPrinter(current.PrinterName, "There was an error printing Order", Enumerable.FirstOrDefault<TableItem>(current.TableItems).Id.ToString(), bt.TableAlias);
                                        string str2 = this.PersonId.ToString() + "_hashtab";
                                        base.Session[str2] = null;
                                        return base.Json(new
                                        {
                                            ErrorMessage = "Please contact the kitchen immediately about this order, there seems to have been a printer error!",
                                            ReturnValue = true,
                                            Success = 0,
                                            InsertError = 1
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                        }
                        if (num3 != num2)
                        {
                            foreach (TableItemGroup group2 in Enumerable.ToList<TableItemGroup>(Enumerable.Select<IGrouping<string, TableItem>, TableItemGroup>((IEnumerable<IGrouping<string, TableItem>>)(from x in list group x by x.ProductDepartment), delegate (IGrouping<string, TableItem> x) {
                                TableItemGroup group1 = new TableItemGroup();
                                group1.PrinterName = x.Key;
                                group1.TableItems = Enumerable.ToList<TableItem>((IEnumerable<TableItem>)x);
                                return group1;
                            })))
                            {
                                base.SendErrorToPrinter(group2.PrinterName, "There was an error printing Order", Enumerable.FirstOrDefault<TableItem>(group2.TableItems).Id.ToString(), bt.TableAlias);
                            }
                            string str3 = this.PersonId.ToString() + "_hashtab";
                            base.Session[str3] = null;
                            return base.Json(new
                            {
                                ErrorMessage = "Please contact the kitchen immediately about this order, there seems to have been a printer error!",
                                ReturnValue = true,
                                Success = 0,
                                InsertError = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception)
            {
               
            }
            return base.Json(new
            {
                ReturnValue = true,
                Success = 1,
                InsertError = 0
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ProcessTheOrderByTableNumberOne(int? tableId, string kitchenNote, string guestTableNumber)
        {
            BarTable table = Enumerable.LastOrDefault<BarTable>(base.GetBarTableByIdNoAsync(tableId.Value));
            if (table == null)
            {
                string str3 = "Sorry, this table has been closed by the cashier!";
                return base.Json(new
                {
                    ErrorMessage = str3,
                    Success = 0
                }, JsonRequestBehavior.AllowGet);
            }
            int staffId = table.StaffId;
            if (string.IsNullOrEmpty(kitchenNote))
            {
                kitchenNote = "NONE";
            }
            int placeId = this.GetPlaceId();
            List<int> list1 = new List<int>();
            CabbashViewModel model = new CabbashViewModel();
            base.ProcessWaiterOrderReady(tableId.Value, this.PersonId, kitchenNote, placeId);
            model.ExistingList = Enumerable.ToList<TableItem>(base.GetMyTableItemsByTableIdAllNoAsync(tableId.Value));
            Enumerable.ToList<TableItem>((IEnumerable<TableItem>)(from x in model.ExistingList
                                                                  where !x.IsActive
                                                                  select x)).ForEach(delegate (TableItem x) {
                                                                      x.SentToPOS = true;
                                                                  });
            string str = this.RenderRazorViewToString("_LoadPOS", model);
            decimal num2 = 0M;
            int num3 = 0;
            foreach (TableItem item in model.ExistingList)
            {
                num2 += item.Qty * item.StockItemPrice;
                num3 += item.Qty;
            }
            model.CanTakePayment = false;
            model.CanCancelSale = true;
            model.CanTakePayment = (model.ExistingList.Count > 0) && !Enumerable.Any<TableItem>(model.ExistingList, x => !x.IsActive);
            model.CanCancelSale = (model.ExistingList.Count > 0) && !Enumerable.Any<TableItem>(model.ExistingList, x => x.IsActive);
            int num4 = Enumerable.Count<TableItem>(model.ExistingList, x => x.SentToPOS && !x.IsActive);
            model.CanSendToDepartment = (model.ExistingList.Count > 0) && (num4 > 0);
            model.TotalItems = num3;
            model.Total = num2;
            model.TableId = tableId.Value;
            return base.Json(new
            {
                ReturnValue = true,
                Success = 1,
                InsertError = 0,
                LoadPosView = str,
                LoadPosBottom = this.RenderRazorViewToString("_LoadPOSBottom", model),
                OpenedTableName = table.TableName + " PIN: " + table.Id.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProcessTheOrderByTableNumberOneOlder(int? tableId, string kitchenNote, string guestTableNumber)
        {
            var btA = GetBarTableByIdNoAsync(tableId.Value);

            BarTable bt = btA.LastOrDefault();

            var tableWaiterId = 0;

            if(bt == null)
            {
                var errorMessageGuest = "Sorry, this table has been closed by the cashier!";

                return Json(new
                {
                    ErrorMessage = errorMessageGuest,
                    Success = 0,
                }, JsonRequestBehavior.AllowGet);
            }

            tableWaiterId = bt.StaffId;

            if (string.IsNullOrEmpty(kitchenNote))
            {
                kitchenNote = "NONE";
            }
            

            int placeId = GetPlaceId();

            List<TableItem> items = new List<TableItem>();

            string tableCart = tableId.ToString();

            var allItemsJustPushed = new List<int>();

            Session[tableCart] = null;

            var vm = new CabbashViewModel();

            var exList = GetMyTableItemsByTableIdAllNoAsync(tableId.Value);

            var allItems = exList.ToList();

            ProcessWaiterOrder(tableId.Value, PersonId, kitchenNote, placeId);

            var lstForPrint = allItems.Where(x => !x.IsActive).ToList();

            vm.ExistingList = allItems.ToList();

            vm.ExistingList.ForEach(x => x.IsActive = true);

            if (lstForPrint.Any())
            {
                var groupList = lstForPrint.GroupBy(x => x.ProductDepartment).Select(x => new TableItemGroup { PrinterName = x.Key, TableItems = x.ToList() }).ToList();

                foreach (var order in groupList)
                {
                    PrintBillOnlyNewByTableItem(order.PrinterName, order.TableItems, tableId, PersonId, 0, kitchenNote);
                }
            }

            var loadPos = RenderRazorViewToString("_LoadPOS", vm);

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);

            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = tableId.Value;

            var loadPosBottom = RenderRazorViewToString("_LoadPOSBottom", vm);

            //OpenedTableName

            return Json(new
            {
                ReturnValue = true,
                Success = 1,
                InsertError = 0,
                LoadPosView = loadPos,
                LoadPosBottom = loadPosBottom,
                OpenedTableName = bt.TableName + " PIN: " + bt.Id.ToString(),
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(int? code, int? tableId, bool? ignore, string name, decimal? price, int? product_id, int? number)//name: name1, price: price1, product_id: id, number: number, registerid:
        {

            code = product_id;

            TableItem ti = new TableItem();

            List<TableItem> items = new List<TableItem>();

            string tableCart = tableId.ToString();

            try
            {
                if (Session[tableCart] != null)
                    items = (List<TableItem>)Session[tableCart];
            }
            catch
            {
                return Json(new
                {
                    InsertError = 1
                }, JsonRequestBehavior.AllowGet);
            }

            var vm = new CabbashViewModel();

            int personId = GetPersonId();

            var addedItem = new TableItem { Cashier = personId, StockItemId = code.Value, ItemId = code.Value, TableId = tableId.Value, Qty = 1, StockItemName = name, StockItemPrice = price.Value };

            var latestId = await InsertTableItem(addedItem);

            addedItem.Id = latestId;

            vm.SingleItem = addedItem;

            items.Add(addedItem);

            Session[tableCart] = items;

            var loadSingleItem = RenderRazorViewToString("_SingleItemLoadPlus", vm);

            return Json(new
            {
                InsertError = 0,
                LoadSingleItem = loadSingleItem,
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<CabbashViewModel> GetAllModel(int? id)
        {
            var vm = new CabbashViewModel();

            //var every = POSService.StockItemService.GetMyTableItems(PersonId).ToList();

            var exList = await GetMyTableItemsByTableIdAll(id.Value);

            vm.ExistingList = exList.ToList();

            var total = decimal.Zero;

            int totalCount = 0;

            foreach (var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItemPrice;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);
            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = id.Value;

            return vm;
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        private const string CacheKey = "availableProducys";
        public IEnumerable<POSService.Entities.StockItem> ProductsList()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(CacheKey))
                return (IEnumerable<POSService.Entities.StockItem>)cache.Get(CacheKey);
            else
            {
                var lst = GetStockItemsPOSNoAsync();

                IEnumerable<POSService.Entities.StockItem> availableProducts = lst.ToList();

                // Store data in the cache    
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(8.0);
                cache.Add(CacheKey, availableProducts, cacheItemPolicy);

                return availableProducts;
            }

        }
    }
}