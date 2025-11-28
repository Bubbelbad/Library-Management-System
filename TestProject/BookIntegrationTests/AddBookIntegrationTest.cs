using Application.Commands.BookCommands.AddBook;
using Application.Dtos.BookDtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TestProject.BookIntegrationTests
{
    [TestFixture]
    [Category("Book/Integration/AddBook")]
    public class AddBookIntegrationTest
    {
        private AddBookCommandHandler _handler;
        private DatabaseContext _database;
        private IMapper _mapper;
        private IGenericRepository<Book, Guid> _repository;

        private static readonly Guid ExampleBookId = new Guid("12345678-1234-1234-1234-1234567890ab");
        private static readonly AddBookDto ExampleBookDto = new AddBookDto
        {
            Title = "Test",
            AuthorId = Guid.NewGuid(),
            Genre = "Fantasy",
            Description = "An example book for Testing"
        };

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _database = new DatabaseContext(options);

            // Initialize the repository with the in-memory database
            _repository = new GenericRepository<Book, Guid>(_database);

            // Set up AutoMapper with actual configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddBookDto, Book>();
                cfg.CreateMap<Book, GetBookDto>();
            });
            _mapper = config.CreateMapper();

            // Initialize the handler with the actual repository and mapper
            _handler = new AddBookCommandHandler(_repository, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidInput_ReturnsBook()
        {
            // Arrange
            var command = new AddBookCommand(ExampleBookDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.Description, Is.EqualTo(ExampleBookDto.Description));
        }



        [Test]
        public async Task Handle_NullInput_ReturnsNull()
        {
            // Arrange
            AddBookDto bookToTest = null!; // Use null-forgiving operator to explicitly indicate null
            var command = new AddBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }

        [Test]
        public async Task Handle_MissingTitle_ReturnsNull()
        {
            // Arrange
            AddBookDto bookToTest = new()
            {
                Title = null!,
                Genre = "Fantasy",
                AuthorId = Guid.NewGuid(),
                Description = "An example book for Testing"
            }; // Use null-forgiving operator to explicitly indicate null
            var command = new AddBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}