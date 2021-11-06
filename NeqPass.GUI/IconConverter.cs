using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace NeqPass.GUI
{
    class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string base64 = (string)value;

            if (string.IsNullOrWhiteSpace(base64))
                return null;

            byte[] bytes;

            try
            {
                bytes = System.Convert.FromBase64String(base64);
            }
            catch
            {
                return null;
            }

            try
            {
                var image = new BitmapImage();

                using (MemoryStream iconStream = new MemoryStream(bytes))
                {
                    iconStream.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = iconStream;
                    image.EndInit();

                };
                image.Freeze();
                return image;
            }
            catch 
            {
                return null;
            }
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
