using System.Windows;
using System.Data.SqlClient; // Ensure you have this namespace for SqlConnection
using MusicApp.Database;    // Namespace where DatabaseManager is located
using MusicApp.Search;      // Namespace where SearchWindow is located

namespace MusicApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToSearch(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=LAPTOP-82HME98S\\SQLEXPRESS01; Initial Catalog=Software2; Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            // Assuming DatabaseManager properly implements IDatabaseManager
            IDatabaseManager databaseManager = new DatabaseManager(sqlConnection);

            SearchWindow searchWindow = new SearchWindow(databaseManager);
            searchWindow.Show();
            Close(); // This will close the main window
        }

        private void GoToChat(object sender, RoutedEventArgs e)
        {
            Chat.MVVM.View.ChatWindow chatWindow = new Chat.MVVM.View.ChatWindow();
            chatWindow.Show();
            Close();
        }

        private void GoToProfile(object sender, RoutedEventArgs e)
        {
            Profile.ProfileWindow profileWindow = new Profile.ProfileWindow();
            profileWindow.Show();
            Close();
        }
    }
}

