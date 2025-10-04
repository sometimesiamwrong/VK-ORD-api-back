using System.ComponentModel.DataAnnotations;
using MediatR;
using VkOrdApi.Person;

namespace WebApp.Models.Requests
{
	public sealed class CreateCounterpartyFromInnRequest : IRequestWithVkOrdKey, IRequest
	{
		[Required]
		[StringLength(12, MinimumLength = 10)]
		public string Inn { get; set; } = string.Empty;


        /// <summary>
        /// Типы контрагента
        /// </summary>
        /// <example>
        /// <see cref="VkOrdPersonRoles"/>
        /// </example>
        public List<VkOrdPersonRoles> Types { get; set; } = new();
	}
}

