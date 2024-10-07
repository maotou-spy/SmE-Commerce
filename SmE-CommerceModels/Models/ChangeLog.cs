using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Models;

public partial class ChangeLog
{
    public Guid LogId { get; set; }

    public string TableName { get; set; } = null!;

    /// <summary>
    /// The Id of record has been updated
    /// </summary>
    public Guid RecordId { get; set; }

    /// <summary>
    /// Values: insert, update, delete
    /// </summary>
    public string Action { get; set; } = null!;

    public string? Reason { get; set; }

    public string? Description { get; set; }

    public DateTime? ChangedAt { get; set; }

    public Guid? ChangedById { get; set; }

    public virtual User? ChangedBy { get; set; }
}
