using Application.Commands.BookCommands.UpdateBook;
using Application.Dtos.BookDtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;

namespace TestProject.BookUnitTests
{
    [TestFixture]
    [Category("Book/UnitTests/UpdateBook")]
    public class UpdateBookUnitTest
    {
        private readonly UpdateBookCommandHandler _handler;


        private static readonly Guid ExampleBookId = Guid.Parse("3e2e66cf-5ba6-4cd0-88a1-c37b71cca899");
        private static readonly Book ExampleBook = new()
        {
            BookId = ExampleBookId,
            Title = "Test",
            Genre = "Fantasy",
            Description = "Test",
            AuthorId = Guid.NewGuid(),
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Handle_ValidInput_ReturnsBook()
        {
            // Arrange
            UpdateBookDto bookToTest = new ()
            {
                BookId = ExampleBookId,
                Title = "Test",
                Genre = "Fantasy",
                Description = "Test",
                AuthorId = Guid.NewGuid(),
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
            UpdateBookDto bookToTest = new()
            {
                BookId = new Guid("12345678-1234-5678-1234-567812345678"),
                Title = null!,
                AuthorId = Guid.NewGuid(),
                Description = "BookService for Testing"
            };
            var command = new UpdateBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Data, Is.EqualTo(null));
        }
    }
}
