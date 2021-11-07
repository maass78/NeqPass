using System;
using System.Windows;
using System.Windows.Controls;
using UsefulExtensions;

namespace NeqPass.GUI.Pages
{
    public partial class PasswordGeneratorPage : Page
    {
        private static readonly Random _random = new Random();
        private static readonly string[] _specialSymbols = new string[]
        {
            "!",
            ".",
            ",",
            ";",
            "@",
            "%",
            "$",
            "#",
            "*",
        };

        public PasswordGeneratorPage()
        {
            InitializeComponent();
            buttonGenerate.Click += (s, e) =>
            {
                string result = RandomStringGenerator.AllSymbolsGenerator.Generate(10);

                result = result.Insert(_random.Next(0, result.Length), _specialSymbols[_random.Next(_specialSymbols.Length)]);

                textPassword.Text = result;
            };

            buttonCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(textPassword.Text))
                    Clipboard.SetText(textPassword.Text);
            };
        }
    }
}
