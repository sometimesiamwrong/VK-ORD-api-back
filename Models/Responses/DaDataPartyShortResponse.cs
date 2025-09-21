namespace VkOrdApiWrapper.Models.Responses
{
	public sealed class DaDataPartyShortResponse
	{
		public string? Value { get; init; }
		public string? Status { get; init; }
		public DaDataOpfShort? Opf { get; init; }
		public DaDataNameShort? Name { get; init; }
		public string? Inn { get; init; }
		public string? Ogrn { get; init; }
		public string? Okpo { get; init; }
		public string? Okato { get; init; }
		public string? Oktmo { get; init; }
		public string? Okogu { get; init; }
		public string? Okfs { get; init; }
		public string? Okved { get; init; }
		public DaDataFioShort? Fio { get; init; }
		public string? Type { get; init; }
		public string? Phone { get; init; }
		public string? Kpp { get; init; }
		public string? Email { get; init; }
	}

	public sealed class DaDataOpfShort
	{
		public string? Type { get; init; }
		public string? Code { get; init; }
		public string? Full { get; init; }
		public string? Short { get; init; }
	}

	public sealed class DaDataNameShort
	{
		public string? FullWithOpf { get; init; }
		public string? ShortWithOpf { get; init; }
		public string? Latin { get; init; }
		public string? Full { get; init; }
		public string? Short { get; init; }
	}

	public sealed class DaDataFioShort
	{
		public string? Surname { get; init; }
		public string? Name { get; init; }
		public string? Patronymic { get; init; }
	}
}

