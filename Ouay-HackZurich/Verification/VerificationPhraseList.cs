using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ouay_HackZurich.Verification
{
	static class VerificationPhraseList
	{
		public static async Task<VerificationPhrase> GetVerificationPhraseForProfileAsync(
		  VerificationProfile profile)
		{
			if (phrases == null)
			{
				restClient = new OxfordSpeakerIdRestClient();
				var results = await restClient.GetVerificationPhrasesAsync();
				phrases = results.Reverse().ToArray();
			}
			var id = Guid.Parse(profile.VerificationProfileId);
			var bits = id.ToByteArray();
			var sum = bits.Sum(b => (int)b);
			var entry = sum % phrases.Length;
			/*Change to get different phrases*/
			return (phrases[entry]);
		}
		static OxfordSpeakerIdRestClient restClient;
		static VerificationPhrase[] phrases;
	}
}