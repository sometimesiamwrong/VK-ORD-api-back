using System.ComponentModel.DataAnnotations;

namespace VkOrdApiWrapper.Models.Requests
{
	public sealed class FindPartyByInnRequest : AuthorizedRequestBase
	{
		[Required]
		[StringLength(12, MinimumLength = 10)]
		public string Inn { get; set; } = string.Empty;
	}
}

