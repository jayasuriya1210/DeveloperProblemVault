using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Data;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager, ILogger<IssueService> logger)
{
    public async Task<IssueResponseDto> SaveIssueAsync(IssueRequestDto dto)
    {
        logger.LogInformation("Saving issue: {Title}", dto.IssueTitle);
        var issue = ToEntity(dto);
        var saved = await issueManager.InsertIssueAsync(issue);
        logger.LogInformation("Issue saved with ID: {Id}", saved.Id);
        return ToDto(saved);
    }

    public async Task<IssueResponseDto> GetIssueByIdAsync(long id)
    {
        logger.LogInformation("Fetching issue with ID: {Id}", id);
        var issue = await issueManager.GetIssueByIdAsync(id)
            ?? throw new ArgumentException(MessageConstants.IssueNotFound);
        return ToDto(issue);
    }

    public async Task<IEnumerable<IssueResponseDto>> GetAllIssuesAsync()
    {
        logger.LogInformation("Fetching all issues");
        var issues = await issueManager.GetAllIssuesAsync();
        logger.LogInformation("Fetched {Count} issues", issues.Count());
        return issues.Select(ToDto);
    }

    public async Task<IssueResponseDto> UpdateIssueAsync(long id, IssueRequestDto dto)
    {
        logger.LogInformation("Updating issue with ID: {Id}", id);
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException(MessageConstants.IssueNotFound);

        var issue = ToEntity(dto);
        var updated = await issueManager.UpdateIssueAsync(id, issue);
        logger.LogInformation("Issue updated with ID: {Id}", id);
        return ToDto(updated!);
    }

    public async Task DeleteIssueAsync(long id)
    {
        logger.LogInformation("Deleting issue with ID: {Id}", id);
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException(MessageConstants.IssueNotFound);

        await issueManager.DeleteIssueAsync(id);
        logger.LogInformation("Issue deleted with ID: {Id}", id);
    }

    private static Issue ToEntity(IssueRequestDto dto) => new()
    {
        IssueTitle  = dto.IssueTitle,
        ProjectName = dto.ProjectName,
        Status      = dto.Status,
        Priority    = dto.Priority,
        Description = dto.Description,
        RootCause   = dto.RootCause,
        Solution    = dto.Solution
    };

    private static IssueResponseDto ToDto(Issue issue) => new()
    {
        Id          = issue.Id,
        IssueTitle  = issue.IssueTitle,
        ProjectName = issue.ProjectName,
        Status      = issue.Status,
        Priority    = issue.Priority,
        Description = issue.Description,
        RootCause   = issue.RootCause,
        Solution    = issue.Solution
    };
}
