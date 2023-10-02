using RL.Data.DataModels.Common;
using System.ComponentModel.DataAnnotations;

namespace RL.Data.DataModels;

public class AssignedUser: IChangeTrackable
{
    [Key]
    public int Id { get; set; }
    public int PlanId { get; set; }
    public int ProcedureId { get; set; }
    public int UserId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}