
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

using TradeWindsBlazorDevEx.Utilities;

namespace UnitTests
{
	public class TestPhoneUtils
	{
		[Fact]
		public void TestMiscUtils()
		{
			Assert.Equal("+17203520676", PhoneUtils.TrimPhone("+17203520676"));
			Assert.Equal("+17203520676", PhoneUtils.TrimPhone(" +17203520676 "));
			Assert.Equal("+17203520676", PhoneUtils.TrimPhone("+1 (720) 352-0676"));
			Assert.Equal("7203520676", PhoneUtils.TrimPhone("720.352.0676"));

			Assert.Equal("+1 (720) 352-0676", PhoneUtils.FormatPhoneNumber("+17203520676"));
			Assert.Equal("+1 (720) 352-0676", PhoneUtils.FormatPhoneNumber("7203520676"));
			Assert.Equal("+1 (720) 352-0676", PhoneUtils.FormatPhoneNumber("720.352.0676"));

			Assert.True(PhoneUtils.IsValidPhoneNumber("+1 (720) 352-0676"));
			Assert.True(PhoneUtils.IsValidPhoneNumber("+17203520676"));
			Assert.True(PhoneUtils.IsValidPhoneNumber("720.352.0676"));
			Assert.False(PhoneUtils.IsValidPhoneNumber("+1 (720) 352-067"));
			Assert.False(PhoneUtils.IsValidPhoneNumber("+1720352067"));
			Assert.False(PhoneUtils.IsValidPhoneNumber("720.352.067"));
		}
	}
}
