using CsvHelper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  operations for interacting with a database
class Database
{
    public readonly SqlConnection Connection;
    public Database(SqlConnection conn)
    {
        Connection = conn ?? throw new ArgumentNullException("Database connection object cannot be null");
    }

    // checks if email is already in a database
    public bool IsEmailRegistered(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.");
        }
        string query = "select count(*) from users where email = @email";
        SqlCommand checkCommand = new SqlCommand(query, Connection);
        try
        {
            checkCommand.Parameters.AddWithValue("@email", email);
            Connection.Open();
            int count = (int)checkCommand.ExecuteScalar(); // ExecuteScalar obj -> int
            return count != 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }

    }

    // validates users credential against the database
    public bool ValidateUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException("User cannot be null.");
        }
        string actualPassword = "";
        string query = "SELECT PASSWRD FROM users WHERE email = @email";
        SqlCommand selectCommand = new SqlCommand(query, Connection);
        selectCommand.Parameters.AddWithValue("@email", user.Email);
        try
        {
            Connection.Open();
            actualPassword = (string)selectCommand.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        return actualPassword == user.Password;
    }

    // adds a new user to a database 
    public void AddUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException("User cannot be null.");
        }
        string query = "INSERT INTO users VALUES (@email, @password)";
        SqlCommand insertCommand = new SqlCommand(query, Connection);
        try
        {
            insertCommand.Parameters.AddWithValue("@email", user.Email);
            insertCommand.Parameters.AddWithValue("@password", user.Password);
            Connection.Open();
            insertCommand.ExecuteNonQuery();
            Console.WriteLine("Your account has been successfully created!");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }

    // lists all track that are liked by a user
    public void ListAllLikedTracks(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException("User cannot be null.");
        }
        string query = "SELECT * FROM tracks t JOIN likes l ON t.track_id = l.track_id WHERE l.usr_email = @email";
        SqlCommand selectCommand = new SqlCommand(query, Connection);
        try
        {
            selectCommand.Parameters.AddWithValue("@email", user.Email);
            Connection.Open();
            using (SqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("You don't have any liked tracks yet.");
                }
                while (reader.Read())
                {
                    Console.WriteLine($"\t- {reader.GetString(1)} by {reader.GetString(2)} (trackId: {reader.GetInt32(0)})");
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }
    
    // lists all tracks in a database
    public void ListAllTracks()
    {
        string query = "SELECT * FROM tracks";
        SqlCommand selectCommand = new SqlCommand(query, Connection);
        try
        {
            Connection.Open();
            using (SqlDataReader reader = selectCommand.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("No tracks in a database yet.");
                }
                while (reader.Read())
                {
                    Console.WriteLine($"\t- {reader.GetString(1)} by {reader.GetString(2)} (trackId: {reader.GetInt32(0)})");
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }

    // adds a track to user's liked tracks
    public void LikeTrack(User user, int trackId)
    {
        if (user == null)
        {
            throw new ArgumentNullException("User cannot be null.");
        }
        string query = "INSERT INTO likes VALUES (@email, @trackId)";
        SqlCommand insertCommand = new SqlCommand(query, Connection);
        insertCommand.Parameters.AddWithValue("@email", user.Email);
        insertCommand.Parameters.AddWithValue("@trackId", trackId);
        try
        {
            Connection.Open();
            insertCommand.ExecuteNonQuery();
            Console.WriteLine($"Track with an ID of {trackId} has been liked.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }

    // removes a track from user's liked tracks
    public void RemoveTrack(User user, int trackId)
    {
        if (user == null)
        {
            throw new ArgumentNullException("User cannot be null.");
        }
        string query = "DELETE FROM likes WHERE usr_email = @email AND track_id = @trackId";
        SqlCommand deleteCommand = new SqlCommand(query, Connection);
        deleteCommand.Parameters.AddWithValue("@email", user.Email);
        deleteCommand.Parameters.AddWithValue("@trackId", trackId);
        try
        {
            Connection.Open();
            deleteCommand.ExecuteNonQuery();
            Console.WriteLine("Track has been deleted.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }

    // adds a new track to a database
    public void AddTrack(List<string> trackList)
    {
        if (trackList == null)
        {
            throw new ArgumentNullException("A list of tracks to add cannot be null.");
        }
        string query = "INSERT INTO tracks (track_name, artist_name) VALUES (@name, @artist)";
        SqlCommand insertCommand = new SqlCommand(query, Connection);
        insertCommand.Parameters.AddWithValue("@name", trackList[0]);
        insertCommand.Parameters.AddWithValue("@artist", trackList[1]);
        try
        {
            Connection.Open();
            insertCommand.ExecuteNonQuery();
            Console.WriteLine($"Track called {trackList[0]} (by {trackList[1]}) has been successfully added to a database!");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }
    
    // imports tracks from a csv file into a database
    public void ImportTracksFromCsv(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException("File path cannot be null or empty.");
        }
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified file does not exist.", filePath);
        }
        string deleteQuery = "DELETE FROM Tracks";
        SqlCommand deleteCommand = new SqlCommand(deleteQuery, Connection);
        try
        {
            Connection.Open();
            deleteCommand.ExecuteNonQuery();
            using (StreamReader reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // skipping the header
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    if (values.Length == 2)
                    {
                        string query = "INSERT INTO Tracks (track_name, artist_name) VALUES (@track, @artist)";
                        SqlCommand insertCommand = new SqlCommand(query, Connection);
                        insertCommand.Parameters.AddWithValue("@track", values[0]);
                        insertCommand.Parameters.AddWithValue("@artist", values[1]);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        Console.WriteLine("Import completed.");
    }
    
    // exports all tracks from a database to a csv file
    public void ExportTracksToCsv(string filePath)
    {
        string query = "SELECT track_name, artist_name FROM tracks";
        SqlCommand selectCommand = new SqlCommand(query, Connection);
        var csv = new StringBuilder();
        try
        {
            Connection.Open();
            using (SqlDataReader reader = selectCommand.ExecuteReader())
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                csv.AppendLine("Track Name,Artist Name");
                while (reader.Read())
                {
                    var newLine = $"{reader.GetString(0)},{reader.GetString(1)}";
                    csv.AppendLine(newLine);
                }
            }
            File.WriteAllText(filePath, csv.ToString());
            Console.WriteLine("Export completed.");

        }
        catch (SqlException ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }

}