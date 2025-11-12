using System.Text.Json.Serialization;

namespace Domain.Models.DaData
{
	public class DaDataFindByIdRequest
	{
		[JsonPropertyName("query")]
		public string Query { get; set; } = string.Empty;
	}
}

