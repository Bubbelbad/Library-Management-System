using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities.Core;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book, Guid>, IBookRepository
    {
        private readonly DatabaseContext _realDatabase;

        public BookRepository(DatabaseContext database) : base(database)
        {
            _realDatabase = database;
        }
    }
}
