using Microsoft.Win32;
using NeqPass.Core;
using NeqPass.GUI.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace NeqPass.GUI.Pages
{
    public partial class SettingsPage : Page
    {
        private static Dictionary<string, TimeSpan> _durations = new Dictionary<string, TimeSpan>()
        {
            ["Не выбрано"] = TimeSpan.MaxValue,
            ["Сразу после сворачивания"] = TimeSpan.Zero,
            ["5 секунд"] = TimeSpan.FromSeconds(5),
            ["30 секунд"] = TimeSpan.FromSeconds(30),
            ["1 минута"] = TimeSpan.FromMinutes(1),
            ["5 минут"] = TimeSpan.FromMinutes(5),
            ["10 минут"] = TimeSpan.FromMinutes(10),
            ["30 минут"] = TimeSpan.FromMinutes(30),
            ["1 час"] = TimeSpan.FromHours(1),
        };

        public SettingsPage(ObservableCollection<EntryModel> entries)
        {
            InitializeComponent();

            checkBoxMinimize.IsChecked = Settings.Current.OpenInMinimized;
            checkBoxOpenSaved.IsChecked = Settings.Current.SavePath;
            textSavedPath.Text = Settings.Current.SavedPath;

            checkBoxAutoLock.IsChecked = Settings.Current.AutoBlock;


            checkBoxMinimize.Checked += (s, e) => { Settings.Current.OpenInMinimized = true; Settings.Current.Save(Settings.SettingsFileName); };
            checkBoxMinimize.Unchecked += (s, e) => { Settings.Current.OpenInMinimized = false; Settings.Current.Save(Settings.SettingsFileName); };

            checkBoxOpenSaved.Checked += (s, e) => { Settings.Current.SavePath = true; Settings.Current.Save(Settings.SettingsFileName); };
            checkBoxOpenSaved.Unchecked += (s, e) => { Settings.Current.SavePath = false; Settings.Current.Save(Settings.SettingsFileName); };

            textSavedPath.TextChanged += (s, e) =>
            {
                Settings.Current.SavedPath = textSavedPath.Text.Replace("\"", string.Empty);
                Settings.Current.Save(Settings.SettingsFileName);
            };

            checkBoxAutoLock.Checked += (s, e) => { Settings.Current.AutoBlock = true; Settings.Current.Save(Settings.SettingsFileName); };
            checkBoxAutoLock.Unchecked += (s, e) => { Settings.Current.AutoBlock = false; Settings.Current.Save(Settings.SettingsFileName); };

            foreach (var item in _durations)
            {
                comboBoxAutoLockDuration.Items.Add(item.Key);
            }

            var duration = _durations.FirstOrDefault(x => x.Value == Settings.Current.AutoBlockDuration);
            comboBoxAutoLockDuration.SelectedItem = duration.Key;

            if (comboBoxAutoLockDuration.SelectedIndex == -1)
            {
                string name = $"{Settings.Current.AutoBlockDuration.TotalSeconds} секунд-(ы)";

                _durations.Add(name, Settings.Current.AutoBlockDuration);
                comboBoxAutoLockDuration.Items.Add(name);
                comboBoxAutoLockDuration.SelectedItem = name;
            }

            comboBoxAutoLockDuration.SelectionChanged += (s, e) =>
            {
                if (comboBoxAutoLockDuration.SelectedIndex == -1)
                    comboBoxAutoLockDuration.SelectedItem = _durations.FirstOrDefault(x => x.Value == TimeSpan.MaxValue).Key;

                Settings.Current.AutoBlockDuration = _durations.FirstOrDefault(x => x.Key == ((string)comboBoxAutoLockDuration.SelectedItem)).Value;
                Settings.Current.Save(Settings.SettingsFileName);
            };

            textBy.MouseLeftButtonDown += (s, e) => Process.Start("https://youtu.be/dQw4w9WgXcQ");
            textBy.MouseRightButtonDown += (s, e) =>
            {
                ShowInfo("Укажите, куда сохранить незашифрованный файл");
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    Title = "Выберите место сохранения файла",
                    DefaultExt = "json",
                    Filter = "Json Files (*.json)|*.json|all files (*.*)|*.*",
                };

                if (saveFile.ShowDialog() == true)
                {
                    File.WriteAllText(saveFile.FileName, JsonConvert.SerializeObject(ModelConverter.ConvertToSave(entries), Formatting.Indented));
                }

                ShowInfo("Незашифрованный файл сохранен");
            };
            textBy.MouseWheel += (s, e) =>
            {
                if (e.Delta > 0)
                {
                    ShowInfo("Выберите файл, который необходимо зашифровать");
                    var window = new PasswordWindow(PasswordWindowType.Encrypt);
                    window.ShowFileDialog();
                }
                else
                {
                    ShowInfo("Выберите файл, который необходимо расшифровать");
                    var window = new PasswordWindow(PasswordWindowType.Decrypt);
                    window.ShowFileDialog();
                }
                
            };

            textVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void ShowInfo(string message, string caption = "Информация")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowError(string message, string caption = "Ошибка!")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
