using Microsoft.Win32;
using NeqPass.Core;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace NeqPass.GUI
{
    public enum PasswordWindowType
    {
        Encrypt,
        Decrypt
    }

    public partial class PasswordWindow : Window
    {
        private string _fileName;
        private PasswordWindowType _windowType;
        private const string _extension = ".np";

        public PasswordWindow(PasswordWindowType windowType)
        {
            InitializeComponent();
            _windowType = windowType;
            buttonApply.Click += Apply;
            closeButton.Click += (s, e) => Close();
            minimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            textPassword.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Return || e.Key == Key.Enter)
                    Apply(null, null);
            };
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textPassword.Text))
                return;

            try
            {
                if (_windowType == PasswordWindowType.Decrypt)
                {
                    string fileName = _fileName.Replace(_extension, string.Empty);

                    if (File.Exists(fileName))
                    {
                        if (MessageBox.Show("Такой файл уже существует. Перезаписать?", "Уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                            return;
                    }

                    File.WriteAllBytes(fileName, EntrySaver.Decrypt(File.ReadAllBytes(_fileName), textPassword.Text));
                    ShowInfo("Файл расшифрован!");
                }
                else if (_windowType == PasswordWindowType.Encrypt)
                {
                    string fileName = _fileName + _extension;

                    if (File.Exists(fileName))
                    {
                        if (MessageBox.Show("Такой файл уже существует. Перезаписать?", "Уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                            return;
                    }

                    File.WriteAllBytes(fileName, EntrySaver.Encrypt(File.ReadAllBytes(_fileName), textPassword.Text));
                    ShowInfo("Файл зашифрован!");
                }

                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        public void ShowFileDialog()
        {
            if (_windowType == PasswordWindowType.Encrypt)
            {
                OpenFileDialog openFile = new OpenFileDialog
                {
                    Title = "Выберите файл, который необходимо зашифровать",
                    Filter = "all files (*.*)|*.*",
                };

                if (openFile.ShowDialog() == true)
                {
                    _fileName = openFile.FileName;
                    Show();
                }
            }
            else if (_windowType == PasswordWindowType.Decrypt)
            {
                OpenFileDialog openFile = new OpenFileDialog
                {
                    Title = "Выберите файл, который необходимо расшифровать",
                    Filter = "NeqPass files (*.np) |*.np| all files (*.*)|*.*",
                };

                if (openFile.ShowDialog() == true)
                {
                    _fileName = openFile.FileName;
                    Show();
                }
            }
        }

        private void ShowInfo(string message, string caption = "Успешно!")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowError(string message, string caption = "Ошибка!")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
