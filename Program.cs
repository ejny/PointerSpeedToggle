// PointerSpeedManager.cs
using System;
using System.IO;
using System.Text.Json;
using System.Runtime.InteropServices;

class PointerSpeedManager
{
    // Import Windows API functions
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, int pvParam, uint fWinIni);

    // Constants
    const uint SPI_GETPOINTERSPEED = 0x0070;
    const uint SPI_SETPOINTERSPEED = 0x0071;
    const uint SPIF_UPDATEINIFILE = 0x01;
    const uint SPIF_SENDCHANGE = 0x02;

    // Class to hold pointer speeds
    class SpeedConfig
    {
        public int Speed1 { get; set; }
        public int Speed2 { get; set; }
    }

    static void Main(string[] args)
    {
        try
        {
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string jsonPath = Path.Combine(exeFolder, "Speeds.json"); // Updated JSON filename

            SpeedConfig config;
            bool firstRun = false;
            bool setupMode = args.Length > 0 && args[0].Equals("-setup", StringComparison.OrdinalIgnoreCase);

            // Setup mode or first run
            if (setupMode || !File.Exists(jsonPath))
            {
                firstRun = true;
                Console.WriteLine(setupMode ? "PointerSpeedToggle - Setup mode" : "PointerSpeedToggle - First run mode");

                Console.WriteLine("");
                Console.WriteLine("Enter first pointer speed (1-11):");
                int cpSpeed1 = ReadSpeedFromUser();

                Console.WriteLine("");
                Console.WriteLine("Enter second pointer speed (1-11):");
                int cpSpeed2 = ReadSpeedFromUser();

                config = new SpeedConfig
                {
                    Speed1 = ConvertCPSpeedToWindows(cpSpeed1),
                    Speed2 = ConvertCPSpeedToWindows(cpSpeed2)
                };

                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, json);

            }
            else
            {
                string json = File.ReadAllText(jsonPath);
                config = JsonSerializer.Deserialize<SpeedConfig>(json) ?? throw new Exception("Invalid JSON file.");
            }

            // Get current pointer speed
            int currentSpeed = 0;
            SystemParametersInfo(SPI_GETPOINTERSPEED, 0, ref currentSpeed, 0);

            // Determine new speed
            int newSpeed;
            if (firstRun)
            {
                newSpeed = config.Speed1; // apply first entered speed
            }
            else
            {
                newSpeed = (currentSpeed == config.Speed1) ? config.Speed2 : config.Speed1;
            }

            // Set new pointer speed
            SystemParametersInfo(SPI_SETPOINTERSPEED, 0, newSpeed, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    // Method to read user input between 1-11
    static int ReadSpeedFromUser()
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= 1 && value <= 11)
                return value;

            Console.WriteLine("Invalid input. Enter a number between 1 and 11:");
        }
    }

    // Exact conversion: Control Panel 1-11 → Windows 0-20
    static int ConvertCPSpeedToWindows(int cpSpeed)
    {
        return (cpSpeed - 1) * 2;
    }

    // Optional: Convert Windows 0-20 → Control Panel 1-11
    static int ConvertWindowsToCPSpeed(int winSpeed)
    {
        return (winSpeed / 2) + 1;
    }
}
