
// Copyright (c) 2024 Trade Winds Studios (David Thielen)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text;
using TradeWindsBlazorDevEx.Utilities;

namespace TradeWindsBlazorDevEx.PageModels
{
	public class PhoneNumberPageModel
	{
		private string _nationalNumber;

		/// <summary>
		/// The country code.
		/// </summary>
		public int CountryCode { get; set; }

		/// <summary>
		/// The number except the country code. Can be formatted and/or have white space.
		/// </summary>
		public string NationalNumber
		{
			get => _nationalNumber;
			set => _nationalNumber = (value ?? string.Empty).Trim();
		}

		/// <summary>
		/// The phone number if E164 format. Does the best it can but only as good as the
		/// property values.
		/// </summary>
		public string E164
		{
			get
			{
				var number = new StringBuilder($"+{CountryCode}");
				if (!string.IsNullOrWhiteSpace(NationalNumber))
					number.Append(NationalNumber);
				return PhoneUtils.TrimPhone(number.ToString());
			}
		}

		/// <summary>
		/// The formatted phone number. This is the number in the form "+1 (303) 555-1212".
		/// Does the best it can with the property values.
		/// </summary>
		public string FormattedNumber => PhoneUtils.FormatPhoneNumber(E164)!;

		/// <summary>
		/// true if this is empty. false if the code and number are set.
		/// </summary>
		public bool IsEmpty => string.IsNullOrWhiteSpace(NationalNumber);

		public PhoneNumberPageModel(int? countryCode, string? nationalNumber)
		{
			CountryCode = countryCode ?? 1;
			_nationalNumber = nationalNumber ?? string.Empty;
		}

		public PhoneNumberPageModel(PhoneNumberPageModel phone)
		{
			CountryCode = phone.CountryCode;
			_nationalNumber = phone.NationalNumber;
		}

		public PhoneNumberPageModel()
		{
			CountryCode = 1;
			_nationalNumber = string.Empty;
		}

		protected bool Equals(PhoneNumberPageModel other)
		{
			return _nationalNumber == other._nationalNumber && CountryCode == other.CountryCode;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PhoneNumberPageModel)obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Combine(_nationalNumber, CountryCode);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{nameof(CountryCode)}: {CountryCode}, {nameof(NationalNumber)}: {NationalNumber}";
		}
	}
}
