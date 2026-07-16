using DeveloperProblemVault.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
[Authorize]
public class IssueController(IssueService issueService) : ControllerBase
{
    [HttpPost("addIssue")]
    public async Task<IActionResult> AddIssue([FromBody] IssueRequestDto dto)
    {
        var validationError = ValidateRequest(dto);
        if (validationError != null)
            return BadRequest(Fail(validationError));

        var saved = await issueService.SaveIssueAsync(dto);
        return StatusCode(201, new ResponseModel { Stat = 1, Message = MessageConstants.IssueAdded, Result = saved });
    }

    [HttpGet("viewIssues")]
    public async Task<ResponseModel> ViewIssues() =>
        new() { Stat = 1, Message = MessageConstants.IssuesFetched, Result = await issueService.GetAllIssuesAsync() };

    [HttpGet("viewIssue/{id}")]
    public async Task<ResponseModel> ViewIssue(long id) =>
        new() { Stat = 1, Message = MessageConstants.IssuesFetched, Result = await issueService.GetIssueByIdAsync(id) };

    [HttpPut("updateIssue/{id}")]
    public async Task<IActionResult> UpdateIssue(long id, [FromBody] IssueRequestDto dto)
    {
        var validationError = ValidateRequest(dto);
        if (validationError != null)
            return BadRequest(Fail(validationError));

        var updated = await issueService.UpdateIssueAsync(id, dto);
        return Ok(new ResponseModel { Stat = 1, Message = MessageConstants.IssueUpdated, Result = updated });
    }

    [HttpDelete("deleteIssue/{id}")]
    public async Task<ResponseModel> DeleteIssue(long id)
    {
        await issueService.DeleteIssueAsync(id);
        return new() { Stat = 1, Message = MessageConstants.IssueDeleted };
    }

    private static string? ValidateRequest(IssueRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.IssueTitle))   return MessageConstants.IssueTitleNull;
        if (string.IsNullOrWhiteSpace(dto.ProjectName))  return MessageConstants.ProjectNameRequired;
        if (string.IsNullOrWhiteSpace(dto.Status))       return MessageConstants.StatusRequired;
        if (string.IsNullOrWhiteSpace(dto.Priority))     return MessageConstants.PriorityRequired;
        if (string.IsNullOrWhiteSpace(dto.Description))  return MessageConstants.DescriptionRequired;
        if (string.IsNullOrWhiteSpace(dto.RootCause))    return MessageConstants.RootCauseRequired;
        if (string.IsNullOrWhiteSpace(dto.Solution))     return MessageConstants.SolutionRequired;
        return null;
    }

    private static ResponseModel Fail(string reason) =>
        new() { Stat = 0, Message = MessageConstants.Failed, Reason = reason };
}
