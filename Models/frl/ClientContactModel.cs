using eFlex.Index.Base.FilterConditions;
using frlnet.Integration.API;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ClientContact")]
	[MapJoin("frl", "Client")]
	public class ClientContactModel : ApiModelBase, IOrganizationModel
	{
		[Map("ClientId")]
		public override Guid ParentId { get; set; }

		[Map(Unique = true)] public override string? ExternalId { get; set; }

		[Show][Map] public virtual string Name { get; set; }

		[Show][Map] public virtual string Email { get; set; }

		[Show][Map] public virtual string? Phone { get; set; }

		[Show][Map] public virtual string? AdditionalInfo { get; set; }

		[Show][Map] public virtual bool IsDefault { get; set; } = true;
		public bool IsDefaultVisible
		{
			get
			{
				var count = AutoProcedure.Of<ClientContactModel>().GetRange(new SqlWhereCondition(nameof(ParentId), ParentId)).Count();
				if (Id.HasValue) return count > 1;
				return count > 0;
			}
		}

		[Map]
		public override Guid? LinkId { get; set; }

		public override ApiChildModelMeta[] ChildModels => [];

		[Map("ClientOrganizationId")]
		public override Guid OrganizationId { get; set; }

		Guid? IOrganizationModel.OrganizationId { get => OrganizationId; set => OrganizationId = value ?? Guid.Empty; }

		public override string ToString()
		{
			return $"{Name} ({Email})";
		}
	}
}
