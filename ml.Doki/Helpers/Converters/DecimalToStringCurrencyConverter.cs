using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ml.Doki.Helpers.Converters
{
    public class DecimalToStringCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal input;
            if (!decimal.TryParse(value.ToString(), NumberStyles.Currency, CultureInfo.CurrentCulture, out input))
                return value;

            return $"{input.ToString("C", CultureInfo.CurrentCulture)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
