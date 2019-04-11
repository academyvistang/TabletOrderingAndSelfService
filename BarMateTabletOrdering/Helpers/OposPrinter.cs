using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;

using POSService.Entities;
using System.Configuration;

using BarMateTabletOrdering.Models;

namespace BarAndRestaurantMate.Helpers
{
    public static class OposPrinter
    {
        public static Image Base64ToImage(string base64String)
        {
            base64String = base64String.Replace("data:image/png;base64,", "");
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static void PrintReceipt(string path, int placeId, string pinCode, string[] splitDetails, List<PrintStockItemModel> grpList, double dTotal, string tax, decimal? taxamount, string discount, decimal discountamount, string resident, decimal? residentAmount, decimal? serviceCharge, string v, int? paymentMethodId, decimal? subtotal, int? totalitems, bool printOnly, decimal? overallPaid, decimal? change, Guest postBillGuest, string thisUserName, string receiptNumber, string guestTableNumber, int loyaltyBalance, string loyaltyTelephone)
        {
            Image qrImage = null;

            if (!string.IsNullOrEmpty(pinCode))
            {
                var qrcode = "https://www.cabbash.com/hot/ViewMyBillQR/?placeId=" + placeId + "&pinCode=" + pinCode;

                //var codeImage = "";
                var codeImage = "";

                using (MemoryStream ms = new MemoryStream())
                {
                    var writer = new BarcodeWriter();
                    writer.Format = BarcodeFormat.QR_CODE;
                    var result = writer.Write(qrcode);

                    using (Bitmap bitMap = result)
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        codeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }

                qrImage = Base64ToImage(codeImage);

            }

            
            var imageLogo = Image.FromFile(path);

            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                if (imageLogo != null)
                {
                    Bitmap logo = (Bitmap)imageLogo;

                    int alignment = PosPrinter.PrinterBitmapCenter;

                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        logo.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms))
                        {
                            //printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, PosPrinter.PrinterBitmapAsIs, alignment);
                            printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, 300, alignment);
                            printer.PrintNormal(PrinterStation.Receipt, "" + Environment.NewLine);
                        }
                    }
                }

                var highItemPrice = grpList.Where(x => x.UnitPrice > 99999).Any();

                if (splitDetails != null)
                {
                    PrintReceiptHeader(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName, receiptNumber, guestTableNumber, loyaltyBalance, loyaltyTelephone, highItemPrice);
                }
                else
                {
                    PrintReceiptHeader(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName, receiptNumber, guestTableNumber, loyaltyBalance, loyaltyTelephone, highItemPrice);
                }

                foreach (var item in grpList)
                {
                    PrintLineItem(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()), item.UnitPrice > 99999);
                }

                //PrintLineItem(printer, "Item 1 And A Plate of Prawns Sandwich To Go", 10, 220000.99, true);
                //PrintLineItem(printer, "Item 2", 101, 0.00, true);
                //PrintLineItem(printer, "Item 3", 9, 0.1, true);
                //PrintLineItem(printer, "Item 4", 1000, 1, true);

                PrintReceiptFooter(printer, dTotal, tax, taxamount, discount, discountamount, resident, residentAmount, serviceCharge, "SCAN CODE BELOW TO VIEW YOUR RECEIPT",
                       paymentMethodId, subtotal, totalitems, printOnly, overallPaid, change, postBillGuest);

                PrintReceiptFooter(printer, dTotal, dTotal, dTotal, "SCAN CODE BELOW TO VIEW YOUR RECEIPT");

                if (qrImage != null)
                {
                    Bitmap logo = (Bitmap)qrImage;

                    int alignment = PosPrinter.PrinterBitmapCenter;

                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        logo.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms))
                        {
                            //printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, PosPrinter.PrinterBitmapAsIs, alignment);
                            printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, 300, alignment);
                            printer.PrintNormal(PrinterStation.Receipt, "" + Environment.NewLine);
                        }
                    }
                }



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
            finally
            {
                DisconnectFromPrinter(printer);
            }
        }

        public static void PrintReceipt(List<SoldItemGB> sitemsGB, string path, decimal splitCash, decimal splitPos, decimal splitTransfers, decimal totalCash, decimal totalPos, decimal totalTransfers)
        {
            var imageLogo = Image.FromFile(path);

            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                if (imageLogo != null)
                {
                    Bitmap logo = (Bitmap)imageLogo;

                    int alignment = PosPrinter.PrinterBitmapCenter;

                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        logo.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms))
                        {
                            //printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, PosPrinter.PrinterBitmapAsIs, alignment);
                            printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, 300, alignment);
                            printer.PrintNormal(PrinterStation.Receipt, "" + Environment.NewLine);
                        }
                    }
                }

               

                foreach (var items in sitemsGB)
                {
                    PrintTextLine(printer, items.PaymentMethodName);
                    PrintTextLine(printer, items.Items.Sum(x => x.TotalPrice).ToString());

                    if(items.PaymentMethodName.ToUpper().StartsWith("SPLIT"))
                    {
                        PrintTextLine(printer, "##########################################");
                        PrintTextLine(printer, "SPLIT CASH ----    " + splitCash.ToString());
                        PrintTextLine(printer, "##########################################");
                        PrintTextLine(printer, "SPLIT POS ----    " + splitPos.ToString());
                        PrintTextLine(printer, "##########################################");
                        PrintTextLine(printer, "SPLIT TRANSFERS ----    " + splitTransfers.ToString());
                    }

                    PrintTextLine(printer, "##########################################");
                    PrintTextLine(printer, String.Empty);

                    foreach (var item in items.Items)
                    {
                        PrintLineItem(printer, item.StockItemName, item.Qty, double.Parse(item.TotalPrice.ToString()), item.TotalPrice > 99999);
                    }
                }

                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, "##########################################");
                PrintTextLine(printer, "TOTAL CASH ----    " + totalCash.ToString());
                PrintTextLine(printer, "##########################################");

                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, "##########################################");
                PrintTextLine(printer, "TOTAL POS ----    " + totalPos.ToString());
                PrintTextLine(printer, "##########################################");

                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, "##########################################");
                PrintTextLine(printer, "TOTAL TRANSFERS ----    " + totalTransfers.ToString());
                PrintTextLine(printer, "##########################################");



                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, "##########################################");
                PrintTextLine(printer, "TOTAL ALL ----    " + sitemsGB.SelectMany(x => x.Items).Sum(x => x.TotalPrice).ToString());
                PrintTextLine(printer, "##########################################");





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
            finally
            {
                DisconnectFromPrinter(printer);
            }
        }

        private static int GetUseSmallReceipt()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["UseSmallReceipt"].ToString(), out hTax);
            }
            catch
            {
                hTax = 1;
            }

            return hTax;
        }

        private static int GetRestaurantTax()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["RestaurantTax"].ToString(), out hTax);
            }
            catch
            {
                hTax = 0;
            }

            return hTax;
        }

        private static void PrintReceiptFooter(PosPrinter printer, double dTotal, string tax, decimal? taxamount, string discount, decimal discountamount, string resident, 
            decimal? residentAmount, decimal? serviceCharge, string footerText, int? paymentMethodId, decimal? subtotal, int? totalitems, bool printOnly, decimal? overallPaid,
            decimal? change, Guest postBillGuest)
        {
            int RecLineChars = 42;

            string offSetString = new string(' ', ((RecLineChars / 2) - 4));
            offSetString = new string(' ', ((RecLineChars / 3) - 4));

            var finalTotal = dTotal;

            var contax = decimal.Zero;
            var vat = decimal.Zero;

            var useSmallReceipt = GetUseSmallReceipt();

            try
            {
                var taxValue = decimal.Zero;

                decimal.TryParse(GetRestaurantTax().ToString(), out taxValue);

                if (taxValue > 0)
                {
                    taxamount = decimal.Divide(taxValue, 100) * (decimal)dTotal;

                    if (taxamount.HasValue && taxamount.Value > 0)
                    {
                        contax = decimal.Divide(taxamount.Value, 2);
                        vat = contax;
                    }

                    subtotal = subtotal - taxamount;
                }
            }
            catch
            {

            }

            if (useSmallReceipt == 0)
            {
               PrintTextLine(printer, new string('-', (RecLineChars / 3) * 2));
               PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL   {0}", subtotal.Value.ToString("#0.00")));
               PrintTextLine(printer, offSetString + String.Format("VAT (5%)    {0}", vat.ToString("#0.00")));
               PrintTextLine(printer, offSetString + String.Format("C'TAX (5%)  {0}", contax.ToString("#0.00")));
               PrintTextLine(printer, offSetString + String.Format("DISCOUNT    {0}", discountamount.ToString("#0.00")));
               PrintTextLine(printer, offSetString + String.Format("S/CHARGE    {0}", serviceCharge.Value.ToString("#0.00")));


                if (residentAmount.HasValue)
                {
                   PrintTextLine(printer, offSetString + String.Format("RESIDENT    {0}", residentAmount.Value.ToString("#0.00")));
                }
            }


           //PrintTextLine(printer, offSetString + new string('-', (RecLineChars / 3)));
           //PrintTextLine(printer, offSetString + String.Format("TOTAL        {0}", finalTotal));
           //PrintTextLine(printer, offSetString + new string('-', (RecLineChars / 3)));



            if (printOnly)
            {
               PrintTextLine(printer, offSetString + String.Format("PRINTONLY -{0}", "BILL NOT CLOSED"));
            }
            else
            {

                //offSetString = new string(' ', printer.RecLineChars / 2);

                //PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
                ////PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL     {0}", subTotal.ToString("#0.00")));
                ////PrintTextLine(printer, offSetString + String.Format("TAX           {0}", tax.ToString("#0.00")));
                ////PrintTextLine(printer, offSetString + String.Format("DISCOUNT      {0}", discount.ToString("#0.00")));
                //PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
                //PrintTextLine(printer, offSetString + String.Format("TOTAL         {0}", finalTotal.ToString("#0.00")));
                //PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
                //PrintTextLine(printer, String.Empty);

                ////Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
                //PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
                //if (overallPaid.HasValue)
                //{
                //   PrintTextLine(printer, offSetString + new string('-', (RecLineChars / 3)));
                //   PrintTextLine(printer, offSetString + String.Format("PAID        {0}", overallPaid.Value));
                //   PrintTextLine(printer, offSetString + new string('-', (RecLineChars / 3)));
                //}

                //if (change.HasValue)
                //{
                //   PrintTextLine(printer, offSetString + new string('-', (RecLineChars / 3)));
                //   PrintTextLine(printer, offSetString + String.Format("CHANGE        {0}", change.Value));
                //   PrintTextLine(printer, offSetString + new string('-', (RecLineChars / 3)));
                //}

                //if (paymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY)
                //{
                //   PrintTextLine(printer, offSetString + String.Format("COMPLIMENT -{0}", "CLOSED"));
                //}
                //else if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL)
                //{
                //   PrintTextLine(printer, offSetString + String.Format("POST BILL -{0}", "CLOSED"));
                //}
                //else if (paymentMethodId == (int)PaymentMethodEnum.CreditCard)
                //{
                //   PrintTextLine(printer, offSetString + String.Format("POS -{0}", "CLOSED"));
                //}
                //else
                //{
                //   PrintTextLine(printer, offSetString + String.Format("CASHED -----{0}", "CLOSED"));
                //}
            }

            if (postBillGuest != null)
            {
               PrintTextLine(printer, offSetString + String.Format("RM {0} ---------------", postBillGuest.RoomNumber));
            }

            ////Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            //var eCentre = Convert.ToChar(27) + Convert.ToChar(97) + "1";
            ////PrintTextLineRaw(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
            //offSetString = new string(' ', RecLineChars / 4);
            //PrintTextLine(printer, offSetString + footerText);
            //byte[] DrawerOpen5 = { 0xA };
           
        }

        private static void PrintReceiptHeader(PosPrinter printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime,
           string cashierName, string receiptNumber, string guestTableNumber, int loyaltyBalance, string loyaltyTelephone, bool highItemPrice)
        {
            int RecLineChars = 42; //farmcity
            //PrintTextLine(printer, companyName);
            //PrintTextLine(printer, addressLine1);
            //PrintTextLine(printer, addressLine2);
            //PrintTextLine(printer, taxNumber);
            //PrintTextLine(printer, new string('-', RecLineChars));
            //PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Format("RECEIPT NO. : {0}", receiptNumber));
            PrintTextLine(printer, String.Format("TABLE : {0}", guestTableNumber));

            if (loyaltyBalance > 0 && !string.IsNullOrEmpty(loyaltyTelephone))
            {
                PrintTextLine(printer, String.Format("LOYALTY ID : {0}", loyaltyTelephone));
                PrintTextLine(printer, String.Format("LOYALTY POINTS : {0}", loyaltyBalance));
            }

            PrintTextLine(printer, String.Empty);

            if(highItemPrice)
            {
                PrintText(printer, "Item             ");
                PrintText(printer, "Qty  ");
                PrintText(printer, "Price     ");
                PrintText(printer, "Total      ");
                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, new string('=', RecLineChars));
                PrintTextLine(printer, String.Empty);
            }
            else
            {
                PrintText(printer, "Item                 ");
                PrintText(printer, "Qty  ");
                PrintText(printer, "Price     ");
                PrintText(printer, "Total      ");
                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, new string('=', RecLineChars));
                PrintTextLine(printer, String.Empty);
            }
           
        }

        public static void PrintReceipt(int placeId, string pinCode)
        {
            Image qrImage = null;

            if (!string.IsNullOrEmpty(pinCode))
            {
                var qrcode = "https://www.cabbash.com/hot/ViewMyBillQR/?placeId=" + placeId + "&pinCode=" + pinCode;

                //var codeImage = "";
                var codeImage = "";

                using (MemoryStream ms = new MemoryStream())
                {
                    var writer = new BarcodeWriter();
                    writer.Format = BarcodeFormat.QR_CODE;
                    var result = writer.Write(qrcode);

                    using (Bitmap bitMap = result)
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        codeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }

                qrImage = Base64ToImage(codeImage);

            }
            

            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                PrintReceiptHeader(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, "ABCDEF");

                PrintLineItem(printer, "Item 1 And A Plate of Prawns Sandwich To Go", 10, 220000.99, true);
                PrintLineItem(printer, "Item 2", 101, 0.00, true);
                PrintLineItem(printer, "Item 3", 9, 0.1, true);
                PrintLineItem(printer, "Item 4", 1000, 1, true);

                PrintReceiptFooter(printer, 1, 0.1, 0.1, "SCAN CODE BELOW TO VIEW YOUR RECEIPT");

                if(qrImage != null)
                {
                    Bitmap logo = (Bitmap)qrImage;

                    int alignment = PosPrinter.PrinterBitmapCenter;

                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        logo.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms))
                        {
                            //printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, PosPrinter.PrinterBitmapAsIs, alignment);
                            printer.PrintMemoryBitmap(PrinterStation.Receipt, bmp, 300, alignment);
                            printer.PrintNormal(PrinterStation.Receipt, "" + Environment.NewLine);
                        }
                    }
                }

                

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
            finally
            {
                DisconnectFromPrinter(printer);
            }
        }

        private static void DisconnectFromPrinter(PosPrinter printer)
        {
            printer.Release();
            printer.Close();
        }

        private static void ConnectToPrinter(PosPrinter printer)
        {
            printer.Open();
            printer.Claim(10000);
            printer.DeviceEnabled = true;
        }

        private static PosPrinter GetReceiptPrinter()
        {
            PosExplorer posExplorer = new PosExplorer();
            DeviceInfo receiptPrinterDevice = posExplorer.GetDevice("PosPrinter", "POS1Printer"); //May need to change this if you don't use a logicial name or use a different one.
            return (PosPrinter)posExplorer.CreateInstance(receiptPrinterDevice);
        }

        private static void PrintReceiptFooter(PosPrinter printer, double subTotal, double tax, double discount, string footerText)
        {
            string offSetString = new string(' ', printer.RecLineChars / 2);

            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
            PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL     {0}", subTotal.ToString("#0.00")));
           // PrintTextLine(printer, offSetString + String.Format("TAX           {0}", tax.ToString("#0.00")));
           // PrintTextLine(printer, offSetString + String.Format("DISCOUNT      {0}", discount.ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, offSetString + String.Format("TOTAL         {0}", (subTotal).ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, String.Empty);

            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
        }

        private static void PrintLineItem(PosPrinter printer, string itemCode, int quantity, double unitPrice, bool largeFormat = false)
        {
            var len = itemCode.Length;

            if(largeFormat)
            {
                if (len > 13)
                {
                    PrintTextLine(printer, TruncateAt(itemCode.PadRight(len), len));
                    PrintText(printer, TruncateAt("".PadRight(14), 14));
                }
                else
                {
                    PrintText(printer, TruncateAt(itemCode.PadRight(14), 14));
                }

                PrintText(printer, TruncateAt(quantity.ToString("#0").PadLeft(4), 4));
                PrintText(printer, TruncateAt(unitPrice.ToString("#,##0.##").PadLeft(11), 11));
                PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#,##0.##").PadLeft(12), 12));
            }
            else
            {
                if (len > 20)
                {
                    PrintTextLine(printer, TruncateAt(itemCode.PadRight(len), len));
                    PrintText(printer, TruncateAt("".PadRight(20), 20));
                }
                else
                {
                    PrintText(printer, TruncateAt(itemCode.PadRight(20), 20));
                }

                PrintText(printer, TruncateAt(quantity.ToString("#0").PadLeft(3), 3));
                PrintText(printer, TruncateAt(unitPrice.ToString("#,##0.##").PadLeft(8), 8));
                PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#,##0.##").PadLeft(9), 9));
            }

            
        }

        private static void PrintReceiptHeader(PosPrinter printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime, string cashierName)
        {
            PrintTextLine(printer, companyName);
            PrintTextLine(printer, addressLine1);
            PrintTextLine(printer, addressLine2);
            PrintTextLine(printer, taxNumber);
            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Empty);
            PrintText(printer, "Item           ");
            PrintText(printer, "Qty  ");
            PrintText(printer, "Price     ");
            PrintTextLine(printer, "Total");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

        }

        private static void PrintText(PosPrinter printer, string text)
        {
            if (text.Length <= printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text); //Print text
            else if (text.Length > printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, TruncateAt(text, printer.RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest.
        }

        private static void PrintTextLine(PosPrinter printer, string text)
        {
            if (text.Length < printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text + Environment.NewLine); //Print text, then a new line character.
            else if (text.Length > printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, TruncateAt(text, printer.RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest, no new line character (printer will probably auto-feed for us)
            else if (text.Length == printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text + Environment.NewLine); //Print text, no new line character, printer will probably auto-feed for us.
        }

        private static string TruncateAt(string text, int maxWidth)
        {
            string retVal = text;
            if (text.Length > maxWidth)
                retVal = text.Substring(0, maxWidth);

            return retVal;
        }

       
    }

}