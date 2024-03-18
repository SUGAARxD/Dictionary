using DictionaryApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DictionaryApp.ViewModel
{
    internal class AdministratorVM : BaseVM
    {
        private static readonly string _filePath = @"..\..\resources\Database\words.json";

        private List<WordModel> _words;
        public ObservableCollection<string> Words { get; set; }

        public AdministratorVM()
        {
            _words = new List<WordModel>();
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _words = JsonConvert.DeserializeObject<List<WordModel>>(jsonString);
            }

            Words = new ObservableCollection<string>();
            foreach (WordModel word in _words)
            {
                Words.Add(word.Text);
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
        }

        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;

                UpdateSuggestions();

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
                NotifyPropertyChanged(nameof(Category));
            }
        }
        public string SelectedListBoxItem
        {
            set
            {
                _word = value;
                NotifyPropertyChanged(nameof(Word));
            }
        }


        private ICommand _updateWordCommand;
        public ICommand UpdateWordCommand
        {
            get
            {
                if (_updateWordCommand == null)
                    _updateWordCommand = new RelayCommand(ExecuteUpdateWord, CanUpdateWord);
                return _updateWordCommand;
            }
        }

        private void ExecuteUpdateWord(object parameter)
        {

        }
        private bool CanUpdateWord(object parameter)
        {
            return true;
        }

        private ICommand _addWordCommand;
        public ICommand AddWordCommand
        {
            get
            {
                if (_addWordCommand == null)
                    _addWordCommand = new RelayCommand(ExecuteAddWord, CanAddWord);
                return _addWordCommand;
            }
        }
        private void ExecuteAddWord(object parameter)
        {
            WordModel newWord = new WordModel(_word, _description, _category, _imagePath);
            _words.Add(newWord);
            Words.Add(_word);
            LoadWordsToJson();
            if (!CategoryList.Contains(_category))
                CategoryList.Add(_category);
        }
        private bool CanAddWord(object parameter)
        {
            return !string.IsNullOrEmpty(_word)
                && !string.IsNullOrEmpty(_category)
                && !string.IsNullOrEmpty(_description)
                && !string.IsNullOrEmpty(_imagePath)
                && _words.Select(item => item.Text).Where(item => item.Equals(_word)).Count() == 0;
        }

        private ICommand _deleteWordCommand;
        public ICommand DeleteWordCommand
        {
            get
            {
                if (_deleteWordCommand == null)
                    _deleteWordCommand = new RelayCommand(ExecuteDeleteWord);
                return _deleteWordCommand;
            }
        }
        private void ExecuteDeleteWord(object parameter)
        {

        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(ExecuteClear);
                return _clearCommand;
            }
        }
        private void ExecuteClear(object parameter)
        {
            Word = string.Empty;
            Category = string.Empty;
            Description = string.Empty;
            ImageSource = null;
        }

        private ICommand _addPhotoCommand;
        public ICommand AddPhotoCommand
        {
            get
            {
                if (_addPhotoCommand == null)
                    _addPhotoCommand = new RelayCommand(ExecuteAddPhoto);
                return _addPhotoCommand;
            }
        }
        private void ExecuteAddPhoto(object parameter)
        {

        }
        private ICommand _removePhotoCommand;
        public ICommand RemovePhotoCommand
        {
            get
            {
                if (_removePhotoCommand == null)
                    _removePhotoCommand = new RelayCommand(ExecuteRemovePhoto);
                return _removePhotoCommand;
            }
        }
        private void ExecuteRemovePhoto(object parameter)
        {
            
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
                Category = string.Empty;
                Description = string.Empty;
                ImageSource = null;
                return;
            }
            foreach (WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    Category = word.Category;
                    Description = word.Description;
                    ImageSource = new BitmapImage(new Uri(word.ImagePath.Replace(@"\\", @"\"), UriKind.RelativeOrAbsolute));
                    break;
                }
            }
        }
        private void UpdateSuggestions()
        {
            Words.Clear();
            if (string.IsNullOrEmpty(_word))
            {
                foreach (WordModel word in _words)
                {
                    Words.Add(word.Text);
                }
                return;
            }

            _words.Where(item => item.Text.ToLower().StartsWith(_word.ToLower())
                 && (item.Category.Equals(_category) || string.IsNullOrEmpty(_category)))
                 .Select(item => item.Text)
                 .ToList()
                 .ForEach(item =>
                 {
                     Words.Add(item);
                 });
        }
        public ObservableCollection<string> CategoryList { get; set; }
        private void LoadWordsToJson()
        {

        }
    }
}
