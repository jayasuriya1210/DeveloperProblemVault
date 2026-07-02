using DeveloperProblemVault.Data;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
public class IssueController(IssueService issueService) : ControllerBase
{
    [HttpPost("addIssue")]
    public async Task<IActionResult> AddIssue([FromBody] Issue issue)
    {
        var validationError = ValidateIssue(issue);
        if (validationError != null)
            return BadRequest(Fail(validationError));

        var saved = await issueService.SaveIssueAsync(issue);
        return StatusCode(201, new ResponseModel { Stat = 1, Message = MessageConstants.IssueAdded, Result = saved });
    }

    [HttpGet("viewIssues")]
    public async Task<ResponseModel> ViewIssues() =>
        new() { Stat = 1, Message = MessageConstants.IssuesFetched, Result = await issueService.GetAllIssuesAsync() };

    [HttpGet("viewIssue/{id}")]
    public async Task<ResponseModel> ViewIssue(long id) =>
        new() { Stat = 1, Message = MessageConstants.IssuesFetched, Result = await issueService.GetIssueByIdAsync(id) };

    [HttpPut("updateIssue/{id}")]
    public async Task<IActionResult> UpdateIssue(long id, [FromBody] Issue issue)
    {
        var validationError = ValidateIssue(issue);
        if (validationError != null)
            return BadRequest(Fail(validationError));

        var updated = await issueService.UpdateIssueAsync(id, issue);
        return Ok(new ResponseModel { Stat = 1, Message = MessageConstants.IssueUpdated, Result = updated });
    }

    [HttpDelete("deleteIssue/{id}")]
    public async Task<ResponseModel> DeleteIssue(long id)
    {
        await issueService.DeleteIssueAsync(id);
        return new() { Stat = 1, Message = MessageConstants.IssueDeleted };
    }

    private static string? ValidateIssue(Issue issue)
    {
        if (string.IsNullOrWhiteSpace(issue.IssueTitle))   return MessageConstants.IssueTitleNull;
        if (string.IsNullOrWhiteSpace(issue.ProjectName))  return MessageConstants.ProjectNameRequired;
        if (string.IsNullOrWhiteSpace(issue.Status))       return MessageConstants.StatusRequired;
        if (string.IsNullOrWhiteSpace(issue.Priority))     return MessageConstants.PriorityRequired;
        if (string.IsNullOrWhiteSpace(issue.Description))  return MessageConstants.DescriptionRequired;
        if (string.IsNullOrWhiteSpace(issue.RootCause))    return MessageConstants.RootCauseRequired;
        if (string.IsNullOrWhiteSpace(issue.Solution))     return MessageConstants.SolutionRequired;
        return null;
    }

    private static ResponseModel Fail(string reason) =>
        new() { Stat = 0, Message = MessageConstants.Failed, Reason = reason };
}
