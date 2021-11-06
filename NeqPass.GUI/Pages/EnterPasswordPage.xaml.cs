using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeqPass.GUI.Pages
{
    public partial class EnterPasswordPage : Page
    {
        public event Action<string> ConfrimClicked;
        public event Action BackClicked;

        public EnterPasswordPage(string fileName, bool isNew)
        {
            InitializeComponent();

            buttonAccept.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(textPassword.Text))
                    ConfrimClicked?.Invoke(textPassword.Text);
            };

            textPassword.KeyDown += OnKeyDown;
            //KeyDown += OnKeyDown;

            buttonBack.Click += (s, e) => BackClicked?.Invoke();

            if (!isNew)
                textTitle.Text += Path.GetFileName(fileName);
            else
                textTitle.Text = "Введите пароль для новой базы данных";
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return || e.Key == Key.Enter))
            {
                ConfrimClicked?.Invoke(textPassword.Text);
            }
        }

        public void Clear()
        {
            textPassword.Text = string.Empty;
        }
    }
}
