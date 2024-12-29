﻿using Application.Commands.ReviewCommands.AddReviewCommand;
using Application.Dtos.ReviewDtos;
using Application.Queries.ReviewQueries.GetAllReviews;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var operationResult = await _mediator.Send(new GetAllReviewsQuery());
                if (operationResult is not null)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddReview(AddReviewDto dto)
        {
            try
            {
                var operationResult = await _mediator.Send(new AddReviewCommand(dto));
                if (operationResult is not null)
                {
                    return Ok(operationResult.Data);
                }
                return BadRequest(new { message = operationResult.Message, errors = operationResult.ErrorMessage });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }
    }
}
