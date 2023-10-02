using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

public class RemoveAssignedUserCommandHandler : IRequestHandler<RemoveAssignedUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    public RemoveAssignedUserCommandHandler(RLContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse<Unit>> Handle(RemoveAssignedUserCommand request, CancellationToken cancellationToken)
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
            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
            var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == request.UserId);
            var assignedUser = await _context.AssignedUser.FirstOrDefaultAsync(a => a.PlanId == request.PlanId & a.ProcedureId == request.ProcedureId & a.UserId == request.UserId);            
          
            if (plan is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
            if (procedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));
            if (user is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));
            if (assignedUser is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"No Mapping Founds UserId:{request.UserId}, PlanId:{request.PlanId} , ProcedureId:{request.ProcedureId}"));
           
            //if valid remove the data
            _context.AssignedUser.Remove(assignedUser);
            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}