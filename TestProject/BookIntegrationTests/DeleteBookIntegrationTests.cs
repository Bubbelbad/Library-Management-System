using Application.Commands.BookCommands.DeleteBook;
using Application.Dtos;
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
    [Category("Book/Integration/DeleteBook")]
    internal class DeleteBookIntegrationTests
    {
        private DeleteBookCommandHandler _handler;
        private DatabaseContext _database;
        private IMapper _mapper;
        private IGenericRepository<Book, Guid> _repository;

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
            _repository = new GenericRepository<Book, Guid>(_database);

            // Set up AutoMapper with actual configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BookDto, Book>()
                   .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => ExampleBookId));
            });
            _mapper = config.CreateMapper();

            var book = new Book
            {
                BookId = ExampleBookId,
                Title = "Test",
                Description = "An example book for Testing"
            };

            _database.Books.Add(book);
            _database.SaveChanges();

            // Initialize the handler with the actual repository and mapper
            _handler = new DeleteBookCommandHandler(_repository, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidInputId_ReturnsTrue()
        {
            // Arrange
            var command = new DeleteBookCommand(ExampleBookId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(true));
        }

        [Test]
        public async Task Handle_NonExistingBookId_ReturnsFalse()
        {
            // Arrange
            var command = new DeleteBookCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}
