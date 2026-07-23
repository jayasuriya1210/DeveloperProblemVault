namespace DeveloperProblemVault.Api.Models;

public static class ResponseHelper
{
    public static ResponseModel Success(string message, object? result = null) =>
        new() { Stat = 1, Message = message, Result = result };

    public static ResponseModel Fail(string reason) =>
        new() { Stat = 0, Message = MessageConstants.Failed, Reason = reason };
}
