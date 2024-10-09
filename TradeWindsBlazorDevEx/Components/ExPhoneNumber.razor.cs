
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

using Microsoft.AspNetCore.Components;
using TradeWindsBlazorDevEx.PageModels;
using TradeWindsBlazorDevEx.Utilities;

namespace TradeWindsBlazorDevEx.Components
{
	public partial class ExPhoneNumber
	{
		/// <summary>
		/// The value of the phone number.
		/// </summary>
		[Parameter]
		public PhoneNumberPageModel Value { get; set; } = new PhoneNumberPageModel();

		/// <summary>
		/// Called when the phone number is changed.
		/// </summary>
		[Parameter]
		public EventCallback<PhoneNumberPageModel> ValueChanged { get; set; }

		[Parameter]
		public bool ReadOnly { get; set; }

		private async Task OnCountryCodeChanged(CountryInfo value)
		{
			if (!ValueChanged.HasDelegate)
				return;
			var phone = new PhoneNumberPageModel(value.PhoneCountryCode!, Value.NationalNumber);
			await ValueChanged.InvokeAsync(phone);
		}

		private async Task OnNumberChanged(string? number)
		{
			if (!ValueChanged.HasDelegate)
				return;
			var phone = new PhoneNumberPageModel(Value.CountryCode, number);
			await ValueChanged.InvokeAsync(phone);
		}
	}
}
