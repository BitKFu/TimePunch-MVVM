using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
using WinRT;

namespace TimePunchIntroPlayerWinUI3.Core
{
    public static class WinUIConversionUtil
    {
        public static void InitFileOpenPicker(FileOpenPicker picker)
        {
            if (Window.Current == null)
            {
                var initializeWithWindowWrapper = picker.As<IInitializeWithWindow>();
                var hwnd = GetActiveWindow();
                initializeWithWindowWrapper.Initialize(hwnd);
            }
        }


        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize([In] IntPtr hwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetActiveWindow();
    }
}
