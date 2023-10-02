using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;


public class AddAssignedUserCommandHandler : IRequestHandler<AddAssignedUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    public AddAssignedUserCommandHandler(RLContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<Unit>> Handle(AddAssignedUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

            //check valid planid & procedure id or not , userid already exist or not

            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PlanId == request.PlanId);        
            if (plan is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));

            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
            if (procedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

            var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == request.UserId);
            if (user is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

            var assignedUser = await _context.AssignedUser.FirstOrDefaultAsync(a => a.PlanId == request.PlanId & a.ProcedureId == request.ProcedureId & a.UserId == request.UserId);
            if ( assignedUser is not null)
              return ApiResponse<Unit>.Fail(new BadRequestException($"Already User Assigned  UserId:{request.UserId} ,PlanId:{request.PlanId}, ProcedureId:{request.ProcedureId}"));

            //if valid add the data
            _context.AssignedUser.Add(new AssignedUser
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId
            });
            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}