
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
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace TradeWindsBlazorDevEx.Utilities;

public static class PhoneUtils
{
	private static PhoneNumberUtil PhoneNumberUtil { get; }
	private static readonly Regex PhoneRegex = new Regex(@"^\+1 \d{3}-\d{3}-\d{4}.*");

	static PhoneUtils()
	{
		PhoneNumberUtil = PhoneNumberUtil.GetInstance();
	}

	/// <summary>
	/// Format an E-164 phone number into the standard international format. This will prepend
	/// "+1" to the passed in number if there is no country code.
	/// </summary>
	/// <param name="e164PhoneNumber">The E-164 phone number in the form "+13035551212"</param>
	/// <returns>The formatted phone number in the form "+1 (303) 555-1212".</returns>
	public static string? FormatPhoneNumber(string? e164PhoneNumber)
	{
		if (string.IsNullOrEmpty(e164PhoneNumber))
			return e164PhoneNumber;

		try
		{
			if (! e164PhoneNumber.StartsWith("+"))
				e164PhoneNumber = $"+1{e164PhoneNumber}";
			var phoneNumber = PhoneNumberUtil.Parse(e164PhoneNumber, null);
			var formattedNumber = PhoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
			// returns "+1 650-253-0000", maybe with an extension
			if (!PhoneRegex.IsMatch(formattedNumber))
				return formattedNumber;
			// Want "+1 (650) 253-0000"
			return $"+1 ({formattedNumber[3..6]}) {formattedNumber[7..]}";

		}
		catch (Exception)
		{
			return e164PhoneNumber;
		}
	}

	/// <summary>
	/// Format an E-164 phone number into the country code and standard local format.
	/// </summary>
	/// <param name="e164PhoneNumber">The E-164 phone number in the form "+13035551212"</param>
	/// <returns>The formatted phone number in the form (1, "(303) 555-1212").</returns>
	public static (int CountryCode, string Number)? PhoneNumberComponents(string? e164PhoneNumber)
	{

		var formattedNumber = FormatPhoneNumber(e164PhoneNumber);
		var index = formattedNumber!.IndexOf(' ');
		if (index < 0)
			return (1, formattedNumber);
		if (int.TryParse(formattedNumber[1..index], out var countryCode))
			return (countryCode, formattedNumber[(index + 1)..].Trim());
		return (1, formattedNumber);
	}

	/// <summary>
	/// Removes all the text an individual will type in a phone number such as () -  and
	/// spaces. This will retain the leading plus sign and numbers. This generally
	/// returns a E164 phone number, but does not guarantee it.
	/// </summary>
	/// <param name="phone">The formatted phone number.</param>
	/// <returns>The clean E164 phone number.</returns>
	public static string TrimPhone(string phone)
	{
		phone = phone.Trim();
		var buffer = new StringBuilder(phone.StartsWith("+") ? "+" : "");
		foreach (var c in phone.Where(char.IsDigit))
			buffer.Append(c);
		return buffer.ToString();
	}

	/// <summary>
	/// Returns true if the phone number is a valid phone number - any format.
	/// </summary>
	/// <param name="phoneNumber">The phone number. Can be E-164 or 1 (303) 555-1212 or whatever.</param>
	/// <returns>true if valid.</returns>
	public static bool IsValidPhoneNumber(string phoneNumber)
	{
		try
		{
			if (!phoneNumber.StartsWith("+"))
				phoneNumber = $"+1{phoneNumber}";
			var phone = PhoneNumberUtil.Parse(TrimPhone(phoneNumber), null);
			return PhoneNumberUtil.IsValidNumber(phone);
		}
		catch (Exception)
		{
			return false;
		}
	}
}