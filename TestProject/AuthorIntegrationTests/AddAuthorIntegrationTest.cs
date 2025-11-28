using Application.Commands.AuthorCommands.Add;
using Application.Dtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;
using FluentValidation;
using Infrastructure.Database;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject.AuthorIntegrationTests
{
    [TestFixture]
    [Category("Author/Integration/AddAuthor")]
    public class AddAuthorIntegrationTest
    {
        private readonly AddAuthorCommandHandler _handler;
        private DatabaseContext _database;
        private IGenericRepository<Author, Guid> _repository;
        private IMapper _mapper;
        private IMediator _mediator;

        private static readonly Guid ExampleAuthorId = new("12345678-1234-1234-1234-1234567890ab");

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

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

            // Register services
            services.AddSingleton(_repository);
            services.AddSingleton(_mapper);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddAuthorCommandHandler).Assembly));
            services.AddTransient<IValidator<AddAuthorCommand>, AddAuthorCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register logging services
            services.AddLogging();

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }


        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidInput_ReturnsCorrectAuthor()
        {
            // Arrange
            AddAuthorDto authorToTest = new("Test", "Testsson");
            var command = new AddAuthorCommand(authorToTest);

            // Act
            var result = await _mediator.Send(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.FirstName, Is.EqualTo(authorToTest.FirstName));
        }

        [Test]
        public async Task Handle_InvalidInput_ReturnsFailure()
        {
            // Arrange
            AddAuthorDto authorToTest = new(null!, "Testsson");
            var command = new AddAuthorCommand(authorToTest);

            // Act
            var result = await _mediator.Send(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("'New Author First Name' must not be empty., Name is required."));
        }

    }
}
