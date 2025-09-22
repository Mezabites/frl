using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	public class ProjectEstimateTaskController : hAreaIndexController<ProjectEstimateTaskModel>
	{
		
		public ProjectEstimateTaskController()
		{
			Instructions.Edit.Editable = false;
			Instructions.Create.Allow = false;
			Instructions.Delete.Allow = false;
			//Instructions.Edit.ModifyControlStructure = updateStructure;
			//Instructions.Index.Grid.ModifyControlStructure = updateStructure;
			Instructions.Index.Filter.SearchParentReferences = true;
			Instructions.Index.Filter.Show = false;
		}

		//private void updateStructure(IndexContolStructure structure)
		//{
		//	var estimateId = IndexNavigation.Current.GetQueryGuid<ProjectEstimateController>();
		//	if (!estimateId.HasValue)
		//		return;

		//	var estimateModel = AutoProcedure.Of<ProjectEstimateModel>().Get(estimateId!.Value)!;
		//	var validColumns = estimateModel.TaskColumns.ToHashSet();
		//	foreach (var fCtrl in structure.GetAllControls().ToArray())
		//		if (!validColumns.Contains(fCtrl.Key))
		//			structure.RemoveControl(fCtrl.Key);

		//	var entries = structure.GetAllControls().FirstOrDefault(f => f.Key == nameof(ProjectEstimateTaskModel.EntriesLink));
		//	if (entries is not null)
		//		entries.Value = Link.ToIndex<ProjectEstimateTaskEntryController>();
		//}
	}
}
