using Microsoft.Win32;
using NeqPass.Core;
using NeqPass.GUI.Controls;
using NeqPass.GUI.Pages;
using NeqPass.GUI.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace NeqPass.GUI
{
    public partial class MainWindow : Window
    {
        private bool _pageShowed => frame.Content != null;
        private string _fileName;
        private string _password;

        public ObservableCollection<EntryModel> Entries { get; set; }

        public MainWindow()
        {
            Settings.Current = Settings.LoadFromFile(Settings.SettingsFileName);

            InitializeComponent();
            Load();
            new WindowResizer(this);
            #region Events
            closeButton.Click += (s, e) => Close();
            minimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            maximizeButton.Click += (s, e) => WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            KeyDown += OnKeyDown;
            list.SelectionChanged += OnSelectionChanged;

            buttonAddAccount.Click += OnAddAccount;
            buttonAddEntry.Click += OnAddEntry;
            buttonDeleteEntry.Click += OnDeleteEntry;

            textSearch.SizeChanged += (s, e) => textSearch.Visibility = textSearch.ActualWidth <= 35 ? Visibility.Hidden : Visibility.Visible;

            buttonEditEntry.Click += (s, e) => ShowPage(new EditEntryPage((EntryModel)list.SelectedItem, false));
            buttonHidePage.Click += (s, e) => ShowPage(null);

            buttonCreate.Click += (s, e) => { buttonHidePage.Visibility = Visibility.Collapsed; CreateNewFile(false); };
            buttonOpen.Click += (s, e) => 
            { 
                buttonHidePage.Visibility = Visibility.Collapsed;
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "NeqPass Files (*.np)|*.np|all files (*.*)|*.*",
                    Title = "Выберите файл"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    OpenFile(openFileDialog.FileName, false);
                }
            };
            
            buttonSettings.Click += (s, e) => ShowPage(new SettingsPage());

            buttonLock.Click += (s, e) =>
            {
                buttonHidePage.Visibility = Visibility.Collapsed;

                DataContext = null;
                Entries = null;
                _password = null;

                OpenFile(_fileName, false);
            };
            #endregion

            if (Settings.Current.OpenInMinimized)
                mainGrid.ColumnDefinitions[0].Width = new GridLength(45, GridUnitType.Pixel);
        }

        private void Load()
        {
            buttonHidePage.Visibility = Visibility.Collapsed;

            if (Settings.Current.SavePath)
                OpenFile(Settings.Current.SavedPath, false);
            else
                ShowStartPage();
        }

        private void ShowStartPage()
        {
            var startPage = new StartPage();
            startPage.CreateClicked += () => CreateNewFile(startPage.SavePath);

            startPage.OpenClicked += () =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "NeqPass Files (*.np)|*.np|all files (*.*)|*.*",
                    Title = "Выберите файл"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    OpenFile(openFileDialog.FileName, startPage.SavePath);
                }
            };

            ShowPage(startPage);
        }

        private void CreateNewFile(bool savePath)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Выберите место сохранения файла",
                DefaultExt = "np",
                Filter = "NeqPass Files (*.np)|*.np|all files (*.*)|*.*",
            };

            saveFile.FileName = "passwords.np";
            if (saveFile.ShowDialog() == true)
            {
                var passwordPage = new EnterPasswordPage(saveFile.FileName, true);
                passwordPage.BackClicked += () => ShowStartPage();

                passwordPage.ConfrimClicked += (pass) =>
                {
                    _fileName = saveFile.FileName;
                    _password = pass;
                    Entries = new ObservableCollection<EntryModel>();
                    DataContext = this;
                    Entries.CollectionChanged += (a, b) => Save();
                    
                    if (savePath)
                    {
                        Settings.Current.SavePath = true;
                        Settings.Current.SavedPath = saveFile.FileName;
                        Settings.Current.Save(Settings.SettingsFileName);
                    }

                    Save();
                    ShowPage(null);
                    buttonHidePage.Visibility = Visibility.Visible;
                };

                ShowPage(passwordPage);
            }
        }

        private void OpenFile(string fileName, bool savePath)
        {
            if (!File.Exists(fileName))
            {
                ShowStartPage();
            }

            var passwordPage = new EnterPasswordPage(fileName, false);

            passwordPage.BackClicked += () => ShowStartPage();

            passwordPage.ConfrimClicked += (pass) =>
            {
                if (EntrySaver.Load(fileName, pass, out var entries))
                {
                    _password = pass;
                    _fileName = fileName;
                    Entries = ModelConverter.Convert(entries);
                    DataContext = this;
                    Entries.CollectionChanged += (a, b) => Save();
                    ShowPage(null);
                    buttonHidePage.Visibility = Visibility.Visible;

                    if (savePath)
                    {
                        Settings.Current.SavePath = true;
                        Settings.Current.SavedPath = fileName;
                        Settings.Current.Save(Settings.SettingsFileName);
                    }
                }
                else
                {
                    passwordPage.Clear();
                    ShowError("Неверный пароль!");
                }
            };

            ShowPage(passwordPage);
        }

        private void OnDeleteEntry(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Точно удалить?", "Вы уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    List<EntryModel> entries = new List<EntryModel>();

                    foreach (EntryModel element in list.SelectedItems)
                        entries.Add(element);

                    foreach (var entry in entries)
                        Entries.Remove(entry);
                }
            }
        }

        private void OnAddAccount(object sender, RoutedEventArgs e)
        {
            var acc = new AccountModel();
            acc.NeedSave += () => Save();

            EntryModel selected = (EntryModel)list.SelectedItem;
            selected.Accounts.Add(acc);

            var control = new AccountControl(acc);

            control.AccountDeleted += (a, b) =>
            {
                stackAccounts.Children.Remove(control);
                selected.Accounts.Remove(acc);
            };

            stackAccounts.Children.Add(control);
        }
        private void OnAddEntry(object sender, RoutedEventArgs e)
        {
            var entry = new EntryModel()
            {
                Name = "",
                Accounts = new ObservableCollection<AccountModel>(),
            };

            var page = new EditEntryPage(entry, true);
            page.Closed += () =>
            {
                ShowPage(null);
                Entries.Add(entry);
                entry.NeedSave += () => Save();
            };
            ShowPage(page);
        }

        private void RefreshAccounts(IList<AccountModel> accounts)
        {
            stackAccounts.Children.Clear();

            foreach (var acc in accounts)
            {
                var control = new AccountControl(acc);

                control.AccountDeleted += (s, e) =>
                {
                    stackAccounts.Children.Remove(control);
                    accounts.Remove(acc);
                };

                stackAccounts.Children.Add(control);
            }
        }
    
        private void ShowPage(Page page)
        {
            if (page == null)
            {
                gridPage.Visibility = Visibility.Collapsed;
                
                gridEntries.Effect = null;
                gridSelected.Effect = null;
                borderSelect.Effect = null;
                borderMainButtons.Effect = null;

                frame.Content = null;

                gridEntries.OpacityMask = null;
                gridSelected.OpacityMask = null;
                borderSelect.OpacityMask = null;
                borderMainButtons.OpacityMask = null;
          
                return;
            }

            frame.Content = page;
            gridPage.Visibility = Visibility.Visible;

            gridEntries.OpacityMask = new SolidColorBrush() { Color = Color.FromArgb(50, 0, 0, 0) };
            gridSelected.OpacityMask = new SolidColorBrush() { Color = Color.FromArgb(50, 0, 0, 0) };
            borderSelect.OpacityMask = new SolidColorBrush() { Color = Color.FromArgb(50, 0, 0, 0) };
            borderMainButtons.OpacityMask = new SolidColorBrush() { Color = Color.FromArgb(50, 0, 0, 0) };

            gridEntries.Effect = new BlurEffect() { KernelType = KernelType.Gaussian };
            gridSelected.Effect = new BlurEffect() { KernelType = KernelType.Gaussian };
            borderSelect.Effect = new BlurEffect() { KernelType = KernelType.Gaussian };
            borderMainButtons.Effect = new BlurEffect() { KernelType = KernelType.Gaussian };
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null)
            {
                borderSelect.Visibility = Visibility.Visible;
                gridSelected.Visibility = Visibility.Collapsed;
            }
            else
            {
                EntryModel entry = (EntryModel)list.SelectedItem;

                textSelectName.Text = entry.Name;

                entry.PropertyChanged += (a, b) =>
                {
                    if (b.PropertyName == nameof(EntryModel.Name))
                        textSelectName.Text = entry.Name;
                };

                RefreshAccounts(entry.Accounts);

                borderSelect.Visibility = Visibility.Collapsed;
                gridSelected.Visibility = Visibility.Visible;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    if (_pageShowed && buttonHidePage.Visibility == Visibility.Visible)
                        ShowPage(null);
                    else
                        list.SelectedItem = null;

                }

                if (list.SelectedItem != null && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (e.Key == Key.C)
                    {
                        var accs = (list.SelectedItem as EntryModel).Accounts;

                        if(accs.Count > 0 && accs[0] != null)
                            Clipboard.SetText(accs[0].Password);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Save()
        {
            EntrySaver.Save(_fileName, ModelConverter.ConvertToSave(Entries), _password);
            //MessageBox.Show("Saving...");
        }
    }
}
