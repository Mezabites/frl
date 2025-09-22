using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{

	[MapSource("frl", "ProjectEstimateTask")]
	[MapJoin("frl", "Project")]
	[MapJoin("frl", "ProjectTask", alias: "", operations: MapJoin.eOperations.Insert | MapJoin.eOperations.Update | MapJoin.eOperations.Delete)]
	public class ProjectEstimateTaskModel : ProjectTaskModel
	{
		[Map] public Guid ProjectEstimateId { get; set; }
		[Map] public Guid ProjectTaskId { get; set; }
		[Map] public Guid SourcePlanTaskId { get; set; }

		[LabelGroupMark]
		[Show(In.Edit)]
		public new Link EntriesLink { get; } = Link.ToIndex<ProjectEstimateTaskEntryController>();

	}
}

