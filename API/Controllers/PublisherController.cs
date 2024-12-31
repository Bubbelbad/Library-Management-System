﻿using Application.Queries.PublisherQueries.GetAllPublishers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(IMediator mediator, ILogger<PublisherController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllPublishers")]
        [SwaggerOperation(Description = "Gets All Publishers")]
        [SwaggerResponse(200, "Successfully retrieved Publishers.")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "Publishers not found")]
        public async Task<IActionResult> GetAllPublishers()
        {
            _logger.LogInformation("Fetching all Publishers at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

            try
            {
                var operationResult = await _mediator.Send(new GetAllPublishersQuery());

                if (operationResult.IsSuccess)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all Publishers at {time}", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                return BadRequest(ex.InnerException);
            }
        }

        //[HttpGet]
        //[Route("GetPublisherById/{id}")]
        //[SwaggerOperation(Description = "Gets Publisher by Id")]
        //[SwaggerResponse(200, "Successfully retrieved Publisher.")]
        //[SwaggerResponse(400, "Invalid input data")]
        //[SwaggerResponse(404, "Publisher not found")]
        //public async Task<IActionResult> GetPublisher([FromRoute] Guid id)
        //{
        //    _logger.LogInformation("Fetching Publisher with ID: {id} at {time}", id, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
        //    try
        //    {
        //        var operationResult = await _mediator.Send(new GetPublisherByIdQuery(id));

        //        if (operationResult.IsSuccess)
        //        {
        //            return Ok(operationResult.Data);
        //        }
        //        return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
        //    }

        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while fetching Publisher with ID: {id} at {time}", id, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
        //        return BadRequest(ex.InnerException);
        //    }
        //}
    }
}
