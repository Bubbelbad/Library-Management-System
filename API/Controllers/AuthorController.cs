using Application.Commands.AuthorCommands.Add;
using Application.Commands.AuthorCommands.DeleteAuthor;
using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Dtos;
using Application.Dtos.AuthorDtos;
using Application.Queries.AuthorQueries.GetAllAuthors;
using Application.Queries.AuthorQueries.GetAuthorById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthorController(IMediator mediator, ILogger<AuthorController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<AuthorController> _logger = logger;


        // [Authorize]
        [Route("GetAllAuthors")]
        [HttpGet]
        [SwaggerOperation(Description = "Gets All Authors")]
        [SwaggerResponse(200, "Successfully retrieved Authors.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "Authors not found")]
        public async Task<IActionResult> GetAllAuthors()
        {
            _logger.LogInformation("Fetching all Authors at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

            try
            {
                var operationResult = await _mediator.Send(new GetAllAuthorsQuery());

                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all Authors at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return BadRequest(ex.InnerException);
            }
        }


        // [Authorize]
        [Route("GetAuthorById/{id}")]
        [HttpGet]
        [SwaggerOperation(Description = "Gets AuthorId by Id")]
        [SwaggerResponse(200, "Successfully retrieved AuthorId.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "AuthorId not found")]
        public async Task<IActionResult> GetAuthor([FromRoute] Guid id)
        {
            _logger.LogInformation("Fetching AuthorId with ID: {id} at {time}", id, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            try
            {
                var operationResult = await _mediator.Send(new GetAuthorByIdQuery(id));

                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching AuthorId with ID: {id} at {time}", id, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return BadRequest(ex.InnerException);
            }
        }


        [Route("Create")]
        [HttpPost]
        [SwaggerOperation(Description = "Adds a new AuthorId to library")]
        [SwaggerResponse(200, "Successfully added AuthorId.")]
        [SwaggerResponse(400, "Invalid input data")]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorDto authorToAdd)
        {
            try
            {
                var operationResult = await _mediator.Send(new AddAuthorCommand(authorToAdd));

                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding new Author at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, ex.Message);
            }
        }


        [Route("Update")]
        [HttpPut]
        [SwaggerOperation(Description = "Updates an existing AuthorId in the collection")]
        [SwaggerResponse(200, "Successfully Updated AuthorId.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "AuthorId not found")]
        public async Task<IActionResult> UpdateAuthor([FromBody, Required] UpdateAuthorDto authorToUpdate)
        {
            try
            {
                var operationResult = await _mediator.Send(new UpdateAuthorCommand(authorToUpdate));
                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }

                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating AuthorId at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [Route("Delete/{id}")]
        [HttpDelete]
        [SwaggerOperation(Description = "Deletes AuthorId from collection")]
        [SwaggerResponse(200, "Successfully Deleted AuthorId.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "AuthorId not found")]
        public async Task<IActionResult> DeleteAuthor([FromRoute] Guid id)
        {
            try
            {
                var operationResult = await _mediator.Send(new DeleteAuthorCommand(id));

                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Message);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting AuthorId with ID: {id} at {time}", id, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return StatusCode(500, ex.Message);
            }
        }
    }
}
