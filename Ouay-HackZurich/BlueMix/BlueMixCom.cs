﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ouay_HackZurich.BlueMix
{
	class BlueMixCom
	{
		protected static string UrlMain = @"https://www.ouay.mybluemix.net/data?";
		protected static string UrlTime = @"time=";
		protected static string UrlDate = @"&date=";
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
			url += UrlDate + now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Year.ToString();
			await httpClient.GetStringAsync(new Uri(url));
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
