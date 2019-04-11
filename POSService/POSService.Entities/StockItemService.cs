using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace POSService.Entities
{
    public static class StockItemService
    {
        private static void InsertPaymentData(DateTime paymentDate, int paymentMethodId, decimal total, decimal subTotal, string tax, decimal taxAmount,
             string discount, decimal discountAmount, string serviceCharge, decimal serviceChargeAmount, string resident, decimal residentAmount,
             int cashierId, int guestId, string notes, string recieptNumber, decimal paid, decimal outstanding, int type, bool isActive,
             int distributionPointId, string connectionString, decimal splitCash = decimal.Zero, decimal splitTransfer = decimal.Zero, decimal splitPOS = decimal.Zero, int paymentTypeId = 4)
        {
            var wrapper = new DbWrapperTest(connectionString);

            List<SqlParameter> list = new List<SqlParameter>();

            SqlParameter parameter1 = new SqlParameter("@PaymentDate", SqlDbType.DateTime)
            {
                Value = paymentDate
            };



            list.Add(parameter1);

            SqlParameter parameter2 = new SqlParameter("@PaymentMethodId", SqlDbType.Int)
            {
                Value = paymentMethodId
            };

            list.Add(parameter2);

            SqlParameter parameter3 = new SqlParameter("@Total", SqlDbType.Decimal)
            {
                Value = total
            };

            list.Add(parameter3);

            SqlParameter parameter4 = new SqlParameter("@SubTotal", SqlDbType.Decimal)
            {
                Value = subTotal
            };

            list.Add(parameter4);

            SqlParameter parameter5 = new SqlParameter("@Tax", SqlDbType.VarChar)
            {
                Value = tax
            };

            list.Add(parameter5);

            SqlParameter parameter6 = new SqlParameter("@TaxAmount", SqlDbType.Decimal)
            {
                Value = taxAmount
            };

            list.Add(parameter6);

            SqlParameter parameter7 = new SqlParameter("@Discount", SqlDbType.VarChar)
            {
                Value = discount
            };

            list.Add(parameter7);

            SqlParameter parameter8 = new SqlParameter("@DiscountAmount", SqlDbType.Decimal)
            {
                Value = discountAmount
            };

            list.Add(parameter8);

            SqlParameter parameter9 = new SqlParameter("@ServiceCharge", SqlDbType.VarChar)
            {
                Value = serviceCharge
            };

            list.Add(parameter9);

            SqlParameter parameter10 = new SqlParameter("@ServiceChargeAmount", SqlDbType.Decimal)
            {
                Value = serviceChargeAmount
            };

            list.Add(parameter10);

            SqlParameter parameter11 = new SqlParameter("@Resident", SqlDbType.VarChar)
            {
                Value = resident
            };

            list.Add(parameter11);

            SqlParameter parameter12 = new SqlParameter("@ResidentAmount", SqlDbType.Decimal)
            {
                Value = residentAmount
            };

            list.Add(parameter12);

            SqlParameter parameter13 = new SqlParameter("@CashierId", SqlDbType.Int)
            {
                Value = cashierId
            };

            list.Add(parameter13);

            SqlParameter parameter14 = new SqlParameter("@GuestId", SqlDbType.Int)
            {
                Value = guestId
            };

            list.Add(parameter14);

            SqlParameter parameter15 = new SqlParameter("@Notes", SqlDbType.VarChar)
            {
                Value = notes
            };

            list.Add(parameter15);

            SqlParameter parameter16 = new SqlParameter("@ReceiptNumber", SqlDbType.VarChar)
            {
                Value = recieptNumber
            };

            list.Add(parameter16);

            SqlParameter parameter17 = new SqlParameter("@Paid", SqlDbType.Decimal)
            {
                Value = paid
            };

            list.Add(parameter17);

            SqlParameter parameter18 = new SqlParameter("@Outstanding", SqlDbType.Decimal)
            {
                Value = outstanding
            };

            list.Add(parameter18);


            SqlParameter parameter19 = new SqlParameter("@Type", SqlDbType.Int)
            {
                Value = type
            };

            list.Add(parameter19);

            SqlParameter parameter20 = new SqlParameter("@IsActive", SqlDbType.Bit)
            {
                Value = isActive
            };

            list.Add(parameter20);

            SqlParameter parameter21 = new SqlParameter("@DistributionPointId", SqlDbType.Int)
            {
                Value = distributionPointId
            };

            list.Add(parameter21);

            SqlParameter parameter22 = new SqlParameter("@PaymentTypeId", SqlDbType.Int)
            {
                Value = paymentTypeId
            };

            list.Add(parameter22);



            SqlParameter parameter23 = new SqlParameter("@TotalCash", SqlDbType.Int)
            {
                Value = splitCash
            };

            list.Add(parameter23);


            SqlParameter parameter24 = new SqlParameter("@TotalTransfer", SqlDbType.Int)
            {
                Value = splitTransfer
            };

            list.Add(parameter24);


            SqlParameter parameter25 = new SqlParameter("@TotalPOS", SqlDbType.Int)
            {
                Value = splitPOS
            };

            list.Add(parameter25);

            SqlParameter parameter26 = new SqlParameter("@TotalCredit", SqlDbType.Int)
            {
                Value = decimal.Zero
            };

            list.Add(parameter26);

            SqlCommand command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "InsertPaymentDataNew"
            };

            wrapper.InsertData(command, list);
        }


        private static void InsertPaymentDataCredit(DateTime paymentDate, int paymentMethodId, decimal total, decimal subTotal, string tax, decimal taxAmount,
            string discount, decimal discountAmount, string serviceCharge, decimal serviceChargeAmount, string resident, decimal residentAmount,
            int cashierId, int businessAccountId, string notes, string recieptNumber, decimal paid, decimal outstanding, int type, bool isActive,
            int distributionPointId, string connectionString, decimal splitCash = decimal.Zero, decimal splitTransfer = decimal.Zero, decimal splitPOS = decimal.Zero, int paymentTypeId = 4)
        {
            DbWrapperTest wrapper = new DbWrapperTest(connectionString);

            List<SqlParameter> list = new List<SqlParameter>();

            SqlParameter parameter1 = new SqlParameter("@PaymentDate", SqlDbType.DateTime)
            {
                Value = paymentDate
            };

            list.Add(parameter1);

            SqlParameter parameter2 = new SqlParameter("@PaymentMethodId", SqlDbType.Int)
            {
                Value = paymentMethodId
            };

            list.Add(parameter2);

            SqlParameter parameter3 = new SqlParameter("@Total", SqlDbType.Decimal)
            {
                Value = total
            };

            list.Add(parameter3);

            SqlParameter parameter4 = new SqlParameter("@SubTotal", SqlDbType.Decimal)
            {
                Value = subTotal
            };

            list.Add(parameter4);

            SqlParameter parameter5 = new SqlParameter("@Tax", SqlDbType.VarChar)
            {
                Value = tax
            };

            list.Add(parameter5);

            SqlParameter parameter6 = new SqlParameter("@TaxAmount", SqlDbType.Decimal)
            {
                Value = taxAmount
            };

            list.Add(parameter6);

            SqlParameter parameter7 = new SqlParameter("@Discount", SqlDbType.VarChar)
            {
                Value = discount
            };

            list.Add(parameter7);

            SqlParameter parameter8 = new SqlParameter("@DiscountAmount", SqlDbType.Decimal)
            {
                Value = discountAmount
            };

            list.Add(parameter8);

            SqlParameter parameter9 = new SqlParameter("@ServiceCharge", SqlDbType.VarChar)
            {
                Value = serviceCharge
            };

            list.Add(parameter9);

            SqlParameter parameter10 = new SqlParameter("@ServiceChargeAmount", SqlDbType.Decimal)
            {
                Value = serviceChargeAmount
            };

            list.Add(parameter10);

            SqlParameter parameter11 = new SqlParameter("@Resident", SqlDbType.VarChar)
            {
                Value = resident
            };

            list.Add(parameter11);

            SqlParameter parameter12 = new SqlParameter("@ResidentAmount", SqlDbType.Decimal)
            {
                Value = residentAmount
            };

            list.Add(parameter12);

            SqlParameter parameter13 = new SqlParameter("@CashierId", SqlDbType.Int)
            {
                Value = cashierId
            };

            list.Add(parameter13);

            SqlParameter parameter14 = new SqlParameter("@BusinessAccountId", SqlDbType.Int)
            {
                Value = businessAccountId
            };

            list.Add(parameter14);

            SqlParameter parameter15 = new SqlParameter("@Notes", SqlDbType.VarChar)
            {
                Value = notes
            };

            list.Add(parameter15);

            SqlParameter parameter16 = new SqlParameter("@ReceiptNumber", SqlDbType.VarChar)
            {
                Value = recieptNumber
            };

            list.Add(parameter16);

            SqlParameter parameter17 = new SqlParameter("@Paid", SqlDbType.Decimal)
            {
                Value = paid
            };

            list.Add(parameter17);

            SqlParameter parameter18 = new SqlParameter("@Outstanding", SqlDbType.Decimal)
            {
                Value = outstanding
            };

            list.Add(parameter18);


            SqlParameter parameter19 = new SqlParameter("@Type", SqlDbType.Int)
            {
                Value = type
            };

            list.Add(parameter19);

            SqlParameter parameter20 = new SqlParameter("@IsActive", SqlDbType.Bit)
            {
                Value = isActive
            };

            list.Add(parameter20);

            SqlParameter parameter21 = new SqlParameter("@DistributionPointId", SqlDbType.Int)
            {
                Value = distributionPointId
            };

            list.Add(parameter21);

            SqlParameter parameter22 = new SqlParameter("@PaymentTypeId", SqlDbType.Int)
            {
                Value = paymentTypeId
            };

            list.Add(parameter22);

            SqlParameter parameter23 = new SqlParameter("@TotalCash", SqlDbType.Int)
            {
                Value = splitCash
            };

            list.Add(parameter23);


            SqlParameter parameter24 = new SqlParameter("@TotalTransfer", SqlDbType.Int)
            {
                Value = splitTransfer
            };

            list.Add(parameter24);


            SqlParameter parameter25 = new SqlParameter("@TotalPOS", SqlDbType.Int)
            {
                Value = splitPOS
            };

            list.Add(parameter25);

            SqlParameter parameter26 = new SqlParameter("@TotalCredit", SqlDbType.Int)
            {
                Value = total
            };

            list.Add(parameter26);



            SqlCommand command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "InsertPaymentDataCreditNew"
            };

            wrapper.InsertData(command, list);
        }

        public static bool UpdateSales(int tableId,List<StockItem> lst, int transactionId, int guestId, int? businessAccountId, int personId, int hotelId, int guestRoomId, string connectionString,
      int paymentMethodId, string paymentMethodNote, DateTime timeOfSale, int distributionPointId, bool isHotel, string recieptNumber,
      decimal total, decimal subTotal, string tax, decimal taxAmount, string discount, decimal discountAmount, string resident, decimal residentAmount,
      string serviceCharge, decimal serviceChargeAmount, decimal paid, decimal outstanding, int cashierId = 0, bool isFrontOffice = false, int waiterId = 0, string chequeNum = "",
      decimal splitCash = decimal.Zero, decimal splittransfer = decimal.Zero, decimal splitPOS = decimal.Zero, int paymentTypeId = 4)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                /* Perform transactional work here */
                //int paymentTypeId = 4;

                var oldReceiptNumber = recieptNumber;


                recieptNumber = Guid.NewGuid().ToString();

                timeOfSale = DateTime.Now;
                DbWrapperTest wrapper = new DbWrapperTest(connectionString);
                decimal num = 0M;
                foreach (StockItem item in lst)
                {
                    decimal num2 = item.UnitPrice * item.Quantity;
                    num += num2;
                    List<SqlParameter> list = new List<SqlParameter>();
                    SqlParameter parameter = new SqlParameter("@ItemId", SqlDbType.Int)
                    {
                        Value = item.Id
                    };
                    list.Add(parameter);

                    SqlParameter parameter2 = new SqlParameter("@Qty", SqlDbType.Int)
                    {
                        Value = item.Quantity
                    };
                    list.Add(parameter2);

                    SqlParameter parameter3 = new SqlParameter("@TotalPrice", SqlDbType.Decimal)
                    {
                        Value = num2
                    };
                    list.Add(parameter3);

                    SqlParameter parameter4 = new SqlParameter("@TransactionId", SqlDbType.Int)
                    {
                        Value = transactionId
                    };

                    list.Add(parameter4);

                    SqlParameter parameter5 = new SqlParameter("@GuestId", SqlDbType.Int)
                    {
                        Value = guestId
                    };
                    list.Add(parameter5);

                    SqlParameter parameter6 = new SqlParameter("@GuestRoomId", SqlDbType.Int)
                    {
                        Value = guestRoomId
                    };
                    list.Add(parameter6);

                    SqlParameter parameter7 = new SqlParameter("@PersonId", SqlDbType.Int)
                    {
                        Value = personId
                    };
                    list.Add(parameter7);

                    SqlParameter parameter8 = new SqlParameter("@DateSold", SqlDbType.DateTime)
                    {
                        Value = timeOfSale
                    };
                    list.Add(parameter8);

                    SqlParameter parameter9 = new SqlParameter("@IsActive", SqlDbType.Bit)
                    {
                        Value = true
                    };
                    list.Add(parameter9);

                    SqlParameter parameterPtId = new SqlParameter("@PaymentTypeId", SqlDbType.Int)
                    {
                        Value = paymentTypeId
                    };

                    list.Add(parameterPtId);

                    SqlParameter parameterPtMId = new SqlParameter("@PaymentMethodId", SqlDbType.Int)
                    {
                        Value = paymentMethodId
                    };

                    list.Add(parameterPtMId);

                    //chequeNum

                    var test = paymentMethodNote;

                    if (!string.IsNullOrEmpty(chequeNum))
                    {
                        paymentMethodNote += " -- TRANS -- " + chequeNum;
                    }

                    SqlParameter parameterPtMNote = new SqlParameter("@PaymentMethodNote", SqlDbType.VarChar)
                    {
                        Value = paymentMethodNote
                        //Value = chequeNum
                    };

                    list.Add(parameterPtMNote);

                    SqlParameter parameterTos = new SqlParameter("@TimeOfSale", SqlDbType.DateTime)
                    {
                        Value = timeOfSale
                    };

                    list.Add(parameterTos);

                    SqlParameter DistributionPoint = new SqlParameter("@DistributionPointId", SqlDbType.Int)
                    {
                        Value = distributionPointId
                    };

                    list.Add(DistributionPoint);

                    SqlParameter RecieptNumberPara = new SqlParameter("@RecieptNumber", SqlDbType.VarChar)
                    {
                        Value = recieptNumber
                    };

                    list.Add(RecieptNumberPara);

                    if (!businessAccountId.HasValue)
                    {
                        businessAccountId = 0;
                    }

                    SqlParameter BusinessAccountIdPara = new SqlParameter("@BusinessAccountId", SqlDbType.Int)//@BusinessAccountId
                    {
                        Value = businessAccountId
                    };

                    list.Add(BusinessAccountIdPara);

                    SqlParameter WaiterPara = new SqlParameter("@WaiterId", SqlDbType.Int)//waiterId
                    {
                        Value = waiterId
                    };

                    list.Add(WaiterPara);


                    //list.Add(decimalPara);

                    if (isHotel)
                    {

                        SqlCommand command = new SqlCommand
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandText = "InsertSalesData"
                        };

                        wrapper.InsertData(command, list);
                    }
                    else
                    {
                        SqlCommand command = new SqlCommand
                        {
                            CommandType = CommandType.StoredProcedure,
                            CommandText = "InsertSalesDataBA"
                        };

                        wrapper.InsertData(command, list);
                    }

                    SqlCommand commandAddition = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "InsertProductQuantityNoTrans"
                    };

                    List<SqlParameter> listAddition = new List<SqlParameter>();
                    /*  @Quantity decimal(18,4),
		                @ItemId int,		 
		                @PlaceId int,
		                @AdditionType int,
		                @PurchaseOrderId int*/
                    decimal qtyDecimal = decimal.Zero;

                    decimal.TryParse(item.Quantity.ToString(), out qtyDecimal);

                    SqlParameter distriPara = new SqlParameter("@PlaceId", SqlDbType.Decimal)//waiterId
                    {
                        Value = distributionPointId
                    };

                    SqlParameter decimalPara = new SqlParameter("@Quantity", SqlDbType.Decimal)//waiterId
                    {
                        Value = qtyDecimal
                    };
                    SqlParameter additionTypePara = new SqlParameter("@AdditionType", SqlDbType.Int)//waiterId
                    {
                        Value = 0
                    };
                    SqlParameter itemIdPara = new SqlParameter("@ItemId", SqlDbType.Int)//waiterId
                    {
                        Value = item.Id
                    };

                    SqlParameter poTypePara = new SqlParameter("@PurchaseOrderId", SqlDbType.Int)//waiterId
                    {
                        Value = DBNull.Value
                    };

                    listAddition.Add(decimalPara);
                    listAddition.Add(itemIdPara);
                    listAddition.Add(distriPara);
                    listAddition.Add(additionTypePara);
                    listAddition.Add(poTypePara);


                    wrapper.InsertData(commandAddition, listAddition);

                }



                if (businessAccountId.HasValue && businessAccountId.Value > 0)
                {
                    InsertPaymentDataCredit(timeOfSale
                             , paymentMethodId
                             , total
                             , subTotal
                             , tax
                             , taxAmount
                             , discount
                             , discountAmount
                             , serviceCharge
                             , serviceChargeAmount
                             , resident
                             , residentAmount
                             , cashierId
                             , businessAccountId.Value
                             , oldReceiptNumber
                             , recieptNumber
                             , paid
                             , outstanding
                             , 1
                             , true
                             , distributionPointId, connectionString, splitCash, splittransfer, splitPOS, paymentTypeId);
                }
                else
                {
                    InsertPaymentData(timeOfSale
             , paymentMethodId
             , total
             , subTotal
             , tax
             , taxAmount
             , discount
             , discountAmount
             , serviceCharge
             , serviceChargeAmount
             , resident
             , residentAmount
             , cashierId
             , guestId
             , oldReceiptNumber
             , recieptNumber
             , paid
             , outstanding
             , 1
             , true
             , distributionPointId, connectionString, splitCash, splittransfer, splitPOS, paymentTypeId);

                }


                if (discountAmount > 0)
                {
                    if (cashierId == 0)
                        cashierId = personId;

                    SqlCommand sqlCommandSD = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "InsertSalesDiscount"
                    };
                    List<SqlParameter> parametersSD = new List<SqlParameter>();

                    SqlParameter parameterDD = new SqlParameter("@DiscountDate", SqlDbType.DateTime)
                    {
                        Value = timeOfSale
                    };
                    parametersSD.Add(parameterDD);

                    SqlParameter parameterRR = new SqlParameter("@ReceiptNumber", SqlDbType.VarChar)
                    {
                        Value = recieptNumber
                    };
                    parametersSD.Add(parameterRR);

                    SqlParameter parameterAmount = new SqlParameter("@Amount", SqlDbType.Decimal)
                    {
                        Value = discountAmount
                    };

                    parametersSD.Add(parameterAmount);

                    SqlParameter parameterDiscountPerson = new SqlParameter("@PersonId", SqlDbType.Int)
                    {
                        Value = personId
                    };

                    parametersSD.Add(parameterDiscountPerson);

                    SqlParameter parameterDiscountActualCashierId = new SqlParameter("@ActualCashierId", SqlDbType.Int)
                    {
                        Value = cashierId
                    };

                    parametersSD.Add(parameterDiscountActualCashierId);

                    wrapper.InsertData(sqlCommandSD, parametersSD);

                }

                if (guestRoomId == 0)
                {
                    //StockItemService.DeleteTableItems(tableId.Value, conn, Person.Value);
                    if (tableId > 0)
                    {
                        DbWrapperTest wrapperD = new DbWrapperTest(connectionString);

                        // string sqlText = "DELETE FROM TABLEITEM WHERE TABLEID = " + tableId.ToString() + " AND CASHIER =  " + personId.ToString();
                        SqlCommand cmd = new SqlCommand("DeleteGuestOrderCompleteByTableId");

                        cmd.CommandType = CommandType.StoredProcedure;

                        List<SqlParameter> parametersD = new List<SqlParameter>();

                        SqlParameter parameterTable = new SqlParameter("@TableId", SqlDbType.Int)
                        {
                            Value = tableId
                        };

                        parametersD.Add(parameterTable);

                        wrapperD.InsertData(cmd, parametersD);

                    }
                    else
                    {
                        DbWrapperTest wrapperD = new DbWrapperTest(connectionString);

                        // string sqlText = "DELETE FROM TABLEITEM WHERE TABLEID = " + tableId.ToString() + " AND CASHIER =  " + personId.ToString();
                        SqlCommand cmd = new SqlCommand("DeleteGuestOrderCompleteByTableIdNoBarTableDelete");

                        cmd.CommandType = CommandType.StoredProcedure;

                        List<SqlParameter> parametersD = new List<SqlParameter>();

                        SqlParameter parameterTable = new SqlParameter("@TableId", SqlDbType.Int)
                        {
                            Value = tableId
                        };

                        parametersD.Add(parameterTable);

                        wrapperD.InsertData(cmd, parametersD);
                    }


                    scope.Complete();

                    return true;
                }


                List<SqlParameter> parameters = new List<SqlParameter>();

                SqlParameter parameter10 = new SqlParameter("@GuestRoomId", SqlDbType.Int)
                {
                    Value = guestRoomId
                };

                parameters.Add(parameter10);

                if (paymentMethodId == 4)
                {
                    SqlParameter parameter11 = new SqlParameter("@Amount", SqlDbType.Decimal)
                    {
                        Value = outstanding
                    };
                    parameters.Add(parameter11);
                }
                else
                {
                    SqlParameter parameter11 = new SqlParameter("@Amount", SqlDbType.Decimal)
                    {
                        Value = total
                    };
                    parameters.Add(parameter11);
                }

                transactionId = personId;

                SqlParameter parameter12 = new SqlParameter("@TransactionId", SqlDbType.Int)
                {
                    Value = transactionId
                };
                parameters.Add(parameter12);

                SqlParameter parameter13 = new SqlParameter("@PaymentTypeId", SqlDbType.Int)
                {
                    Value = paymentTypeId
                };
                parameters.Add(parameter13);

                SqlParameter parameter14 = new SqlParameter("@TransactionDate", SqlDbType.DateTime)
                {
                    Value = timeOfSale
                };
                parameters.Add(parameter14);

                SqlParameter parameterPtMId15 = new SqlParameter("@PaymentMethodId", SqlDbType.Int)
                {
                    Value = paymentMethodId
                };

                parameters.Add(parameterPtMId15);

                paymentMethodNote = "BAR/RESTAURANT";

                if (isFrontOffice)
                {
                    paymentMethodNote = "FRONT OFFICE SALES";
                }


                SqlParameter parameterPtMNote16 = new SqlParameter("@PaymentMethodNote", SqlDbType.VarChar)
                {
                    Value = paymentMethodNote
                };

                parameters.Add(parameterPtMNote16);


                SqlCommand sqlCommand = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "InsertGuestRoomSales"
                };

                wrapper.InsertData(sqlCommand, parameters);

                if (tableId > 0)
                {
                    DbWrapperTest wrapperD = new DbWrapperTest(connectionString);

                    // string sqlText = "DELETE FROM TABLEITEM WHERE TABLEID = " + tableId.ToString() + " AND CASHIER =  " + personId.ToString();
                    SqlCommand cmd = new SqlCommand("DeleteGuestOrderCompleteByTableId");

                    cmd.CommandType = CommandType.StoredProcedure;

                    List<SqlParameter> parametersD = new List<SqlParameter>();

                    SqlParameter parameterTable = new SqlParameter("@TableId", SqlDbType.Int)
                    {
                        Value = tableId
                    };

                    parametersD.Add(parameterTable);

                    wrapperD.InsertData(cmd, parametersD);

                }
                else
                {
                    DbWrapperTest wrapperD = new DbWrapperTest(connectionString);

                    // string sqlText = "DELETE FROM TABLEITEM WHERE TABLEID = " + tableId.ToString() + " AND CASHIER =  " + personId.ToString();
                    SqlCommand cmd = new SqlCommand("DeleteGuestOrderCompleteByTableIdNoBarTableDelete");

                    cmd.CommandType = CommandType.StoredProcedure;

                    List<SqlParameter> parametersD = new List<SqlParameter>();

                    SqlParameter parameterTable = new SqlParameter("@TableId", SqlDbType.Int)
                    {
                        Value = tableId
                    };

                    parametersD.Add(parameterTable);

                    wrapperD.InsertData(cmd, parametersD);
                }

                scope.Complete();

                return true;
            }

        }

    }
}
