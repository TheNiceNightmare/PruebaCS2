using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class SqlDbHelper
{
    private readonly string _connectionString;

    public SqlDbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SegurosChubbiDB");
    }

    public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }

    public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }
    }
}
