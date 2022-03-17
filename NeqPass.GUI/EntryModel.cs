using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NeqPass.GUI
{
    public class EntryModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name 
        {
            get => _name;
            set { _name = value; Notify(nameof(Name)); }  
        }

        private string _siteUrl;
        public string SiteUrl
        {
            get => _siteUrl;
            set { _siteUrl = value; Notify(nameof(SiteUrl)); }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set { _comment = value; Notify(nameof(Comment)); }
        }

        private string _iconBase64;
        public string IconBase64
        {
            get => _iconBase64;
            set { _iconBase64 = value; Notify(nameof(IconBase64)); }
        }

        private ObservableCollection<AccountModel> _accounts;
        public ObservableCollection<AccountModel> Accounts
        {
            get => _accounts;
            set
            {
                _accounts = value;
                Notify(nameof(Accounts));

                if (_accounts != null)
                    _accounts.CollectionChanged += (s, e) => NeedSave?.Invoke();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action NeedSave;

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            NeedSave?.Invoke();
        }
    }

    public class AccountModel : INotifyPropertyChanged
    {
        private string _comment;
        public string Comment
        {
            get => _comment;
            set { _comment = value; Notify(nameof(Comment)); }
        }

        private string _login;
        public string Login
        {
            get => _login;
            set { _login = value; Notify(nameof(Login)); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; Notify(nameof(Password)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action NeedSave;

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            NeedSave?.Invoke();
        }
    }
}
