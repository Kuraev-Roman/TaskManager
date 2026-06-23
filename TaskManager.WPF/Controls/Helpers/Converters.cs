using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Core.Models;

namespace TaskManager.WPF.Controls.Helpers
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskStatus status)
                return status switch
                {
                    TaskStatus.Completed => new SolidColorBrush(Color.FromRgb(232, 245, 233)),
                    TaskStatus.InProgress => new SolidColorBrush(Color.FromRgb(227, 242, 253)),
                    _ => Brushes.White
                };
            return Brushes.White;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? "★" : "☆";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class OverdueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Brushes.Red : Brushes.Black;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority p)
                return p switch
                {
                    TaskPriority.High => new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                    TaskPriority.Medium => new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                    _ => new SolidColorBrush(Color.FromRgb(76, 175, 80))
                };
            return Brushes.Gray;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StatusToRussianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TaskStatus s ? s switch
            {
                TaskStatus.New => "Новая",
                TaskStatus.InProgress => "В процессе",
                TaskStatus.Completed => "Завершена",
                _ => value.ToString()!
            } : string.Empty;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriorityToRussianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TaskPriority p ? p switch
            {
                TaskPriority.Low => "Низкий",
                TaskPriority.Medium => "Средний",
                TaskPriority.High => "Высокий",
                _ => value.ToString()!
            } : string.Empty;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}