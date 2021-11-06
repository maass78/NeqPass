using System;
using System.Windows.Controls;

namespace NeqPass.GUI.Pages
{
    public partial class StartPage : Page
    {
        public event Action CreateClicked;
        public event Action OpenClicked;
        public bool SavePath => checkBoxSavePath.IsChecked == true ? true : false;

        public StartPage()
        {
            InitializeComponent();

            buttonCreate.Click += (s, e) => CreateClicked?.Invoke();
            buttonOpen.Click += (s, e) => OpenClicked?.Invoke();
        }
    }
}
