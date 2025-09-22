using eFlex.Index.Base.ActionResults;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	public class ProjectController : hAreaIndexController<ProjectModel>
	{

		protected override ProjectModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			model.StatusClvId = Classifiers._ProjectStatus._New.Id;
			model.Number = GetNewNumber();
			return model;
		}

		private string GetNewNumber()
		{
			return string.Concat("PR", DateTime.Now.Ticks.ToString().AsSpan(3, 9));
		}

		public override IndexResult CreateConfirm(ProjectModel model)
		{
			if (model.Start > model.End)
				return new IndexResult(Messages.ProjectInvalidDateRange);
			return base.CreateConfirm(model);
		}

		public override IndexResult EditConfirm(ProjectModel model)
		{
			if (model.Start > model.End)
				return new IndexResult(Messages.ProjectInvalidDateRange);
			return base.EditConfirm(model);
		}

	}
}
