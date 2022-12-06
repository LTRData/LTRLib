
// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NETFRAMEWORK

using LTRLib.LTRGeneric;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
#if NET35_OR_GREATER
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
#endif

namespace LTRLib.Extensions;

public static class SqlExtensions
{
    public static void OpenSqlQuery(this SqlConnection Connection, string Query, bool GetData, out SqlDataAdapter DataAdapter, out DataTable DataTable)
    {
        DataAdapter = new SqlDataAdapter(Query, Connection);
        _ = new SqlCommandBuilder(DataAdapter);

        DataTable = new DataTable();
        if (GetData == true)
        {
            DataAdapter.Fill(DataTable);
        }
        else
        {
            DataAdapter.FillSchema(DataTable, SchemaType.Source);
        }
    }

    public static SqlDataReader GetSqlReader(this SqlConnection Connection, string Query)
    {
        using var Command = Connection.CreateCommand();
        Command.CommandText = Query;
        return Command.ExecuteReader();
    }

    public static DataRow? GetSqlRow(this SqlConnection Connection, string Query)
    {
        Connection.OpenSqlQuery(Query, true, out _, out var Table);
        if (Table.Rows.Count != 1)
        {
            return null;
        }
        else
        {
            return Table.Rows[0];
        }
    }

    public static object GetSqlValue(this SqlConnection Connection, string Query)
    {
        using var Command = Connection.CreateCommand();
        Command.CommandText = Query;
        return Command.ExecuteScalar();
    }

    public static int RunSqlCommand(this SqlConnection Connection, string Query)
    {
        using var Command = Connection.CreateCommand();
        Command.CommandText = Query;
        return Command.ExecuteNonQuery();
    }

#if NET40_OR_GREATER
    public static T[] ExecuteStoreQueryReflectionSafe<T>(this ObjectContext context, string query, params DbParameter[] parameters) where T : new()
    {
        var connection = ((EntityConnection)context.Connection).StoreConnection;
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        try
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddRange(parameters);

            return command.ExecuteReader().OfType<IDataRecord>().Select(DataExtensions.RecordToEntityObject<T>).ToArray();
        }
        finally
        {
            connection.Close();
        }
    }
#endif
}

#endif

