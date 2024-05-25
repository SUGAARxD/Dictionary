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
    internal class SearchVM : BaseNotify
    {

        public SearchVM()
        {
            IsListBoxVisible = Visibility.Hidden;


            _words = new List<WordModel>();
            ReadWordsFromFile();
            
            Suggestions = new ObservableCollection<string>();

            CategoryList = new ObservableCollection<string>();
            UpdateCategoryList();

            ResetValues();
        }

        #region Properties and members

        private List<WordModel> _words;
        public ObservableCollection<string> Suggestions { get; set; }
        public ObservableCollection<string> CategoryList { get; set; }

        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;
                
                UpdateSuggestions();

                IsListBoxVisible = (string.IsNullOrEmpty(value) || Suggestions.Count() == 0) ? Visibility.Hidden : Visibility.Visible;

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

        #endregion

        #region File

        private static readonly string _filePath = @"..\..\resources\Database\words.json";
        private void ReadWordsFromFile()
        {
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _words = JsonConvert.DeserializeObject<List<WordModel>>(jsonString);
            }
        }

        #endregion

        #region Commands

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
                ResetValues();
                return;
            }
            foreach (WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    Description = word.Description;
                    CategoryLabel = "Category: " + word.Category;
                    ChangeImage(word.ImagePath);
                    IsImageBorderVisible = Visibility.Visible;
                    break;
                }
            }
        }

        #endregion

        #region Image

        private void ChangeImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            ImageSource = bitmapImage;
        }

        #endregion

        #region Util

        private void UpdateCategoryList()
        {
            CategoryList.Clear();
            CategoryList.Add("");
            foreach (WordModel word in _words)
            {
                if (!CategoryList.Contains(word.Category))
                    CategoryList.Add(word.Category);
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

        private void ResetValues()
        {
            Word = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
            CategoryLabel = "Category: ";
            ImageSource = null;
            IsImageBorderVisible = Visibility.Hidden;
        }

        #endregion

    }
}