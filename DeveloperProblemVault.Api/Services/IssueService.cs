using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Data;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager)
{
    public async Task<IssueResponseDto> SaveIssueAsync(IssueRequestDto dto)
    {
        var issue = ToEntity(dto);
        var saved = await issueManager.InsertIssueAsync(issue);
        return ToDto(saved);
    }

    public async Task<IssueResponseDto> GetIssueByIdAsync(long id)
    {
        var issue = await issueManager.GetIssueByIdAsync(id)
            ?? throw new ArgumentException(MessageConstants.IssueNotFound);
        return ToDto(issue);
    }

    public async Task<IEnumerable<IssueResponseDto>> GetAllIssuesAsync()
    {
        var issues = await issueManager.GetAllIssuesAsync();
        return issues.Select(ToDto);
    }

    public async Task<IssueResponseDto> UpdateIssueAsync(long id, IssueRequestDto dto)
    {
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException(MessageConstants.IssueNotFound);

        var issue = ToEntity(dto);
        var updated = await issueManager.UpdateIssueAsync(id, issue);
        return ToDto(updated!);
    }

    public async Task DeleteIssueAsync(long id)
    {
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException(MessageConstants.IssueNotFound);

        await issueManager.DeleteIssueAsync(id);
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
