using Application.Commands.AuthorCommands.Add;
using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Dtos;
using Application.Dtos.AuthorDtos;
using Application.Interfaces.RepositoryInterfaces;
using Application.Mappings;
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
    [Category("Author/Integration/UpdateAuthor")]
    public class UpdateAuthorIntegrationTests
    {
        private DatabaseContext _database;
        private IGenericRepository<Author, Guid> _repository;
        private IMapper _mapper;
        private IMediator _mediator;

        private static readonly Guid ExampleAuthorId = new("12345678-1234-1234-1234-1234567890ab");
        private readonly Author ExampleAuthorDto = new()
        {
            AuthorId = ExampleAuthorId,
            FirstName = "Test",
            LastName = "Testsson",
            DateOfBirth = DateTime.Now,
            Biography = "Biography"
        };

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
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<AddAuthorDto, Author>()
            //       .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => ExampleAuthorId));
            //});
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AuthorMappingProfiles>();
            }).CreateMapper();

            // Register services
            services.AddSingleton(_repository);
            services.AddSingleton(_mapper);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateAuthorCommandHandler).Assembly));
            services.AddTransient<IValidator<UpdateAuthorCommand>, UpdateAuthorCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register logging services
            services.AddLogging();

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();

            _database.Authors.Add(ExampleAuthorDto);
            _database.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_MissingFirstName_ReturnsNull()
        {
            // Arrange
            var invalidAuthorDto = new UpdateAuthorDto
            {
                AuthorId = ExampleAuthorId,
                FirstName = null!,
                LastName = "Updatedsson",
                DateOfBirth = DateTime.Now,
                Biography = "Biography"
            };

            var command = new UpdateAuthorCommand(invalidAuthorDto);

            // Act
            var result = await _mediator.Send(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }

        [Test]
        public async Task Handle_ValidInput_ResurnsTrue()
        {
            // Arrange
            var updatedAuthor = new UpdateAuthorDto
            {
                AuthorId = ExampleAuthorId,
                FirstName = "Test",
                LastName = "Updatesson",
                DateOfBirth = DateTime.Now,
                Biography = "Biography"
            };

            // Act
            var result = await _mediator.Send(new UpdateAuthorCommand(updatedAuthor), CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.FirstName, Is.EqualTo(ExampleAuthorDto.FirstName));
            Assert.That(result.Data.LastName, Is.EqualTo(ExampleAuthorDto.LastName));
        }

        [Test]
        public async Task Handle_EmptyGuid_ReturnsIsFailure()
        {
            // Arrange
            var authorToTest = new UpdateAuthorDto
            {
                AuthorId = Guid.Empty,
                FirstName = "Test",
                LastName = "Updatesson",
                DateOfBirth = DateTime.Now,
                Biography = "Biography"
            };
            var command = new UpdateAuthorCommand(authorToTest);

            // Act
            var result = await _mediator.Send(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Id is required, cannot be empty."));
        }
    }
}
