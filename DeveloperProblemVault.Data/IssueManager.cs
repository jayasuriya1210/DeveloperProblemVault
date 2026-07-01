using Dapper;
using MySqlConnector;

namespace DeveloperProblemVault.Data;

public class IssueManager(string connectionString)
{
    private MySqlConnection CreateConnection() => new(connectionString);

    public async Task<Issue> InsertIssueAsync(Issue issue)
    {
        const string sql = """
            INSERT INTO issues (issue_title, project_name, status, priority, description, root_cause, solution)
            VALUES (@IssueTitle, @ProjectName, @Status, @Priority, @Description, @RootCause, @Solution);
            SELECT LAST_INSERT_ID();
            """;

        using var conn = CreateConnection();
        issue.Id = await conn.ExecuteScalarAsync<long>(sql, issue);
        return issue;
    }

    public async Task<IEnumerable<Issue>> GetAllIssuesAsync()
    {
        const string sql = """
            SELECT id, issue_title AS IssueTitle, project_name AS ProjectName, status, priority,
                   description, root_cause AS RootCause, solution
            FROM issues ORDER BY id DESC
            """;

        using var conn = CreateConnection();
        return await conn.QueryAsync<Issue>(sql);
    }

    public async Task<Issue?> GetIssueByIdAsync(long id)
    {
        const string sql = """
            SELECT id, issue_title AS IssueTitle, project_name AS ProjectName, status, priority,
                   description, root_cause AS RootCause, solution
            FROM issues WHERE id = @id
            """;

        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Issue>(sql, new { id });
    }

    public async Task<Issue?> UpdateIssueAsync(long id, Issue issue)
    {
        const string sql = """
            UPDATE issues SET
                issue_title = @IssueTitle,
                project_name = @ProjectName,
                status = @Status,
                priority = @Priority,
                description = @Description,
                root_cause = @RootCause,
                solution = @Solution
            WHERE id = @Id
            """;

        using var conn = CreateConnection();
        issue.Id = id;
        await conn.ExecuteAsync(sql, issue);
        return await GetIssueByIdAsync(id);
    }

    public async Task DeleteIssueAsync(long id)
    {
        const string sql = "DELETE FROM issues WHERE id = @id";
        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> ExistsByIdAsync(long id)
    {
        const string sql = "SELECT COUNT(*) FROM issues WHERE id = @id";
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { id }) > 0;
    }

    public async Task EnsureCreatedAsync()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS issues (
                id           BIGINT AUTO_INCREMENT PRIMARY KEY,
                issue_title  VARCHAR(255),
                project_name VARCHAR(255),
                status       VARCHAR(100),
                priority     VARCHAR(100),
                description  TEXT,
                root_cause   TEXT,
                solution     TEXT
            );
            """;

        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql);
    }
}
