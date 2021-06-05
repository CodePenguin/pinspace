using GongSolutions.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Pinspaces.Shell.Converters
{
    public class ShellIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pidl = (IntPtr)value;
            var result = Shell32.SHGetFileInfo(pidl, 0, out var info,
                Marshal.SizeOf(typeof(SHFILEINFO)),
                SHGFI.ADDOVERLAYS | SHGFI.ICON |
                SHGFI.SHELLICONSIZE | SHGFI.PIDL);

            if (result == IntPtr.Zero || info.hIcon == IntPtr.Zero)
            {
                return null;
            }

            var imageSource = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            User32.DestroyIcon(info.hIcon);

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
