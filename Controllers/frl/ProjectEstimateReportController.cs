//using eFlex.Index.Base.ActionResults;
//using eFlex.Index.Base.Models.Admin;
//using eFlex.Index.Base.Models.App;
//using frlnet.Models.frl;
//using Kendo.Mvc.UI;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace frlnet.Controllers.frl
//{
//	[Claim(eClaimName.Read)]
//	[Claim(eClaimName.Create)]
//	[Claim(eClaimName.Delete)]
//	public class ProjectEstimateReportController : hAreaIndexController<ProjectEstimateReportModel>
//	{

//		public ProjectEstimateReportController()
//		{
//			Instructions.Index.Filter.SearchParentReferences = true;
//		}

//		protected override ProjectEstimateReportModel CreateViewModel()
//		{
//			var model = base.CreateViewModel();
//			var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
//			var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId!.Value)!;
//			var estimateId = IndexNavigation.Current.GetQueryGuid<ProjectEstimateController>();
//			var estimateModel = AutoProcedure.Of<ProjectModel>().Get(projectId!.Value)!;
//			model.ClientContactId = projectModel.ClientContactId;
//			model.EmailSubject = $"{projectModel.Name}: {estimateModel.Number}";
//			model.StatusClvId = Classifiers._ProjectEstimateStatus._New.Id!;
//			return model;
//		}

//		[AllowAnonymous]
//		public override ViewResult Edit(Guid id)
//		{
//			var result = base.Edit(id);
//			var indexModel = extractIndexModel(result);
//			if (indexModel is not null)
//			{

//				var model = indexModel.ExactModel;
//				var isAdminUser = UserModel.CurrentId.HasValue;
//				if (isAdminUser)
//				{
//					var estimateController = new ProjectEstimateController();
//					estimateController.ControllerContext = ControllerContext;
//					var allow = estimateController.EditAllow(id);
//					if (allow.HasErrors)
//						isAdminUser = false;
//				}

//				var approveLabel = LabelFieldModel.GetValue(typeof(ProjectEstimateReportController), nameof(Approve)).Text;
//				var rejectLabel = LabelFieldModel.GetValue(typeof(ProjectEstimateReportController), nameof(Reject)).Text;
//				var approveBtn = ButtonBuilder.DefaultButtons.DefaultIndexResult(approveLabel, new Link(nameof(Approve), new { id }));
//				var rejectBtn = ButtonBuilder.DefaultButtons.DefaultIndexResult(rejectLabel, new Link(nameof(Reject), new { id }));

//				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._New.Id || isAdminUser)
//				{
//					var sendLabel = LabelFieldModel.GetValue(typeof(ProjectEstimateReportController), nameof(Send)).Text;
//					Instructions.Edit.Buttons.List.Add(ButtonBuilder.DefaultButtons.DefaultIndexResult(sendLabel, new Link(nameof(Send), new { id })));
//				}
//				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._Waiting.Id || isAdminUser)
//				{
//					Instructions.Edit.Buttons.List.Add(approveBtn);
//					Instructions.Edit.Buttons.List.Add(rejectBtn);
//				}
//				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._Approved.Id)
//				{
//					indexModel.Messages.Add(new(Messages.ProjectEstimateApproved.ToString(), IndexResult.Message.eLevel.Success));
//					if (isAdminUser)
//						Instructions.Edit.Buttons.List.Add(rejectBtn);
//				}
//				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._Rejected.Id)
//				{
//					indexModel.Messages.Add(new(Messages.ProjectEstimateRejected.ToString(), IndexResult.Message.eLevel.Warning));
//					if (isAdminUser)
//						Instructions.Edit.Buttons.List.Add(approveBtn);
//				}

//			}

//			return result;
//		}

//		public IndexResult Send(Guid id)
//		{
//			if (!UserModel.CurrentId.HasValue)
//				return new IndexResult(Messages.IndexGenericFailure);

//			var model = Procedure.Get(id);
//			if (model is null)
//				return new IndexResult(Messages.IndexGenericFailure);

//			var clientContactModel = AutoProcedure.Of<ClientContactModel>().Get(model.ClientContactId!.Value)!;
//			var email = clientContactModel.Email;
//			if (string.IsNullOrEmpty(email))
//				return new IndexResult(Messages.ProjectEstimateSendNoEmail);


//			var tempalte = GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateSend.Id);
//			if (tempalte is null)
//				return new IndexResult(string.Format(Messages.ProjectEstimateSendNoTemplate, Classifiers._Emailtemplatetype._ProjectEstimateSend.Name));

//			return new IndexResult(new Link(nameof(Create), typeof(ProjectEstimateReportSendEmailController), new {  }), true, true, null, null);

//			//model.StatusClvId = Classifiers._ProjectEstimateStatus._Waiting.Id;
//			//Procedure.Update(model);


//			//SendEmail(id, email, tempalte);
//			//var message = string.Format(Messages.ProjectEstimateSend.ToString(), email);
//			//return new IndexResult(new Link(nameof(Edit), new { id }), false, new IndexResult.Message(message, IndexResult.Message.eLevel.Success));
//		}


//		[AllowAnonymous]
//		public IndexResult Approve(Guid id)
//		{
//			var model = Procedure.Get(id);
//			if (model is null)
//				return new IndexResult(Messages.IndexGenericFailure);

//			var clientContactModel = AutoProcedure.Of<ClientContactModel>().Get(model.ClientContactId!.Value)!;
//			var email = clientContactModel.Email;
//			if (string.IsNullOrEmpty(email))
//				return new IndexResult(Messages.ProjectEstimateSendNoEmail);

//			model.StatusClvId = Classifiers._ProjectEstimateStatus._Approved.Id;
//			Procedure.Update(model);

//			var tempalte = GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Id);
//			if (tempalte is null)
//				return new IndexResult(string.Format(Messages.ProjectEstimateSendNoTemplate, Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Name));

//			SendEmail(id, email, tempalte);
//			return new IndexResult(new Link(nameof(Edit), new { id }), false, new IndexResult.Message(Messages.ProjectEstimateApproved.ToString(), IndexResult.Message.eLevel.Success));
//		}

//		[AllowAnonymous]
//		public IndexResult Reject(Guid id)
//		{
//			var model = Procedure.Get(id);
//			if (model is null)
//				return new IndexResult(Messages.IndexGenericFailure);

//			var clientContactModel = AutoProcedure.Of<ClientContactModel>().Get(model.ClientContactId!.Value)!;
//			var email = clientContactModel.Email;
//			if (string.IsNullOrEmpty(email))
//				return new IndexResult(Messages.ProjectEstimateSendNoEmail);

//			model.StatusClvId = Classifiers._ProjectEstimateStatus._Rejected.Id;
//			Procedure.Update(model);

//			var tempalte = GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Id);
//			if (tempalte is null)
//				return new IndexResult(string.Format(Messages.ProjectEstimateSendNoTemplate, Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Name));

//			SendEmail(id, email, tempalte);
//			return new IndexResult(new Link(nameof(Edit), new { id }), false, new IndexResult.Message(Messages.ProjectEstimateRejected.ToString(), IndexResult.Message.eLevel.Warning));
//		}

//		internal static EmailTemplateModel? GetTemplate(Guid emailTemplateClassifierValueId)
//		{
//			var emailTemplates = AutoProcedure.Of<EmailTemplateModel>().GetRange(f => f.TypeClvId, emailTemplateClassifierValueId);
//			var emailTemplate = emailTemplates.FirstOrDefault(f => f.LabelLanguageId == LabelLanguageModel.CurrentId);
//			emailTemplate ??= emailTemplates.FirstOrDefault();
//			if (emailTemplate?.Content == null)
//				emailTemplate = null;

//			return emailTemplate;
//		}

//		private void SendEmail(Guid estimateId, string email, EmailTemplateModel emailTemplateModel)
//		{
//			var reportLink = new Link(nameof(Edit), typeof(ProjectEstimateReportController), new { id = estimateId });
//			var projectLink = IndexNavigation.Current.Routes.First(f => f.ControllerType == typeof(ProjectController)).ToLink().ToString();
//			var fullUrl = reportLink.ToFullString(HttpContext) + "&" + IndexNavigation.Splitter + projectLink;
//			var body = string.Format(emailTemplateModel.Content, fullUrl);
//			var emailTo = new eFlex.Common.Email.SendTo(email, emailTemplateModel.Subject, body, true);
//			eFlex.Index.Base.Helpers.Email.Send(emailTo, true);
//		}

//		//private string? GetContactEmail(Guid projectId)
//		//{
//		//    var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId);
//		//    if (projectModel?.ClientContactId.HasValue != true)
//		//        return null;

//		//    var contactModel = AutoProcedure.Of<ClientContactModel>().Get(projectModel.ClientContactId!.Value);
//		//    if (string.IsNullOrEmpty(contactModel?.Email))
//		//        return null;

//		//    return contactModel?.Email;
//		//}


//	}
//}
