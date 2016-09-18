using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ouay_HackZurich.BlueMix
{
	public class BlueMixCom
	{
		protected static string UrlMain = @"https://ouay.mybluemix.net/data?";
		protected static string UrlTime = @"time=";
		protected static string UrlDay = @"&day=";
		protected static string UrlResponse = @"https://ouay.mybluemix.net/poll";
		protected static string UrlNotif = @"https://ouay.mybluemix.net/notify?";
		protected static string UrlWhat = @"what=";

		/// <summary>
		/// Send a DateTime to BlueMix to store it
		/// </summary>
		/// <param name="now">The time of the entrance</param>
		public async static Task<bool> SendEntrance(int hour, int minute, string day)
		{
			try
			{
				var httpClient = new HttpClient();
				string url = UrlMain;
				url += UrlTime + hour + "-" + minute;
				url += UrlDay + day;
				await httpClient.GetStringAsync(new Uri(url));
				Debug.WriteLine("Set new entrance to server");
				return true;
			}
			catch
			{
				Debug.WriteLine("Cannot give entrance message to server");
				return false;
			}
		}

		/// <summary>
		/// Send a DateTime to BlueMix to store it
		/// </summary>
		/// <param name="now">The time of the entrance</param>
		public async static Task<bool> SendEntrance(DateTime now)
		{
			try
			{
				var httpClient = new HttpClient();
				string url = UrlMain;
				url += UrlTime + now.Hour.ToString() + "-" + now.Minute.ToString();
				url += UrlDay + ConvertDay(now.DayOfWeek);
				await httpClient.GetStringAsync(new Uri(url));
				Debug.WriteLine("Set new entrance to server");
				List<StdTime> list =  await GetNormalRange();
				foreach(var item in list)
				{
					if (item.Day.ToString() == ConvertDay(now.DayOfWeek))
					{
						double total = now.Hour * 60 + now.Minute;
						double totalDay = item.MeanHour * 60 + item.MeanMin;
						if (total > totalDay + item.Std || total < totalDay - item.Std)
							Alert(string.Empty);
						else
							return true;
						return false;
					}
				}
				return true;
			}
			catch
			{
				Debug.WriteLine("Cannot give entrance message to server");
				return false;
			}
		}

		public async static void Alert(string detail)
		{
			try
			{
				var httpClient = new HttpClient();
				string url = UrlNotif;
				url += UrlWhat + GenRand();
				await httpClient.GetStringAsync(new Uri(url));
				Debug.WriteLine("Set new entrance to notification Server");
			}
			catch
			{
				Debug.WriteLine("Cannot give entrance notification to server");
			}
		}

		private static string GenRand()
		{
			Random rand = new Random();
			return rand.Next(2000).ToString();
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
		public async static Task<List<StdTime>> GetNormalRange()
		{
			JArray json = new JArray();
			json = await RetreiveJson();
			return ParseJson(json);
		}

		private static List<StdTime> ParseJson(JArray json)
		{
			List<StdTime> list = new List<StdTime>();
			foreach (JObject o in json.Children<JObject>())
			{
				int _day = 0; double _std = 0; double _mean = 0;
				foreach (JProperty p in o.Properties())
				{
					string name = p.Name;
					string value = (string)p.Value;
					if (name == "day")
						_day = Convert.ToInt32(value);
					if (name == "std")
						_std = Convert.ToDouble(value);
					if (name == "mean")
						_mean = Convert.ToDouble(value);
				}
				list.Add(new StdTime(_day, _std, _mean));
			}
			return list;
		}

		private async static Task<JArray> RetreiveJson()
		{
			var httpClient = new HttpClient();
			var content = await httpClient.GetStringAsync(UrlResponse);
			return await Task.Run(() => JArray.Parse(content));
		}

		public async static void Populate(int nbOfItem = 50)
		{
			int WeekDayHour = 19;
			int WeekDayMin = 0;
			int WeekEndHour = 21;
			int WeekEndMin = 0;
			int totalMin = WeekDayHour * 60 + WeekDayMin;
			/*Populate weekdays*/
			for (int i = 1; i < 6; i++)
			{
				Debug.WriteLine(@"Populate Day #" + i);
				for (int j = 0; j < 50; j++)
				{
					WeekDayHour = 19;
					WeekDayMin = 0;
					totalMin += GetDifference();
					WeekDayHour = totalMin / 60;
					WeekDayMin = totalMin % 60;
					if (!await SendEntrance(WeekDayHour, WeekDayMin, i.ToString()))
						j -= 1;
					await Task.Delay(50);
				}
			}
			/*Populate Weekend*/
			totalMin = WeekEndHour * 60 + WeekEndMin;
			for (int i = 6; i < 8; i++)
			{
				Debug.WriteLine(@"Populate Day #" + i);
				for (int j = 0; j < 50; j++)
				{
					WeekEndHour = 21;
					WeekEndMin = 0;
					totalMin += GetDifference();
					WeekDayHour = totalMin / 60;
					WeekDayMin = totalMin % 60;
					if (!await SendEntrance(WeekDayHour, WeekDayMin, i.ToString()))
						j -= 1;
					await Task.Delay(50);
				}
			}
			Debug.WriteLine("Finished populate");
		}

		private static int GetDifference()
		{
			Random rand = new Random();

			return rand.Next(40) - 20;
		}
	}

	public class StdTime
	{
		public int Day { get; }
		public double Std { get; }
		public double MeanHour { get; }
		public double MeanMin { get; }

		public StdTime(int _day, double _std, double _mean)
		{
			Day = _day;
			Std = _std;
			string _hour = _mean.ToString();
			if (_mean != 0)
			{
				MeanHour = Convert.ToDouble(_hour.Substring(0, 2));
				MeanMin = Convert.ToDouble(_hour.Substring(2, 2)) / 100 * 60;
			}
			else
			{
				MeanHour = 0;
				MeanMin = 0;
			}
		}
	}
}
