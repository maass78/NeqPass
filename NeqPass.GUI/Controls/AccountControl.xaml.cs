using System.Windows;
using System.Windows.Controls;

namespace NeqPass.GUI.Controls
{
    public partial class AccountControl : UserControl
    {
        public event AccountChangedEventHaldler AccountDeleted;

        public AccountControl(AccountModel accountModel)
        {
            InitializeComponent();

            textBoxLogin.Text = accountModel.Login;
            textBoxPassword.Text = accountModel.Password;
            textBoxComment.Text = accountModel.Comment;

            checkBoxEditLogin.Checked += (s, e) => accountModel.Login = textBoxLogin.Text;
            checkBoxEditPassword.Checked += (s, e) => accountModel.Password = textBoxPassword.Text;
            checkBoxEditComment.Checked += (s, e) => accountModel.Comment = textBoxComment.Text;

            ContextMenu cm = FindResource("ActionsContextMenu") as ContextMenu;

            buttonMore.Click += (s, e) =>
            {
                if (!cm.IsOpen)
                {
                    cm.PlacementTarget = s as StackPanel;
                    cm.IsOpen = true;
                }
            };

            (cm.Items[0] as MenuItem).Click += (s, e) =>
            {
                if (MessageBox.Show("Точно удалить?", "Вы уверены?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    AccountDeleted?.Invoke(this, new AccountChangedEventArgs(accountModel));
                }
            };

            (cm.Items[1] as MenuItem).Click += (s, e) =>
            {
                Clipboard.SetText($"{accountModel.Login}:{accountModel.Password}");
            };

            buttonCopyLogin.Click += (s, e) => Clipboard.SetText(accountModel.Login);
            buttonCopyPassword.Click += (s, e) => Clipboard.SetText(accountModel.Password);
        }
    }

    public delegate void AccountChangedEventHaldler(object sender, AccountChangedEventArgs eventArgs);

    public class AccountChangedEventArgs 
    {
        public AccountModel Account { get; }

        public AccountChangedEventArgs(AccountModel account)
        {
            Account = account;
        }
    }
}
