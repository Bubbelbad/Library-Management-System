using Application.Commands.BookCommands.UpdateBook;
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
    [Category("Book/Integration/UpdateBook")]
    public class UpdateBookIntegrationTests
    {
        private UpdateBookCommandHandler _handler;
        private DatabaseContext _database;
        private IGenericRepository<Book, Guid> _repository;
        private IMapper _mapper;

        private static readonly Guid ExampleBookId = new Guid("12345678-1234-1234-1234-1234567890ab");
        private static readonly BookDto ExampleBookDto = new(ExampleBookId, "Test", Guid.NewGuid(), "Description");

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
                cfg.CreateMap<BookDto, Book>()
                   .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => ExampleBookId));
                cfg.CreateMap<Book, GetBookDto>();
            });
            _mapper = config.CreateMapper();

            Book book = new()
            {
                BookId = ExampleBookId,
                Title = "Test",
                Genre = "Fantasy",
                AuthorId = Guid.NewGuid(),
                Description = "Description"
            };

            _database.Books.Add(book);
            _database.SaveChanges();

            // Initialize the handler with the actual repository and mapper
            _handler = new UpdateBookCommandHandler(_repository, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidInput_ReturnsBook()
        {
            // Arrange
            UpdateBookDto bookToTest = new UpdateBookDto
            {
                BookId = ExampleBookId,
                Title = "Test",
                Description = "New Description",
                Genre = "Fantasy",
                AuthorId = Guid.NewGuid()
            };
            var command = new UpdateBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.Description, Is.EqualTo(bookToTest.Description));
        }


        [Test]
        public async Task Handle_NullInput_ReturnsNull()
        {
            // Arrange
            UpdateBookDto bookToTest = null!; // Use null-forgiving operator to explicitly indicate null
            var command = new UpdateBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Data, Is.EqualTo(null));
        }


        [Test]
        public async Task Handle_MissingTitle_ReturnsNull()
        {
            // Arrange
            UpdateBookDto bookToTest = new UpdateBookDto
            {
                BookId = new Guid("12345678-1234-5678-1234-567812345678"),
                Title = null!,
                Genre = "Fantasy",
                Description = "BookService for Testing",
                AuthorId = Guid.NewGuid()
            };
            var command = new UpdateBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Data, Is.EqualTo(null));
        }
    }
}
