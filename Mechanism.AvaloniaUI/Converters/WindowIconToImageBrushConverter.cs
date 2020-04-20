using Avalonia;
using WindowIcon = Avalonia.Controls.WindowIcon;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using Icon = System.Drawing.Icon;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mechanism.AvaloniaUI.Converters
{
    public class WindowIconToImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var wIcon = value as WindowIcon;
                MemoryStream stream = new MemoryStream();
                //if (icon.PlatformImpl is IconImpl impl)
                wIcon.Save(stream);
                stream.Position = 0;
                try
                {
                    return new ImageBrush(new Bitmap(stream));
                }
                catch (ArgumentNullException ex)
                {
                    try
                    {

                        Icon icon = new Icon(stream);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        bmp.Save(stream, ImageFormat.Png);
                        return new ImageBrush(new Bitmap(stream));
                    }
                    catch (ArgumentException e)
                    {
                        Icon icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        Stream stream3 = new MemoryStream();
                        bmp.Save(stream3, ImageFormat.Png);
                        return new ImageBrush(new Bitmap(stream3));
                    }
                }
            }
            else
                return new ImageBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}