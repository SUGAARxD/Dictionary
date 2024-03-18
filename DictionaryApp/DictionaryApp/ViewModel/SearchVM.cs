using DictionaryApp.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System;

namespace DictionaryApp.ViewModel
{
    internal class SearchVM : BaseVM
    {
        private static readonly string _filePath = @"..\..\resources\Database\words.json";

        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;
                
                UpdateSuggestions();

                IsListBoxVisible = (string.IsNullOrEmpty(value) || Suggestions.Count() == 0)? Visibility.Hidden : Visibility.Visible;

                NotifyPropertyChanged(nameof(Word));
            }
        }
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            private set
            {
                _imageSource = value;
                NotifyPropertyChanged(nameof(ImageSource));
            }
        }

        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                _category = value;

                UpdateSuggestions();

                NotifyPropertyChanged(nameof(Category));
            }
        }

        private readonly List<WordModel> _words;
        public ObservableCollection<string> Suggestions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> CategoryList { get; set; }

        public SearchVM()
        {
            IsListBoxVisible = Visibility.Hidden;
            IsImageBorderVisible = Visibility.Hidden;
            Word = string.Empty;
            _words = new List<WordModel>();
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _words = JsonConvert.DeserializeObject<List<WordModel>>(jsonString);
            }

            Category = "";
            CategoryList = new ObservableCollection<string>
            {
                _category
            };
            foreach (WordModel word in _words)
            {
                CategoryList.Add(word.Category);
            }
            ImageSource = null;
            CategoryLabel = "Category: ";
            Description = string.Empty;
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand(ExecuteSearch);
                return _searchCommand;
            }
        }
        private void ExecuteSearch(object parameter)
        {
            IsListBoxVisible = Visibility.Hidden;
            if (string.IsNullOrEmpty(_word))
            {
                Description = string.Empty;
                Category = string.Empty;
                CategoryLabel = "Category:";
                ImageSource = null;
                IsImageBorderVisible = Visibility.Hidden;
                return;
            }
            foreach (WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    Description = word.Description;
                    CategoryLabel = "Category: " + word.Category;
                    ImageSource = new BitmapImage(new Uri(word.ImagePath.Replace(@"\\", @"\"), UriKind.RelativeOrAbsolute));
                    IsImageBorderVisible = Visibility.Visible;
                    break;
                }
            }
        }
        private Visibility _isListBoxVisible;
        public Visibility IsListBoxVisible
        {
            get => _isListBoxVisible;
            set
            {
                _isListBoxVisible = value;
                NotifyPropertyChanged(nameof(IsListBoxVisible));
            }
        }
        public string SelectedListBoxItem
        {
            set
            {
                _word = value;
                NotifyPropertyChanged(nameof(Word));
                
                IsListBoxVisible = Visibility.Hidden;
            }
        }
        private void UpdateSuggestions()
        {
            Suggestions.Clear();
            if (string.IsNullOrEmpty(_word))
                return;

            _words.Where(item => item.Text.ToLower().StartsWith(_word.ToLower())
                 && (item.Category.Equals(_category) || string.IsNullOrEmpty(_category)))
                 .Select(item => item.Text)
                 .ToList()
                 .ForEach(item =>
                 {
                     Suggestions.Add(item);
                 });
        }
        private string _categoryLabel;
        public string CategoryLabel
        {
            get => _categoryLabel;
            set
            {
                _categoryLabel = value;
                NotifyPropertyChanged(nameof(CategoryLabel));
            }
        }
        private Visibility _isImageBorderVisible;
        public Visibility IsImageBorderVisible
        {
            get => _isImageBorderVisible;
            set
            {
                _isImageBorderVisible = value;
                NotifyPropertyChanged(nameof(IsImageBorderVisible));
            }
        }
    }
}