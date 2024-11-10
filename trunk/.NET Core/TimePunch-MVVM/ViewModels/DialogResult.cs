#if NETFRAMEWORK

using System;
using System.Windows;

namespace TimePunch.MVVM.ViewModels
{
    public class DialogResult
    {
        #region DialogResult

        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(DialogResult), new PropertyMetadata(default(bool?), OnDialogResultChanged));

        private static void OnDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wnd = d as Window;
            if (wnd == null)
                return;

            if (wnd.DialogResult == null)
                wnd.DialogResult = (bool?)e.NewValue;
        }

        public static bool? GetDialogResult(DependencyObject dp)
        {
            if (dp == null) throw new ArgumentNullException(nameof(dp));

            return (bool?)dp.GetValue(DialogResultProperty);
        }

        public static void SetDialogResult(DependencyObject dp, object value)
        {
            if (dp == null) throw new ArgumentNullException(nameof(dp));

            dp.SetValue(DialogResultProperty, value);
        }

        #endregion
    }
}

#endif