
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AmeisenBotX.Utils
{
    public static partial class ImageExtensions
    {
        [LibraryImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            if (bitmap == null) return null;

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                wpfBitmap.Freeze();

                return wpfBitmap;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }
    }
}
