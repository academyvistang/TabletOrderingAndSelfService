

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace POSService.Entities

{
    public interface IDbWrapper
    {
        DataSet GetDataSet(SqlCommand sqlCommand);
        DataSet GetDataSet(SqlCommand sqlCommand, List<SqlParameter> parameters);
        DataSet GetDataSet(SqlCommand sqlCommand, object paramObject);
        int InsertData(SqlCommand sqlCommand, List<SqlParameter> parameters);
        int InsertData(SqlCommand sqlCommand, object paramObject);
    }
}
