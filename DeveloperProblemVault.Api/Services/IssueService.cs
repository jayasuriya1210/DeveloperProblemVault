using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Data;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager, IDistributedCache cache, ILogger<IssueService> logger)
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    private static string IssueKey(long id) => $"issues:{id}";
    private const string AllIssuesKey = "issues:all";
    public async Task<IssueResponseDto> SaveIssueAsync(IssueRequestDto dto)
    {
        logger.LogInformation("Saving issue: {Title}", dto.IssueTitle);
        var issue = ToEntity(dto);
        var saved = await issueManager.InsertIssueAsync(issue);
        await cache.RemoveAsync(AllIssuesKey);
        logger.LogInformation("Issue saved with ID: {Id}", saved.Id);
        return ToDto(saved);
    }

    public async Task<IssueResponseDto> GetIssueByIdAsync(long id)
    {
        logger.LogInformation("Fetching issue with ID: {Id}", id);
        var cached = await cache.GetStringAsync(IssueKey(id));
        if (cached is not null)
        {
            logger.LogInformation("Cache hit for issue ID: {Id}", id);
            return JsonSerializer.Deserialize<IssueResponseDto>(cached)!;
        }
        var issue = await issueManager.GetIssueByIdAsync(id)
            ?? throw new ArgumentException(MessageConstants.IssueNotFound);
        var dto = ToDto(issue);
        await cache.SetStringAsync(IssueKey(id), JsonSerializer.Serialize(dto), CacheOptions);
        return dto;
    }

    public async Task<IEnumerable<IssueResponseDto>> GetAllIssuesAsync()
    {
        logger.LogInformation("Fetching all issues");
        var cached = await cache.GetStringAsync(AllIssuesKey);
        if (cached is not null)
        {
            logger.LogInformation("Cache hit for all issues");
            return JsonSerializer.Deserialize<IEnumerable<IssueResponseDto>>(cached)!;
        }
        var issues = await issueManager.GetAllIssuesAsync();
        var dtos = issues.Select(ToDto).ToList();
        await cache.SetStringAsync(AllIssuesKey, JsonSerializer.Serialize(dtos), CacheOptions);
        logger.LogInformation("Fetched {Count} issues", dtos.Count);
        return dtos;
    }

    public async Task<IssueResponseDto> UpdateIssueAsync(long id, IssueRequestDto dto)
    {
        logger.LogInformation("Updating issue with ID: {Id}", id);
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException(MessageConstants.IssueNotFound);

        var issue = ToEntity(dto);
        var updated = await issueManager.UpdateIssueAsync(id, issue);
        await cache.RemoveAsync(IssueKey(id));
        await cache.RemoveAsync(AllIssuesKey);
        logger.LogInformation("Issue updated with ID: {Id}", id);
        return ToDto(updated!);
    }

    public async Task DeleteIssueAsync(long id)
    {
        logger.LogInformation("Deleting issue with ID: {Id}", id);
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException(MessageConstants.IssueNotFound);

        await issueManager.DeleteIssueAsync(id);
        await cache.RemoveAsync(IssueKey(id));
        await cache.RemoveAsync(AllIssuesKey);
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
