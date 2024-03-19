using DictionaryApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DictionaryApp.ViewModel
{
    internal class GameVM : BaseVM
    {
        public GameVM()
        {
            _words = new List<WordModel>();
            ReadWordsFromFile();
            _wordsPositions = new List<int>();
            
            Reset();
        }

        #region Properties and members

        private List<WordModel> _words;
        private List<int> _wordsPositions;
        private int _wordsCounter;
        private string _nextWord;

        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;
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
        private string _buttonContent;
        public string ButtonContent
        {
            get => _buttonContent;
            private set
            {
                _buttonContent = value;
                NotifyPropertyChanged(nameof(ButtonContent));
            }
        }
        private bool _isTextBoxReadOnly;
        public bool IsTextBoxEnabled
        {
            get => _isTextBoxReadOnly;
            set
            {
                _isTextBoxReadOnly = value;
                NotifyPropertyChanged(nameof(IsTextBoxEnabled));
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

        private ICommand _buttonCommand;
        public ICommand ButtonCommand
        {
            get
            {
                if (_buttonCommand == null)
                    _buttonCommand = new RelayCommand(ExecuteGameLogic, CanExecute);
                return _buttonCommand;
            }
        }

        private void ExecuteGameLogic(object parameter)
        {

            if (ButtonContent.Equals("Finish"))
            {
                Reset();
                return;
            }

            

            if (ButtonContent.Equals("Start"))
            {
                NextWord();
                ButtonContent = "Next";
                IsTextBoxEnabled = true;
                return;
            }

            if (Word.ToLower().Equals(_nextWord.ToLower()))
            {
                MessageBox.Show("Correct");
            }
            else
            {
                MessageBox.Show("Incorrect");
            }


            Word = string.Empty;

            if (_wordsCounter == _words.Count || _wordsCounter == 5)
            {
                ImageSource = null;
                Description = string.Empty;
                ButtonContent = "Finish";
                IsTextBoxEnabled = false;
                return;
            }

            NextWord();
        }
        private bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_word) || ButtonContent.Equals("Start") || ButtonContent.Equals("Finish");
        }
        private void NextWord()
        {
            WordModel currentWord = _words[_wordsPositions[_wordsCounter]];

            _nextWord = currentWord.Text;
            if (currentWord.ImagePath == WordModel.DefaultImagePath)
            {
                ImageSource = null;
                Description = currentWord.Description;
            }
            else
            {
                Random random = new Random();
                if (random.Next(2) % 2 == 0)
                {
                    ImageSource = null;
                    Description = currentWord.Description;
                }
                else
                {
                    Description = string.Empty;
                    ChangeImage(currentWord.ImagePath);
                }
            }
            _wordsCounter++;
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

        private void Reset()
        {
            ButtonContent = "Start";
            Random random = new Random();
            HashSet<int> uniqueNumbers = new HashSet<int>();
            while (uniqueNumbers.Count < _words.Count)
            {
                int randomNumber = random.Next(_words.Count);
                uniqueNumbers.Add(randomNumber);
            }
            _wordsPositions.Clear();
            foreach(int number in uniqueNumbers)
            {
                _wordsPositions.Add(number); 
            }
            _wordsCounter = 0;
            IsTextBoxEnabled = false;
        }

        #endregion

    }
}
