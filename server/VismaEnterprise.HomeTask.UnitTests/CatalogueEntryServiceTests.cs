using Moq;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Repositories;
using VismaEnterprise.HomeTask.Domain.Services;
using VismaEnterprise.HomeTask.Domain.ValueObjects;
using Xunit;

namespace VismaEnterprise.HomeTask.UnitTests;

[Collection(nameof(CatalogueEntryServiceTests))]
public class CatalogueEntryServiceTests
{
    private readonly Mock<ICatalogueEntryRepository> _mockCatalogueEntryRepository;
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CatalogueEntryService _catalogueEntryService;

    private readonly User _defaultUser;
    private readonly User _otherUser;

    private readonly Guid _defaultUserAccountId;
    private readonly Guid _otherUserAccountId;

    public CatalogueEntryServiceTests()
    {
        _mockCatalogueEntryRepository = new Mock<ICatalogueEntryRepository>();
        _mockBookRepository = new Mock<IBookRepository>();
        _mockUserRepository = new Mock<IUserRepository>();

        _catalogueEntryService = new CatalogueEntryService(
            _mockCatalogueEntryRepository.Object,
            _mockBookRepository.Object,
            _mockUserRepository.Object);

        _defaultUser = User.Create("DefaultUser", UserAccount.Create("DefaultUser", "password", true));
        _otherUser = User.Create("OtherUser", UserAccount.Create("OtherUser", "password", false));

        _defaultUserAccountId = _defaultUser.UserAccount.PublicId;
        _otherUserAccountId = _otherUser.UserAccount.PublicId;
    }

    private CatalogueEntry CreateSampleCatalogueEntry(User? user = null)
    {
        return CatalogueEntry.Create(user ?? _defaultUser, Book.Create("Sample Title", Author.Create("Name Surname"), 2023, Genre.Fiction), 5);
    }

    [Fact]
    public async Task GetEntriesAsync_IsAdmin_ReturnsAllEntries()
    {
        // Arrange
        var expectedEntries = new List<CatalogueEntry>
        {
            CreateSampleCatalogueEntry(),
            CreateSampleCatalogueEntry(_otherUser)
        };

        _mockCatalogueEntryRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedEntries);

        // Act
        var result = await _catalogueEntryService.GetEntriesAsync(_defaultUserAccountId, true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEntries.Count, result.Count);

        _mockCatalogueEntryRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockCatalogueEntryRepository.Verify(r => r.GetAllByUserAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetEntriesAsync_IsNotAdmin_ReturnsEntriesForUser()
    {
        // Arrange
        var expectedEntries = new List<CatalogueEntry>
        {
            CreateSampleCatalogueEntry()
        };

        _mockCatalogueEntryRepository
            .Setup(r => r.GetAllByUserAsync(_defaultUserAccountId))
            .ReturnsAsync(expectedEntries);

        // Act
        var result = await _catalogueEntryService.GetEntriesAsync(_defaultUserAccountId, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEntries.Count, result.Count);

        _mockCatalogueEntryRepository.Verify(r => r.GetAllAsync(), Times.Never);
        _mockCatalogueEntryRepository.Verify(r => r.GetAllByUserAsync(_defaultUserAccountId), Times.Once);
    }

    [Fact]
    public async Task GetEntryAsync_EntryExistsAndUserIsAdmin_ReturnsEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var expectedEntry = CreateSampleCatalogueEntry(_otherUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(expectedEntry);

        // Act
        var result = await _catalogueEntryService.GetEntryAsync(_defaultUserAccountId, true, entryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEntry.PublicId, result.PublicId);

        _mockCatalogueEntryRepository.Verify(r => r.GetByIdAsync(entryId), Times.Once);
    }

    [Fact]
    public async Task GetEntryAsync_EntryExistsAndUserOwnsEntry_ReturnsEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var expectedEntry = CreateSampleCatalogueEntry(_defaultUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(expectedEntry);

        // Act
        var result = await _catalogueEntryService.GetEntryAsync(_defaultUserAccountId, false, entryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEntry.PublicId, result.PublicId);

        _mockCatalogueEntryRepository.Verify(r => r.GetByIdAsync(entryId), Times.Once);
    }

    [Fact]
    public async Task GetEntryAsync_EntryNotFound_ReturnsNull()
    {
        // Arrange
        var entryId = Guid.NewGuid();

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync((CatalogueEntry?)null);

        // Act
        var result = await _catalogueEntryService.GetEntryAsync(_defaultUserAccountId, true, entryId);

        // Assert
        Assert.Null(result);

        _mockCatalogueEntryRepository.Verify(r => r.GetByIdAsync(entryId), Times.Once);
    }

    [Fact]
    public async Task GetEntryAsync_UserDoesNotOwnEntryAndIsNotAdmin_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entry = CreateSampleCatalogueEntry(_otherUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(entry);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _catalogueEntryService.GetEntryAsync(_defaultUserAccountId, false, entryId));

        _mockCatalogueEntryRepository.Verify(r => r.GetByIdAsync(entryId), Times.Once);
    }

    [Fact]
    public async Task UpdateEntryAsync_EntryExistsAndUserIsAdmin_UpdatesAndReturnsEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var originalEntry = CreateSampleCatalogueEntry(_otherUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(originalEntry);

        _mockBookRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        _mockCatalogueEntryRepository
            .Setup(r => r.UpdateAsync(It.IsAny<CatalogueEntry>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _catalogueEntryService.UpdateEntryAsync(_defaultUserAccountId, true, entryId, 2025, Genre.ScienceFiction, 4);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2025u, result.Book.PublishedYear);
        Assert.Equal(Genre.ScienceFiction, result.Book.Genre);
        Assert.Equal(4u, result.Mark);

        _mockBookRepository.Verify(r => r.UpdateAsync(originalEntry.Book), Times.Once);
        _mockCatalogueEntryRepository.Verify(r => r.UpdateAsync(originalEntry), Times.Once);
    }
    
    [Fact]
    public async Task UpdateEntryAsync_EntryExistsAndUserOwnsEntry_UpdatesAndReturnsEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var originalEntry = CreateSampleCatalogueEntry(_defaultUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(originalEntry);

        _mockBookRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        _mockCatalogueEntryRepository
            .Setup(r => r.UpdateAsync(It.IsAny<CatalogueEntry>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _catalogueEntryService.UpdateEntryAsync(_defaultUserAccountId, false, entryId, 2025, Genre.ScienceFiction, 4);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2025u, result.Book.PublishedYear);
        Assert.Equal(Genre.ScienceFiction, result.Book.Genre);
        Assert.Equal(4u, result.Mark);

        _mockBookRepository.Verify(r => r.UpdateAsync(originalEntry.Book), Times.Once);
        _mockCatalogueEntryRepository.Verify(r => r.UpdateAsync(originalEntry), Times.Once);
    }

    [Fact]
    public async Task UpdateEntryAsync_EntryNotFound_ReturnsNull()
    {
        // Arrange
        var entryId = Guid.NewGuid();

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync((CatalogueEntry?)null);

        // Act
        var result = await _catalogueEntryService.UpdateEntryAsync(_defaultUserAccountId, false, entryId, 2025, Genre.ScienceFiction, 4);

        // Assert
        Assert.Null(result);

        _mockBookRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        _mockCatalogueEntryRepository.Verify(r => r.UpdateAsync(It.IsAny<CatalogueEntry>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateEntryAsync_UserDoesNotOwnEntryAndIsNotAdmin_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entry = CreateSampleCatalogueEntry(_otherUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(entry);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _catalogueEntryService.UpdateEntryAsync(_defaultUserAccountId, false, entryId, 2025, Genre.ScienceFiction, 4));

        _mockBookRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        _mockCatalogueEntryRepository.Verify(r => r.UpdateAsync(It.IsAny<CatalogueEntry>()), Times.Never);
    }

    [Fact]
    public async Task CreateEntryAsync_UserFound_CreatesAndReturnsEntry()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.GetByUserAccountIdAsync(_defaultUserAccountId))
            .ReturnsAsync(_defaultUser);

        _mockBookRepository
            .Setup(r => r.AddAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        _mockCatalogueEntryRepository
            .Setup(r => r.AddAsync(It.IsAny<CatalogueEntry>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _catalogueEntryService.CreateEntryAsync(_defaultUserAccountId, "New Book", "New Author", 2024, Genre.History, 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Book", result.Book.Title);
        Assert.Equal("New", result.Book.Author.Name);
        Assert.Equal("Author", result.Book.Author.Surname);
        Assert.Equal(2024u, result.Book.PublishedYear);
        Assert.Equal(Genre.History, result.Book.Genre);
        Assert.Equal(3u, result.Mark);
        Assert.Equal(_defaultUser.PublicId, result.User.PublicId);

        _mockBookRepository.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        _mockCatalogueEntryRepository.Verify(r => r.AddAsync(It.IsAny<CatalogueEntry>()), Times.Once);
    }

    [Fact]
    public async Task CreateEntryAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockUserRepository
            .Setup(r => r.GetByUserAccountIdAsync(_defaultUserAccountId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _catalogueEntryService.CreateEntryAsync(_defaultUserAccountId, "Title", "Author", 2023, Genre.Fiction, 5));

        _mockBookRepository.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        _mockCatalogueEntryRepository.Verify(r => r.AddAsync(It.IsAny<CatalogueEntry>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEntryAsync_EntryExistsAndUserIsAdmin_DeletesAndReturnsEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entryToDelete = CreateSampleCatalogueEntry(_otherUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(entryToDelete);

        _mockCatalogueEntryRepository
            .Setup(r => r.DeleteAsync(entryToDelete))
            .Returns(Task.CompletedTask);

        _mockBookRepository
            .Setup(r => r.DeleteAsync(entryToDelete.Book))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _catalogueEntryService.DeleteEntryAsync(_defaultUserAccountId, true, entryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entryToDelete.PublicId, result.PublicId);

        _mockCatalogueEntryRepository.Verify(r => r.DeleteAsync(entryToDelete), Times.Once);
        _mockBookRepository.Verify(r => r.DeleteAsync(entryToDelete.Book), Times.Once);
    }
    
    [Fact]
    public async Task DeleteEntryAsync_EntryExistsAndUserOwnsEntry_DeletesAndReturnsEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entryToDelete = CreateSampleCatalogueEntry(_defaultUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(entryToDelete);

        _mockCatalogueEntryRepository
            .Setup(r => r.DeleteAsync(entryToDelete))
            .Returns(Task.CompletedTask);

        _mockBookRepository
            .Setup(r => r.DeleteAsync(entryToDelete.Book))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _catalogueEntryService.DeleteEntryAsync(_defaultUserAccountId, false, entryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entryToDelete.PublicId, result.PublicId);

        _mockCatalogueEntryRepository.Verify(r => r.DeleteAsync(entryToDelete), Times.Once);
        _mockBookRepository.Verify(r => r.DeleteAsync(entryToDelete.Book), Times.Once);
    }

    [Fact]
    public async Task DeleteEntryAsync_EntryNotFound_ReturnsNull()
    {
        // Arrange
        var entryId = Guid.NewGuid();

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync((CatalogueEntry?)null);

        // Act
        var result = await _catalogueEntryService.DeleteEntryAsync(_defaultUserAccountId, true, entryId);

        // Assert
        Assert.Null(result);

        _mockCatalogueEntryRepository.Verify(r => r.DeleteAsync(It.IsAny<CatalogueEntry>()), Times.Never);
        _mockBookRepository.Verify(r => r.DeleteAsync(It.IsAny<Book>()), Times.Never);
    }
    
    [Fact]
    public async Task DeleteEntryAsync_UserDoesNotOwnEntryAndIsNotAdmin_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entry = CreateSampleCatalogueEntry(_otherUser);

        _mockCatalogueEntryRepository
            .Setup(r => r.GetByIdAsync(entryId))
            .ReturnsAsync(entry);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _catalogueEntryService.DeleteEntryAsync(_defaultUserAccountId, false, entryId));

        _mockCatalogueEntryRepository.Verify(r => r.DeleteAsync(It.IsAny<CatalogueEntry>()), Times.Never);
        _mockBookRepository.Verify(r => r.DeleteAsync(It.IsAny<Book>()), Times.Never);
    }
}