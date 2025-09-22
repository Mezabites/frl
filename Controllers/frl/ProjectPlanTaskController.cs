using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.frl
{
	[ClaimFrom<ProjectController>]
	public class ProjectPlanTaskController : hAreaIndexController<ProjectPlanTaskModel>
	{

		public ProjectPlanTaskController()
		{
			Instructions.Index.Filter.SearchParentReferences = true;
			Instructions.Index.Filter.Show = false;
			Instructions.Edit.MonitorChanges = false;
			Instructions.Edit.AfterSave = EditBuilder.eAfterSave.RefreshPage;
		}

		protected override ProjectPlanTaskModel CreateViewModel()
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

		public override ViewResult Edit(Guid id)
		{
			var res = base.Edit(id);
			var indexModel = extractIndexModel(res);
			if (indexModel is null)
				return res;

			var model = indexModel.ExactModel;
			var blockingEstimateModels = GetBlockingEstimates(model);
			if (blockingEstimateModels.Any())
			{
				indexModel.Instructions.Edit.Editable = false;
				var messages = new List<IndexResult.Message>(blockingEstimateModels.Count());
				foreach (var f in blockingEstimateModels)
				{
					indexModel.Messages.Add(new(string.Format(Messages.ProjectPlanTaskBlockedFromEstimate, f.Name), IndexResult.Message.eLevel.Warning));
				}
			}

			return res;
		}


		public static IEnumerable<ProjectEstimateModel> GetBlockingEstimates(Guid planTaskId)
		{
			var model = AutoProcedure.Of<ProjectPlanTaskModel>().Get(planTaskId)!;
			return GetBlockingEstimates(model);
		}

		public static IEnumerable<ProjectEstimateModel> GetBlockingEstimates(ProjectPlanTaskModel model)
		{
			var estimateTaskModels = AutoProcedure.Of<ProjectEstimateTaskModel>().GetRange(f => f.SourcePlanTaskId, model!.Id!.Value);
			if (!estimateTaskModels.Any())
				return Enumerable.Empty<ProjectEstimateModel>();

			var estimateModels = AutoProcedure.Of<ProjectEstimateModel>().GetRange(estimateTaskModels.Select(f => f.ProjectEstimateId).Distinct());
			var blockingEstimateModels = estimateModels.Where(f => f.StatusClvId != Classifiers._ProjectEstimateStatus._Rejected.Id);
			return blockingEstimateModels;
		}

		//protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
		//{
		//	var where = base.TranslateFilter(indexModel);
		//	var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
		//	where.Parts.Add(new SqlWherePart(nameof(ProjectTaskModel.ProjectId), projectId));
		//	return where;
		//}

	}
}
