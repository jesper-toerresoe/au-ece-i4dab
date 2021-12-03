using Microsoft.Data.SqlClient;
using System;

namespace L13Security
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test af SQL Injections!");
            Test1("","");
        }



        public static void Test1(string USername,string PW)
        {
            using (SqlConnection connection =
            new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Database = L13Security ;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand($@"SELECT * FROM SYSUser WHERE LoginName ='JRT' or 'Hacked' = 'Hacked' and PasswordEncryptedText =')/&FALJH)#WE?' or 'Hacked' = 'Hacked';", connection);
                SqlCommand commandSecure = new SqlCommand($@"SELECT * FROM SYSUser WHERE LoginName =@ln and PasswordEncryptedText =@PET;", connection);
                commandSecure.Parameters.AddWithValue("@ln", "JRT' or 'Hacked' = 'Hacked'");
                commandSecure.Parameters.AddWithValue("@PET", ")/&FALJH)#WE? JRT' or 'Hacked' = 'Hacked'");

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    Console.WriteLine("SQL Injected !");
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                    Console.WriteLine("Test af SQL with parameters!");
                    reader = commandSecure.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }                        
        }

        static public void Test2() 
        {
            
        }
    }
}
