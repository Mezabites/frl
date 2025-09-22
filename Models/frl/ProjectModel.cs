using eFlex.Common.Extensions;
using eFlex.Index.Base.Types;
using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
	[MapSource("frl", "Project")]
	public class ProjectModel : ModelBase
	{
		[Map, Show(editableIn: In.Filter | In.Create)] public string Name { get; set; } = null!;
		[Map, Show(editableIn: In.Filter | In.Create)] public string Number { get; set; } = null!;
		[Map, Show(In.Filter | In.Grid | In.Edit)] public Guid StatusClvId { get; set; }

		[TextBoxSettings(ControlSize = 10)]
		[Map, Show(In.Edit | In.Create)] public string? Description { get; set; }

		[Map, Show(editableIn: In.Filter | In.Create)] public DateOnly Start { get; set; } = DateTime.Now.Date.ToDateOnly();
		public DatePickerSettings StartSettings => new() { MinDate = DateTime.Now.AddYears(-1) };
		[Map, Show(editableIn: In.Filter | In.Create)] public DateOnly End { get; set; } = DateTime.Now.Date.AddDays(1).ToDateOnly();
		public DatePickerSettings EndSettings => new() { MinDate = Start.ToDateTime() };

		[Map, Show(editableIn: In.Filter | In.Create)]
		[DropListSettings(typeof(ClientController))]
		public Guid ClientId
		{
			get => _ClientId; set
			{
				_ClientId = Guid.Empty;
				if (value == Guid.Empty)
				{
					ClientContactId = null;
					_ClientContactIdSource = new();
				}
				else if (_ClientId != value)
				{
					_ClientId = value;
					var contacts = AutoProcedure.Of<ClientContactModel>().GetRange(f => f.ParentId, value).OrderBy(f => f.IsDefault);
					ClientContactId = contacts.FirstOrDefault()?.Id;
					_ClientContactIdSource = contacts.ToNullableSelectList(f => f.Name, f => f.Id!.Value);
				}
			}
		}
		private Guid _ClientId { get; set; }
		//public static SelectList<Guid> ClientIdSource =>
		//	AutoProcedure.Of<ClientModel>().GetRange().ToNullableSelectList(f => $"{f.Name} [{f.RegNumber}]", f => f.Id!.Value);


		[DropListSettings(typeof(ClientContactController))]
		[Map, Show] public Guid? ClientContactId { get; set; }
		public bool ClientContactIdEditable => ClientId != Guid.Empty;

		public SelectList<Guid> _ClientContactIdSource { get; set; } = new();
		//public static SelectList<Guid> ClientContactIdSource => AutoProcedure.Of<ClientContactModel>().GetRange().ToNullableSelectList(f => f.Name, f => f.Id!.Value);

		[Map, Show(editableIn: In.Filter | In.Create)] public string Address { get; set; } = null!;

		[LabelTabMark]
		[Show(In.Edit)]
		public Link Tasks => Link.ToIndex<ProjectPlanTaskController>();

		[LabelTabMark]
		[Show(In.Edit)]
		public Link Estimates => Link.ToIndex<ProjectEstimateController>();

		[LabelTabMark]
		[Show(In.Edit)]
		public Link Reports => Link.ToIndex<ProjectReportTaskController>();
	}
}
