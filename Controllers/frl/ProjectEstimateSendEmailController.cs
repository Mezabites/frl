using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Controllers.Admin;
using eFlex.Index.Base.Models.Admin;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
	public class ProjectEstimateSendEmailController : SendEmailController
	{
		public ProjectEstimateSendEmailController()
		{
			Instructions.Edit.Allow = false;
			Instructions.Create.AfterSave = CreateBuilder.eAfterSave.ReturnToPrevius;
		}

		protected override SendEmailModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			var reportId = IndexNavigation.Current.GetQueryGuid<ProjectEstimateController>()!.Value;
			var reportModel = AutoProcedure.Of<ProjectEstimateModel>().Get(reportId)!;
			var contactModel = AutoProcedure.Of<ClientContactModel>().Get(reportModel.ClientContactId!.Value)!;
			var template = ProjectEstimateController.GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateSend.Id)!;

			var reportLink = new Link(nameof(Edit), typeof(ProjectEstimateController), new { id = reportId });
			var projectLink = IndexNavigation.Current.Routes.First(f => f.ControllerType == typeof(ProjectController)).ToLink().ToString();
			var fullUrl = reportLink.ToFullString(HttpContext) + "&" + IndexNavigation.Splitter + projectLink;
			var body = string.Format(template.DecodedContent!, fullUrl);
			
			model.To = contactModel.Email;
			model.Subject = template.Subject; //+ " " + reportModel.EmailSubject;
			model.Body = body;

			return model;
		}

		public override IndexResult CreateConfirm(SendEmailModel model)
		{
			var res = base.CreateConfirm(model);
			if (res.HasErrors)
				return res;

			var estimateId = IndexNavigation.Current.GetQueryGuid<ProjectEstimateController>()!.Value;
			var estimateModel = AutoProcedure.Of<ProjectEstimateModel>().Get(estimateId)!;
			estimateModel.StatusClvId = Classifiers._ProjectEstimateStatus._Waiting.Id;
			AutoProcedure.Of<ProjectEstimateModel>().Update(estimateModel);
			return res;
		}

	}
}
