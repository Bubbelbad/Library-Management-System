using Application.Dtos.BookDtos;
using Application.Interfaces.RepositoryInterfaces;
using Application.Queries.BookQueries.GetBookById;
using AutoMapper;
using Domain.Entities.Core;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TestProject.BookIntegrationTests
{
    [TestFixture]
    [Category("Book/Integration/GetBook")]
    public class GetBookIntegrationTests
    {
        private GetBookByIdQueryHandler _handler;
        private DatabaseContext _database;
        private IGenericRepository<Book, Guid> _repository;
        private IMapper _mapper;

        private static readonly Guid ExampleBookId = new Guid("12345678-1234-1234-1234-1234567890ab");

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _database = new DatabaseContext(options);

            // Initialize the repository with the in-memory database
            _repository = new GenericRepository<Book, Guid>(_database); // Use a concrete implementation

            // Set up AutoMapper with actual configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Book, GetBookDto>()
                   .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => ExampleBookId));
            });
            _mapper = config.CreateMapper();

            // Add a book with ExampleBookId to the database
            var book = new Book
            {
                BookId = ExampleBookId,
                Title = "Test",
                Genre = "Fantasy",
                Description = "Descriptive indeed",
                AuthorId = new Guid("8e0feced-361a-4c63-8009-67b9b467d130")
            };

            _database.Books.Add(book);
            _database.SaveChanges();

            // Initialize the handler with the actual repository and mapper
            _handler = new GetBookByIdQueryHandler(_repository, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidId_ReturnsCorrectBook()
        {
            // Arrange
            var query = new GetBookByIdQuery(ExampleBookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data.BookId, Is.EqualTo(ExampleBookId));
        }

        [Test]
        public async Task Handle_NonExisitingId_ReturnsNull()
        {
            // Arrange
            var query = new GetBookByIdQuery(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}
