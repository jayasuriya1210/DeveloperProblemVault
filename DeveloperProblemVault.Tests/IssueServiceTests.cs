using DeveloperProblemVault.Api;
using DeveloperProblemVault.Api.DTOs;
using DeveloperProblemVault.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace DeveloperProblemVault.Tests;

public class IssueServiceTests
{
    private readonly Mock<IIssueManager> _managerMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ILogger<IssueService>> _loggerMock;
    private readonly IssueService _service;

    public IssueServiceTests()
    {
        _managerMock = new Mock<IIssueManager>();
        _cacheMock   = new Mock<IDistributedCache>();
        _loggerMock  = new Mock<ILogger<IssueService>>();
        _service     = new IssueService(_managerMock.Object, _cacheMock.Object, _loggerMock.Object);
    }

    private static IssueRequestDto SampleRequest() => new()
    {
        IssueTitle  = "Test Issue",
        ProjectName = "Test Project",
        Status      = "Open",
        Priority    = "High",
        Description = "Test Description",
        RootCause   = "Test Root Cause",
        Solution    = "Test Solution"
    };

    private static Issue SampleIssue(long id = 1) => new()
    {
        Id          = id,
        IssueTitle  = "Test Issue",
        ProjectName = "Test Project",
        Status      = "Open",
        Priority    = "High",
        Description = "Test Description",
        RootCause   = "Test Root Cause",
        Solution    = "Test Solution"
    };

    [Fact]
    public async Task SaveIssueAsync_ReturnsDto_WhenSaved()
    {
        var issue = SampleIssue();
        _managerMock.Setup(m => m.InsertIssueAsync(It.IsAny<Issue>())).ReturnsAsync(issue);

        var result = await _service.SaveIssueAsync(SampleRequest());

        Assert.Equal(issue.Id, result.Id);
        Assert.Equal(issue.IssueTitle, result.IssueTitle);
        _cacheMock.Verify(c => c.RemoveAsync("issues:all", default), Times.Once);
    }

    [Fact]
    public async Task GetIssueByIdAsync_ReturnsCachedDto_WhenCacheHit()
    {
        var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new IssueResponseDto { Id = 1, IssueTitle = "Cached" });
        _cacheMock.Setup(c => c.GetAsync("issues:1", default)).ReturnsAsync(json);

        var result = await _service.GetIssueByIdAsync(1);

        Assert.Equal("Cached", result.IssueTitle);
        _managerMock.Verify(m => m.GetIssueByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task GetIssueByIdAsync_ReturnsDto_WhenCacheMiss()
    {
        _cacheMock.Setup(c => c.GetAsync("issues:1", default)).ReturnsAsync((byte[]?)null);
        _managerMock.Setup(m => m.GetIssueByIdAsync(1)).ReturnsAsync(SampleIssue());

        var result = await _service.GetIssueByIdAsync(1);

        Assert.Equal(1, result.Id);
        Assert.Equal("Test Issue", result.IssueTitle);
    }

    [Fact]
    public async Task GetIssueByIdAsync_Throws_WhenNotFound()
    {
        _cacheMock.Setup(c => c.GetAsync("issues:99", default)).ReturnsAsync((byte[]?)null);
        _managerMock.Setup(m => m.GetIssueByIdAsync(99)).ReturnsAsync((Issue?)null);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetIssueByIdAsync(99));
    }

    [Fact]
    public async Task GetAllIssuesAsync_ReturnsList_WhenCacheMiss()
    {
        _cacheMock.Setup(c => c.GetAsync("issues:all", default)).ReturnsAsync((byte[]?)null);
        _managerMock.Setup(m => m.GetAllIssuesAsync()).ReturnsAsync(new List<Issue> { SampleIssue(1), SampleIssue(2) });

        var result = await _service.GetAllIssuesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateIssueAsync_ReturnsUpdatedDto_WhenExists()
    {
        _managerMock.Setup(m => m.ExistsByIdAsync(1)).ReturnsAsync(true);
        _managerMock.Setup(m => m.UpdateIssueAsync(1, It.IsAny<Issue>())).ReturnsAsync(SampleIssue());

        var result = await _service.UpdateIssueAsync(1, SampleRequest());

        Assert.Equal(1, result.Id);
        Assert.Equal("Test Issue", result.IssueTitle);
    }

    [Fact]
    public async Task UpdateIssueAsync_Throws_WhenNotFound()
    {
        _managerMock.Setup(m => m.ExistsByIdAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateIssueAsync(99, SampleRequest()));
    }

    [Fact]
    public async Task DeleteIssueAsync_Deletes_WhenExists()
    {
        _managerMock.Setup(m => m.ExistsByIdAsync(1)).ReturnsAsync(true);
        _managerMock.Setup(m => m.DeleteIssueAsync(1)).Returns(Task.CompletedTask);

        await _service.DeleteIssueAsync(1);

        _managerMock.Verify(m => m.DeleteIssueAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteIssueAsync_Throws_WhenNotFound()
    {
        _managerMock.Setup(m => m.ExistsByIdAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteIssueAsync(99));
    }
}
