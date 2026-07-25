using ERP.Application.Common.Models;
using FluentAssertions;

namespace ERP.Domain.Tests.Common;

public class PagedResultTests
{
    // -----------------------------------------------------------------------
    // TotalPages
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(10, 5, 2)]    // Exact division
    [InlineData(11, 5, 3)]    // Rounds up
    [InlineData(1, 10, 1)]    // Fewer items than page size
    [InlineData(100, 10, 10)] // Exactly 10 pages
    public void TotalPages_CalculatesCorrectly(int totalCount, int pageSize, int expectedPages)
    {
        // Arrange
        var result = new PagedResult<string>([], totalCount, page: 1, pageSize);

        // Act & Assert
        result.TotalPages.Should().Be(expectedPages);
    }

    // -----------------------------------------------------------------------
    // HasPreviousPage
    // -----------------------------------------------------------------------

    [Fact]
    public void HasPreviousPage_WhenOnFirstPage_ReturnsFalse()
    {
        // Arrange
        var result = new PagedResult<string>([], totalCount: 50, page: 1, pageSize: 10);

        // Act & Assert
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_WhenOnSecondPage_ReturnsTrue()
    {
        // Arrange
        var result = new PagedResult<string>([], totalCount: 50, page: 2, pageSize: 10);

        // Act & Assert
        result.HasPreviousPage.Should().BeTrue();
    }

    // -----------------------------------------------------------------------
    // HasNextPage
    // -----------------------------------------------------------------------

    [Fact]
    public void HasNextPage_WhenOnLastPage_ReturnsFalse()
    {
        // Arrange
        var result = new PagedResult<string>([], totalCount: 50, page: 5, pageSize: 10);

        // Act & Assert
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_WhenNotOnLastPage_ReturnsTrue()
    {
        // Arrange
        var result = new PagedResult<string>([], totalCount: 50, page: 2, pageSize: 10);

        // Act & Assert
        result.HasNextPage.Should().BeTrue();
    }
}
