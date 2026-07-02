namespace DeveloperProblemVault.Api;

public class ResponseModel
{
    public int Stat { get; set; }
    public string? Message { get; set; }
    public string? Reason { get; set; }
    public object? Result { get; set; }
}
