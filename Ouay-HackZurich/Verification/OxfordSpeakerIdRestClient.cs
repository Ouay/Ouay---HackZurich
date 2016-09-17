using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Ouay_HackZurich.Verification
{
	// Lazy not turning this into a proper service and injecting it.
	class OxfordSpeakerIdRestClient
	{
		public OxfordSpeakerIdRestClient()
		{

		}
		byte[] HackOxfordWavPcmStream(IInputStream inputStream, out int offset)
		{
			Debug.WriteLine("[HackOxfordWavPcmStream]");
			var netStream = inputStream.AsStreamForRead();
			Debug.WriteLine("[1] ");
			var bits = new byte[netStream.Length];
			Debug.WriteLine("[2]");
			netStream.Read(bits, 0, bits.Length);
			Debug.WriteLine("[3]");
			Debug.WriteLine("bits size = " + bits.Length);
			// original file length
			var pcmFileLength = BitConverter.ToInt32(bits, 4);
			Debug.WriteLine("[4]");
			// take away 36 bytes for the JUNK chunk
			pcmFileLength -= 36;
			Debug.WriteLine("[5]");
			// now copy 12 bytes from start of bytes to 36 bytes further on
			for (int i = 0; i < 12; i++)
			{
				bits[i + 36] = bits[i];
			}
			Debug.WriteLine("[6]");
			// now put modified file length into byts 40-43
			var newLengthBits = BitConverter.GetBytes(pcmFileLength);
			newLengthBits.CopyTo(bits, 40);
			Debug.WriteLine("[7]");
			// the bits that we want are now 36 onwards in this array
			offset = 36;
			Debug.WriteLine("[8]");
			return (bits);
		}
		public async Task<VerificationResult> VerifyAsync(VerificationProfile profile,
		  IInputStream inputStream)
		{
			Debug.WriteLine("[VerifyAsync]");
			var uri = new Uri(
			  $"{OXFORD_BASE_URL}{OXFORD_VERIFICATION_ENDPOINT}" +
			  Uri.EscapeDataString(profile.VerificationProfileId));

			var result = await this.SendPcmStreamToOxfordEndpointAsync<VerificationResult>(
			  uri, inputStream);

			return (result);
		}
		public async Task<EnrollmentResult> EnrollAsync(VerificationProfile profile,
		  IInputStream inputStream)
		{
			var uri = new Uri(
				$"{OXFORD_BASE_URL}{OXFORD_VERIFICATION_PROFILES_ENDPOINT}/" +
				Uri.EscapeDataString(profile.VerificationProfileId) +
				$"/{OXFORD_ENROLL}");

			var result = await this.SendPcmStreamToOxfordEndpointAsync<EnrollmentResult>(
			  uri, inputStream);

			return (result);
		}
		async Task<T> SendPcmStreamToOxfordEndpointAsync<T>(Uri uri, IInputStream inputStream)
		{
			Debug.WriteLine("[SendPcmStreamToOxfordEndpointAsync]");
			int offset;
			byte[] bits = this.HackOxfordWavPcmStream(inputStream, out offset);
			Debug.WriteLine("[Before array]");
			ByteArrayContent content = new ByteArrayContent(bits, offset, bits.Length - offset);
			Debug.WriteLine("[After array]");
			var response = await this.HttpClient.PostAsync(uri, content);
			Debug.WriteLine("[After response]");
			var result = await this.HandleHttpJsonResultAsync<T>(response);
			Debug.WriteLine("[After result]");
			return (result);
		}
		public async Task RemoveVerificationProfileAsync(VerificationProfile profile)
		{
			var response = await this.HttpClient.DeleteAsync(
			  new Uri($"{OXFORD_BASE_URL}/{OXFORD_VERIFICATION_PROFILES_ENDPOINT}/" +
				Uri.EscapeDataString(profile.VerificationProfileId)));

			this.ThrowOnFailStatus(response, string.Empty);
		}
		void ThrowOnFailStatus(HttpResponseMessage response, string content)
		{
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException(
				  $"Something went wrong - I got [{content}]");
			}
		}
		public async Task<IEnumerable<VerificationPhrase>> GetVerificationPhrasesAsync()
		{
			var results = await this.GetEndpointJsonResultAsync<VerificationPhrase[]>(
			  OXFORD_VERIFICATION_PHRASES_ENDPOINT);

			return (results);
		}
		public async Task<VerificationProfile> AddVerificationProfileAsync()
		{
			var jObject = new JObject();

			jObject["locale"] = "en-us";

			var result = await this.PostEndpointJsonResultAsync<VerificationProfile>(
			  OXFORD_VERIFICATION_PROFILES_ENDPOINT, jObject);

			return (result);
		}
		public async Task<IEnumerable<VerificationProfile>> GetVerificationProfilesAsync()
		{
			var results = await this.GetEndpointJsonResultAsync<VerificationProfile[]>(
			  OXFORD_VERIFICATION_PROFILES_ENDPOINT);

			return (results);
		}
		async Task<T> PostEndpointJsonResultAsync<T>(string endpoint, JObject jsonObject)
		{
			var content = new StringContent(jsonObject.ToString(),
			  Encoding.UTF8, "application/json");

			var response = await this.HttpClient.PostAsync(
			  new Uri($"{OXFORD_BASE_URL}/{endpoint}"), content);

			var result = await this.HandleHttpJsonResultAsync<T>(response);

			return (result);
		}
		async Task<T> GetEndpointJsonResultAsync<T>(string endpoint)
		{
			var response = await this.HttpClient.GetAsync(new Uri(
			  $"{OXFORD_BASE_URL}/{endpoint}"));

			var result = await this.HandleHttpJsonResultAsync<T>(response);

			return (result);
		}
		async Task<T> HandleHttpJsonResultAsync<T>(HttpResponseMessage response)
		{
			var stringContent = await response.Content.ReadAsStringAsync();

			this.ThrowOnFailStatus(response, stringContent);

			var jsonObject = JsonConvert.DeserializeObject<T>(stringContent);

			return (jsonObject);
		}
		HttpClient HttpClient
		{
			get
			{
				if (this.httpClient == null)
				{
					this.httpClient = new HttpClient();
					this.httpClient.DefaultRequestHeaders.Add(
					  OXFORD_SUB_KEY_HEADER, Keys.OxfordKey);
				}
				return (this.httpClient);
			}
		}
		static readonly string OXFORD_BASE_URL = "https://api.projectoxford.ai/spid/v1.0/";
		static readonly string OXFORD_ENROLL = "enroll";
		static readonly string OXFORD_VERIFICATION_ENDPOINT = "verify?verificationProfileId=";
		static readonly string OXFORD_VERIFICATION_PROFILES_ENDPOINT = "verificationProfiles";
		static readonly string OXFORD_VERIFICATION_PHRASES_ENDPOINT = "verificationPhrases?locale=en-us";
		static readonly string OXFORD_SUB_KEY_HEADER = "Ocp-Apim-Subscription-Key";
		HttpClient httpClient;
	}
}