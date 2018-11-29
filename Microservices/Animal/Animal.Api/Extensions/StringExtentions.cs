using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MicroServices.Animal.Api.Extensions
{
	public static class StringExtentions
	{
		public static string ToBase64(this string str)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}

		public static string FromBase64(this string str)
		{
			if (str == null)
				return null;

			var base64EncodedBytes = Convert.FromBase64String(str);

			return Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static string ToTitleCase(this string str)
		{
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			return textInfo.ToTitleCase(str);
		}

		public static Dictionary<String, String> ExtractParameterValues(this string str)
		{
			var result = new Dictionary<String, String>();
			var parameters = str
				.Replace("(", "")
				.Replace(")", "")
				.Split(',');
			foreach (var param in parameters)
			{
				var paramArray = param.Split('=');
				result.Add(paramArray[0].Trim(), paramArray.Length >= 2 ? paramArray[1].Trim() : string.Empty);
			}
			return result;
		}

		public static string GetDeviceIdentifier(this string deviceCompositeKey)
		{
			var deviceKeys = deviceCompositeKey.ExtractParameterValues();

			string animalIdentifier = string.Empty;
			// removing nlis_id or rf_id keys
			if (deviceKeys.ContainsKey("nlis_id"))
				animalIdentifier = Regex.Replace(deviceKeys["nlis_id"], "nlis_id", string.Empty);
			else if (deviceKeys.ContainsKey("rf_id"))
				animalIdentifier = Regex.Replace(deviceKeys["rf_id"], "rf_id", string.Empty);

			return animalIdentifier;

		}


		public static bool IsDigitsOnly(this string str)
		{
			foreach (var c in str)
			{
				if (c < '0' || c > '9')
					return false;
			}

			return true;
		}

		public static bool IsNlisId(this string str)
		{
			if (!str.Contains(" ") && str.Length == 16 && !str.All(char.IsDigit))
			{
				return true;
			}

			return false;
		}

		public static bool IsRfId(this string str)
		{
			if (str.Length != 16 || str.Substring(3, 1) != " ")
			{
				return false;
			}

			var rfidWithoutSpace = str.Replace(" ", "");
			if (rfidWithoutSpace.All(char.IsDigit))
			{
				return true;
			}

			return false;
		}

		public static bool ValidDeviceIdentifier(this string str)
		{
			if (str.IsNlisId() || str.IsRfId())
			{
				return true;
			}
			return false;
		}

		public static string ToEndpointKey(this string pathAndQuery)
		{
			int resourceCountInUri = 0; // HACK: this needs to be fixed asap
			if (string.IsNullOrEmpty(pathAndQuery))
				return string.Empty;

			// all our uris are like /api/version/resource1/{resource1Key}/resource2/{resource2Key}
			var elements = pathAndQuery.Split('?').First().Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
			var pathWithoutResourceId = new StringBuilder();

			for (int i = 0; i < elements.Count; i++)
			{
				// ignore route's resource paramaters
				if (elements[i].Contains("{") || elements[i].Contains("}")
				    || elements[i].Contains("(") || elements[i].Contains(")")
				    || Char.IsDigit(elements[i], 0))
					continue;

				pathWithoutResourceId = pathWithoutResourceId.Append(elements[i]).Append("/");
				resourceCountInUri++;
				if (resourceCountInUri >= 3)
					break;
			}
			return pathWithoutResourceId.ToString().ToLower();
		}
	}
}