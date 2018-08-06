#region Usings

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#endregion Usings

namespace PlusLayerCreator
{
	#region Enums

	/// <summary>
	/// The concatenation operator values.
	/// </summary>
	public enum ConcatenationOperator
	{
		/// <summary>
		/// The and concatenation operator.
		/// </summary>
		AND,
		/// <summary>
		/// The or concatenation operator.
		/// </summary>
		OR
	}

	#endregion Enums

	[ValueConversion(typeof(string[]), typeof(string))]
	public sealed class InvertedMultiBoolConverter : IMultiValueConverter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the concatenation operator.
		/// </summary>
		/// <value>The concatenation operator.</value>
		public ConcatenationOperator ConcatenationOperator
		{
			get;
			set;
		}

		#endregion Properties

		#region Construction / Finalization

		/// <summary>
		/// Initializes a new instance of the <see cref="InvertedMultiBoolConverter"/> class.
		/// </summary>
		public InvertedMultiBoolConverter()
		{
			// set defaults
			ConcatenationOperator = ConcatenationOperator.AND;
		}

		#endregion Construction / Finalization

		#region IMultiValueConverter Members

		/// <summary>
		/// Converts source values to a value for the binding target. The data binding engine calls
		/// this method when it propagates the values from source bindings to the binding target.
		/// </summary>
		/// <param name="values">
		/// The array of values that the source bindings in the
		/// <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value
		/// <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source
		/// binding has no value to provide for conversion.
		/// </param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value.If the method returns null, the valid null value is used.A return
		/// value of <see cref="T:System.Windows.DependencyProperty"/>.
		/// <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the
		/// converter did not produce a value, and that the binding will use the
		/// <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or
		/// else will use the default value.A return value of
		/// <see cref="T:System.Windows.Data.Binding"/>.
		/// <see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does
		/// not transfer the value or use the
		/// <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
		/// </returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var result = (ConcatenationOperator == ConcatenationOperator.AND);

			if (values != null)
			{
				foreach (var boolValue in values.OfType<bool>())
				{
					//If OR concatenation, one true value needs to set all to true.
					if (!boolValue)
					{
						if (ConcatenationOperator == ConcatenationOperator.OR)
						{
							result = true;
							break;
						}
					}
					//If AND concatenation, one false value needs to set all to false.
					else
					{
						if (ConcatenationOperator == ConcatenationOperator.AND)
						{
							result = false;
							break;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Converts a binding target value to the source binding values.
		/// </summary>
		/// <param name="value">The value that the binding target produces.</param>
		/// <param name="targetTypes">
		/// The array of types to convert to. The array length indicates the number and types of
		/// values that are suggested for the method to return.
		/// </param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// An array of values that have been converted from the target value back to the source values.
		/// </returns>
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			var retval = new object[targetTypes.Length];
			for (var i = 0; i < targetTypes.Length; i++)
			{
				var type = targetTypes[i];
				try
				{
					retval[i] = System.Convert.ChangeType(value, type, culture);
				}
				catch (Exception)
				{
					retval[i] = type.IsValueType ? Activator.CreateInstance(type) : null;
				}
			}
			return retval;
		}

		#endregion IMultiValueConverter Members
	}
}