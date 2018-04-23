using System;
using Xamarin.Forms;
using System.Globalization;

namespace CustomEditFormDemo {
    class Int32ToStringConverter : IValueConverter {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            int result = 0;
            try {
                result = Int32.Parse(value as String);
            } catch {
                result = 0;
            }

            return result;
        }
    }

	class Int32ToPriceStringConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return "$" + value.ToString();
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			int result = 0;
			try {
				result = Int32.Parse(value as String);
			} catch {
				result = 0;
			}

			return result;
		}
	}

	class Int32ToDoubleConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Convert.ToDouble(value);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return Convert.ToInt32(value);
		}
	}

	class DoubleToInt32Converter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Convert.ToInt32(value);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return Convert.ToDouble(value);
		}
	}
}