﻿using Application.Commands.BookCommands.AddBook;
using Application.Commands.BookCommands.DeleteBook;
using Application.Commands.BookCommands.UpdateBook;
using Application.Dtos.BookDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Application.Queries.BookQueries.GetBookById;
using Application.Queries.BookQueries.GetAllBooks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BookController(IMediator mediator, ILogger<BookController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<BookController> _logger = logger;

        [Route("GetAllBooks")]
        [HttpGet]
        [SwaggerOperation(Description = "Gets all books")]
        [SwaggerResponse(200, "Successfully retrieved Books.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "Books not found")]
        public async Task<IActionResult> GetAllBooks()
        {
            _logger.LogInformation("Fetching all Books at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

            try
            {
                var operationResult = await _mediator.Send(new GetAllBooksQuery());

                if (operationResult.IsSuccess)
                {
                    _logger.LogInformation("Successfully retrieved all Books");
                    return Ok(operationResult.Data);
                }

                _logger.LogError($"Could not fetch all books. Errors: {operationResult.ErrorMessage}");
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all Books at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [Route("GetBookById/{bookId}")]
        [HttpGet]
        [SwaggerOperation(Description = "Gets a book by Id")]
        [SwaggerResponse(200, "Successfully retrieved Book.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "Book not found")]
        public async Task<IActionResult> GetBook([FromRoute] Guid bookId)
        {
            try
            {
                var operationResult = await _mediator.Send(new GetBookByIdQuery(bookId));
                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching Book with ID: {id} at {time}", bookId, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [Route("Create")]
        [HttpPost]
        [SwaggerOperation(Description = "Adds a new Book to library")]
        [SwaggerResponse(200, "Successfully added Book.")]
        [SwaggerResponse(400, "Invalid input data.")]
        public async Task<IActionResult> AddBook([FromBody, Required] AddBookDto bookToAdd)
        {
            try
            {
                var operationResult = await _mediator.Send(new AddBookCommand(bookToAdd));
                if (operationResult.IsSuccess)
                {
                    _logger.LogInformation("Book added successfully");
                    return Ok(operationResult.Data);
                }

                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding new Book at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [Route("Update")]
        [HttpPut]
        [SwaggerOperation(Description = "Updates an existing Book in collection")]
        [SwaggerResponse(200, "Successfully Updated Book.", typeof(BookDto))]
        [SwaggerResponse(400, "Invalid input data.")]
        [SwaggerResponse(404, "Book not found.")]
        public async Task<IActionResult> UpdateBook([FromBody, Required] UpdateBookDto book)
        {
            try
            {
                var operationResult = await _mediator.Send(new UpdateBookCommand(book));
                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }

                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Book at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [Route("Delete/{id}")]
        [HttpDelete]
        [SwaggerOperation(Description = "Deletes a Book from collection")]
        [SwaggerResponse(200, "Successfully Deleted Book.")]
        [SwaggerResponse(404, "Book not found.")]
        public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
        {
            try
            {
                var operationResult = await _mediator.Send(new DeleteBookCommand(id));

                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Message);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Book at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return BadRequest(ex.InnerException);
            }
        }
    }
}
