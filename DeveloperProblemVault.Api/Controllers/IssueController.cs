using DeveloperProblemVault.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperProblemVault.Api.Controllers;

[ApiController]
[Authorize]
public class IssueController(IssueService issueService, ILogger<IssueController> logger) : ControllerBase
{
    [HttpPost("addIssue")]
    public async Task<IActionResult> AddIssue([FromBody] IssueRequestDto dto)
    {
        logger.LogInformation("Add issue request received for {IssueTitle}", dto.IssueTitle);

        var validationError = ValidateRequest(dto);
        if (validationError != null)
        {
            logger.LogWarning("Add issue validation failed - {Reason}", validationError);
            return BadRequest(Fail(validationError));
        }

        try
        {
            var saved = await issueService.SaveIssueAsync(dto);
            logger.LogInformation("Issue added successfully with Id {Id}", saved.Id);
            return StatusCode(201, new ResponseModel { Stat = 1, Message = MessageConstants.IssueAdded, Result = saved });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while adding issue");
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
    }

    [HttpGet("viewIssues")]
    public async Task<IActionResult> ViewIssues()
    {
        logger.LogInformation("View all issues request received");
        try
        {
            var issues = await issueService.GetAllIssuesAsync();
            return Ok(new ResponseModel { Stat = 1, Message = MessageConstants.IssuesFetched, Result = issues });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching issues");
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
    }

    [HttpGet("viewIssue/{id}")]
    public async Task<IActionResult> ViewIssue(long id)
    {
        logger.LogInformation("View issue request received for Id {Id}", id);
        try
        {
            var issue = await issueService.GetIssueByIdAsync(id);
            return Ok(new ResponseModel { Stat = 1, Message = MessageConstants.IssuesFetched, Result = issue });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Issue not found for Id {Id} - {Reason}", id, ex.Message);
            return BadRequest(Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while fetching issue with Id {Id}", id);
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
    }

    [HttpPut("updateIssue/{id}")]
    public async Task<IActionResult> UpdateIssue(long id, [FromBody] IssueRequestDto dto)
    {
        logger.LogInformation("Update issue request received for Id {Id}", id);

        var validationError = ValidateRequest(dto);
        if (validationError != null)
        {
            logger.LogWarning("Update issue validation failed for Id {Id} - {Reason}", id, validationError);
            return BadRequest(Fail(validationError));
        }

        try
        {
            var updated = await issueService.UpdateIssueAsync(id, dto);
            logger.LogInformation("Issue updated successfully for Id {Id}", id);
            return Ok(new ResponseModel { Stat = 1, Message = MessageConstants.IssueUpdated, Result = updated });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Update failed for Id {Id} - {Reason}", id, ex.Message);
            return BadRequest(Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while updating issue with Id {Id}", id);
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
    }

    [HttpDelete("deleteIssue/{id}")]
    public async Task<IActionResult> DeleteIssue(long id)
    {
        logger.LogInformation("Delete issue request received for Id {Id}", id);
        try
        {
            await issueService.DeleteIssueAsync(id);
            logger.LogInformation("Issue deleted successfully for Id {Id}", id);
            return Ok(new ResponseModel { Stat = 1, Message = MessageConstants.IssueDeleted });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Delete failed for Id {Id} - {Reason}", id, ex.Message);
            return BadRequest(Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while deleting issue with Id {Id}", id);
            return StatusCode(500, Fail("An unexpected error occurred"));
        }
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
