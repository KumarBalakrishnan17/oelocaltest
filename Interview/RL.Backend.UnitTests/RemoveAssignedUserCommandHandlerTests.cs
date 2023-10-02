using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveAssignedUserCommandHandlerTests
{
    [TestMethod]
    public async Task InvalidPlanId()
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddAssignedUserCommandHandler(context.Object);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 0,
            ProcedureId = 0,
            UserId = 0
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }
    [TestMethod]
    public async Task InvalidProcedureId()
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddAssignedUserCommandHandler(context.Object);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 1,
            ProcedureId = 0,
            UserId = 0
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }
    [TestMethod]
    public async Task InvalidUserId()
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddAssignedUserCommandHandler(context.Object);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 0
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task PlanId_NotFound()
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddAssignedUserCommandHandler(context);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 2,
            ProcedureId = 1,
            UserId = 1
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task ProcedureId_NotFound()
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddAssignedUserCommandHandler(context);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 1,
            ProcedureId = 2,
            UserId = 1
        };

        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1 });

        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task UserId_NotFound()
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddAssignedUserCommandHandler(context);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 2
        };

        context.Users.Add(new Data.DataModels.User { UserId = 1 });

        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task AlreadyMapped_BadRequest()
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddAssignedUserCommandHandler(context);
        var request = new AddAssignedUserCommand()
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 1
        };
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1 });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        context.AssignedUser.Add(new Data.DataModels.AssignedUser { Id = 1, PlanId = 1, ProcedureId = 1, UserId = 1 });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task Remove()
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAssignedUserCommandHandler(context);
        var request = new RemoveAssignedUserCommand()
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 1
        };
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1 });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        context.AssignedUser.Add(new Data.DataModels.AssignedUser { Id = 1, PlanId = 1, ProcedureId = 1, UserId = 1 });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }
}