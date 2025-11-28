using Application.Commands.BookCommands.AddBook;
using Application.Dtos.BookDtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;


namespace TestProject.BookUnitTests
{
    [TestFixture]
    [Category("Book/UnitTests/AddBook")]
    public class AddBookUnitTest
    {
        private readonly AddBookCommandHandler _handler;

        private static readonly Guid ExampleBookId = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
        private static readonly AddBookDto ExampleBookDto = new()
        {
            Title = "Test",
            Genre = "Fantasy",
            Description = "An example book for Testing",
            AuthorId = Guid.Parse("12345678-1234-1234-1234-1234567890ab")
        };

        [SetUp]
        public void SetUp()
        {

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
            };
            var command = new AddBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}
