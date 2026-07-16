using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Data;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager)
{
    public async Task<IssueResponseDto> SaveIssueAsync(IssueRequestDto dto)
    {
        try
        {
            var issue = ToEntity(dto);
            var saved = await issueManager.InsertIssueAsync(issue);
            return ToDto(saved);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IssueResponseDto> GetIssueByIdAsync(long id)
    {
        try
        {
            var issue = await issueManager.GetIssueByIdAsync(id)
                ?? throw new ArgumentException(MessageConstants.IssueNotFound);
            return ToDto(issue);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<IssueResponseDto>> GetAllIssuesAsync()
    {
        try
        {
            var issues = await issueManager.GetAllIssuesAsync();
            return issues.Select(ToDto);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IssueResponseDto> UpdateIssueAsync(long id, IssueRequestDto dto)
    {
        try
        {
            if (!await issueManager.ExistsByIdAsync(id))
                throw new ArgumentException(MessageConstants.IssueNotFound);

            var issue = ToEntity(dto);
            var updated = await issueManager.UpdateIssueAsync(id, issue);
            return ToDto(updated!);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteIssueAsync(long id)
    {
        try
        {
            if (!await issueManager.ExistsByIdAsync(id))
                throw new ArgumentException(MessageConstants.IssueNotFound);

            await issueManager.DeleteIssueAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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
