//using frlnet.Models.frl;
//using Microsoft.AspNetCore.Authorization;

//namespace frlnet.Controllers.frl
//{
//	[NoClaim]
//	[AllowAnonymous]
//	public class ProjectEstimateReportTaskEntryController : ProjectEstimateTaskEntryController
//	{
//		public ProjectEstimateReportTaskEntryController()
//		{
//			Instructions.Edit.Editable = false;
//			Instructions.Create.Allow = false;
//			Instructions.Delete.Allow = false;
//			Instructions.Edit.ModifyControlStructure = updateStructure;
//			Instructions.Index.Grid.ModifyControlStructure = updateStructure;
//		}

//		private void updateStructure(IndexContolStructure structure)
//		{
//			var reportId = IndexNavigation.Current.GetQueryGuid<ProjectEstimateReportController>();
//			var reportModel = AutoProcedure.Of<ProjectEstimateReportModel>().Get(reportId!.Value)!;
//			var validColumns = reportModel.EntryColumns.ToHashSet();
//			foreach (var fCtrl in structure.GetAllControls().ToArray())
//				if (!validColumns.Contains(fCtrl.Key))
//					structure.RemoveControl(fCtrl.Key);
//		}
//	}
//}
