using Moq;
using VismaEnterprise.HomeTask.Application.Data;
using VismaEnterprise.HomeTask.Application.DTOs;
using VismaEnterprise.HomeTask.Application.Mappers;
using VismaEnterprise.HomeTask.Application.Services;
using VismaEnterprise.HomeTask.Application.Services.Interfaces;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;
using VismaEnterprise.HomeTask.Domain.ValueObjects;
using Xunit;

namespace VismaEnterprise.HomeTask.UnitTests;

[Collection(nameof(BookCatalogueServiceTests))]
public class BookCatalogueServiceTests
{
    private readonly Mock<IUserContextResolver> _mockUserContextResolver;
    private readonly Mock<ICatalogueEntryService> _mockCatalogueEntryService;
    private readonly BookCatalogueService _bookCatalogueService;

    private readonly UserContext _defaultUserContext = new() { UserAccountId = Guid.NewGuid(), IsAdmin = false };
    private readonly UserContext _adminUserContext = new() { UserAccountId = Guid.NewGuid(), IsAdmin = true };

    public BookCatalogueServiceTests()
    {
        _mockUserContextResolver = new Mock<IUserContextResolver>();
        _mockCatalogueEntryService = new Mock<ICatalogueEntryService>();

        _bookCatalogueService = new BookCatalogueService(
            _mockUserContextResolver.Object,
            _mockCatalogueEntryService.Object,
            new CatalogueEntryMapper());
    }

    private CatalogueEntry CreateSampleCatalogueEntry()
    {
        return CatalogueEntry.Create(User.Create("UserName", UserAccount.Create("UserName", "111", true)), Book.Create("Sample Title", Author.Create("Name Surname"), 2023, Genre.Fiction), 5);
    }

    [Fact]
    public async Task GetEntriesAsync_UserContextIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns((UserContext?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookCatalogueService.GetEntriesAsync());
    }

    [Fact]
    public async Task GetEntriesAsync_ValidUserContext_ReturnsMappedEntries()
    {
        // Arrange
        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        var catalogueEntries = new List<CatalogueEntry>
        {
            CreateSampleCatalogueEntry(),
            CreateSampleCatalogueEntry()
        };

        _mockCatalogueEntryService
            .Setup(s => s.GetEntriesAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin))
            .ReturnsAsync(catalogueEntries);

        // Act
        var result = await _bookCatalogueService.GetEntriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(catalogueEntries.Count, result.Count);
        CheckMapped(result);
    }

    [Fact]
    public async Task GetEntryAsync_UserContextIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns((UserContext?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookCatalogueService.GetEntryAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetEntryAsync_EntryNotFound_ReturnsNull()
    {
        // Arrange
        var entryId = Guid.NewGuid();

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.GetEntryAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin, entryId))
            .ReturnsAsync((CatalogueEntry?)null);

        // Act
        var result = await _bookCatalogueService.GetEntryAsync(entryId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEntryAsync_EntryFound_ReturnsMappedEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var catalogueEntry = CreateSampleCatalogueEntry();

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.GetEntryAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin, entryId))
            .ReturnsAsync(catalogueEntry);

        // Act
        var result = await _bookCatalogueService.GetEntryAsync(entryId);

        // Assert
        Assert.NotNull(result);
        CheckMapped(result);
    }

    [Fact]
    public async Task CreateEntryAsync_UserContextIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns((UserContext?)null);

        var dto = new CatalogueEntryMapper().Map(CreateSampleCatalogueEntry());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookCatalogueService.CreateEntryAsync(dto));
    }

    [Fact]
    public async Task CreateEntryAsync_ValidInput_ReturnsMappedCreatedEntry()
    {
        // Arrange
        var entry = CreateSampleCatalogueEntry();
        var dto = new CatalogueEntryMapper().Map(entry);

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.CreateEntryAsync(_defaultUserContext.UserAccountId, dto.Title, dto.Author, dto.PublishedYear, Enum.Parse<Genre>(dto.Genre!), dto.Mark))
            .ReturnsAsync(entry);

        // Act
        var result = await _bookCatalogueService.CreateEntryAsync(dto);

        // Assert
        Assert.NotNull(result);
        CheckMapped(result);
    }

    [Fact]
    public async Task EditEntryAsync_UserContextIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns((UserContext?)null);

        var dto = new CatalogueEntryMapper().Map(CreateSampleCatalogueEntry());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookCatalogueService.EditEntryAsync(Guid.NewGuid(), dto));
    }

    [Fact]
    public async Task EditEntryAsync_EntryNotFoundOrNotAuthorized_ReturnsNull()
    {
        // Arrange
        var entry = CreateSampleCatalogueEntry();
        var dto = new CatalogueEntryMapper().Map(entry);

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.UpdateEntryAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin, entry.PublicId, dto.PublishedYear, Enum.Parse<Genre>(dto.Genre!), dto.Mark))
            .ReturnsAsync((CatalogueEntry?)null);

        // Act
        var result = await _bookCatalogueService.EditEntryAsync(entry.PublicId, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EditEntryAsync_SuccessfulUpdate_ReturnsMappedEntry()
    {
        // Arrange
        var entry = CreateSampleCatalogueEntry();
        var dto = new CatalogueEntryMapper().Map(entry);

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.UpdateEntryAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin, entry.PublicId, dto.PublishedYear, Enum.Parse<Genre>(dto.Genre!), dto.Mark))
            .ReturnsAsync(entry);

        // Act
        var result = await _bookCatalogueService.EditEntryAsync(entry.PublicId, dto);

        // Assert
        Assert.NotNull(result);
        CheckMapped(result);
    }

    [Fact]
    public async Task DeleteEntryAsync_UserContextIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns((UserContext?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookCatalogueService.DeleteEntryAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteEntryAsync_EntryNotFoundOrNotAuthorized_ReturnsNull()
    {
        // Arrange
        var entryId = Guid.NewGuid();

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.DeleteEntryAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin, entryId))
            .ReturnsAsync((CatalogueEntry?)null);

        // Act
        var result = await _bookCatalogueService.DeleteEntryAsync(entryId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteEntryAsync_SuccessfulDeletion_ReturnsMappedEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var catalogueEntry = CreateSampleCatalogueEntry();

        _mockUserContextResolver
            .Setup(r => r.Resolve())
            .Returns(_defaultUserContext);

        _mockCatalogueEntryService
            .Setup(s => s.DeleteEntryAsync(_defaultUserContext.UserAccountId, _defaultUserContext.IsAdmin, entryId))
            .ReturnsAsync(catalogueEntry);

        // Act
        var result = await _bookCatalogueService.DeleteEntryAsync(entryId);

        // Assert
        Assert.NotNull(result);
        CheckMapped(result);
    }

    private static void CheckMapped(List<CatalogueEntryDto> result)
    {
        Assert.Contains(result, e => e.Title == "Sample Title");
        Assert.Contains(result, e => e.Author == "Name Surname");
        Assert.Contains(result, e => e.PublishedYear == 2023);
        Assert.Contains(result, e => e.Genre == "Fiction");
        Assert.Contains(result, e => e.Mark == 5);
    }

    private static void CheckMapped(CatalogueEntryDto result)
    {
        Assert.Equal("Sample Title", result.Title);
        Assert.Equal("Name Surname", result.Author);
        Assert.Equal(2023u, result.PublishedYear);
        Assert.Equal("Fiction", result.Genre);
        Assert.Equal(5u, result.Mark);
    }
}