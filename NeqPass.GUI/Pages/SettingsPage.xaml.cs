using NeqPass.GUI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public SettingsPage()
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
        }
    }
}
