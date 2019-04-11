﻿using HotelMate.DataWrapper;
using Microsoft.Practices.EnterpriseLibrary.Data;
using POSData;
using POSService.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSService
{
    public static class StockItemServiceXXXX
    {
        // Methods
        public static IEnumerable<TableItem> GetMyTableItems(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetMyTableItems", id);
        }

        
        public static IEnumerable<TableItem> GetMyTableItemsByTableIdAll(int tableId)
        {
            SqlParameter parameter = new SqlParameter("@TableId", SqlDbType.Int)
            {
                Value = tableId
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetMyTableItemsByTableIdAll", tableId);
        }

        public static IEnumerable<TableItem> GetMyTableItemsByTableId(int tableId)
        {
            SqlParameter parameter = new SqlParameter("@TableId", SqlDbType.Int)
            {
                Value = tableId
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetMyTableItemsByTableId", tableId);
        }

        public static IEnumerable<CabbashUser> GetPersonsByUserName(string Username)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<CabbashUser>("GetUserByUsername", Username);
        }

        public static IEnumerable<CabbashUser> GetPersons(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<CabbashUser>("GetAllUsers", id);
        }

        public static void MoveItem(int leftSideTableId, int rightSideTableId, int id, int personId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("MoveTableItem", leftSideTableId, rightSideTableId,id, personId);
        }

        public static IEnumerable<TableItem> GetDrinkItems(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetDrinkItems", id);
        }

        public static IEnumerable<TableItem> GetSmokingItems(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetSmokingItems", id);
        }

        public static IEnumerable<TableItem> GetKitchenItemsByRoleId(int roleId)
        {
           

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetKitchenItemsByRoleId", roleId);

        }

        public static IEnumerable<TableItem> GetKitchenItemsByRole(string role)
        {
            SqlParameter parameter = new SqlParameter("@Role", SqlDbType.Int)
            {
                Value = role
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetKitchenItemsByRole", role);

        }

        public static IEnumerable<TableItem> GetKitchenItems(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetKitchenItems", id);

        }

        public static IEnumerable<CabbashUser> GetUserById(int personId)
        {
            SqlParameter parameter = new SqlParameter("@PersonId", SqlDbType.Int)
            {
                Value = personId
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();

            return factory.CreateDefault().ExecuteList<CabbashUser>("GetUserByPersonId", personId);

        }

        public static void ResetClearAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("DeleteClearAllAdmin");
        }

        public static void ResetUserLogin()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("ResetUserLogin");
        }

        public static IEnumerable<CabbashUser> GetUser(string code)
        {
            SqlParameter parameter = new SqlParameter("@Code", SqlDbType.NVarChar)
            {
                Value = code
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();

            return factory.CreateDefault().ExecuteList<CabbashUser>("GetUserByCode", code);

        }

        public static IEnumerable<BarTable> GetBarTableByTableId(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<BarTable>("GetBarTableByTableId", id);
        }

        public static IEnumerable<BarTable> GetBarTableByTableNum(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<BarTable>("GetBarTableByTableNum", id);
        }

        public static IEnumerable<BarTable> GetBarTableById(int id)
        {
           if(id > 0)
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                return factory.CreateDefault().ExecuteList<BarTable>("GetBarTableById", id);
            }
            else
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                return factory.CreateDefault().ExecuteList<BarTable>("GetBarTableByTableId", id);
            }
        }

        public static IEnumerable<BarTable> GetMyTablesWhatever(int id)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<BarTable>("GetMyTablesWhateverTotal", id);
        }

        public static IEnumerable<BarTable> GetMyTables(int id)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<BarTable>("GetMyTablesWithTotal", id);
        }

        public static IEnumerable<StockItem> GetSpecificItem(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<StockItem>("GetSpecificItem", id);
        }

        public static IEnumerable<StockItem> GetSpecificItemRaw(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<StockItem>("GetSpecificItemRaw", id);
        }

        public static IEnumerable<TableItem> GetTableItemById(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("GetTableItemById", id);
        }

        public static IEnumerable<CabbashUser> GetUsedTables()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<CabbashUser>("GetUsedTables");
        }

        public static void SignUserIn(string code)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("SignUserIn", code);
        }

        public static void SignUserOut(int personId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("SignUserOut", personId);
        }

        public static void UpdateTableItemNote(int tableItemId, string note, string personName, bool completed = false)
        {
          
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("InsertOrUpdateTableItemNote", tableItemId, note, personName, completed);
        }

        public static IEnumerable<TableItem> UpdateTableItemFullyReceivedById(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("UpdateTableItemFullyReceivedById", id);
        }

        public static IEnumerable<TableItem> UpdateTableItemFullyById(int id, bool sentToPOS = false)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("UpdateTableItemFullyById", id, sentToPOS);
        }

        public static IEnumerable<TableItem> UpdateTableItemCollectedById(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();

            return factory.CreateDefault().ExecuteList<TableItem>("UpdateTableItemCollectedById", id);
        }

        public static IEnumerable<TableItem> GetTableItemsByTableTel(string telephone)
        {
            SqlParameter parameter = new SqlParameter("@Telephone", SqlDbType.Int)
            {
                Value = telephone
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();

            return factory.CreateDefault().ExecuteList<TableItem>("GetTableItemsByTableTel", telephone);
        }

        public static IEnumerable<BarTable> UpdateTableWithTelephone(int id, string telephone)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<BarTable>("UpdateTableWithTelephone", id, telephone);
        }

        public static void UpdateTableItemQtyById(int id, int qty)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            factory.CreateDefault().ExecuteNonQuery("UpdateTableItemQtyById", id, qty);
        }

        public static IEnumerable<TableItem> UpdateTableItemById(int id)
        {
            SqlParameter parameter = new SqlParameter("@Id", SqlDbType.Int)
            {
                Value = id
            };

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<TableItem>("UpdateTableItemById", id);
        }

       

        public static IEnumerable<Category> GetCategories(int p)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<Category>("GetCategories", new object[0]);
        }

        public static IEnumerable<BusinessAccount> GetCurrentAccounts(int hotelId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<BusinessAccount>("GetCurrentBusinessAccounts", new object[0]);
        }

        public static IEnumerable<Guest> GetCurrentGuests(int hotelId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<Guest>("GetCurrentGuests", new object[0]);
        }

        public static IEnumerable<StockItem> GetStockItems(int hotelId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<StockItem>("GetStockItems", new object[0]);
        }


        public static IEnumerable<StockItem> GetStockItemsRaw(int hotelId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<StockItem>("GetStockItemsRaw", new object[0]);
        }
        public static IEnumerable<StockItem> GetStockItemsPOS()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<StockItem>("GetStockItemsPOS", new object[0]);
        }

        public static IEnumerable<StockItem> GetStockItemsPOS(int distributionPointId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            return factory.CreateDefault().ExecuteList<StockItem>("GetStockItemsPOS", distributionPointId);
        }




        public static void UpdateSalesHouseKeeping(List<StockItem> lst, int transactionId, int guestId, int personId, int hotelId, int guestRoomId, string connectionString,
   int paymentMethodId, string paymentMethodNote, DateTime timeOfSale, int distributionPointId, int paymentTypeId = 1)
        {
            timeOfSale = DateTime.Now;
            DbWrapper wrapper = new DbWrapper(connectionString);
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
                    Value = DateTime.Now
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

                SqlParameter parameterPtMNote = new SqlParameter("@PaymentMethodNote", SqlDbType.VarChar)
                {
                    Value = paymentMethodNote
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

                SqlCommand command = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "InsertHouseKeepingData"
                };

                wrapper.InsertData(command, list);
            }
        }

        public static void UpdateSales(List<StockItem> lst, int transactionId, int guestId, int personId, int hotelId, int guestRoomId, string connectionString,
        int paymentMethodId, string paymentMethodNote, DateTime timeOfSale, int distributionPointId, bool isHotel, string recieptNumber, int paymentTypeId = 1,
        decimal discountAmount = decimal.Zero, int cashierId = 0)
        {
            timeOfSale = DateTime.Now;
            DbWrapper wrapper = new DbWrapper(connectionString);
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

                SqlParameter parameterPtMNote = new SqlParameter("@PaymentMethodNote", SqlDbType.VarChar)
                {
                    Value = paymentMethodNote
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


            }


            //InsertSalesDiscount

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
                return;

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter parameter10 = new SqlParameter("@GuestRoomId", SqlDbType.Int)
            {
                Value = guestRoomId
            };
            parameters.Add(parameter10);

            SqlParameter parameter11 = new SqlParameter("@Amount", SqlDbType.Decimal)
            {
                Value = num
            };
            parameters.Add(parameter11);

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
                Value = DateTime.Now
            };
            parameters.Add(parameter14);

            SqlParameter parameterPtMId15 = new SqlParameter("@PaymentMethodId", SqlDbType.Int)
            {
                Value = paymentMethodId
            };

            parameters.Add(parameterPtMId15);


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


        }

        public static void UpdateSales(List<StockItem> lst, int transactionId, int guestId, int? businessAccountId, int personId, int hotelId, int guestRoomId, string connectionString,
        int paymentMethodId, string paymentMethodNote, DateTime timeOfSale, int distributionPointId, bool isHotel, string recieptNumber,
        decimal total, decimal subTotal, string tax, decimal taxAmount, string discount, decimal discountAmount, string resident, decimal residentAmount,
        string serviceCharge, decimal serviceChargeAmount, decimal paid, decimal outstanding, int cashierId = 0, bool isFrontOffice = false)
        {

            int paymentTypeId = 4;

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

                SqlParameter parameterPtMNote = new SqlParameter("@PaymentMethodNote", SqlDbType.VarChar)
                {
                    Value = paymentMethodNote
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

                SqlParameter WaiterId = new SqlParameter("@WaiterId", SqlDbType.Int)//@BusinessAccountId
                {
                    Value = personId
                };

                list.Add(WaiterId);

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
                         , ""
                         , recieptNumber
                         , paid
                         , outstanding
                         , 1
                         , true
                         , distributionPointId, connectionString);
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
         , ""
         , recieptNumber
         , paid
         , outstanding
         , 1
         , true
         , distributionPointId, connectionString);

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
                return;

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
        }

        private static void InsertPaymentData(DateTime paymentDate, int paymentMethodId, decimal total, decimal subTotal, string tax, decimal taxAmount,
            string discount, decimal discountAmount, string serviceCharge, decimal serviceChargeAmount, string resident, decimal residentAmount,
            int cashierId, int guestId, string notes, string recieptNumber, decimal paid, decimal outstanding, int type, bool isActive,
            int distributionPointId, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);

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
                Value = 4
            };

            list.Add(parameter22);



            SqlCommand command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "InsertPaymentData"
            };

            wrapper.InsertData(command, list);
        }

        

        private static void InsertPaymentDataCredit(DateTime paymentDate, int paymentMethodId, decimal total, decimal subTotal, string tax, decimal taxAmount,
            string discount, decimal discountAmount, string serviceCharge, decimal serviceChargeAmount, string resident, decimal residentAmount,
            int cashierId, int businessAccountId, string notes, string recieptNumber, decimal paid, decimal outstanding, int type, bool isActive,
            int distributionPointId, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);

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
                Value = 2
            };

            list.Add(parameter22);



            SqlCommand command = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "InsertPaymentDataCredit"
            };

            wrapper.InsertData(command, list);
        }
        public static void CloseTill(int cashierId, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = "UPDATE SOLDITEMSAll SET TILLOPEN = 0 WHERE PERSONID = " + cashierId.ToString();
            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            wrapper.InsertData(cmd, parameters);
        }

        public static DataSet GetSoldItemsForCashierOpenTill(int cashierId, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = @"SELECT SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE,SIA.DATESOLD,P.DISPLAYNAME,RPT.NAME,SIA.PAYMENTMETHODNOTE,PT.NAME,SIA.TIMEOFSALE,SIA.GUESTID,SIA.PaymentMethodId,
                        SIA.DISTRIBUTIONPOINTID,SIA.TILLOPEN,SIA.RECIEPTNUMBER
                        FROM  SOLDITEMSAll SIA INNER JOIN STOCKITEM 
                        SI ON SI.ID = SIA.ITEMID INNER JOIN [dbo].[Person] P ON P.PersonID = SIA.PersonId INNER JOIN ROOMPAYMENTTYPE RPT ON RPT.ID = SIA.PAYMENTTYPEID
                        INNER JOIN PAYMENTMETHOD PT ON PT.Id = SIA.PaymentMethodId WHERE SIA.ISACTIVE = 1 AND PT.Id != 4 AND SIA.TILLOPEN = 1 AND SIA.PersonId = " + cashierId.ToString();

            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            return wrapper.GetDataSet(cmd, parameters);
        }

        public static DataSet GetSoldItemsForCashier(int cashierId, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = @"SELECT SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE,SIA.DATESOLD,P.DISPLAYNAME,RPT.NAME,SIA.PAYMENTMETHODNOTE,PT.NAME,SIA.TIMEOFSALE,SIA.GUESTID,SIA.PaymentMethodId,
                        SIA.DistributionPointId,SIA.TILLOPEN
                        FROM  SOLDITEMSAll SIA INNER JOIN STOCKITEM 
                        SI ON SI.ID = SIA.ITEMID INNER JOIN [dbo].[Person] P ON P.PersonID = SIA.PersonId INNER JOIN ROOMPAYMENTTYPE RPT ON RPT.ID = SIA.PAYMENTTYPEID
                        INNER JOIN PAYMENTMETHOD PT ON PT.Id = SIA.PaymentMethodId WHERE SIA.ISACTIVE = 1 AND PT.Id = 1 AND SIA.PersonId = " + cashierId.ToString();

            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            return wrapper.GetDataSet(cmd, parameters);
        }

        public static DataSet GetSoldItems(int cashierId, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = "SELECT SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE FROM  SOLDITEMSAll SIA INNER JOIN STOCKITEM SI ON SI.ID = SIA.ITEMID  WHERE SIA.TILLOPEN = 1 AND SIA.ISACTIVE = 1 AND SIA.PERSONID = " + cashierId.ToString();
            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            return wrapper.GetDataSet(cmd, parameters);
        }

        //
        public static DataSet GetSoldItemsPrint(string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = @"SELECT SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE,SIA.DATESOLD,P.DISPLAYNAME,RPT.NAME,SIA.PAYMENTMETHODNOTE,PT.NAME,SIA.TIMEOFSALE,
                         SIA.GUESTID,SIA.PaymentMethodId,SIA.DistributionPointId,SIA.TILLOPEN,SIA.RECIEPTNUMBER
                    FROM  SOLDITEMSAll SIA INNER JOIN STOCKITEM 
                    SI ON SI.ID = SIA.ITEMID INNER JOIN [dbo].[Person] P ON P.PersonID = SIA.PersonId INNER JOIN ROOMPAYMENTTYPE RPT ON RPT.ID = SIA.PAYMENTTYPEID
                    INNER JOIN PAYMENTMETHOD PT ON PT.Id = SIA.PaymentMethodId WHERE SIA.ISACTIVE = 1";
            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            return wrapper.GetDataSet(cmd, parameters);
        }
        public static DataSet GetSoldItems(string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = @"SELECT SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE,SIA.DATESOLD,P.DISPLAYNAME,RPT.NAME,SIA.PAYMENTMETHODNOTE,PT.NAME,SIA.TIMEOFSALE,SIA.GUESTID,SIA.PaymentMethodId,SIA.DistributionPointId
                        FROM  SOLDITEMSAll SIA INNER JOIN STOCKITEM 
                        SI ON SI.ID = SIA.ITEMID INNER JOIN [dbo].[Person] P ON P.PersonID = SIA.PersonId INNER JOIN ROOMPAYMENTTYPE RPT ON RPT.ID = SIA.PAYMENTTYPEID
                        INNER JOIN PAYMENTMETHOD PT ON PT.Id = SIA.PaymentMethodId WHERE SIA.ISACTIVE = 1 AND PT.Id != 6";
            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            return wrapper.GetDataSet(cmd, parameters);
        }

        public static DataSet GetManagerialDeleteItems(string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = @"SELECT SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE,SIA.DATESOLD,P.DISPLAYNAME,SIA.GUESTID
                            FROM  SOLDITEMS SIA 
                            INNER JOIN STOCKITEM SI ON SI.ID = SIA.ITEMID INNER JOIN [dbo].[Person] P ON P.PersonID = SIA.PersonId
                            WHERE SIA.ISACTIVE = 1";
            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            return wrapper.GetDataSet(cmd, parameters);
        }


        public static void DeleteTableItemsNoDeleteTable(int tableId, string connectionString, int personId)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);

            // string sqlText = "DELETE FROM TABLEITEM WHERE TABLEID = " + tableId.ToString() + " AND CASHIER =  " + personId.ToString();
            SqlCommand cmd = new SqlCommand("DeleteGuestOrderCompleteByTableIdNoBarTableDelete");
            cmd.CommandType = CommandType.StoredProcedure;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter parameterTable = new SqlParameter("@TableId", SqlDbType.Int)
            {
                Value = tableId
            };

            parameters.Add(parameterTable);

            wrapper.InsertData(cmd, parameters);


        }

        public static void DeleteTableItems(int tableId, string connectionString, int personId)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);

            // string sqlText = "DELETE FROM TABLEITEM WHERE TABLEID = " + tableId.ToString() + " AND CASHIER =  " + personId.ToString();
            SqlCommand cmd = new SqlCommand("DeleteGuestOrderCompleteByTableId");
            cmd.CommandType = CommandType.StoredProcedure;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter parameterTable = new SqlParameter("@TableId", SqlDbType.Int)
            {
                Value = tableId
            };

            parameters.Add(parameterTable);

            wrapper.InsertData(cmd, parameters);


        }

        public static void TransferTill(int currentCashier, int newCashier, string connectionString)
        {
            DbWrapper wrapper = new DbWrapper(connectionString);
            string sqlText = "UPDATE TABLEITEM SET CASHIER  = " + newCashier.ToString() + " WHERE CASHIER =  " + currentCashier.ToString();
            SqlCommand cmd = new SqlCommand(sqlText);
            List<SqlParameter> parameters = new List<SqlParameter>();
            wrapper.InsertData(cmd, parameters);


            wrapper = new DbWrapper(connectionString);
            sqlText = "UPDATE BARTABLE SET StaffId  = " + newCashier.ToString() + " WHERE ISACTIVE = 1 AND StaffId =  " + currentCashier.ToString();
            cmd = new SqlCommand(sqlText);
            parameters = new List<SqlParameter>();
            wrapper.InsertData(cmd, parameters);
        }


    }
}
