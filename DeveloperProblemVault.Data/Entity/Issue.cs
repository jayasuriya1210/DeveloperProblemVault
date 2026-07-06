using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeveloperProblemVault.Data;

[Table("issues")]
public class Issue
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("issue_title")]
    public string? IssueTitle { get; set; }

    [Column("project_name")]
    public string? ProjectName { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("priority")]
    public string? Priority { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("root_cause")]
    public string? RootCause { get; set; }

    [Column("solution")]
    public string? Solution { get; set; }
}
