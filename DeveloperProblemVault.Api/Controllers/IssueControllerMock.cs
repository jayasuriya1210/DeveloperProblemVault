using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
[Authorize]
public class IssueControllerMock(IssueService issueService, ILogger<IssueControllerMock> logger) : ControllerBase
{
    [HttpPost("addIssue")]
    public async Task<IActionResult> AddIssue([FromBody] IssueRequestDto dto)
    {
        var validationError = ValidateRequest(dto);
        if (validationError != null)
        {
            logger.LogWarning("Add issue validation failed - {Reason}", validationError);
            return BadRequest(ResponseHelper.Fail(validationError));
        }

        var saved = await issueService.SaveIssueAsync(dto);
        logger.LogInformation("Issue added successfully with ID: {Id}", saved.Id);
        return StatusCode(201, ResponseHelper.Success(MessageConstants.IssueAdded, saved));
    }

    [HttpGet("viewIssues")]
    public async Task<IActionResult> ViewIssues()
    {
        var issues = await issueService.GetAllIssuesAsync();
        return Ok(ResponseHelper.Success(MessageConstants.IssuesFetched, issues));
    }

    [HttpGet("viewIssue/{id}")]
    public async Task<IActionResult> ViewIssue(long id)
    {
        var issue = await issueService.GetIssueByIdAsync(id);
        return Ok(ResponseHelper.Success(MessageConstants.IssuesFetched, issue));
    }

    [HttpPut("updateIssue/{id}")]
    public async Task<IActionResult> UpdateIssue(long id, [FromBody] IssueRequestDto dto)
    {
        var validationError = ValidateRequest(dto);
        if (validationError != null)
        {
            logger.LogWarning("Update issue validation failed for ID: {Id} - {Reason}", id, validationError);
            return BadRequest(ResponseHelper.Fail(validationError));
        }

        var updated = await issueService.UpdateIssueAsync(id, dto);
        logger.LogInformation("Issue updated successfully for ID: {Id}", id);
        return Ok(ResponseHelper.Success(MessageConstants.IssueUpdated, updated));
    }

    [HttpDelete("deleteIssue/{id}")]
    public async Task<IActionResult> DeleteIssue(long id)
    {
        await issueService.DeleteIssueAsync(id);
        logger.LogInformation("Issue deleted successfully for ID: {Id}", id);
        return Ok(ResponseHelper.Success(MessageConstants.IssueDeleted));
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
}
