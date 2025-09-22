using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	public  class ProjectReportTaskEntryController : ProjectTaskEntryController
	{
		public ProjectReportTaskEntryController()
		{
			var planTaskId = IndexNavigation.Current.GetQueryGuid<ProjectReportTaskController>()!.Value;
			//var estimates = ProjectPlanTaskController.GetBlockingEstimates(planTaskId);
			//if (estimates.Any())
			//{
			//	Instructions.Edit.Editable = false;
			//	Instructions.Create.Allow = false;
			//	Instructions.Delete.Allow = false;
			//}
		}
	}
}
