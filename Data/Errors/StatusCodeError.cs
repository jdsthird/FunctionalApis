using System.Net;

namespace Data.Errors;

public record StatusCodeError(HttpStatusCode Code, string Message);