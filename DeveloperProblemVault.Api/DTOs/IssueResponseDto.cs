namespace DeveloperProblemVault.Api.DTOs;

public class IssueResponseDto
{
    public long Id { get; set; }
    public string? IssueTitle { get; set; }
    public string? ProjectName { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Description { get; set; }
    public string? RootCause { get; set; }
    public string? Solution { get; set; }
}
