﻿using DictionaryApp.Model;
using Microsoft.Win32;
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
                if (!CategoryList.Contains(word.Category))
                    CategoryList.Add(word.Category);
            }
            _imagePath = WordModel.DefaultImagePath;
            ChangeImage(_imagePath.Replace(@"\\", @"\"));
            ChangeableCategory = string.Empty;
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
                ChangeableCategory = Category;
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
        private string _changeableWord;
        public string ChangeableWord
        {
            get => _changeableWord;
            set
            {
                _changeableWord = value;
                NotifyPropertyChanged(nameof(ChangeableWord));
            }
        }
        private string _changeableCategory;
        public string ChangeableCategory
        {
            get => _changeableCategory;
            set
            {
                _changeableCategory = value;
                NotifyPropertyChanged(nameof(ChangeableCategory));
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
            foreach (WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    Word.Replace(word.Text, _changeableWord);
                    word.Text = _changeableWord;
                    word.Category = _changeableCategory;
                    if (!CategoryList.Contains(_changeableCategory))
                        CategoryList.Add(_changeableCategory);
                    word.Description = _description;
                    word.ImagePath = _imagePath;
                    Words.Clear();
                    foreach (WordModel word1 in _words)
                    {
                        Words.Add(word1.Text);
                    }
                    UpdateJsonFile();
                    MessageBox.Show("Word updated successfully!");
                    break;
                }
            }
        }
        private bool CanUpdateWord(object parameter)
        {
            return !string.IsNullOrEmpty(_changeableWord)
                && !string.IsNullOrEmpty(_changeableCategory)
                && !string.IsNullOrEmpty(_description);
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
            _words.Add(new WordModel(_changeableWord, _description, _changeableCategory, _imagePath));
            Words.Clear();
            foreach (WordModel word in _words)
            {
                Words.Add(word.Text);
            }
            if (!CategoryList.Contains(_changeableCategory))
                CategoryList.Add(_changeableCategory);
            Clear();
            UpdateJsonFile();
            MessageBox.Show("Word added successfully!");
        }
        private bool CanAddWord(object parameter)
        {
            return !string.IsNullOrEmpty(_changeableWord)
                && !string.IsNullOrEmpty(_changeableCategory)
                && !string.IsNullOrEmpty(_description)
                && !Words.Contains(_changeableWord);
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
            foreach(WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    _words.Remove(word);
                    Word = string.Empty;
                    CategoryList.Clear();
                    CategoryList.Add("");
                    foreach (WordModel word1 in _words)
                    {
                        if (!CategoryList.Contains(word.Category))
                            CategoryList.Add(word.Category);
                    }
                    Clear();
                    UpdateJsonFile();
                    MessageBox.Show("Word deleted successfully!");
                    break;
                }
            }
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
            Clear();
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                string projectFolderPath = @"..\..\resources\Images";

                if (!Directory.Exists(projectFolderPath))
                {
                    MessageBox.Show("Error in loading photo. Images folder not found!");
                    return;
                }

                string fileName = Path.GetFileName(selectedFilePath);

                string destinationFilePath = Path.Combine(projectFolderPath, fileName);

                try
                {
                    if (!File.Exists(destinationFilePath))
                        File.Copy(selectedFilePath, destinationFilePath, true);

                    _imagePath = "..\\..\\resources\\Images\\" + fileName;

                    ChangeImage(destinationFilePath);
                    MessageBox.Show("Photo added successfully!");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
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
            _imagePath = WordModel.DefaultImagePath;
            ChangeImage(_imagePath.Replace(@"\\", @"\"));
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
            foreach (WordModel word in _words)
            {
                if (word.Text.Equals(_word))
                {
                    ChangeableWord = word.Text;
                    Category = word.Category;
                    Description = word.Description;
                    _imagePath = word.ImagePath;
                    ChangeImage(_imagePath.Replace(@"\\", @"\"));
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
        private void UpdateJsonFile()
        {
            string json = JsonConvert.SerializeObject(_words);
            File.WriteAllText(_filePath, json);
        }
        private void Clear()
        {
            ChangeableWord = string.Empty;
            Category = string.Empty;
            ChangeableCategory = string.Empty;
            Description = string.Empty;
            _imagePath = WordModel.DefaultImagePath;
            ChangeImage(_imagePath.Replace(@"\\", @"\"));
        }
        private void ChangeImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            ImageSource = bitmapImage;
        }
    }
}
