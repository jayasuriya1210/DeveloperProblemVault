using DeveloperProblemVault.Data;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager)
{
    public async Task<Issue> SaveIssueAsync(Issue issue)
    {
        ValidateIssue(issue);
        return await issueManager.InsertIssueAsync(issue);
    }

    public Task<IEnumerable<Issue>> GetAllIssuesAsync() =>
        issueManager.GetAllIssuesAsync();

    public async Task<Issue> UpdateIssueAsync(long id, Issue issue)
    {
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException("Issue not found");

        ValidateIssue(issue);
        return (await issueManager.UpdateIssueAsync(id, issue))!;
    }

    public async Task DeleteIssueAsync(long id)
    {
        if (!await issueManager.ExistsByIdAsync(id))
            throw new ArgumentException("Issue not found");

        await issueManager.DeleteIssueAsync(id);
    }

    private static void ValidateIssue(Issue issue)
    {
        RequireNonBlank(issue.IssueTitle,   MessageConstants.IssueTitleNull);
        RequireNonBlank(issue.ProjectName,  MessageConstants.ProjectNameRequired);
        RequireNonBlank(issue.Status,       MessageConstants.StatusRequired);
        RequireNonBlank(issue.Priority,     MessageConstants.PriorityRequired);
        RequireNonBlank(issue.Description,  MessageConstants.DescriptionRequired);
        RequireNonBlank(issue.RootCause,    MessageConstants.RootCauseRequired);
        RequireNonBlank(issue.Solution,     MessageConstants.SolutionRequired);
    }

    private static void RequireNonBlank(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(message);
    }
}
