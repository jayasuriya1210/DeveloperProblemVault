using Microsoft.EntityFrameworkCore;

namespace DeveloperProblemVault.Data;

public class IssueManager(AppDbContext context) : IIssueManager
{
    public async Task<Issue> InsertIssueAsync(Issue issue)
    {
        context.Issues.Add(issue);
        await context.SaveChangesAsync();
        return issue;
    }

    public async Task<IEnumerable<Issue>> GetAllIssuesAsync() =>
        await context.Issues.OrderByDescending(i => i.Id).ToListAsync();

    public async Task<Issue?> GetIssueByIdAsync(long id) =>
        await context.Issues.FindAsync(id);

    public async Task<Issue?> UpdateIssueAsync(long id, Issue issue)
    {
        var existing = await context.Issues.FindAsync(id);
        if (existing is null) return null;

        existing.IssueTitle  = issue.IssueTitle;
        existing.ProjectName = issue.ProjectName;
        existing.Status      = issue.Status;
        existing.Priority    = issue.Priority;
        existing.Description = issue.Description;
        existing.RootCause   = issue.RootCause;
        existing.Solution    = issue.Solution;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteIssueAsync(long id)
    {
        var issue = await context.Issues.FindAsync(id);
        if (issue is not null)
        {
            context.Issues.Remove(issue);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByIdAsync(long id) =>
        await context.Issues.AnyAsync(i => i.Id == id);
}
