using eFlex.Common.Extensions;
using frlnet.Controllers.Admin;
using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ProjectTask")]
	[MapJoin("frl", "Project")]
	public class ProjectTaskModel : ModelBase
	{

		[Map] public Guid ProjectId { get; set; }
		[Map] public DateOnly ProjectStart { get; set; }
		[Map] public DateOnly ProjectEnd { get; set; }

		[Map, Show] public string Name { get; set; } = null!;

		[NoneSettings]
		[Show(In.Edit | In.Create)]
		public object? NameWhitespace { get; set; }

		[Map, Show] public DateTime StartTime { get; set; }
		[Map, Show] public DateTime EndTime { get; set; }
		public DatePickerSettings StartTimeSettings => new DatePickerSettings() { MinDate = ProjectStart.ToDateTime(), MaxDate = ProjectEnd.ToDateTime() };
		//public DatePickerSettings StartTimeSettings => new DatePickerSettings() { MinDate = ProjectStart.ToDateTime(), MaxDate = EndTime };
		public DatePickerSettings EndTimeSettings => new DatePickerSettings() { MinDate = ProjectStart.ToDateTime(), MaxDate = ProjectEnd.ToDateTime() };
		//public DatePickerSettings EndTimeSettings => new DatePickerSettings() { MinDate = StartTime, MaxDate = ProjectEnd.ToDateTime() };

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

		[DropListSettings(typeof(UserController), AllowCreate = false)]
		[Map, Show] public Guid? HolderUserId { get; set; }

		[Map, Show(editableIn: In.Filter)] public int Hours { get => _Hours ?? (EndTime - StartTime).Hours; set => _Hours = value; }
		private int? _Hours;

		[Map][Show(In.Edit | In.Grid, In.None)] public int EntryCount { get; set; }
		[Map][Show(In.Edit | In.Grid, In.None)] public Decimal TotalWorkerPayment { get; set; }
		[Map][Show(In.Edit | In.Grid, In.None)] public Decimal TotalClientPayment { get; set; }

		[TextBoxSettings(ControlSize =10)]
		[Map, Show] public string? Notes { get; set; }

		public override string ToString()
		{
			return $"[{StartTime}] {Name}";
		}

		[LabelGroupMark]
		[Show(In.Edit)]
		public virtual Link EntriesLink { get; } = Link.ToIndex<ProjectTaskEntryController>();

	}

}
