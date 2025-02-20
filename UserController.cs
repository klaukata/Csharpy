using System.Text.Json;
using System.Text.Json.Serialization;

// operations for user interactions (picks registering or logging in)
class UserController
{
    private readonly AuthService AuthService;
    private readonly Database Database;
    private User? loggedInUser;

    public UserController(AuthService authService, Database db)
    {
        AuthService = authService ?? throw new ArgumentNullException("AuthService object can't be null.");
        Database = db ?? throw new ArgumentNullException("AuthService object can't be null.");
    }
    public void Start()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("Hi! Pick what would you like to do:\n1. Register\n2. Login\n3. Exit");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AuthService.Register();
                        break;
                    case "2":
                        loggedInUser = AuthService.Login();
                        if (loggedInUser != null)
                        {
                            ShowUserMenu();
                        }
                        break;
                    case "3":
                        Console.WriteLine("Exiting the program.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}. \nPlease try again.");
            }
        }
    }
    private void ShowUserMenu()
    {
        try
        {
            while (true)
            {
                Console.WriteLine($"\nWelcome, {loggedInUser?.Email}! Choose an option:");
                Console.WriteLine("1. View liked songs");
                Console.WriteLine("2. Remove a liked song");
                Console.WriteLine("3. Like a new track");
                Console.WriteLine("4. Browse all tracks");
                Console.WriteLine("5. Add new track from an API");
                Console.WriteLine("6. Import tracks from a CSV file");
                Console.WriteLine("7. Export all tracks to a CSV file");
                Console.WriteLine("8. Logout");
                Console.Write("Enter your choice: ");

                int trackId;
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Database.ListAllLikedTracks(loggedInUser);
                            break;
                        case "2":
                            Console.Write("Enter track ID to remove: ");
                            if (int.TryParse(Console.ReadLine(), out trackId))
                            {
                                Database.RemoveTrack(loggedInUser, trackId);
                            }
                            else
                            {
                                Console.WriteLine("Invalid track ID format");
                            }
                            break;
                        case "3":
                            Console.Write("Enter track ID to like: ");
                            if (int.TryParse(Console.ReadLine(), out trackId))
                            {
                                Database.LikeTrack(loggedInUser, trackId);
                            }
                            else
                            {
                                Console.WriteLine("Invalid track ID format");
                            }
                            break;
                        case "4":
                            Database.ListAllTracks();
                            break;
                        case "5":
                            HandleAddTrackFromApi().Wait();
                            break;
                        case "6":
                            try
                            {
                                Console.Write("Enter full CSV file path: ");
                                var filePath = Console.ReadLine();

                                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                                {
                                    Database.ImportTracksFromCsv(filePath);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid file path or file doesn't exist");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Import failed: {ex.Message}");
                            }
                            break;
                        case "7":
                            try
                            {
                                Console.Write("Enter full CSV file path: ");
                                var filePath = Console.ReadLine();

                                if (!string.IsNullOrWhiteSpace(filePath))
                                {
                                    Database.ExportTracksToCsv(filePath);
                                    Console.WriteLine($"Data exported to {filePath}");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid file path");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Export failed: {ex.Message}");
                            }
                            break;
                        case "8":
                            Console.WriteLine("Logging out...");
                            loggedInUser = null;
                            return;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Operation failed: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Session error: {ex.Message}");
        }
    }
    private async Task HandleAddTrackFromApi()
    {
        try
        {
            Console.Write("Enter track name to search: ");
            var trackName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(trackName))
            {
                Console.WriteLine("Empty track name");
                return;
            }

            Client client = new Client();

            string responseBodyJson = client.GetTrackList(trackName).Result;

            //  deserializing json
            var response = JsonSerializer.Deserialize<ApiResponse>(responseBodyJson);

            if (response == null || response.Recordings.Count == 0)
            {
                Console.WriteLine("No tracks found");
                return;
            }

            List<List<string>> trackArtistList = new List<List<string>>();
            Console.WriteLine($"Tracks containing '{trackName}':");
            for (int i = 0; i < response.Recordings.Count; i++)
            {
                string title = response.Recordings[i].Title;
                string artist = response.Recordings[i].Artist[0].Name;
                trackArtistList.Add(new List<string> { title, artist });

                Console.WriteLine($"{i + 1}. {title} - {artist}");
            }

            int selectedIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 1 || selectedIndex > trackArtistList.Count)
            {
                Console.Write("Select an artist by entering a number: ");
            }

            List<string> selectedTrack = trackArtistList[selectedIndex - 1];
            Database.AddTrack(selectedTrack);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API error: {ex.Message}");
        }
    }


}