
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

using System.Globalization;
using PhoneNumbers;

namespace TradeWindsBlazorDevEx.Utilities
{
	public class CountryInfo
	{
		/// <summary>
		/// The text for the United States as returned in CountryInfo.EnglishName.
		/// </summary>
		public static readonly string UnitedStates = "United States";

		/// <summary>
		/// The UniqueId for the U.S.
		/// </summary>
		public static readonly string UsUniqueId = "US";

		/// <summary>
		/// The RegionInfo.EnglishName of a country.
		/// </summary>
		public string EnglishName { get; }

		/// <summary>
		/// The RegionInfo.TwoLetterISORegionName of a country.
		/// </summary>
		public string TwoLetterIsoRegionName { get; }

		/// <summary>
		/// The telephone country code of a country. null if a country does not have one.
		/// </summary>
		public int?  PhoneCountryCode { get; private set; }

		/// <summary>
		/// The DevExpress format/mask for the postal code. null if not known.
		/// </summary>
		public string? PostalCodeFormat { get; private set; }

		/// <summary>
		/// The DevExpress format/mask for the phone number. null if not known.
		/// </summary>
		public string? PhoneNumberFormat { get; private set; }

		public string TextCountryCode => PhoneCountryCode == null ? string.Empty : $"+{PhoneCountryCode}";

		public static IReadOnlyList<CountryInfo> SortByNameUsFirst { get; }

		public static IReadOnlyList<CountryInfo> SortByName { get; }

		public static IReadOnlyList<CountryInfo> SortByIso { get; }

		public static IReadOnlyList<CountryInfo> SortByCode { get; }

		/// <summary>
		/// Converts the passed in full name of a country to the two letter ISO code.
		/// </summary>
		/// <param name="englishName">The RegionInfo.EnglishName of a country.</param>
		/// <returns>The matching RegionInfo.TwoLetterIsoRegionName of the country.</returns>
		public static string? EnglishNameToTwoLetterIso(string? englishName)
		{
			if (englishName == null)
				return null;
			var country = SortByName.FirstOrDefault(c => c.EnglishName == englishName);
			return country?.TwoLetterIsoRegionName;
		}

		/// <summary>
		/// Converts the passed in two letter ISO code to the full name of a country.
		/// </summary>
		/// <param name="twoLetterIsoName">The RegionInfo.TwoLetterIsoRegionName of a country.</param>
		/// <returns>The matching RegionInfo.EnglishName of the country.</returns>
		public static string? TwoLetterIsoToEnglishName(string? twoLetterIsoName)
		{
			if (twoLetterIsoName == null)
				return null;
			var country = SortByIso.FirstOrDefault(c => c.TwoLetterIsoRegionName == twoLetterIsoName);
			return country?.EnglishName;
		}

		private CountryInfo(string englishName, string twoLetterIsoRegionName, int? phoneCountryCode = null)
		{
			EnglishName = englishName;
			TwoLetterIsoRegionName = twoLetterIsoRegionName;
			PhoneCountryCode = phoneCountryCode;
		}

		static CountryInfo()
		{

			// get all countries from RegionInfo
			var regions = new Dictionary<string, RegionInfo>();
			var allCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			foreach (var culture in allCultures)
			{
				try
				{
					var region = new RegionInfo(culture.LCID);
					regions.TryAdd(region.TwoLetterISORegionName, region);
				}
				catch (ArgumentException)
				{
					// Ignore cultures that do not have an associated region
				}
			}

			// put in a list
			var list = regions.Select(info => new CountryInfo(info.Value.EnglishName, info.Value.TwoLetterISORegionName)).ToList();

			var phoneNumberUtil = PhoneNumberUtil.GetInstance();
			foreach (var info in list)
			{
				int countryCode = phoneNumberUtil.GetCountryCodeForRegion(info.TwoLetterIsoRegionName);
				if (countryCode > 0)
					info.PhoneCountryCode = countryCode;
			}

			SortByName = list.OrderBy(ci => ci.EnglishName).ToList();
			SortByIso = list.OrderBy(ci => ci.TwoLetterIsoRegionName).ToList();

			// eliminate dups - keep US for +1, RU for +7
			var ccList = list.Where(c => c.PhoneCountryCode.HasValue).OrderBy(ci => ci.PhoneCountryCode).ToList();
			ccList = ccList.Where(c => c.PhoneCountryCode != 1 || c.TwoLetterIsoRegionName == "US").ToList();
			ccList = ccList.Where(c => c.PhoneCountryCode != 7 || c.TwoLetterIsoRegionName == "RU").ToList();
			SortByCode = ccList;

			var usList = list.OrderBy(ci => ci.EnglishName).ToList();
			usList.Insert(0, new CountryInfo("-----", "--"));
			usList.Insert(0, new CountryInfo(UnitedStates, "US", 1));
			SortByNameUsFirst = usList;
		}
	}
}