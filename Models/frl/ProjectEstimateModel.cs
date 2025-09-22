using eFlex.Common.Extensions;
using eFlex.Index.Base.Controllers;
using eFlex.Index.Base.Models.App;
using eFlex.Index.Base.Types;
using frlnet.Controllers.frl;
using frlnet.Helpers;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ProjectEstimate")]
	public class ProjectEstimateModel : ModelBase
	{
		public static string[] TempalteKeys { get; } = TemplateParser.ReadTemplateKeys(TemplateParser.ReadTemplate("ProjectEstimate.txt")).ToArray();
		private static SelectList<string> GetColumnSelectList()
		{
			var keys = TempalteKeys.Where(f=>f.FirstOrDefault() == '?').Select(f=>f.RemovePrefix("?")).GroupBy(f => f.Split(".")[0]);
			var dic = keys.ToDictionary(f => f.Key, f => f.Select(ff => ff.Split(".")[1]).Distinct().ToArray());

			var list = dic.SelectMany(f => f.Value.Select(ff => $"{f.Key}.{ff}")).Distinct().ToArray();
			var allType = new Type[] { typeof(ProjectController), typeof(ProjectEstimateController), typeof(ProjectEstimateTaskController), typeof(ProjectEstimateTaskEntryController) };

			var selectList = list.ToSelectList(f =>
			{
				var p1 = f.Split(".")[0];
				var p2 = f.Split(".")[1];
				var type = allType.FirstOrDefault(ff => ff.FullName!.Contains(p1));
				if (type is not null)
				{
					var l1 = LabelFieldModel.GetValue(type, Link.GetShortName(type));
					var l2 = LabelFieldModel.GetValue(type, p2);
					//return $"{l1.Text} - {l2.Text}";
					return $"{l2.Text}";
				}
				return f;
			});
			return selectList;
		}

		[ShowMark(In.Filter | In.Grid | In.Edit | In.Create)]

		[Map] public Guid ProjectId { get; set; }
		[Map, Show] public string Name { get; set; } = null!;
		[Map, Show] public string Number { get; set; } = null!;

		[DropListSettings(typeof(ClientContactController))]
		[Map, Show] public Guid? ClientContactId { get; set; }
		public DropListSettings ClientContactIdSettings
		{ get; } = new DropListSettings(typeof(ClientContactController),
			filter: (w) => w.Parts.Add(new(nameof(ClientContactModel.ParentId), GetClientId)),
			create: (m) => ((ClientContactModel)m).ParentId = GetClientId);

		private static Guid GetClientId
		{
			get
			{
				var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>()!.Value;
				var projectModel = AutoProcedure.Of<ProjectModel>().Get(projectId)!;
				return projectModel.ClientId;
			}
		}

		[DropListSettings(SourceClassifierCode = Classifiers._ProjectEstimateStatus.Code)]
		[Map, Show(In.Filter | In.Grid | In.Edit, In.Filter)] public Guid StatusClvId { get; set; }

		[TextBoxSettings(ControlSize = 10)]
		[Map, Show(In.Edit | In.Create)] public string? Notes { get; set; }

		[MultiDropListSettings(nameof(TaskColumnsSource), ControlSize =10)]
		[Show(In.Create | In.Edit)]
		[Map] public StringList<string> TaskColumns { get; set; } = new StringList<string>(TaskColumnsSource.GetValues());
		public static SelectList<string> TaskColumnsSource  => GetColumnSelectList();

		//[MultiDropListSettings(nameof(EntryColumnsSource))]
		//[Show(In.Create | In.Edit)]
		//[Map] public StringList<string> EntryColumns { get; set; } = new StringList<string>(EntryColumnsSource.GetValues());
		//public static SelectList<string> EntryColumnsSource { get; } = GetColumnSelectList<ProjectEstimateTaskEntryController>("Entry");


		[Map][Show(In.Edit | In.Grid, In.None)] public int TotalTaskCount { get; set; }
		[Map][Show(In.Edit | In.Grid, In.None)] public int TotalEntryCount { get; set; }
		[Map][Show(In.Edit | In.Grid, In.None)] public Decimal TotalWorkerPayment { get; set; }
		[Map][Show(In.Edit | In.Grid, In.None)] public Decimal TotalClientPayment { get; set; }

		[ListBoxSettings(nameof(TaskImportSource), Height = 300)]
		[Show(visibleIn: In.Create)]
		public StringList<Guid> TaskImport { get; set; } = null!;

		public SelectList<Guid> TaskImportSource
		{
			get
			{
				var projectId = IndexNavigation.Current.GetQueryGuid<ProjectController>();
				if (!projectId.HasValue)
					return new SelectList<Guid>();

				var applicableTasks = ProjectEstimateController.GetApplicableTasks(projectId.Value);

				return applicableTasks.ToSelectList(f => f.ToString(), f => f.Id!.Value);
			}
		}

		[LabelGroupMark]
		[Show(In.Edit)]
		[RawSettings()]
		public string? Report { get; set; }

		[LabelGroupMark]
		[Show(In.Edit)]
		public Link Taks => Link.ToIndex<ProjectEstimateTaskController>();


	}

	public class ProjectEstimateProcedure : AutoProcedure<ProjectEstimateModel>
	{
		public override void Insert(IEnumerable<ProjectEstimateModel> models)
		{
			base.Insert(models);

			foreach (var fModel in models)
			{
				var curTasks = Of<ProjectPlanTaskModel>().GetRange(fModel.TaskImport);
				var curEntries = Of<ProjectTaskEntryModel>().GetRange(nameof(ProjectTaskEntryModel.ProjectTaskId), curTasks.Select(f => f.ProjectTaskId));
				var newTasks = curTasks.Select(f => Cast.CopyValues(f, new ProjectEstimateTaskModel())).ToArray();

				var oldTaskKeys = new List<Guid>(curTasks.Count());
				var newTaskKeys = new List<Guid>(curTasks.Count());
				foreach (var fTask in newTasks)
				{
					fTask.SourcePlanTaskId = fTask.Id!.Value;
					fTask.ProjectEstimateId = fModel.Id!.Value;
					fTask.Id = null;
					oldTaskKeys.Add(fTask.ProjectTaskId);
					fTask.ProjectTaskId = Guid.Empty;
				}
				Of<ProjectEstimateTaskModel>().Insert(newTasks);

				foreach (var fTask in newTasks)
				{
					newTaskKeys.Add(fTask.ProjectTaskId);
				}

				foreach (var fEntry in curEntries)
				{
					fEntry.Id = null;
					var taskIndex = oldTaskKeys.IndexOf(fEntry.ProjectTaskId);
					fEntry.ProjectTaskId = newTaskKeys[taskIndex];
				}
				Of<ProjectTaskEntryModel>().Insert(curEntries);
			}
		}

		//public override void Update(ProjectEstimateModel model)
		//{
		//	base.Update(model);
		//	var oldTasks = Of<ProjectEstimateTaskModel>().GetRange(f => f.ProjectEstimateId, model.Id!.Value);
		//	var newTasks = model.TaskImport.Select(f => new ProjectEstimateTaskModel()
		//	{
		//		ProjectEstimateId = model.Id!.Value,
		//		ProjectTaskId = f
		//	});
		//	var prepare = newTasks.PrepareWith(oldTasks, (f)=>f.ProjectTaskId);
		//	Of<ProjectEstimateTaskModel>().Delete(prepare.ToDelete.Select(f => f.Id!.Value));
		//	Of<ProjectEstimateTaskModel>().Insert(prepare.ToInsert);
		//}
	}
}
