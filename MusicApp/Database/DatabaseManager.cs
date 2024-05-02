using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows; // Only if MessageBox usage is necessary, otherwise consider removing it
using MusicApp.Search;

namespace MusicApp.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly SqlConnection connection;

        public DatabaseManager(SqlConnection connection)
        {
            this.connection = connection;  // Assigning the incoming parameter to the class's field
        }

        public bool RegisterUser(string username, string password, string salt)
        {
            try
            {
                this.connection.Open();
                string query = "INSERT INTO [USER] (userName, hashedPassword, salt) VALUES (@userName, @hashedPassword, @salt);";
                using (SqlCommand command = new SqlCommand(query, this.connection))
                {
                    command.Parameters.AddWithValue("@userName", username);
                    command.Parameters.AddWithValue("@hashedPassword", password);
                    command.Parameters.AddWithValue("@salt", salt);
                    command.ExecuteNonQuery();
                }
                this.connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error registering user: " + ex.Message);
                return false;
            }
        }

        public (string, string) GetCredentials(string username)
        {
            try
            {
                this.connection.Open();
                string query = "SELECT hashedPassword, salt FROM [USER] WHERE userName = @userName;";
                using (SqlCommand command = new SqlCommand(query, this.connection))
                {
                    command.Parameters.AddWithValue("@userName", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashedPassword = reader["hashedPassword"].ToString();
                            string salt = reader["salt"].ToString();
                            return (hashedPassword, salt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting credentials: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is always closed, even if an exception occurs
                if (this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
            }
            return (null, null);
        }

        public List<SearchResultItemControl> LoadUserSearchItems(List<SearchResultItemControl> searchItems)
        {
            try
            {
                this.connection.Open();
                string query = "SELECT userName, profilePicture FROM [USER];";
                using (SqlCommand command = new SqlCommand(query, this.connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SearchResultItemControl item = new SearchResultItemControl();
                            item.SetTitle(reader["userName"].ToString());
                            item.SetImage(reader["profilePicture"].ToString());
                            item.SetSubTitle1("User");  // Assuming role or description is "User"
                            searchItems.Add(item);
                        }
                    }
                }
                this.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user search items: " + ex.Message);
            }
            return searchItems;
        }

        public List<SearchResultItemControl> LoadArtistSearchItems(List<SearchResultItemControl> searchItems)
        {
            try
            {
                this.connection.Open();
                string query = "SELECT artistName, artistPicture FROM ARTIST;";
                using (SqlCommand command = new SqlCommand(query, this.connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SearchResultItemControl item = new SearchResultItemControl();
                            item.SetTitle(reader["artistName"].ToString());
                            item.SetImage(reader["artistPicture"].ToString());
                            item.SetSubTitle1("Artist");  // Additional info as needed
                            searchItems.Add(item);
                        }
                    }
                }
                this.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading artist search items: " + ex.Message);
            }
            return searchItems;
        }

        public List<SearchResultItemControl> LoadSongSearchItems(List<SearchResultItemControl> searchItems, string genreFilter = "")
        {
            try
            {
                this.connection.Open();
                string query = genreFilter != string.Empty ?
                    "SELECT songName, albumId FROM SONG WHERE albumId IN (SELECT albumId FROM ALBUM WHERE genre = @genre);" :
                    "SELECT songName, albumId FROM SONG;";
                using (SqlCommand command = new SqlCommand(query, this.connection))
                {
                    if (genreFilter != string.Empty)
                    {
                        command.Parameters.AddWithValue("@genre", genreFilter);
                    }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SearchResultItemControl item = new SearchResultItemControl();
                            item.SetTitle(reader["songName"].ToString());
                            searchItems.Add(item);
                                // Additional properties like albumId could be processed further to get more details;
                        }
                    }
                }
                this.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading song search items: " + ex.Message);
            }
            return searchItems;
        }

        public List<SearchResultItemControl> LoadAlbumSearchItems(List<SearchResultItemControl> searchItems, string genreFilter = "")
        {
            try
            {
                this.connection.Open();
                string query = genreFilter != string.Empty ?
                    "SELECT albumName, albumPicture FROM ALBUM WHERE genre = @genre;" :
                    "SELECT albumName, albumPicture FROM ALBUM;";
                using (SqlCommand command = new SqlCommand(query, this.connection))
                {
                    if (genreFilter != string.Empty)
                    {
                        command.Parameters.AddWithValue("@genre", genreFilter);
                    }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SearchResultItemControl item = new SearchResultItemControl();
                            item.SetTitle(reader["albumName"].ToString());
                            item.SetImage(reader["albumPicture"].ToString());
                            searchItems.Add(item);
                                // Additional details could be added here
                        }
                    }
                }
                this.connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading album search items: " + ex.Message);
            }
            return searchItems;
        }
    }
}
