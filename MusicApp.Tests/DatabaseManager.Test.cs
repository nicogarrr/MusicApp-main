using Moq;
using MusicApp.Database;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

// Interfaz para envolver IDbCommand
public interface IDbCommandWrapper
{
    int ExecuteNonQuery();
    // Otros miembros que necesites
}

// Implementación de IDbCommandWrapper que envuelve SqlCommand
public class SqlCommandWrapper : IDbCommandWrapper
{
    private SqlCommand _sqlCommand;

    public SqlCommandWrapper(SqlCommand sqlCommand)
    {
        _sqlCommand = sqlCommand;
    }

    public int ExecuteNonQuery()
    {
        return _sqlCommand.ExecuteNonQuery();
    }
    // Implementa otros miembros según sea necesario
}

// Clase de pruebas
[TestClass]
public class DatabaseManagerTests
{
    private Mock<DatabaseManager.IDbCommandWrapper> _mockDbCommandWrapper;
    private DatabaseManager _databaseManager;

    [TestInitialize]
    public void Initialize()
    {
        _mockDbCommandWrapper = new Mock<DatabaseManager.IDbCommandWrapper>();
        _databaseManager = new DatabaseManager(_mockDbCommandWrapper.Object);
    }

    [TestMethod]

    public void RegisterUser_ValidData_ReturnsTrue()
    {
        // Arrange
        var userName = "testUser";
        var hashedPassword = "testPassword";
        var salt = "testSalt";
        _mockDbCommandWrapper.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1); // Simulate one row affected

        // Act
        _databaseManager.RegisterUser(userName, hashedPassword, salt);
    }

    [TestMethod]
    public void RegisterUser_DatabaseError_ReturnsFalse()
    {
        // Arrange
        var userName = "testUser";
        var hashedPassword = "testPassword";
        var salt = "testSalt";
        _mockDbCommandWrapper.Setup(cmd => cmd.ExecuteNonQuery()).Throws<CustomSqlException>(); // Use your custom exception class

        // Act
        bool result = false;
        try
        {
            result = _databaseManager.RegisterUser(userName, hashedPassword, salt);
        }
        catch (CustomSqlException)
        {
            // Si la excepción es lanzada, result seguirá siendo false
        }

        // Assert
        Assert.IsFalse(result);
    }


}

// Clase personalizada de excepción
public class CustomSqlException : Exception
{
    public CustomSqlException() : base()
    {
    }

    public CustomSqlException(string message) : base(message)
    {
    }
}



// Clase DatabaseManager que utiliza IDbCommandWrapper en lugar de SqlCommand
public class DatabaseManager
{
    private IDbCommandWrapper _dbCommand;

    // Constructor
    public DatabaseManager(IDbCommandWrapper dbCommand)
    {
        _dbCommand = dbCommand;
    }

    // Propiedad para IDbCommandWrapper
    public IDbCommandWrapper DbCommand
    {
        get { return _dbCommand; }
        set { _dbCommand = value; }
    }

    // Interfaz IDbCommandWrapper
    public interface IDbCommandWrapper
    {
        void SetCommandText(string commandText);
        void AddParameter(string name, object value);
        int ExecuteNonQuery();
        // Otros miembros que necesites
    }

    // Método para registrar un usuario
    public bool RegisterUser(string userName, string hashedPassword, string salt)
    {
        try
        {
            _dbCommand.SetCommandText("INSERT INTO [USER] (userName, profilePicture, subscriptionPlan, hashedPassword, salt) VALUES (@userName, @profilePicture, @subscriptionPlan, @hashedPassword, @salt)");


            _dbCommand.AddParameter("@userName", userName);
            _dbCommand.AddParameter("@profilePicture", string.Empty); // Assuming profile picture is empty by default
            _dbCommand.AddParameter("@subscriptionPlan", string.Empty); // Assuming subscription plan is empty by default
            _dbCommand.AddParameter("@hashedPassword", hashedPassword);
            _dbCommand.AddParameter("@salt", salt);

            int rowsAffected = _dbCommand.ExecuteNonQuery();
            Console.WriteLine($"Rows affected: {rowsAffected}");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during database operation: {ex.Message}");
            return false;
        }

    }


}