using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Dtos.AuthorDtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;

namespace TestProject.AuthorUnitTests
{
    [TestFixture]
    [Category("Author/UnitTests/UpdateAuthor")]
    public class UpdateAuthorUnitTest
    {
        private readonly UpdateAuthorCommandHandler _handler;


        private static readonly Guid ExampleAuthorId = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
        private static readonly UpdateAuthorDto ExampleAuthorDto = new()
        {
            AuthorId = ExampleAuthorId,
            FirstName = "Test",
            LastName = "Testsson"
        };

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task Handle_ValidInput_ReturnsAuthor()
        {
            // Arrange
            var command = new UpdateAuthorCommand(ExampleAuthorDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Data.FirstName, Is.EqualTo(ExampleAuthorDto.FirstName));
                Assert.That(result.Data.LastName, Is.EqualTo(ExampleAuthorDto.LastName));
            });
        }

        [Test]
        public async Task Handle_EmptyGuid_ReturnsNull()
        {
            // Arrange
            UpdateAuthorDto authorToUpdate = new()
            {
                AuthorId = Guid.Empty,
                FirstName = "Test",
                LastName = "Testsson"
            };
            var command = new UpdateAuthorCommand(authorToUpdate);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }


        [Test]
        public async Task Handle_MissingFirstName_ReturnsNull()
        {
            // Arrange
            var invalidAuthorDto = new UpdateAuthorDto
            {
                AuthorId = Guid.NewGuid(),
                FirstName = null!,
                LastName = "Testsson"
            };

            var command = new UpdateAuthorCommand(invalidAuthorDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}
