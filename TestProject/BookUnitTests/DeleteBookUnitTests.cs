using Application.Commands.BookCommands.DeleteBook;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;

namespace TestProject.BookUnitTests
{
    [TestFixture]
    [Category("Book/UnitTests/DeleteBook")]
    public class DeleteBookUnitTest
    {
        private readonly DeleteBookCommandHandler _handler;


        private static readonly Guid ExampleBookId = Guid.Parse("783307e1-ea3b-400b-919d-0c40b2bbae78");

        [SetUp]
        public void Setup()
        {

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
