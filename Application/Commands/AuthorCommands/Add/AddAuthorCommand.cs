using Application.Dtos;
using Application.Models;
using Domain.Entities.Core;
using MediatR;

namespace Application.Commands.AuthorCommands.Add
{
    public class AddAuthorCommand(AddAuthorDto author) : IRequest<OperationResult<Author>>
    {
        public AddAuthorDto NewAuthor { get; set; } = author;
    }
}
