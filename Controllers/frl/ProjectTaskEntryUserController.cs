using eFlex.Index.Base.Controllers;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	public class ProjectTaskEntryUserController : hGridController<ProjectTaskEntryUserModel>
	{
		protected override ProjectTaskEntryUserModel CreateViewModel(IEnumerable<ProjectTaskEntryUserModel> currentCollection)
		{
			var model = base.CreateViewModel(currentCollection);
			var baseIndexModel = IndexRegistyFilter.GetModel(PageId, typeof(ProjectReportTaskEntryController));
			if (baseIndexModel?.Model is ProjectTaskEntryModel baseModel)
			{
				var postModel = AutoProcedure.Of<PostModel>().Get(baseModel.PostId!.Value);
				if (postModel is not null)
				{
					var payment = postModel.GetPlanPayment(baseModel.Hours!.Value, baseModel.PostShiftId);
					model.DateTime = baseModel.ProjectTaskStartTime;
					model.WorkerPayment = payment.WorkerPayment;
					model.Days = baseModel.Days;
					model.Hours = baseModel.Hours;
				}
			}
			return model;
		}
	}
}
