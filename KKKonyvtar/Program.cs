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

    static void AddDevice(MySqlConnection connection, Devices device)
        {
            try
            {
                connection.Open();

                string sql = $"INSERT INTO devices (DeviceName, Qty) VALUES ('{device.DeviceName}', {device.Qty})";
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();

                Console.WriteLine("Device added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding device: {ex.Message}");
            }
        }

        static void DeleteDevice(MySqlConnection connection, int deviceId)
        {
            try
            {
                connection.Open();

                string sql = $"DELETE FROM devices WHERE Id = {deviceId}";
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();

                Console.WriteLine("Device deleted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting device: {ex.Message}");
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

        string[] menuItems = { "Add device", "Delete device", "Exit" };
        int selectedIndex = 0;
        bool menuSelected = false;

        while (!menuSelected)
        {
            Console.Clear();

            for (int i = 0; i < 3; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Change the color of the selected item
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White; // Reset the color for other items
                }

                Console.WriteLine($"{i + 1}. {(i == selectedIndex ? "> " : "")}{menuItems[i]}");
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? 2 : selectedIndex - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == 2) ? 0 : selectedIndex + 1;
                    break;

                case ConsoleKey.Enter:
                    menuSelected = true;
                    break;
            }
        }

        switch (selectedIndex)
        {
            case 0:
                Console.Write("Enter device name: ");
                string deviceName = Console.ReadLine();
                Console.Write("Enter quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                Devices newDevice = new Devices
                {
                    DeviceName = deviceName,
                    Qty = quantity
                };

                AddDevice(connection, newDevice);
                break;

            case 1:
                Console.Write("Enter device ID to delete: ");
                int deviceId = int.Parse(Console.ReadLine());

                DeleteDevice(connection, deviceId);
                break;

            case 2:
                Console.WriteLine("Exiting...");
                return;
        }

        Console.ResetColor(); // Reset the console color after the menu selection
    }
}