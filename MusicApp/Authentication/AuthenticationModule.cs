using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text;
using MusicApp.Database;

namespace MusicApp.Authentication
{
    public class AuthenticationModule
    {
        // User register part, it will hash the password and store it in the database.
        public static bool RegisterUser(string username, string password)
        {
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            // Create a SqlConnection with the appropriate connection string
            string connectionString = "Data Source=LAPTOP-82HME98S\\SQLEXPRESS01;Initial Catalog=Software2;Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                // Open the connection
                sqlConnection.Open();

                // Create an instance of DatabaseManager passing the SqlConnection
                var databaseManager = new DatabaseManager(sqlConnection);

                // Call the RegisterUser method on the instance of DatabaseManager
                bool result = databaseManager.RegisterUser(username, hashedPassword, salt);

                // Close the connection
                sqlConnection.Close();

                return result;
            }
        }

        // This part checks if the user is in the database and if the password is correct.
        public static bool AuthenticateUser(string username, string password)
        {
            string connectionString = "Data Source=LAPTOP-82HME98S\\SQLEXPRESS01;Initial Catalog=Software2;Integrated Security=True";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                // Create an instance of DatabaseManager
                var databaseManager = new DatabaseManager(sqlConnection);

                // Open the connection
                sqlConnection.Open();

                // Retrieve credentials using the DatabaseManager instance
                (string hashedPassword, string salt) = databaseManager.GetCredentials(username);

                // Close the connection
                sqlConnection.Close();

                // Compare the hashed passwords
                return hashedPassword == HashPassword(password, salt);
            }
        }

        // This part generates a random salt for the password hashing.
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Convertimos la sal a una cadena Base-64
            string base64Salt = Convert.ToBase64String(saltBytes);

            // Verificamos la validez de la cadena Base-64 generada
            try
            {
                Convert.FromBase64String(base64Salt);
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error al generar la sal: " + ex.Message);
                throw; // Relanzamos la excepción para detener la ejecución
            }

            return base64Salt;
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltBytes = Convert.FromBase64String(salt); // Convertimos la sal de Base-64 a bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedPassword, passwordBytes.Length, saltBytes.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Convertimos los bytes hash a una cadena hexadecimal
                string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }
    }
}

