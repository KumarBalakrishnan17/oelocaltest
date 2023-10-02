using Microsoft.AspNetCore.Mvc;
using RL.Data.DataModels;
using RL.Data;
using MediatR;
using RL.Backend.Commands;
using RL.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace RL.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssignedUserController : ControllerBase
    {
   
        private readonly RLContext _context;
        private readonly IMediator _mediator;

        public AssignedUserController(RLContext context, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }       
        [HttpGet("GetAssignedUsers")]          
        public async Task<List<AssignedUser>> GetAssignedUserByPlanIdProcedureId([FromQuery] int planId, [FromQuery] int procedureId)
        {
            var data = await _context.AssignedUser.Where(w => w.PlanId == planId & w.ProcedureId == procedureId).ToListAsync();
            return data;
        }
        [HttpPost("AssignUser")]
        public async Task<IActionResult> AddAssignedUser(AddAssignedUserCommand command, CancellationToken token)
        {
            var response = await _mediator.Send(command, token);
            return response.ToActionResult();
        }
        [HttpDelete("UnAssignUser")]
        public async Task<IActionResult> RemoveAssignedUser(RemoveAssignedUserCommand command, CancellationToken token)
        {
            var response = await _mediator.Send(command, token);
            return response.ToActionResult();
        }
    }
}
