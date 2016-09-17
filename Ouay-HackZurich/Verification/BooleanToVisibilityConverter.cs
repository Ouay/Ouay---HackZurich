using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Ouay_HackZurich.Verification
{
  public class BooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return ((bool)value ? Visibility.Visible : Visibility.Collapsed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
