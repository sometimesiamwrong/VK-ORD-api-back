using System.Text.Json.Serialization;
using WebApp.Models.Responses;

namespace WebApp.Models.DaData
{
	public sealed class DaDataPartyResponse
	{
		[JsonPropertyName("suggestions")]
		public List<DaDataPartySuggestion> Suggestions { get; set; } = new();

        internal DaDataPartyShortResponse? ToDaDataPartyShortResponse()
        {
			var suggestion = Suggestions.FirstOrDefault();
			if (suggestion == null) 
			{
				return null;
			}

			var data = suggestion.Data;
			if (data == null) 
			{
				return null;			
			}

            return new DaDataPartyShortResponse{
				Value = suggestion.Value,
				Status = data.State?.Status,
				Opf = data.Opf == null ? null : new DaDataOpfShort{
					Type = data.Opf.Type,
					Code = data.Opf.Code,
					Full = data.Opf.Full,
					Short = data.Opf.Short
				},
				Name = data.Name == null ? null : new DaDataNameShort{
					FullWithOpf = data.Name.FullWithOpf,
					ShortWithOpf = data.Name.ShortWithOpf,
					Latin = data.Name.Latin,
					Full = data.Name.Full,
					Short = data.Name.Short
				},
				Inn = data.Inn,
				Kpp = data.Kpp,
				Ogrn = data.Ogrn,
				Okpo = data.Okpo,
				Okato = data.Okato,
				Oktmo = data.Oktmo,
				Okogu = data.Okogu,
				Okfs = data.Okfs,
				Okved = data.Okved,
				Phone = data.Phones.FirstOrDefault(),
				Email = data.Emails.FirstOrDefault(),
				Fio = data.Fio == null ? null : new DaDataFioShort{
					Surname = data.Fio.Surname,
					Name = data.Fio.Name,
					Patronymic = data.Fio.Patronymic
				},
				Type = data.Type
			};
        }
    }

	public sealed class DaDataPartySuggestion
	{
		[JsonPropertyName("value")]
		public string? Value { get; set; }

		[JsonPropertyName("unrestricted_value")]
		public string? UnrestrictedValue { get; set; }

		[JsonPropertyName("data")]
		public DaDataPartyData? Data { get; set; }
	}

	public sealed class DaDataPartyData
	{
		[JsonPropertyName("type")]
		public string? Type { get; set; }

		[JsonPropertyName("state")]
		public DaDataState? State { get; set; }

		[JsonPropertyName("opf")]
		public DaDataOpf? Opf { get; set; }

		[JsonPropertyName("name")]
		public DaDataName? Name { get; set; }

		[JsonPropertyName("inn")]
		public string? Inn { get; set; }
		[JsonPropertyName("kpp")]
		public string? Kpp { get; set; }

		[JsonPropertyName("ogrn")]
		public string? Ogrn { get; set; }

		[JsonPropertyName("okpo")]
		public string? Okpo { get; set; }

		[JsonPropertyName("okato")]
		public string? Okato { get; set; }

		[JsonPropertyName("oktmo")]
		public string? Oktmo { get; set; }

		[JsonPropertyName("okogu")]
		public string? Okogu { get; set; }

		[JsonPropertyName("okfs")]
		public string? Okfs { get; set; }

		[JsonPropertyName("okved")]
		public string? Okved { get; set; }

		[JsonPropertyName("fio")]
		public DaDataFio? Fio { get; set; }

		[JsonPropertyName("phones")]
		public List<string> Phones { get; set; } = new();

		[JsonPropertyName("emails")]
		public List<string> Emails { get; set; } = new();
 	}

 	public sealed class DaDataState
 	{
 		[JsonPropertyName("status")]
 		public string? Status { get; set; }
 	}

 	public sealed class DaDataOpf
 	{
 		[JsonPropertyName("type")]
 		public string? Type { get; set; }

 		[JsonPropertyName("code")]
 		public string? Code { get; set; }

 		[JsonPropertyName("full")]
 		public string? Full { get; set; }

 		[JsonPropertyName("short")]
 		public string? Short { get; set; }
 	}

 	public sealed class DaDataName
 	{
 		[JsonPropertyName("full_with_opf")]
 		public string? FullWithOpf { get; set; }

 		[JsonPropertyName("short_with_opf")]
 		public string? ShortWithOpf { get; set; }

 		[JsonPropertyName("latin")]
 		public string? Latin { get; set; }

 		[JsonPropertyName("full")]
 		public string? Full { get; set; }

 		[JsonPropertyName("short")]
 		public string? Short { get; set; }
 	}

 	public sealed class DaDataFio
 	{
 		[JsonPropertyName("surname")]
 		public string? Surname { get; set; }

 		[JsonPropertyName("name")]
 		public string? Name { get; set; }

 		[JsonPropertyName("patronymic")]
 		public string? Patronymic { get; set; }
 	}
}

