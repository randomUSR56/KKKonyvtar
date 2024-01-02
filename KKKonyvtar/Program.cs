using MySql.Data.MySqlClient;

namespace KKKonyvtar;

class Program
{
    static string db_connection(string server, string database, string username, string password)
    {
        string connectionString = $"Data Source={server};Initial Catalog={database};User ID={username};Password={password};";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                return "Connceted!";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }

    static async Task Main(string[] args)
    {
        Task<string> connectingTask = Task.Run(() => db_connection("localhost", "kkkonyvtar", "root", "root"));

        while (!connectingTask.IsCompleted)
        {
            Console.Write("Loading");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
        }

        string result = await connectingTask;

        Console.Write(result);
    }
}

