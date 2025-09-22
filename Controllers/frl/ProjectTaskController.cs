using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	[ClaimFrom<ProjectController>]
	public class ProjectTaskController : hAreaIndexController<ProjectTaskModel>
	{

		public ProjectTaskController()
		{
			Instructions.Index.Filter.Show = false;
		}

		protected override ProjectTaskModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
			var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId!.Value)!;
			var taskModels = AutoProcedure.Of<ProjectTaskModel>().GetRange(f => f.ProjectId, projectId);
			if (taskModels.Any())
			{
				model.StartTime = taskModels.OrderByDescending(f => f.RowCreated).First().StartTime;
			}
			else
			{
				model.StartTime = projectModel!.Start.ToDateTime();
			}

			model.EndTime = projectModel!.End.ToDateTime();

			model.ClientContactId = projectModel.ClientContactId;
			return model;
		}

		protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
		{
			var where = base.TranslateFilter(indexModel);
			var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
			where.Parts.Add(new SqlWherePart(nameof(ProjectTaskModel.ProjectId), projectId));
			return where;
		}

		public override IndexResult CreateConfirm(ProjectTaskModel model)
		{
			if (model.StartTime >= model.EndTime)
				return new IndexResult(Messages.ProjectInvalidDateRange);
			return base.CreateConfirm(model);
		}

		public override IndexResult EditConfirm(ProjectTaskModel model)
		{
			if (model.StartTime >= model.EndTime)
				return new IndexResult(Messages.ProjectInvalidDateRange);
			return base.EditConfirm(model);
		}


	}
}
