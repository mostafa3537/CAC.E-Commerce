using System.ComponentModel.DataAnnotations;

namespace CAC.Domain.Common;

public abstract class AggregateRoot<TKey> : IAuditedEntity, ISoftDelete
{
	[Key]
	public required TKey Id { get; set; }
	public bool IsDeleted { get; set; }
	public string? CreatedBy { get; set; }
	public DateTime CreationDate { get; set; }
	public string? UpdatedBy { get; set; }
	public DateTime? UpdationDate { get; set; }
}