using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TimePunchIntroPlayer
{
    public class AttachedProperties
    {
        #region DialogResult

        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(AttachedProperties), new PropertyMetadata(default(bool?), OnDialogResultChanged));

        private static void OnDialogResultChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is Window window))
                return;

            if (window.DialogResult == null && window.IsActive)
                window.DialogResult = (bool?)eventArgs.NewValue;
        }

        public static bool? GetDialogResult(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));

            return (bool?)dependencyObject.GetValue(DialogResultProperty);
        }

        public static void SetDialogResult(DependencyObject dependencyObject, object value)
        {
            if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));

            dependencyObject.SetValue(DialogResultProperty, value);
        }

        #endregion

        #region Focus

        public static readonly DependencyProperty TextFocusProperty =
            DependencyProperty.RegisterAttached("TextFocus", typeof (bool), typeof (AttachedProperties),
                new PropertyMetadata(false, OnTextFocusChanged));

        private static void OnTextFocusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is TextBoxBase textBoxBase))
                return;

            if ((bool)eventArgs.NewValue)
                textBoxBase.GotFocus += OnFocusTextContent;
            else
                textBoxBase.GotFocus -= OnFocusTextContent;
        }

        private static void OnFocusTextContent(object sender, RoutedEventArgs eventArgs)
        {
            TextBoxBase textBoxBase = sender as TextBoxBase;
            textBoxBase?.SelectAll();
        }

        public static bool GetTextFocus(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));
            return (bool) dependencyObject.GetValue(TextFocusProperty);
        }

        public static void SetTextFocus(DependencyObject dependencyObject, object value)
        {
            if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));
            dependencyObject.SetValue(TextFocusProperty, value);
        }

        #endregion
    }
}
