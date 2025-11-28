using Application.Interfaces.RepositoryInterfaces;
using Application.Queries.AuthorQueries.GetAuthorById;
using AutoMapper;
using Domain.Entities.Core;

namespace TestProject.AuthorUnitTests
{
    [TestFixture]
    [Category("Author/UnitTests/GetAuthorById")]
    public class GetAuthorUnitTest
    {

        private readonly GetAuthorByIdQueryHandler _handler;

        private static readonly Guid ExampleAuthorId = Guid.Parse("fc22325e-0fa3-4615-aa6c-c7fe459a2735");
        private static readonly Author ExampleAuthor = new()  { AuthorId = ExampleAuthorId, FirstName = "Test", LastName = "AuthorId" };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Handle_ValidId_ReturnsCorrectAuthor()
        {
            // Arrange
            var query = new GetAuthorByIdQuery(ExampleAuthorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.AuthorId, Is.EqualTo(ExampleAuthorId));
        }

        [Test]
        public async Task Handle_InvalidId_ReturnsNull()
        {
            // Arrange
            var invalidAuthorId = Guid.NewGuid();
            var query = new GetAuthorByIdQuery(invalidAuthorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Data, Is.Null);
        }
    }
}
