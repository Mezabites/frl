using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	public  class ProjectPlanTaskEntryController : ProjectTaskEntryController
	{
		public ProjectPlanTaskEntryController()
		{
			var planTaskId = IndexNavigation.Current.GetQueryGuid<ProjectPlanTaskController>()!.Value;
			var estimates = ProjectPlanTaskController.GetBlockingEstimates(planTaskId);
			if (estimates.Any())
			{
				Instructions.Edit.Editable = false;
				Instructions.Create.Allow = false;
				Instructions.Delete.Allow = false;
			}
		}
	}
}
