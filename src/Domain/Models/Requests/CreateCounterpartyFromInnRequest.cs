using System.ComponentModel.DataAnnotations;
using Domain.Entities.Enums.VkOrd;
using MediatR;

namespace Domain.Models.Requests
{
	public sealed class CreateCounterpartyFromInnRequest : IRequest
	{
		[Required]
		[StringLength(12, MinimumLength = 10)]
		public string Inn { get; set; } = string.Empty;


        /// <summary>
        /// Типы контрагента
        /// </summary>
        /// <example>
        /// <see cref="VkOrdApiPersonRoles"/>
        /// </example>
        public List<VkOrdApiPersonRoles> Types { get; set; } = new();
	}
}

