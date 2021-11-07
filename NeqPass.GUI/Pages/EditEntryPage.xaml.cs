using Microsoft.Win32;
using NeqPass.GUI.Utilities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NeqPass.GUI.Pages
{
    public partial class EditEntryPage : Page
    {
        private EntryModel _entry;
        public event Action Closed;

        public EditEntryPage(EntryModel entry, bool add)
        {
            _entry = entry;
            InitializeComponent();

            textBoxName.Text = _entry.Name;
            textBoxUrl.Text = _entry.SiteUrl;
            textBoxComment.Text = _entry.Comment;

            buttonSetIcon.Click += SetIcon;
            buttonSetIconFromSite.Click += SetIconFromSite;
            Subscribe(true);

            Loaded += (s, e) =>
            {
                if (add)
                {
                    buttonAccept.Visibility = Visibility.Visible;
                    checkBoxEditComment.IsChecked = checkBoxEditName.IsChecked = checkBoxEditUrl.IsChecked = false;
                    textBoxName.Focus();
                }
            };

            buttonAccept.Click += (s, e) => 
            {
                checkBoxEditComment.IsChecked = checkBoxEditName.IsChecked = checkBoxEditUrl.IsChecked = true; 
                Closed?.Invoke();
            };
        }

        private void Subscribe(bool value)
        {
            if (value)
            {
                checkBoxEditName.Checked += EditName;
                checkBoxEditUrl.Checked += EditUrl;
                checkBoxEditComment.Checked += EditComment;
            }
            else
            {
                checkBoxEditName.Checked -= EditName;
                checkBoxEditUrl.Checked -= EditUrl;
                checkBoxEditComment.Checked -= EditComment;
            }
        }

        private async void SetIconFromSite(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_entry.SiteUrl))
            {
                ShowError("Сначала введите url сайта!");
                return;
            }

            try
            {
                using (var wc = new TimeoutedWebClient())
                {
                    string domain;

                    if (_entry.SiteUrl.Contains("://"))
                        domain = new Uri(_entry.SiteUrl).Host;
                    else
                        domain = _entry.SiteUrl;

                    byte[] bytes = await wc.DownloadDataTaskAsync($"http://www.google.com/s2/favicons?domain={domain}");
                    _entry.IconBase64 = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Возникла ошибка!\n{ex.Message}");
            }
        }
        private void SetIcon(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Multiselect = false,
                    Title = "Выберите иконку",
                    Filter = "Изображения (*.png; *.jpg; *.jpeg; *.ico)|*.png;*.jpg;*.jpeg;*.ico"
                };

                if (openFileDialog.ShowDialog() == true)
                    _entry.IconBase64 = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName));
            }
            catch (Exception ex)
            {
                ShowError($"Возникла ошибка!\n{ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void EditName(object sender, RoutedEventArgs e)
        {
            _entry.Name = textBoxName.Text;
        }

        private void EditUrl(object sender, RoutedEventArgs e)
        {
            _entry.SiteUrl = textBoxUrl.Text;
        }

        private void EditComment(object sender, RoutedEventArgs e)
        {
            _entry.Comment = textBoxComment.Text;
        }
    }
}
