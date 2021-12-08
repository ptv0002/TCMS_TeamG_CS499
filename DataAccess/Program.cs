using System;
using System.Data.SqlClient;
using System.Data;

namespace DataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            String ConnectionString = "Data Source=.; Initial Catalog=TCMS_DB;Integrated Security=True;MultipleActiveResultSets=True";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select Address FROM dbo.Company";
            cmd.Connection = conn;

            conn.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                Console.WriteLine(sdr[0]);
            }

        }
    }
}