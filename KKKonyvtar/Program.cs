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

            return "OK: Devices loaded"; //Debug miatt ez a függvény visszatér a lekérdezési, majd a feltöltési kísérlet eredményével
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
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
                return "OK: Connceted";
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }
    }

    static async Task Main(string[] args)
    {
        Task<string> connectingTask = Task.Run(() => db_connection("bfehuno3vjcd8qb1iyak-mysql.services.clever-cloud.com", "bfehuno3vjcd8qb1iyak", "ufxgthjtwc8fo5x3", "uqhVijgTR93wchcttD7e"));

        Console.SetCursorPosition((Console.WindowWidth - "+------------------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+------------------------------+");
        Console.SetCursorPosition((Console.WindowWidth - "|     Üdvözöl a KKKönyvtár     |".Length) / 2, Console.CursorTop);
        Console.WriteLine("|     Üdvözöl a KKKönyvtár     |");
        Console.SetCursorPosition((Console.WindowWidth - "+------------------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+------------------------------+");

        while (!connectingTask.IsCompleted) //Betöltési képernyő
        {
            Console.SetCursorPosition((Console.WindowWidth - "Betöltés...".Length) / 2, Console.CursorTop);
            Console.Write("Betöltés");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r"); //Ezzel csak egyetlen sort törlök a terminálban
        }

        string connectionResult = await connectingTask;

        Task<string> readTask = Task.Run(() => GetDevices(connection));
        string readResult = await readTask;

        /*foreach (Devices device in devices)
        {
            Console.WriteLine($"ID: {device.Id}, Device: {device.DeviceName}, Quantity: {device.Qty}");
        }*/
    }
}

