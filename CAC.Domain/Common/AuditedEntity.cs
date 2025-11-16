using System.ComponentModel.DataAnnotations;

namespace CAC.Domain.Common
{
	public abstract class AuditedEntity<TKey> : IAuditedEntity
	{
		[Key]
		public TKey Id { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime CreationDate { get; set; }
		public string? UpdatedBy { get; set; }
		public DateTime? UpdationDate { get; set; }
	}
}
