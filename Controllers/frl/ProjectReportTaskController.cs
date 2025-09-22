using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.frl
{
	[ClaimFrom<ProjectController>]
	public class ProjectReportTaskController : hAreaIndexController<ProjectReportTaskModel>
	{

		public ProjectReportTaskController()
		{
			Instructions.Index.Filter.SearchParentReferences = true;
			Instructions.Index.Filter.Show = false;
			Instructions.Edit.MonitorChanges = false;
			Instructions.Edit.AfterSave = EditBuilder.eAfterSave.RefreshPage;
		}

		protected override ProjectReportTaskModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
			var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId!.Value)!;
			var taskModels = AutoProcedure.Of<ProjectTaskModel>().GetRange(f => f.ProjectId, projectId);

			model.ProjectStart = projectModel.Start;
			model.ProjectEnd = projectModel.End;

			if (taskModels.Any())
			{
				model.StartTime = taskModels.OrderByDescending(f => f.RowCreated).First().StartTime;
			}
			else
			{
				model.StartTime = projectModel!.Start.ToDateTime();
			}

			model.EndTime = new DateTime[] { model.StartTime.AddHours(6), projectModel.End.ToDateTime() }.Min();

			model.ClientContactId = projectModel.ClientContactId;
			return model;
		}

	}
}
