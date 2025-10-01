using System.ComponentModel.DataAnnotations;

namespace VkOrdApiWrapper.Models.Requests
{
	public sealed class CreateCounterpartyFromInnRequest : AuthorizedRequestBase
	{
		[Required]
		[StringLength(12, MinimumLength = 10)]
		public string Inn { get; set; } = string.Empty;


		/// <summary>
		/// Типы контрагента
		/// </summary>
		/// <example>
		/// <see cref="VkOrdApiWrapper.Models.VkOrd.VkPersonRoles"/>
		/// </example>
		public List<string> Types { get; set; } = new();
	}
}

