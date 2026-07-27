namespace DeveloperProblemVault.Data;

public interface IIssueManager
{
    Task<Issue> InsertIssueAsync(Issue issue);
    Task<IEnumerable<Issue>> GetAllIssuesAsync();
    Task<Issue?> GetIssueByIdAsync(long id);
    Task<Issue?> UpdateIssueAsync(long id, Issue issue);
    Task DeleteIssueAsync(long id);
    Task<bool> ExistsByIdAsync(long id);
}
