using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	[ClaimFrom<ProjectController>]
	public class ProjectEstimateTaskEntryController : ProjectTaskEntryController
	{
		public ProjectEstimateTaskEntryController()
		{
			Instructions.Edit.Allow = false;
			Instructions.Create.Allow = false;
			Instructions.Delete.Allow = false;
			//Instructions.Edit.ModifyControlStructure = updateStructure;
			//Instructions.Index.Grid.ModifyControlStructure = updateStructure;
		}

		//private void updateStructure(IndexContolStructure structure)
		//{
		//	var estimateId = IndexNavigation.Current.GetQueryGuid<ProjectEstimateController>();
		//	if (!estimateId.HasValue)
		//		return;

		//	var estimateModel = AutoProcedure.Of<ProjectEstimateModel>().Get(estimateId!.Value)!;
		//	var validColumns = estimateModel.EntryColumns.ToHashSet();
		//	foreach (var fCtrl in structure.GetAllControls().ToArray())
		//		if (!validColumns.Contains(fCtrl.Key))
		//			structure.RemoveControl(fCtrl.Key);
		//}
	
	}
}
