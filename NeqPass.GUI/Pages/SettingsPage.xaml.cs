using NeqPass.GUI.Types;
using System.Windows.Controls;

namespace NeqPass.GUI.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            checkBoxMinimize.IsChecked = Settings.Current.OpenInMinimized;
            checkBoxOpenSaved.IsChecked = Settings.Current.SavePath;
            textSavedPath.Text = Settings.Current.SavedPath;

            checkBoxMinimize.Checked += (s, e) => { Settings.Current.OpenInMinimized = true; Settings.Current.Save(Settings.SettingsFileName); };
            checkBoxMinimize.Unchecked += (s, e) => { Settings.Current.OpenInMinimized = false; Settings.Current.Save(Settings.SettingsFileName); };

            checkBoxOpenSaved.Checked += (s, e) => { Settings.Current.SavePath = true; Settings.Current.Save(Settings.SettingsFileName); };
            checkBoxOpenSaved.Unchecked += (s, e) => { Settings.Current.SavePath = false; Settings.Current.Save(Settings.SettingsFileName); };

            textSavedPath.TextChanged += (s, e) =>
            {
                Settings.Current.SavedPath = textSavedPath.Text.Replace("\"", string.Empty);
                Settings.Current.Save(Settings.SettingsFileName);
            };
        }
    }
}
