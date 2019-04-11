using BarMateTabletOrdering.Models;
using POSService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZXing;

namespace BarMateTabletOrdering.Controllers
{
    public class HomeController : BaseController
    {
        
        private int _distributionPointId = 2;

        public HomeController()
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



        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    filterContext.ExceptionHandled = true;

        //    //Log the error!!
        //    //_Logger.Error(filterContext.Exception);

        //    //Redirect or return a view, but not both.
        //    filterContext.Result = RedirectToAction("Index", "ErrorHandler");
        //    // OR 
        //    //filterContext.Result = new ViewResult
        //    //{
        //    //    ViewName = "~/Views/ErrorHandler/Index.cshtml"
        //    //};
        //}

        private const string CacheKey = "availableProducys";
        public async Task<IEnumerable<POSService.Entities.StockItem>> ProductsList()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(CacheKey))
                return (IEnumerable<POSService.Entities.StockItem>)cache.Get(CacheKey);
            else
            {
                var lst = await GetStockItemsPOS();

                IEnumerable<POSService.Entities.StockItem> availableProducts = lst.ToList();

                // Store data in the cache    
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(8.0);
                cache.Add(CacheKey, availableProducts, cacheItemPolicy);

                return availableProducts;
            }

        }
        public async Task<ActionResult> Menu(string name)
        {
            var folderPath = Path.Combine(Server.MapPath("~/Images/Display/"));

            DirectoryInfo d = new DirectoryInfo(folderPath);

            var imagePathListAboutUsImages = d.GetFiles("*.*").OrderByDescending(x => x.CreationTime).Select(x => x.Name).ToList();

            CabbashViewModel model = new CabbashViewModel { };

            model.DisplayImages = imagePathListAboutUsImages;

            var pList = await ProductsList();

            var products = pList.Where(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId).ToList();

            var catList = products.OrderBy(x => x.CategoryName).Select(x => x.CategoryName).ToList();

            catList = catList.Distinct().ToList();

            ViewBag.Menu = true;

            ViewBag.MenuList = catList.Take(17).ToList();

            return PartialView("_Menu", model);
        }

        public string GenerateToken(int length)
        {
            RNGCryptoServiceProvider cryptRNG = new RNGCryptoServiceProvider();
            byte[] tokenBuffer = new byte[length];
            cryptRNG.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);
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

        public ActionResult GenerateMenuUrl()
        {
            int placeId = GetPlaceId();

            CabbashViewModel vm = new CabbashViewModel();

            var qrcode = "https://www.cabbash.com";

            var transactionDate = DateTime.Now;

            qrcode = "https://www.cabbash.com" + Url.Action("ViewMenu", "Hot", new { id = placeId }).ToString();

            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.QR_CODE;
                var result = writer.Write(qrcode);

                using (Bitmap bitMap = result)
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    vm.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }


            return PartialView("_QRCodeMenu", vm);
        }


        //
        public ActionResult NumberPad()
        {
            return PartialView("_NumberPad");
        }

        //[OutputCache(Duration = int.MaxValue)]
        public ActionResult Index(bool? invalidCred, bool? tableClash, string tables)
        {
            if(Request.IsAuthenticated)
            {
                var userRole = Roles.GetRolesForUser().FirstOrDefault();

                if (userRole.StartsWith("WAITER") || userRole.StartsWith("DELIVERY") || userRole.StartsWith("MANAGER") || userRole.StartsWith("CLUBWAITER"))
                {
                    return RedirectToAction("Index", "Pos");
                }
                else if (userRole.StartsWith("SELFSERVICE"))
                {
                    return RedirectToAction("Start", "SelfService");
                }
                else if (userRole.StartsWith("ACCOUNTANT"))
                {
                    return RedirectToAction("Index", "Audit");
                }
                else
                {
                    return RedirectToAction("Kitchen", "Pos");
                }
            }

            //var folderPath = Path.Combine(Server.MapPath("~/Images/Display/"));

            //DirectoryInfo d = new DirectoryInfo(folderPath);

            //var imagePathListAboutUsImages = d.GetFiles("*.*").OrderByDescending(x => x.CreationTime).Select(x => x.Name).ToList();

            CabbashViewModel model = new CabbashViewModel { };

            //model.DisplayImages = imagePathListAboutUsImages;

            //var products = ProductsList.Where(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId).ToList();

            //var catList = products.OrderBy(x => x.CategoryName).Select(x => x.CategoryName).ToList();

            //catList = catList.Distinct().ToList();

            //ViewBag.Menu = true;

            ViewBag.MenuList = new List<string>(); // catList.Take(17).ToList();

            model.InvalidCred = invalidCred;

            model.TableClash = tableClash;

            model.TablesClashing = tables;


            return View(model);
        }

       
    }
}