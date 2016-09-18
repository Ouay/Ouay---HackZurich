using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherNet;
using WeatherNet.Clients;

namespace Ouay_HackZurich.Weather
{
	class Weather
	{
		/// <summary>
		/// DateTime representing the date
		/// </summary>
		public DateTime Day { get; set; }

		/// <summary>
		/// Temperature actual
		/// </summary>
		public double Temp { get; set; }

		/// <summary>
		/// Minimum temp
		/// </summary>
		public double TempMin { get; set; }

		/// <summary>
		/// Maximum temp
		/// </summary>
		public double TempMax { get; set; }

		/// <summary>
		/// Humidity
		/// </summary>
		public double Humidity { get; set; }

		/// <summary>
		/// A small description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Current Windspeed
		/// </summary>
		public double WindSpeed { get; set; }

		/// <summary>
		/// Icon representing the state
		/// </summary>
		public string Icon { get; set; }

		public Weather()
		{

		}

		public async static Task<Weather> GetCurrent(string town, string country, string language, string type)
		{
			ClientSettings.ApiUrl = "http://api.openweathermap.org/data/2.5";

			ClientSettings.ApiKey = "c0fef2f73a39d4f8bf97ebf8dfc02e88";

			var result = await CurrentWeather.GetByCityNameAsync(town, country, language, type);

			if (!result.Success)
				return null;
			Weather t = new Weather()
			{
				Day = result.Item.Date,
				Temp = result.Item.Temp,
				TempMax = result.Item.TempMax,
				TempMin = result.Item.TempMin,
				Humidity = result.Item.Humidity,
				Description = result.Item.Description,
				WindSpeed = result.Item.WindSpeed,
				Icon = result.Item.Icon
			};

			return t;
		}
	}
}
