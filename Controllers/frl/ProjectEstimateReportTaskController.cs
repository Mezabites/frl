//using eFlex.Index.Base.Models;
//using frlnet.Models.frl;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace frlnet.Controllers.frl
//{
//	[NoClaim]
//	[AllowAnonymous]
//	public class ProjectEstimateReportTaskController : ProjectEstimateTaskController
//	{
//		public ProjectEstimateReportTaskController()
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
//			var validColumns = reportModel.TaskColumns.ToHashSet();
//			foreach (var fCtrl in structure.GetAllControls().ToArray())
//				if (!validColumns.Contains(fCtrl.Key))
//					structure.RemoveControl(fCtrl.Key);

//			var entries = structure.GetAllControls().FirstOrDefault(f => f.Key == nameof(ProjectEstimateTaskModel.Entries));
//			if(entries is not null)
//				entries.Value = Link.ToIndex<ProjectEstimateReportTaskEntryController>();
//		}
//	}
//}
