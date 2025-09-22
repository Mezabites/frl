using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Models.App;
using eFlex.Index.Base.Types;
using frlnet.Helpers;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace frlnet.Controllers.frl
{
	[Claim(eClaimName.Read)]
	[Claim(eClaimName.Create)]
	[Claim(eClaimName.Delete)]
	public class ProjectEstimateController : hAreaIndexController<ProjectEstimateModel>
	{
		public ProjectEstimateController()
		{
			Instructions.Index.Filter.SearchParentReferences = true;
			Instructions.Index.Filter.Show = false;
			Instructions.Edit.Editable = false;
		}

		protected override ProjectEstimateModel CreateViewModel()
		{
			var model = base.CreateViewModel();
			model.StatusClvId = Classifiers._ProjectEstimateStatus._New.Id;
			model.Number = GetNewNumber();
			var projectModel = IndexNavigation.Current.DownloadQueryModel<ProjectModel>();
			model.ClientContactId = projectModel?.ClientContactId;
			return model;
		}

		private string GetNewNumber()
		{
			return string.Concat("ES", DateTime.Now.Ticks.ToString().AsSpan(3, 9));
		}



		public override ViewResult Edit(Guid id)
		{
			var result = base.Edit(id);
			var indexModel = extractIndexModel(result);
			if (indexModel is not null)
			{

				var model = indexModel.ExactModel;
				var isAdminUser = UserModel.CurrentId.HasValue;
				if (isAdminUser)
				{
					var estimateController = new ProjectEstimateController();
					estimateController.ControllerContext = ControllerContext;
					var allow = estimateController.EditAllow(id);
					if (allow.HasErrors)
						isAdminUser = false;
				}

				var approveLabel = LabelFieldModel.GetValue(typeof(ProjectEstimateController), nameof(Approve)).Text;
				var rejectLabel = LabelFieldModel.GetValue(typeof(ProjectEstimateController), nameof(Reject)).Text;
				var approveBtn = ButtonBuilder.DefaultButtons.DefaultIndexResult(approveLabel, new Link(nameof(Approve), new { id }));
				var rejectBtn = ButtonBuilder.DefaultButtons.DefaultIndexResult(rejectLabel, new Link(nameof(Reject), new { id }));

				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._New.Id || isAdminUser)
				{
					var sendLabel = LabelFieldModel.GetValue(typeof(ProjectEstimateController), nameof(Send)).Text;
					Instructions.Edit.Buttons.List.Add(ButtonBuilder.DefaultButtons.DefaultIndexResult(sendLabel, new Link(nameof(Send), new { id })));
				}
				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._Waiting.Id || isAdminUser)
				{
					Instructions.Edit.Buttons.List.Add(approveBtn);
					Instructions.Edit.Buttons.List.Add(rejectBtn);
				}
				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._Approved.Id)
				{
					indexModel.Messages.Add(new(Messages.ProjectEstimateApproved.ToString(), IndexResult.Message.eLevel.Success));
					if (isAdminUser)
						Instructions.Edit.Buttons.List.Add(rejectBtn);
				}
				if (model?.StatusClvId == Classifiers._ProjectEstimateStatus._Rejected.Id)
				{
					indexModel.Messages.Add(new(Messages.ProjectEstimateRejected.ToString(), IndexResult.Message.eLevel.Warning));
					if (isAdminUser)
						Instructions.Edit.Buttons.List.Add(approveBtn);
				}

				//Generate body.
				indexModel.ExactModel.Report = CreateBody(id);
			}


			return result;
		}

		//public override IndexResult EditConfirm(ProjectEstimateModel model)
		//{
		//	var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
		//	var validTaskIds = GetApplicableTasks(projectId!.Value)
		//		.Select(f => f.Id!.Value).ToHashSet();
		//	if (model.TaskImport.Any(f => !validTaskIds.Contains(f)))
		//		return new IndexResult(Messages.IndexGenericFailure);

		//	//if (model.Start > model.End)
		//	//	return new IndexResult(Messages.ProjectInvalidDateRange);
		//	return base.EditConfirm(model);
		//}

		public static IEnumerable<ProjectPlanTaskModel> GetApplicableTasks(Guid projectId)
		{
			var allTasks = AutoProcedure.Of<ProjectPlanTaskModel>().GetRange(f => f.ProjectId, projectId);
			var allEstamates = AutoProcedure.Of<ProjectEstimateModel>()
				.GetRange(f => f.ProjectId, projectId).Where(f => f.StatusClvId != Classifiers._ProjectEstimateStatus._Rejected.Id);

			var allUsedTasksIds = allEstamates.Where(f => f.TaskImport is not null).SelectMany(f => f.TaskImport).ToHashSet();
			var allUnusedTasks = allTasks.Where(f => !allUsedTasksIds.Contains(f.Id!.Value));

			return allUnusedTasks;
		}


		
		private string CreateBody(Guid estimateId)
		{
			var template = TemplateParser.ReadTemplate("ProjectEstimate.txt");

			var estimateModel = AutoProcedure.Of<ProjectEstimateModel>().Get(estimateId)!;
			var projectModel = AutoProcedure.Of<ProjectModel>().Get(estimateModel.ProjectId)!;
			var taskModels = AutoProcedure.Of<ProjectEstimateTaskModel>().GetRange(f => f.ProjectEstimateId, estimateId)!;
			var entryModels = AutoProcedure.Of<ProjectTaskEntryModel>().GetRange(f => f.ProjectTaskId, taskModels.Select(f => f.ProjectTaskId));

			template = FilterTemplateLines(template, estimateModel.TaskColumns);

			{
				var indexModel = new IndexModel<ProjectModel>(this.GetType(), nameof(CreateBody));
				indexModel.Instructions = IndexInstructions.CreateInstructions();
				indexModel.Model = projectModel;
				template = TemplateParser.FillValues(template, indexModel, "Project");
			}

			{
				var indexModel = new IndexModel<ProjectEstimateModel>(this.GetType(), nameof(CreateBody));
				indexModel.Instructions = IndexInstructions.CreateInstructions();
				indexModel.Model = estimateModel;
				template = TemplateParser.FillValues(template, indexModel, "Estimate");
			}

			// Extract the task and entry parts
			var taskStartTag = "<!---[TASK START]-->";
			var taskEndTag = "<!---[TASK END]-->";
			var entryStartTag = "<!---[ENTRY START]-->";
			var entryEndTag = "<!---[ENTRY END]-->";

			var taskPart = TemplateParser.ExtractTemplatePart(template, taskStartTag, taskEndTag);
			if (string.IsNullOrWhiteSpace(taskPart))
				throw new InvalidOperationException("Task section not found in the template.");

			var entryPart = TemplateParser.ExtractTemplatePart(taskPart, entryStartTag, entryEndTag);
			if (string.IsNullOrWhiteSpace(entryPart))
				throw new InvalidOperationException("Entry section not found in the template.");

			var taskTemplate = string.Empty;

			// Build task and entry sections
			foreach (var fTask in taskModels)
			{
				var currentTaskTemplate = string.Empty;
				{
					var indexModel = new IndexModel<ProjectTaskModel>(this.GetType(), nameof(CreateBody));
					indexModel.Instructions = IndexInstructions.CreateInstructions();
					indexModel.Model = fTask;
					currentTaskTemplate = TemplateParser.FillValues(taskPart, indexModel, "Task");
				}

				var entryTemplate = string.Empty;
				{
					var indexModel = new IndexModel<ProjectTaskEntryModel>(this.GetType(), nameof(CreateBody));
					indexModel.Instructions = IndexInstructions.CreateInstructions();

					//Fill entry labels.
					currentTaskTemplate = TemplateParser.FillValues(currentTaskTemplate, indexModel, "Entry");
					
					foreach (var fEntry in entryModels.Where(f => f.ProjectTaskId == fTask.ProjectTaskId))
					{
						indexModel.Model = fEntry;
						entryTemplate += TemplateParser.FillValues(entryPart, indexModel, "Entry");
					}
				}

				currentTaskTemplate = currentTaskTemplate.Replace(entryPart, entryTemplate);
				taskTemplate += currentTaskTemplate;
			}

			template = template.Replace(taskPart, taskTemplate);
			return template;
		}

		public string FilterTemplateLines(string template, IEnumerable<string> validColumnNames)
		{
			// Split template into lines (preserve line endings carefully if needed)
			var lines = template.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			var filteredLines = new List<string>();

			// Regex to match anything in curly braces: { ... }
			var placeholderRegex = new Regex(@"\{\?([^}]*)\}");

			foreach (var line in lines)
			{
				// Find all placeholders, e.g. "{Project.Name.Label}"
				var matches = placeholderRegex.Matches(line);

				// If there are no placeholders on this line, it's automatically valid
				if (matches.Count == 0)
				{
					filteredLines.Add(line);
					continue;
				}

				bool removeLine = false;

				foreach (Match match in matches)
				{
					// rawPlaceholder might be "Project.Name.Label" or "Estimate.Number.f2"
					var rawPlaceholder = match.Groups[1].Value;

					// Split by dot
					var parts = rawPlaceholder.Split('.');

					// We expect at least 2 parts (e.g. ["Project","Name"]) 
					// but placeholders might have more (["Project","Name","Label"]).
					if (parts.Length < 2)
					{
						// Not in the correct "X.Y" format => remove this line
						removeLine = true;
						break;
					}

					// Base key = first two segments joined by a dot, e.g. "Project.Name"
					var baseKey = parts[0] + "." + parts[1];

					// If the base key is not in the valid column names, mark line for removal
					if (!validColumnNames.Contains(baseKey))
					{
						removeLine = true;
						break;
					}
				}

				// If we never set removeLine to true, we keep the line
				if (!removeLine)
				{
					filteredLines.Add(line);
				}
			}

			// Rejoin the kept lines
			return string.Join("\n", filteredLines);
		}





		//private string FillValues(string template, hIndexModel indexModel, string prefix)
		//{
		//	var structure = indexModel.Instructions.Edit.GetControlStructure(indexModel);

		//	foreach (var fCtrl in structure.GetAllControls())
		//	{
		//		if (indexModel.Model is not null)
		//		{
		//			// 1) ---------- VALUE PLACEHOLDERS ----------
		//			var basePlaceholder = $"{{{prefix}{fCtrl.Key}}}";

		//			// Regex to match placeholders like {MyKey$f2}, {MyKey$N2}, etc.
		//			var valuePlaceholderRegex = new System.Text.RegularExpressions.Regex(
		//				$@"\{{{prefix}{fCtrl.Key}\$(.+?)\}}");

		//			// Replace {prefix}{Key}$<format> for numeric formatting
		//			template = valuePlaceholderRegex.Replace(template, match =>
		//			{
		//				var format = match.Groups[1].Value;
		//				var valueObj = fCtrl.Value;

		//				valueObj ??= string.Empty;  // fallback if null

		//				// Try numeric formatting
		//				if (!string.IsNullOrEmpty(format) && decimal.TryParse(valueObj.ToString(), out var numericValue))
		//				{
		//					try
		//					{
		//						return numericValue.ToString(format);
		//					}
		//					catch
		//					{
		//						// Fallback if the specified format is invalid
		//						return valueObj.ToString();
		//					}
		//				}

		//				// If not numeric or format is empty, return as string
		//				return valueObj.ToString();
		//			});

		//			// Replace standard {prefix}{Key} with the raw or DropList-transformed value
		//			if (template.Contains(basePlaceholder))
		//			{
		//				var valueObj = fCtrl.Value;

		//				// Apply DropListSettings if needed
		//				if (fCtrl.Settings is DropListSettings dropListSettings)
		//				{
		//					var itemSource = dropListSettings.GetSelectList(fCtrl.Key, indexModel.ModelType, indexModel.ControllerType, indexModel.Model);
		//					var valueIdx = Array.IndexOf(itemSource.GetValues(), valueObj?.ToString());
		//					if (valueIdx > 0)
		//						valueObj = itemSource.GetTexts()[valueIdx];
		//				}

		//				template = template.Replace(basePlaceholder, valueObj?.ToString() ?? string.Empty);
		//			}
		//		}

		//		// 2) ---------- LABEL PLACEHOLDERS ----------
		//		// For example: {prefix}{Key}.Label
		//		var labelPlaceholder = $"{{{prefix}{fCtrl.Key}.Label}}";

		//		// Replace standard {prefix}{Key}.Label
		//		if (template.Contains(labelPlaceholder))
		//		{
		//			var labelValue = fCtrl.Label?.Text ?? string.Empty;
		//			template = template.Replace(labelPlaceholder, labelValue);
		//		}
		//	}

		//	return template;
		//}


		public override IndexResult DeleteAllow([FromQuery] Guid[] id)
		{

			return base.DeleteAllow(id);
		}


		public IndexResult Send(Guid id)
		{
			if (!UserModel.CurrentId.HasValue)
				return new IndexResult(Messages.IndexGenericFailure);

			var model = Procedure.Get(id);
			if (model is null)
				return new IndexResult(Messages.IndexGenericFailure);

			var clientContactModel = AutoProcedure.Of<ClientContactModel>().Get(model.ClientContactId!.Value)!;
			var email = clientContactModel.Email;
			if (string.IsNullOrEmpty(email))
				return new IndexResult(Messages.ProjectEstimateSendNoEmail);


			var tempalte = GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateSend.Id);
			if (tempalte is null)
				return new IndexResult(string.Format(Messages.ProjectEstimateSendNoTemplate, Classifiers._Emailtemplatetype._ProjectEstimateSend.Name));

			return new IndexResult(new Link(nameof(Create), typeof(ProjectEstimateSendEmailController), new { }), true, true, null, null);

			//model.StatusClvId = Classifiers._ProjectEstimateStatus._Waiting.Id;
			//Procedure.Update(model);


			//SendEmail(id, email, tempalte);
			//var message = string.Format(Messages.ProjectEstimateSend.ToString(), email);
			//return new IndexResult(new Link(nameof(Edit), new { id }), false, new IndexResult.Message(message, IndexResult.Message.eLevel.Success));
		}


		[AllowAnonymous]
		public IndexResult Approve(Guid id)
		{
			var model = Procedure.Get(id);
			if (model is null)
				return new IndexResult(Messages.IndexGenericFailure);

			var clientContactModel = AutoProcedure.Of<ClientContactModel>().Get(model.ClientContactId!.Value)!;
			var email = clientContactModel.Email;
			if (string.IsNullOrEmpty(email))
				return new IndexResult(Messages.ProjectEstimateSendNoEmail);

			model.StatusClvId = Classifiers._ProjectEstimateStatus._Approved.Id;
			Procedure.Update(model);

			var tempalte = GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Id);
			if (tempalte is null)
				return new IndexResult(string.Format(Messages.ProjectEstimateSendNoTemplate, Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Name));

			SendEmail(id, email, tempalte);
			return new IndexResult(new Link(nameof(Edit), new { id }), false, new IndexResult.Message(Messages.ProjectEstimateApproved.ToString(), IndexResult.Message.eLevel.Success));
		}

		[AllowAnonymous]
		public IndexResult Reject(Guid id)
		{
			var model = Procedure.Get(id);
			if (model is null)
				return new IndexResult(Messages.IndexGenericFailure);

			var clientContactModel = AutoProcedure.Of<ClientContactModel>().Get(model.ClientContactId!.Value)!;
			var email = clientContactModel.Email;
			if (string.IsNullOrEmpty(email))
				return new IndexResult(Messages.ProjectEstimateSendNoEmail);

			model.StatusClvId = Classifiers._ProjectEstimateStatus._Rejected.Id;
			Procedure.Update(model);

			var tempalte = GetTemplate(Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Id);
			if (tempalte is null)
				return new IndexResult(string.Format(Messages.ProjectEstimateSendNoTemplate, Classifiers._Emailtemplatetype._ProjectEstimateUpdate.Name));

			SendEmail(id, email, tempalte);
			return new IndexResult(new Link(nameof(Edit), new { id }), false, new IndexResult.Message(Messages.ProjectEstimateRejected.ToString(), IndexResult.Message.eLevel.Warning));
		}

		internal static EmailTemplateModel? GetTemplate(Guid emailTemplateClassifierValueId)
		{
			var emailTemplates = AutoProcedure.Of<EmailTemplateModel>().GetRange(f => f.TypeClvId, emailTemplateClassifierValueId);
			var emailTemplate = emailTemplates.FirstOrDefault(f => f.LabelLanguageId == LabelLanguageModel.CurrentId);
			emailTemplate ??= emailTemplates.FirstOrDefault();
			if (emailTemplate?.Content == null)
				emailTemplate = null;

			return emailTemplate;
		}

		private void SendEmail(Guid estimateId, string email, EmailTemplateModel emailTemplateModel)
		{
			var reportLink = new Link(nameof(Edit), typeof(ProjectEstimateController), new { id = estimateId });
			var projectLink = IndexNavigation.Current.Routes.First(f => f.ControllerType == typeof(ProjectController)).ToLink().ToString();
			var fullUrl = reportLink.ToFullString(HttpContext) + "&" + IndexNavigation.Splitter + projectLink;
			var body = string.Format(emailTemplateModel.Content, fullUrl);
			var emailTo = new eFlex.Common.Email.SendTo(email, emailTemplateModel.Subject, body, true);
			eFlex.Index.Base.Helpers.Email.Send(emailTo, true);
		}

	}
}
