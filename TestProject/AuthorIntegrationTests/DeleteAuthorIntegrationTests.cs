using Application.Commands.AuthorCommands.DeleteAuthor;
using Application.Dtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TestProject.AuthorIntegrationTests
{
    [TestFixture]
    [Category("Author/Integration/DeleteAuthor")]
    public class DeleteAuthorIntegrationTests
    {
        private DeleteAuthorCommandHandler _handler;
        private DatabaseContext _database;
        private IGenericRepository<Author, Guid> _repository;
        private IMapper _mapper;

        private static readonly Guid ExampleAuthorId = new("12345678-1234-1234-1234-1234567890ab");

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _database = new DatabaseContext(options);

            // Initialize the repository with the in-memory database
            _repository = new GenericRepository<Author, Guid>(_database);

            // Set up AutoMapper with actual configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddAuthorDto, Author>()
                   .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => ExampleAuthorId));
            });
            _mapper = config.CreateMapper();

            // Add an author with ExampleAuthorId to the database
            var author = new Author
            {
                AuthorId = ExampleAuthorId,
                FirstName = "Test",
                LastName = "AuthorId"
            };

            _database.Authors.Add(author);
            _database.SaveChanges();

            // Initialize the handler with the actual repository and mapper
            _handler = new DeleteAuthorCommandHandler(_repository, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidInputId_ReturnsTrue()
        {
            // Arrange
            var command = new DeleteAuthorCommand(ExampleAuthorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(true));
        }

        [Test]
        public async Task Handle_NonExistingBookId_ReturnsFalse()
        {
            // Arrange
            var command = new DeleteAuthorCommand(new Guid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}
