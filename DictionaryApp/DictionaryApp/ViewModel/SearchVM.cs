using DictionaryApp.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows;

namespace DictionaryApp.ViewModel
{
    internal class SearchVM :BaseVM
    {
        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;

                Suggestions = new ObservableCollection<string>(_words
                    .Where(item => item.Text.StartsWith(_word)
                    && (item.Category.Equals(_category) || string.IsNullOrEmpty(_category)))
                    .Select(item => item.Text)
                    .ToList());

                _isListBoxVisible = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible; 

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
        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                NotifyPropertyChanged(nameof(ImagePath));
            }
        }
        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                _category = value;

                NotifyPropertyChanged(nameof(Category));
            }
        }

        private static readonly string _filePath = @"..\..\resources\Database\words.json";

        private readonly List<WordModel> _words;
        public ObservableCollection<string> Suggestions { get; set; }
        public ObservableCollection<string> CategoryList { get; set; }
        
        public SearchVM()
        {
            _isListBoxVisible = Visibility.Visible;
            _word = string.Empty;
            _category = "";
            CategoryList = new ObservableCollection<string>
            {
                _category
            };

            _words = new List<WordModel>();
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _words = JsonConvert.DeserializeObject<List<WordModel>>(jsonString);
            }
            foreach(WordModel word in _words)
            {
                CategoryList.Add(word.Category);
            }
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
            if (string.IsNullOrEmpty(_word))
            {
                _description = string.Empty;
                _imagePath = string.Empty;
                return;
            }
            foreach (WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    _description = word.Description;
                    _imagePath = word.ImagePath;
                    _category = word.Category;
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
    }
}