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
        private List<int> _wordsPositions;
        private int _wordsCounter;
        private static readonly string _filePath = @"..\..\resources\Database\words.json";
        private  List<WordModel> _words;
        public GameVM()
        {
            _words = new List<WordModel>();
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _words = JsonConvert.DeserializeObject<List<WordModel>>(jsonString);
            }
            _wordsPositions = new List<int>();
            Reset();
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

        private string _nextWord;
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
            foreach(int number in uniqueNumbers)
            {
                _wordsPositions.Add(number); 
            }
            _wordsCounter = 0;
            IsTextBoxEnabled = false;
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
                if (random.Next(42069) % 2 == 0)
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
    }
}
