namespace CAC.Domain.Common;

public interface IAuditedEntity
{
	public string? CreatedBy { get; set; }

	public DateTime CreationDate { get; set; }

	public string? UpdatedBy { get; set; }

	public DateTime? UpdationDate { get; set; }
}
