using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace aspire.ApiDbModel;

public class Todo
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedOn { get; }

    public DateTime? CompletedOn { get; set; }

    public bool IsComplete => CompletedOn.HasValue;

}