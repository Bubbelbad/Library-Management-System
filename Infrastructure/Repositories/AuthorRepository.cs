using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities.Core;
using Infrastructure.Database;

namespace Infrastructure.Repositories
{
    public class AuthorRepository : GenericRepository<Author, Guid>, IAuthorRepository
    {
        private readonly DatabaseContext _realDatabase;

        public AuthorRepository(DatabaseContext database) : base(database)
        {
            _realDatabase = database;
        }
    }
}
