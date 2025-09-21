namespace VkOrdApiWrapper.Models.Responses
{
	public class StatusResponse
	{
		public string Status { get; set; } = "error";
		public string Message { get; set; } = string.Empty;

		public static StatusResponse Success(string message = "Success") => new()
		{
			Status = "success",
			Message = message
		};

		public static StatusResponse Error(string message) => new()
		{
			Status = "error",
			Message = message
		};
	}
}

