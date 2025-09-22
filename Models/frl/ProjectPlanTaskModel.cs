using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ProjectPlanTask")]
	[MapJoin("frl", "Project")]
	[MapJoin("frl", "ProjectTask", alias:"", operations: MapJoin.eOperations.Insert | MapJoin.eOperations.Update | MapJoin.eOperations.Delete)]
	public class ProjectPlanTaskModel : ProjectTaskModel
	{
		[Map] public Guid ProjectTaskId { get; set; } 

		[Map] public new Guid ProjectId { get; set; }

		[LabelGroupMark]
		[Show(In.Edit)]
		public override  Link EntriesLink { get; } = Link.ToIndex<ProjectPlanTaskEntryController>();
	}
}
