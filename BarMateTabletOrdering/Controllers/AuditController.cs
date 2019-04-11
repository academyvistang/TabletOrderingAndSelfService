using BarAndRestaurantMate.Helpers;
using BarMateTabletOrdering.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BarMateTabletOrdering.Controllers
{
    public class AuditController : BaseController
    {

        [HttpPost]
        public async Task<ActionResult> PrintPos(DateTime startDate, DateTime endDate)
        {
            
            var paymentA = await GetPayments(startDate, endDate);
            var payments = paymentA.ToList();
            var sitemsA = await GetSoldItems(startDate, endDate);
            var sitems = sitemsA.ToList();

            var all = payments.Select(x => new PaymentsSoldItems { Payment = x, SoldItems = GetSoldItemsMine(sitems, x.ReceiptNumber) }).ToList();

            CabbashViewModel vm = new CabbashViewModel { };

            vm.AllPayments = all;

            vm.StartDate = startDate;

            vm.EndDate = endDate;

            var sitemsGB = sitems.GroupBy(x => x.PaymentMethodName).Select(x => new SoldItemGB { PaymentMethodName = x.Key, Items = x.ToList() }).ToList();

            var ovTotalCash = payments.Sum(x => x.TotalCash);

            var ovTotalpos = payments.Sum(x => x.TotalPOS);

            var ovTotalTransfers = payments.Sum(x => x.TotalTransfer);


            var splitPayments = payments.Where(x => x.PaymentMethodId == 7).ToList();

            var splitCash = splitPayments.Sum(x => x.TotalCash);

            var splitPos = splitPayments.Sum(x => x.TotalPOS);

            var splitTransfers = splitPayments.Sum(x => x.TotalTransfer);

            var path = Path.Combine(Server.MapPath("~/Images/"), "logo.png");

            OposPrinter.PrintReceipt(sitemsGB, path, splitCash, splitPos, splitTransfers, ovTotalCash, ovTotalpos, ovTotalTransfers);

            return View("Index", vm);
        }
        // GET: Audit
        public async Task<ActionResult> Index()
        {
            var auditA = await GetLastAudit();
            var audit = auditA.FirstOrDefault();

            var startDate = DateTime.Now.AddDays(-7);
            var enddate = DateTime.Now;

            if(audit != null)
            {
                startDate = audit.ClosureEndDate.AddSeconds(3);
                enddate = DateTime.Now;
            }
            //GetSoldItems
            var paymentA = await GetPayments(startDate, enddate);
            var payments = paymentA.ToList();
            var sitemsA = await GetSoldItems(startDate, enddate);
            var sitems = sitemsA.ToList();

            var all = payments.Select(x => new PaymentsSoldItems { Payment = x, SoldItems = GetSoldItemsMine(sitems, x.ReceiptNumber) }).ToList();

            CabbashViewModel vm = new CabbashViewModel { };

            vm.AllPayments = all;

            vm.StartDate = startDate;

            vm.EndDate = enddate;
            
            var realTablesA = await GetMyTables(0);

            var realTables = realTablesA.GroupBy(x => x.TableId).Any();
            //.Select(x => new BarTable { TableId = x.Key, Id = x.FirstOrDefault().Id, IsActive = x.FirstOrDefault().IsActive, StaffId = x.FirstOrDefault().StaffId }).ToList();

            vm.WeHaveOpenTables = realTables;

            return View(vm);
        }

        private List<SoldItem> GetSoldItemsMine(List<SoldItem> sitems, string receiptNumber)
        {
            return sitems.Where(x => x.ReceiptNumber.ToUpper() == receiptNumber.ToUpper()).ToList();
        }
    }
}