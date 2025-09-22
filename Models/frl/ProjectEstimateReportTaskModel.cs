using eFlex.Index.Base.Types;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ProjectEstimateTask")]
	[MapJoin("frl", "ProjectTask")]
	public class ProjectEstimateReportTaskModel : ModelBase
	{
		[Map] public Guid ProjectEstimateId { get; set; }
		[Map] public Guid ProjectTaskId { get; set; }

		[Map, Show]  public DateOnly ProjectTaskDay { get; set; }

		[Map, Show] public string ProjectTaskName { get; set; } = null!;
		[Map, Show] public Guid? ProjectTaskPostId { get; set; }
		public static SelectList<Guid> ProjectTaskPostIdSource => AutoProcedure.Of<PostModel>().GetRange().ToSelectList(f => f.Name, f => f.Id!.Value);

		[Map, Show] public int ProjectTaskUnits { get; set; }
		[Map, Show] public decimal ProjectTaskClientPayment { get; set; }

	
	}
}
