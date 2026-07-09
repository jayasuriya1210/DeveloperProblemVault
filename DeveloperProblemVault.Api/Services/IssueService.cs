using DeveloperProblemVault.Data;

namespace DeveloperProblemVault.Api;

public class IssueService(IssueManager issueManager)
{
    public async Task<Issue> SaveIssueAsync(Issue issue)
    {
        try
        {
            ValidateIssue(issue);
            return await issueManager.InsertIssueAsync(issue);
        }
        
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<Issue> GetIssueByIdAsync(long id)
    {
        try
        {
            return await issueManager.GetIssueByIdAsync(id)
                ?? throw new ArgumentException(MessageConstants.IssueNotFound);
        }
        
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<Issue>> GetAllIssuesAsync()
    {
        try
        {
            return await issueManager.GetAllIssuesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Issue> UpdateIssueAsync(long id, Issue issue)
    {
        try
        {
            if (!await issueManager.ExistsByIdAsync(id))
                throw new ArgumentException(MessageConstants.IssueNotFound);

            ValidateIssue(issue);
            return (await issueManager.UpdateIssueAsync(id, issue))!;
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
