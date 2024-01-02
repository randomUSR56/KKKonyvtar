using MySql.Data.MySqlClient;

namespace KKKonyvtar;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=localhost;Database=kkkonyvtar;User=root;Password=root;";

        using MySqlConnection connection = new MySqlConnection(connectionString);
        connection.Open();

        Console.WriteLine("Connected to MySQL!");

        // Perform database operations here

        connection.Close();
    }
}

