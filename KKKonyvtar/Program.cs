using System;
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
    static void NewRent(MySqlConnection connection, int deviceId)
    {
        try
        {
            connection.Open();

            string sql = $"UPDATE devices SET qty = qty - 1 WHERE Id = {deviceId} AND qty > 0";
            MySqlCommand command = new MySqlCommand(sql, connection);
            int rowsAffected = command.ExecuteNonQuery();

            connection.Close();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Device rented successfully");
            }
            else
            {
                Console.WriteLine("Unable to rent device: Not enough quantity or device not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error renting device: {ex.Message}");
        }
    }

    static void DevicesTable()
    {
        Console.SetCursorPosition((Console.WindowWidth - "+-----------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+-----------------------+");
        Console.SetCursorPosition((Console.WindowWidth - "| KKKönyvtár - Eszközök |".Length) / 2, Console.CursorTop);
        Console.WriteLine("| KKKönyvtár - Eszközök |");
        Console.SetCursorPosition((Console.WindowWidth - "+-----------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+-----------------------+");

        Console.SetCursorPosition((Console.WindowWidth - "+---------------------+---------------------------+---------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+----------------------+---------------------------+--------------------+");
        Console.SetCursorPosition((Console.WindowWidth - "+----------------------+---------------------------+--------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine($"| Azonosító {"| Eszköz neve",24} {"| Darabszám",25} {"|",10}");
        Console.SetCursorPosition((Console.WindowWidth - "+----------------------+---------------------------+--------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+----------------------+---------------------------+--------------------+");

        foreach (Devices device in devices)
        {
            Console.SetCursorPosition((Console.WindowWidth - "+----------------------+---------------------------+--------------------+".Length) / 2, Console.CursorTop);
            Console.WriteLine($"| {device.Id} {"| ",21} {device.DeviceName,24} | {device.Qty,18} |");
        }

        Console.SetCursorPosition((Console.WindowWidth - "+----------------------+---------------------------+--------------------+".Length) / 2, Console.CursorTop);
        Console.WriteLine("+----------------------+---------------------------+--------------------+");
        Console.WriteLine("\nNyomjon egy billentyűt a kilépéshez...");
    }

    static void RentDevices()
    {
        string[] subMenuItems = { "Új Kölcsönzés", "Meglévő Kölcsönzések", "Vissza" };
        int selectedIndex = 0;
        bool backSelected = false;

        while (true)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Black;

            Console.SetCursorPosition((Console.WindowWidth - "+-------------------------+".Length) / 2, Console.CursorTop);
            Console.WriteLine("+-------------------------+");
            Console.SetCursorPosition((Console.WindowWidth - "| KKKönyvtár - Kölcsönzés |".Length) / 2, Console.CursorTop);
            Console.WriteLine("| KKKönyvtár - Kölcsönzés |");
            Console.SetCursorPosition((Console.WindowWidth - "+-------------------------+".Length) / 2, Console.CursorTop);
            Console.WriteLine("+-------------------------+");

            for (int i = 0; i < 3; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed; // Change the color of the selected item
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Black; // Reset the color for other items
                }

                Console.WriteLine("");
                Console.SetCursorPosition((Console.WindowWidth - $"| {subMenuItems[i]} |".Length) / 2, Console.CursorTop);
                Console.WriteLine($"| {subMenuItems[i]} |");

                Console.ResetColor();
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
                    if (selectedIndex == 2)
                    {
                        backSelected = true;
                        break;
                    }
                    else
                    {
                        switch (selectedIndex)
                        {
                            case 0:
                                Console.Clear();
                                Console.SetCursorPosition((Console.WindowWidth - "+----------------------------+".Length) / 2, Console.CursorTop);
                                Console.WriteLine("+----------------------------+");
                                Console.SetCursorPosition((Console.WindowWidth - "| KKKönyvtár - Új Kölcsönzés |".Length) / 2, Console.CursorTop);
                                Console.WriteLine("| KKKönyvtár - Új Kölcsönzés |");
                                Console.SetCursorPosition((Console.WindowWidth - "+----------------------------+".Length) / 2, Console.CursorTop);
                                Console.WriteLine("+----------------------------+");
                                NewRent(connection, 3);
                                Console.ReadKey();
                                break;
                            case 1:
                                Console.Clear();
                                Console.SetCursorPosition((Console.WindowWidth - "+-----------------------------------+".Length) / 2, Console.CursorTop);
                                Console.WriteLine("+-----------------------------------+");
                                Console.SetCursorPosition((Console.WindowWidth - "| KKKönyvtár - Meglévő Kölcsönzések |".Length) / 2, Console.CursorTop);
                                Console.WriteLine("| KKKönyvtár - Meglévő Kölcsönzések |");
                                Console.SetCursorPosition((Console.WindowWidth - "+-----------------------------------+".Length) / 2, Console.CursorTop);
                                Console.WriteLine("+-----------------------------------+");
                                Console.ReadKey();
                                break;

                            case 2:
                                backSelected = true;
                                break;
                        }
                        break;
                    }
            }
            if (backSelected)
            {
                return;
            }
        }
    }

    static void MainMenu(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                Console.Clear();
                DevicesTable();
                Console.ReadKey();
                break;
            case 1:
                Console.Clear();
                RentDevices();
                break;

            case 2:
                Console.WriteLine("Exiting...");
                return;
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
            Console.SetCursorPosition((Console.WindowWidth - "Betöltés...".Length) / 2, 4);
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

        string[] menuItems = { "Eszközök", "Kölcsönzés", "Admin", "Kilépés" };
        int selectedIndex = 0;
        bool menuSelected = false;

        while (!menuSelected)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Black;

            Console.SetCursorPosition((Console.WindowWidth - "+------------------------------+".Length) / 2, Console.CursorTop);
            Console.WriteLine("+------------------------------+");
            Console.SetCursorPosition((Console.WindowWidth - "|     Üdvözöl a KKKönyvtár     |".Length) / 2, Console.CursorTop);
            Console.WriteLine("|     Üdvözöl a KKKönyvtár     |");
            Console.SetCursorPosition((Console.WindowWidth - "+------------------------------+".Length) / 2, Console.CursorTop);
            Console.WriteLine("+------------------------------+");

            for (int i = 0; i < 3; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed; // Change the color of the selected item
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Black; // Reset the color for other items
                }

                Console.WriteLine("");
                Console.SetCursorPosition((Console.WindowWidth - $"| {menuItems[i]} |".Length) / 2, Console.CursorTop);
                Console.WriteLine($"| {menuItems[i]} |");

                Console.ResetColor();
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
                    MainMenu(selectedIndex);
                    break;
            }
        }

        Console.ResetColor(); // Reset the console color after the menu selection
    }
}