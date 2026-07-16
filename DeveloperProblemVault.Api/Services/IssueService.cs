using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Data;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager, ILogger<IssueService> logger)
{
    public async Task<IssueResponseDto> SaveIssueAsync(IssueRequestDto dto)
    {
        try
        {
            logger.LogInformation("Saving issue {IssueTitle}", dto.IssueTitle);
            var issue = ToEntity(dto);
            var saved = await issueManager.InsertIssueAsync(issue);
            logger.LogInformation("Issue saved with Id {Id}", saved.Id);
            return ToDto(saved);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving issue {IssueTitle}", dto.IssueTitle);
            throw;
        }
    }

    public async Task<IssueResponseDto> GetIssueByIdAsync(long id)
    {
        try
        {
            logger.LogInformation("Fetching issue with Id {Id}", id);
            var issue = await issueManager.GetIssueByIdAsync(id)
                ?? throw new ArgumentException(MessageConstants.IssueNotFound);
            return ToDto(issue);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching issue with Id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<IssueResponseDto>> GetAllIssuesAsync()
    {
        try
        {
            logger.LogInformation("Fetching all issues");
            var issues = await issueManager.GetAllIssuesAsync();
            logger.LogInformation("Fetched {Count} issues", issues.Count());
            return issues.Select(ToDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all issues");
            throw;
        }
    }

    public async Task<IssueResponseDto> UpdateIssueAsync(long id, IssueRequestDto dto)
    {
        try
        {
            logger.LogInformation("Updating issue with Id {Id}", id);
            if (!await issueManager.ExistsByIdAsync(id))
                throw new ArgumentException(MessageConstants.IssueNotFound);

            var issue = ToEntity(dto);
            var updated = await issueManager.UpdateIssueAsync(id, issue);
            logger.LogInformation("Issue updated with Id {Id}", id);
            return ToDto(updated!);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating issue with Id {Id}", id);
            throw;
        }
    }

    public async Task DeleteIssueAsync(long id)
    {
        try
        {
            logger.LogInformation("Deleting issue with Id {Id}", id);
            if (!await issueManager.ExistsByIdAsync(id))
                throw new ArgumentException(MessageConstants.IssueNotFound);

            await issueManager.DeleteIssueAsync(id);
            logger.LogInformation("Issue deleted with Id {Id}", id);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting issue with Id {Id}", id);
            throw;
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
