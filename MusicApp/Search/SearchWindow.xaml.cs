using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using MusicApp.Database;

namespace MusicApp.Search
{
    /// <summary>
    /// Lógica de interacción para SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private IDatabaseManager databaseManager;

        // Constructor that accepts IDatabaseManager for dependency injection
        public SearchWindow(IDatabaseManager databaseManager)
        {
            InitializeComponent();
            this.databaseManager = databaseManager;
        }

        public static SearchResultItemControl AddSearchResult(string imagePath, string title, string subTitle1 = "", string subTitle2 = "", string subTitle3 = "")
        {
            SearchResultItemControl resultItem = new SearchResultItemControl();
            resultItem.SetImage(imagePath);
            resultItem.SetTitle(title);
            resultItem.SetSubTitle1(subTitle1);
            resultItem.SetSubTitle2(subTitle2);
            resultItem.SetSubTitle3(subTitle3);
            return resultItem;
        }

        public void SearchButton(object sender, RoutedEventArgs e)
        {
            searchResultsStackPanel.Children.Clear();
            int filter = filterComboBox.SelectedIndex;
            string keywords = searchInput.Text;
            List<SearchResultItemControl> searchItems = FilterSearch(filter);
            List<SearchResultItemControl> searchResults = FuzzyMatchingSearch(keywords, searchItems);
            int sorter = sortComboBox.SelectedIndex;
            searchResults = SortSearchResults(searchResults, sorter);
            DisplaySearchResults(searchResults);
        }

        public List<SearchResultItemControl> FuzzyMatchingSearch(string keywords, List<SearchResultItemControl> searchItems)
        {
            List<SearchResultItemControl> matches = new List<SearchResultItemControl>();
            foreach (SearchResultItemControl item in searchItems)
            {
                string itemTitle = item.title.Text;
                if (LevenshteinDistance.IsFuzzyMatch(keywords.ToLower(), itemTitle.ToLower(), 2))
                {
                    matches.Add(item);
                }
            }
            return matches;
        }

        public List<SearchResultItemControl> FilterSearch(int filter)
        {
            List<SearchResultItemControl> searchItems = new List<SearchResultItemControl>();
            switch (filter)
            {
                case 0:
                    searchItems = databaseManager.LoadSongSearchItems(searchItems);
                    searchItems = databaseManager.LoadArtistSearchItems(searchItems);
                    searchItems = databaseManager.LoadAlbumSearchItems(searchItems);
                    searchItems = databaseManager.LoadUserSearchItems(searchItems);
                    break;
                case 1:
                    searchItems = databaseManager.LoadSongSearchItems(searchItems);
                    break;
                case 2:
                    searchItems = databaseManager.LoadArtistSearchItems(searchItems);
                    break;
                case 3:
                    searchItems = databaseManager.LoadAlbumSearchItems(searchItems);
                    break;
                case 4:
                    string genre = genreInput.Text;
                    searchItems = databaseManager.LoadSongSearchItems(searchItems, genre);
                    searchItems = databaseManager.LoadAlbumSearchItems(searchItems, genre);
                    break;
                case 5:
                    searchItems = databaseManager.LoadUserSearchItems(searchItems);
                    break;
            }
            return searchItems;
        }

        public List<SearchResultItemControl> SortSearchResults(List<SearchResultItemControl> searchResults, int sorter)
        {
            List<SearchResultItemControl> sortedResults = new List<SearchResultItemControl>();
            switch (sorter)
            {
                case 0:
                    sortedResults = searchResults; // PROVISIONAL
                    break;
                case 1:
                    sortedResults = searchResults; // PROVISIONAL
                    break;
                case 2:
                    if (filterComboBox.SelectedIndex == 0 || filterComboBox.SelectedIndex == 2 || filterComboBox.SelectedIndex == 5)
                    {
                        MessageBox.Show("Cannot sort artists or users by date. Please select another sorter or filter", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        sortedResults = searchResults;
                    }
                    else
                    {
                        sortedResults = Sorters.NumericalQuickSort(searchResults);
                    }
                    break;
                case 3:
                    sortedResults = Sorters.AlphabeticalQuickSort(searchResults);
                    break;
            }
            return sortedResults;
        }

        public void DisplaySearchResults(List<SearchResultItemControl> searchResults)
        {
            foreach (SearchResultItemControl item in searchResults)
            {
                searchResultsStackPanel.Children.Add(item);
            }
            searchResultsStackPanel.UpdateLayout();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (genreInput != null && filterComboBox.SelectedIndex == 4)
            {
                genreInput.Visibility = Visibility.Visible;
            }
            else
            {
                genreInput.Visibility = Visibility.Collapsed;
            }
        }
    }
}
