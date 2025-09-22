//using eFlex.Index.Base.Controllers;
//using eFlex.Index.Base.Types;
//using frlnet.Controllers.frl;

//namespace frlnet.Models.frl
//{
//	[MapSource("frl", "ProjectEstimateReport")]
//	public class ProjectEstimateReportModel : ModelBase
//	{
//		[Map] public Guid ProjectEstimateId { get; set; }

//		[DropListSettings(typeof(ClientContactController))]
//		[Map, Show] public Guid? ClientContactId { get; set; }
//		public static DropListSettings ClientContactIdSettings => new(typeof(ClientContactController), (w) => {
//			var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
//			var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId!.Value)!;
//			w.Parts.Add(new(nameof(ClientContactModel.ParentId), projectModel.ClientId));
//		}, (m)=> { });

//		[MultiDropListSettings(nameof(TaskColumnsSource))]
//		[Show(In.Create)]
//		[Map] public StringList<string> TaskColumns { get; set; } = new StringList<string>(TaskColumnsSource.GetValues());
//		public static SelectList<string> TaskColumnsSource { get; } = GetColumnSelectList<ProjectEstimateTaskController>();

//		[MultiDropListSettings(nameof(EntryColumnsSource))]
//		[Show(In.Create)]
//		[Map] public StringList<string> EntryColumns { get; set; } = new StringList<string>(EntryColumnsSource.GetValues());
//		public static SelectList<string> EntryColumnsSource { get; } = GetColumnSelectList<ProjectEstimateTaskEntryController>();

//		private static SelectList<string> GetColumnSelectList<TController>() where TController : hIndexController
//		{
//			var controller = Services.GetService<TController>()!;
//			var instructions = controller.Instructions;
//			var builder = instructions.Edit;
//			var modelType = hIndexController.GetGenericType(typeof(TController));
//			var indexModelType = typeof(IndexModel<>).MakeGenericType(modelType);
//			var indexModel = (hIndexModel)Activator.CreateInstance(indexModelType, [typeof(ProjectEstimateTaskController), nameof(GetColumnSelectList)])!;
//			var structure = builder.GetControlStructure(indexModel);
//			var controls = structure.GetAllControls();//.Where(f => f.Settings.GetType() != typeof(LinkSettings));
//			var selectList = controls.ToSelectList(f => f.Label.Text, f => f.Key);
//			return selectList;
//		}

//		[DropListSettings(SourceClassifierCode = Classifiers._ProjectEstimateStatus.Code)]
//		[Map, Show(In.Filter | In.Grid | In.Edit, In.Filter)] public Guid StatusClvId { get; set; }

//		[TextBoxSettings(ControlSize = 10)]
//		[Map, Show(In.Edit | In.Create)] public string? Notes { get; set; }

//		[Show(In.Edit)]
//		[LabelGroupMark]
//		public object Email { get; }

//		[Show(In.Edit)]
//		[Map] public string EmailSubject { get; set; } = null!;

//		[Show(In.Edit)]
//		[Map] public string EmailBody { get; set; } = null!;

//		[LabelGroupMark]
//		[Show(In.Edit)]
//		public Link Taks => Link.ToIndex<ProjectEstimateReportTaskController>();

//	}
//}
