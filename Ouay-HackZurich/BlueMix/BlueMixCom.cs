using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ouay_HackZurich.BlueMix
{
	public class BlueMixCom
	{
		protected static string UrlMain = @"https://www.ouay.mybluemix.net/data?";
		protected static string UrlTime = @"time=";
		protected static string UrlDay = @"&day=";
		protected static string UrlResponse = @"https://www.ouay.mybluemix.net/poll";

		/// <summary>
		/// Send a DateTime to BlueMix to store it
		/// </summary>
		/// <param name="now">The time of the entrance</param>
		public async static void SendEntrance(DateTime now)
		{
			var httpClient = new HttpClient();
			string url = UrlMain;
			url += UrlTime + now.Hour.ToString() + "-" + now.Minute.ToString();
			url += UrlDay + ConvertDay(now.DayOfWeek);
			await httpClient.GetStringAsync(new Uri(url));
		}

		private static string ConvertDay(DayOfWeek dayOfWeek)
		{
			switch(dayOfWeek)
			{
				case DayOfWeek.Monday:
					return "1";
				case DayOfWeek.Tuesday:
					return "2";
				case DayOfWeek.Wednesday:
					return "3";
				case DayOfWeek.Thursday:
					return "4";
				case DayOfWeek.Friday:
					return "5";
				case DayOfWeek.Saturday:
					return "6";
				case DayOfWeek.Sunday:
					return "7";
				default:
					return "1";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public async static Task<DateTime[]> GetNormalRange()
		{
			JObject json = new JObject();
			json = await RetreiveJson();
			return ParseJson(json);
		}

		private static DateTime[] ParseJson(JObject json)
		{
			return null;
		}

		private async static Task<JObject> RetreiveJson()
		{
			var httpClient = new HttpClient();
			var content = await httpClient.GetStringAsync(UrlResponse);
			return await Task.Run(() => JObject.Parse(content));
		}
	}
}
