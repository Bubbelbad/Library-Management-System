﻿using Application.Commands.AuthorCommands.AddAuthor;
using Application.Commands.LibraryBranchCommands.AddLibraryBranch;
using Application.Commands.LibraryBranchCommands.UpdateLibraryBranch;
using Application.Dtos.LibraryBranchDtos;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities.Locations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryBranchController(IMediator mediator, ILogger<LibraryBranchController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<LibraryBranchController> _logger = logger;

        [Route("Add")]
        [HttpPost]
        public async Task<IActionResult> AddLibraryBranch(AddLibraryBranchDto dto)
        {
            var operationResult = await _mediator.Send(new AddLibraryBranchCommand(dto));
            if (operationResult.IsSuccess)
            {
                return Ok(operationResult.Data);
            }
            return BadRequest();
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> UpdateLibraryBranch(UpdateLibraryBranchDto dto)
        {
            var operationResult = await _mediator.Send(new UpdateLibraryBranchCommand(dto));
            if (operationResult.IsSuccess)
            {
                return Ok(operationResult.Data);
            }
            return BadRequest();
        }
    }
}
