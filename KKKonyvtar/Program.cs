using MySql.Data.MySqlClient;

namespace KKKonyvtar;

class Program
{
    public class Devices
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public int Qty { get; set; }
    }

    static MySqlConnection connection; //A MySQL kapcsolat globalizálása

    static List<Devices> devices; //Eszközök globalizálása

    static string GetDevices(MySqlConnection connection) //Ebben a függvényben kérem le az eszközöket és töltöm fel őket az objektumba 
    {
        devices = new List<Devices>();

        try
        {
            connection.Open();

            string sql = "SELECT * FROM devices";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                devices.Add(new Devices()
                {
                    Id = reader.GetInt32(0),
                    DeviceName = reader.GetString(1),
                    Qty = reader.GetInt32(2)
                });
            }

            reader.Close();
            connection.Close();

            return "\nDevices loaded!"; //Debug miatt ez a függvény visszatér a lekérdezési, majd a feltöltési kísérlet eredményével
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}"; //Valamiért még akkor is ezzel tér vissza, amikor sikeres volt minden
        }
    }

    static string db_connection(string server, string database, string username, string password) //MySQL kapcsolat létrehozésa
    {
        string connectionString = $"Data Source={server};Initial Catalog={database};User ID={username};Password={password};";

        using (connection = new MySqlConnection(connectionString))
        {
            try //Ez szintén Debug miatt van itt, illetve akár a felhasználónak is jelezhetjük, hogy sikeres volt-e a csatlakozás
            {
                connection.Open(); //Ahoz, hogy a függvény visszatérjen a csatlakozási kísérlet eredményével, ahoz nyitnom kellett egy kapcsolatot.
                connection.Close(); //Mivel erre a kapcsolatra nincs szükség, így egyből be is zárom
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
        Task<string> readTask = Task.Run(() => GetDevices(connection));

        while (!connectingTask.IsCompleted) //Betöltési képernyő
        {
            Console.Write("Loading");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r"); //Ezzel csak egyetlen sort törlök a terminálban
        }
        
        string connection_result = await connectingTask;
        string read_result = await readTask;

        Console.Write(connection_result);
        Console.Write(read_result);

        Thread.Sleep(3000); //Ez mind csak debug

        GetDevices(connection);

        foreach (Devices device in devices)
        {
            Console.WriteLine($"ID: {device.Id}, Device: {device.DeviceName}, Quantity: {device.Qty}");
        }

    }
}

