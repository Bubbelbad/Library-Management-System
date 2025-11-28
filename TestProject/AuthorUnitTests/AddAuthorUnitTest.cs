//using Application.Commands.AuthorCommands.Add;
//using Application.Dtos;
//using Application.Interfaces.RepositoryInterfaces;
//using AutoMapper;
//using Domain.Entities.Core;
//using Moq;

//namespace TestProject.AuthorUnitTests
//{
//    [TestFixture]
//    [Category("AuthorId/UnitTests/AddAuthor")]
//    public class AddAuthorUnitTest
//    {
//        private AddAuthorCommandHandler _handler;
//        private Mock<IAuthorRepository> _mockRepository;
//        private Mock<IMapper> _mockMapper;

//        private static readonly Guid ExampleAuthorId = Guid.Parse("12345678-1234-1234-1234-1234567890ab");

//        [SetUp]
//        public void SetUp()
//        {
//            // Initialize the mocks
//            _mockRepository = new Mock<IAuthorRepository>();
//            _mockMapper = new Mock<IMapper>();

//            // Set up the mock repository to return a new AuthorId when AddAuthor is called
//            _mockRepository.Setup(repo => repo.AddAuthor(It.IsAny<Author>()))
//                           .ReturnsAsync((Author author) => author);

//            _mockMapper.Setup(mapper => mapper.Map<Author>(It.IsAny<Author>()));
//            _mockMapper.Setup(mapper => mapper.Map<Author>(It.IsAny<Author>()))
//                       .Returns((Author author) =>
//                           new Author
//                           {
//                               AuthorId = ExampleAuthorId,
//                               FirstName = author.FirstName,
//                               LastName = author.LastName
//                           });

//            // Initialize the handler with the mock repository
//            _handler = new AddAuthorCommandHandler(_mockRepository.Object, _mockMapper.Object);
//        }


//        [Test]
//        public async Task Handle_ValidInput_ReturnsCorrectAuthor()
//        {
//            // Arrange
//            AddAuthorDto authorToTest = new("Test", "Testsson");
//            var command = new AddAuthorCommand(authorToTest);

//            // Act
//            var result = await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.Data.FirstName, Is.EqualTo(authorToTest.FirstName));
//        }

//        [Test]
//        public async Task Handle_NullInput_ReturnsNull()
//        {
//            // Arrange
//            AddAuthorDto authorToTest = null!; // Use null-forgiving operator to explicitly indicate null
//            var command = new AddAuthorCommand(authorToTest);

//            // Act
//            var result = await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.IsSuccess, Is.EqualTo(false));
//        }

//        [Test]
//        public async Task Handle_MissingFirstName_ReturnsNull()
//        {
//            // Arrange
//            AddAuthorDto authorToTest = new(null!, "Testsson"); // Use null-forgiving operator to explicitly indicate null
//            var command = new AddAuthorCommand(authorToTest);

//            // Act
//            var result = await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.IsSuccess, Is.EqualTo(false));
//        }
//    }
//}