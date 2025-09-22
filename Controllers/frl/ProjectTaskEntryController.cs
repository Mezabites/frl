using eFlex.Common.Extensions;
using eFlex.Index.Base.Controllers;
using frlnet.Models.frl;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace frlnet.Controllers.frl
{
	[ClaimFrom<ProjectController>]
	public class ProjectTaskEntryController : hAreaIndexController<ProjectTaskEntryModel>
	{
		public ProjectTaskEntryController()
		{
			Instructions.Create.AfterSave = CreateBuilder.eAfterSave.DoNothingToConfuseTheHellOutOfUser;
			Instructions.Create.OpenInDialog = true;
			Instructions.Edit.OpenInDialog = true;
			Instructions.Index.Filter.SearchParentReferences = true;
			Instructions.Index.Filter.Show = false;
		}

		protected override ProjectTaskEntryModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			var taskModel = AutoProcedure.Of<ProjectTaskModel>().Get(model.ProjectTaskId);
			if (taskModel is not null)
			{
				model.ProjectTaskStartTime = taskModel.StartTime;
				model.ProjectTaskEndTime = taskModel.EndTime;
			}
			return model;
		}

		private Guid GetTaskId()
		{
			foreach (var fRoute in IndexNavigation.Current.Routes)
			{
				var controllerType = fRoute.ControllerType;
				var modelType = hIndexController.GetGenericType(controllerType);
				if (modelType.HasBaseType(typeof(ProjectTaskModel)))
				{
					var id = Guid.Parse(fRoute.GetQueryValue("id")!);
					var model = AutoProcedure.Of(modelType).Get(id);
					var property = modelType.GetProperty("ProjectTaskId")!;
					return (Guid)property.GetValue(model)!;
				}
			}
			throw new Exception();
		}

		protected override Dictionary<PropertyInfo, Guid> FindReferenceIds()
		{
			var dic = base.FindReferenceIds();
			var property = typeof(ProjectTaskEntryModel).GetProperty(nameof(ProjectTaskEntryModel.ProjectTaskId))!;
			dic.Set(property, GetTaskId());
			return dic;
		}


		//protected override ProjectTaskEntryModel CreateViewModel()
		//{
		//	var model = base.CreateViewModel();
		//	var planTaskId = IndexNavigation.Current.GetQueryGuid<ProjectPlanTaskController>();
		//	if (planTaskId.HasValue)
		//	{
		//		var planTaskModel = AutoProcedure.Of<ProjectPlanTaskModel>().Get(planTaskId.Value);
		//		model.ProjectTaskId = planTaskModel!.ProjectTaskId;
		//	}
		//	return model;
		//}

	}
}
