using System.Text.Json.Serialization;

namespace WebApp.Models.DaData
{
	public class DaDataFindByIdRequest
	{
		[JsonPropertyName("query")]
		public string Query { get; set; } = string.Empty;
	}
}

