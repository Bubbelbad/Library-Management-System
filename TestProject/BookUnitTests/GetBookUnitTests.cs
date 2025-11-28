using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;
using Application.Queries.BookQueries.GetBookById;
using Application.Dtos.BookDtos;

namespace TestProject.BookUnitTests
{
    [TestFixture]
    [Category("Book/UnitTests/GetBookById")]
    public class GetBookUnitTests
    {
        private readonly GetBookByIdQueryHandler _handler;


        private static readonly Guid ExampleBookId = Guid.Parse("2bfaf5e9-d978-464c-b778-7567ef2dde29");
        private static readonly Book ExampleBook = new()
        {
            BookId = ExampleBookId,
            Title = "Test",
            Genre = "Fantasy",
            AuthorId = Guid.NewGuid(),
            Description = "Description"
        };

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public async Task Handle_ValidId_ReturnsCorrectBook()
        {
            // Arrange
            var query = new GetBookByIdQuery(ExampleBookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Data.BookId, Is.EqualTo(ExampleBookId));
            });

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
