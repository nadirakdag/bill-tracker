using System;
using System.Net;
using System.Threading.Tasks;
using Application.BillServices.Commands;
using Application.BillServices.Queries;
using Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly ILogger<BillController> _logger;
        private readonly IMediator _mediator;

        public BillController(ILogger<BillController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Getting all the bills");
                var bills = await _mediator.Send(new GetBillsRequest());
                _logger.LogInformation("Got all the bills");
                return Ok(bills);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while getting bills");
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting the bill");
                var bill = await _mediator.Send(new GetBillByIdRequest {Id = id});
                if (bill == null)
                {
                    _logger.LogInformation("Bill not found");
                    return NotFound();
                }

                _logger.LogInformation("Got the bill");
                return Ok(bill);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while getting bill");
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateBillRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new bill");
                var bill = await _mediator.Send(request);
                _logger.LogInformation("Created new bill");
                return CreatedAtAction(nameof(Get), new {id = bill.Id}, bill);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while creating new bill");
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateBillRequest request)
        {
            try
            {
                _logger.LogInformation("Updating bill");
                await _mediator.Send(request);
                _logger.LogInformation("Updated bill");
                return NoContent();
            }
            catch (BillNotFoundException e)
            {
                _logger.LogInformation("Bill not found");
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while Updating bill");
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting the bill");
                await _mediator.Send(new DeleteBillRequest() {Id = id});
                _logger.LogInformation("Bill deleted");
                return NoContent();
            }
            catch (BillNotFoundException e)
            {
                _logger.LogInformation("Bill not found");
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while getting bill");
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}