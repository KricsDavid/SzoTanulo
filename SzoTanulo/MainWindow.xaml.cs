using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SzoTanulo;

namespace SzoTanulo
{
    public partial class MainWindow : Window
    {
        private List<Word> _words;
        private Word _currentQuestion;
        private int _correctAnswers;

        public MainWindow()
        {
            InitializeComponent();
            LoadWords();
            StartQuiz();
        }

        private void LoadWords()
        {
            using (var context = new AppDbContext())
            {
                _words = context.Words.ToList();
            }
        }

        private void StartQuiz()
        {
            Random rand = new Random();
            _currentQuestion = _words[rand.Next(_words.Count)];
            QuestionText.Text = _currentQuestion.WordText;

            var options = new List<string> { _currentQuestion.Meaning };
            while (options.Count < 4)
            {
                var randomWord = _words[rand.Next(_words.Count)].Meaning;
                if (!options.Contains(randomWord))
                    options.Add(randomWord);
            }
            AnswerOptions.ItemsSource = options.OrderBy(x => rand.Next()).ToList();
        }

        private void OnAnswerClick(object sender, RoutedEventArgs e)
        {
            string? selectedAnswer = AnswerOptions.SelectedItem as string;
            if (selectedAnswer == _currentQuestion.Meaning)
            {
                MessageBox.Show("Helyes válasz!");
                _correctAnswers++;
                if (_correctAnswers >= 2)
                {
                    _currentQuestion.Learned = 1;
                    using (var context = new AppDbContext())
                    {
                        context.Words.Update(_currentQuestion);
                        context.SaveChanges();
                    }
                    _correctAnswers = 0; 
                }
            }
            else
            {
                MessageBox.Show($"Hibás válasz! A helyes válasz: {_currentQuestion.Meaning}");
            }
            StartQuiz(); 
        }
    }
}