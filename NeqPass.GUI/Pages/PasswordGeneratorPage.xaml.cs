using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NeqPass.GUI.Pages
{
    public partial class PasswordGeneratorPage : Page
    {
        public PasswordGeneratorPage()
        {
            var rngCrypto = new RNGCryptoServiceProvider();

            InitializeComponent();
            buttonGenerate.Click += (s, e) =>
            {
                textPassword.Text = GetRandomString(10);
            };

            buttonCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(textPassword.Text))
                    Clipboard.SetText(textPassword.Text);
            };
        }

        private static string GetRandomString(int length)
        {
            const string dictionary = "!.,;@%$#*abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder sb = new StringBuilder();
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    random.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    sb.Append(dictionary[(int)(num % (uint)dictionary.Length)]);
                }
            }

            return sb.ToString();
        }
    }
}
