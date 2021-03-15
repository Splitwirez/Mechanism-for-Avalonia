using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using MemoryStream = System.IO.MemoryStream;
using System.Text;
using Avalonia.Controls.Presenters;

namespace Mechanism.AvaloniaUI.Core
{
    public class AttachedIcon
    {
        /*[Obsolete]
        public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.RegisterAttached<AttachedIcon, Control, object>("Icon", null);

        [Obsolete]
        public static object GetIcon(IAvaloniaObject obj)
        {
            return obj.GetValue(IconProperty);
        }

        [Obsolete]
        public static void SetIcon(IAvaloniaObject obj, object value)
        {
            obj.SetValue(IconProperty, value);
        }*/

        public static readonly StyledProperty<object> IconProperty =
            AvaloniaProperty.RegisterAttached<AttachedIcon, IAvaloniaObject, object>("Icon", null);

        public static object GetIcon(IAvaloniaObject obj)
        {
            return obj.GetValue(IconProperty);
        }

        public static void SetIcon(IAvaloniaObject obj, object value)
        {
            obj.SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<double> IconGapProperty = AvaloniaProperty.RegisterAttached<AttachedIcon, IAvaloniaObject, double>("IconGap", 0.0);

        public static double GetIconGap(IAvaloniaObject obj)
        {
            return obj.GetValue(IconGapProperty);
        }

        public static void SetIconGap(IAvaloniaObject obj, double value)
        {
            obj.SetValue(IconGapProperty, value);
        }
    }

    public class ValueToIsNullBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectToTemplateConverter : IValueConverter
    {
        static readonly string EXPLICIT_SIZE_PREFIX = "Size(";
        static readonly string EXPLICIT_SIZE_SUFFIX = ");";
        static readonly char EXPLICIT_SIZE_SEPARATOR = ',';
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IControlTemplate ctrlTemplate)
                return ctrlTemplate;
            else if (value is IControl ctrl)
            {
                ctrl.Measure(Size.Infinity);
                Thickness ctrlMargin = ctrl.Margin;
                double ctrlWidth = ctrl.DesiredSize.Width + ctrlMargin.Left + ctrlMargin.Right;
                double ctrlHeight = ctrl.DesiredSize.Height + ctrlMargin.Top + ctrlMargin.Bottom;
                
                VisualBrush visBrush = new VisualBrush(ctrl);
                return new FuncControlTemplate((tctrl, namescope) => new Rectangle()
                {
                    Width = ctrlWidth,
                    Height = ctrlHeight,
                    Fill = visBrush
                });
            }
            else if (value is Geometry geom)
            {
                return new FuncControlTemplate((tctrl, namescope) => 
                {
                    var path = new Path()
                    {
                        Data = geom
                    };
                    path[!Path.FillProperty] = new TemplateBinding(TextBlock.ForegroundProperty);
                    return path;
                });
            }
            else if (value is Drawing draw)
            {
                DrawingImage drImage = new DrawingImage()
                {
                    Drawing = draw
                };
                return new FuncControlTemplate((tctrl, namescope) => new Image()
                {
                    Source = drImage,
                    Width = drImage.Size.Width,
                    Height = drImage.Size.Height
                });
            }
            else if (value is string str)
            {
                string inStr = str;
                bool explicitSize = inStr.StartsWith(EXPLICIT_SIZE_PREFIX);
                double explicitWidth = 0;
                double explicitHeight = 0;
                if (explicitSize)
                {
                    inStr = str.Substring(EXPLICIT_SIZE_PREFIX.Length);
                    int splitIndex = inStr.IndexOf(EXPLICIT_SIZE_SUFFIX);
                    string before = inStr.Substring(0, splitIndex).Replace(" ", string.Empty);
                    inStr = inStr.Substring(splitIndex + EXPLICIT_SIZE_SUFFIX.Length);

                    if (before.Contains(EXPLICIT_SIZE_SEPARATOR.ToString()))
                    {
                        string[] b4 = before.Split(EXPLICIT_SIZE_SEPARATOR);
                        explicitWidth = double.Parse(b4[0]);
                        explicitHeight = double.Parse(b4[1]);
                    }
                    else
                    {
                        explicitWidth = double.Parse(before);
                        explicitHeight = explicitWidth;
                    }

                    while (inStr.StartsWith(" "))
                        inStr = inStr.Substring(1);
                }


                try
                {
                    var stUri = new Uri(inStr, UriKind.Absolute);

                    var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
                    Bitmap bmp = null;
                    using (var stream = assetLoader.Open(stUri))
                    {
                        bmp = new Bitmap(stream);
                    }
                    if (!explicitSize)
                    {
                        explicitWidth = bmp.Size.Width;
                        explicitHeight = bmp.Size.Height;
                    }

                    return new FuncControlTemplate((tctrl, namescope) => new Image()
                    {
                        Source = bmp,
                        Width = explicitWidth,
                        Height = explicitHeight
                    });
                }
                catch
                {
                    try
                    {
                        var geome = PathGeometry.Parse(inStr);
                        return new FuncControlTemplate((tctrl, namescope) => 
                        {
                            var path = new Path()
                            {
                                Data = geome
                            };
                            if (explicitSize)
                            {
                                path.Width = explicitWidth;
                                path.Height = explicitHeight;
                                path.Stretch = Stretch.Fill;
                            }
                            path[!Path.FillProperty] = new TemplateBinding(TextBlock.ForegroundProperty);
                            return path;
                        });
                    }
                    catch { }
                }
            }
            else if (value is IImage img)
                return new FuncControlTemplate((tctrl, namescope) => new Image()
                {
                    Source = img
                });
            else if (value is Uri uri)
            {
                return new FuncControlTemplate((tctrl, namescope) => new Image()
                {
                    Source = new Bitmap(uri.ToString())
                });
            }
            else if (value is PathFigures fig)
            {
                return new FuncControlTemplate((tctrl, namescope) => 
                {
                    var path = new Path()
                    {
                        Data = new PathGeometry()

                        {
                            Figures = fig
                        }
                    };
                    path[!Path.FillProperty] = new TemplateBinding(TextBlock.ForegroundProperty);
                    return path;
                });
            }
            else if (value is PathFigure figure)
            {
                return new FuncControlTemplate((tctrl, namescope) => 
                {
                    var path = new Path()
                    {
                        Data = new PathGeometry()

                        {
                            Figures = { figure }
                        }
                    };
                    path[!Path.FillProperty] = new TemplateBinding(TextBlock.ForegroundProperty);
                    return path;
                });
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
