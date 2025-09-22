using eFlex.Index.Base.Types;
using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ProjectTaskEntry")]
	[MapJoin("frl", "Post")]
	[MapJoin("frl", "ProjectTask")]
	public class ProjectTaskEntryModel : ModelBase
	{
		[Map] public Guid ProjectTaskId { get; set; }

		#region "Post"
		[Map, Show(editableIn: In.Create)]
		public Guid? PostId
		{
			get => _PostId; set
			{
				if (_PostId != value)
				{
					_PostId = value;
					PostShiftId = null;
				}
				_PostId = value;
			}
		}
		private Guid? _PostId { get; set; }

		[Map] public string? PostName { get; set; }

		public static DropListSettings PostIdSettings { get; } = new DropListSettings(typeof(PostController), null, null)
		{ AllowCreate = false };

		public DropListSettings _PostIdSettings { get; } = new DropListSettings(typeof(PostController), (w) =>
		{
			var projectModel = IndexNavigation.Current.DownloadQueryModel<ProjectModel>();
			if (projectModel is null) return;
			w.Parts.Add(new SqlWherePart(nameof(PostModel.Start), projectModel.Start, SqlWherePart.CommonPatterns.DbDateTimeEqualOrSmallerOrNull));
			w.Parts.Add(new SqlWherePart(nameof(PostModel.End), projectModel.End, SqlWherePart.CommonPatterns.DbDateTimeEqualOrLargerOrNull));
		}, null)
		{ AllowCreate = false };
		#endregion

		#region "PostShift"
		[Map, Show(editableIn: In.Create)]
		public Guid? PostShiftId
		{
			get => _PostShiftId; set
			{
				_PostShiftId = value;
				if (!PostId.HasValue) return;

				var postModel = AutoProcedure.Of<PostModel>().Get(PostId.Value);
				if (postModel is null) return;

				if (_PostShiftId.HasValue)
				{
					var postShiftModel = postModel.Shift.FirstOrDefault(f => f.Id == _PostShiftId);
					if (postShiftModel is null) return;
					_Hours = postShiftModel.MaxHours;
					WorkerPayment = postShiftModel.WorkerPayment * Units * Days;
					ClientPayment = postShiftModel.ClientPayment * Units * Days;
				}
				else
				{
					var payments = postModel.GetPlanPayment(Hours ?? 1, value);
					WorkerPayment = payments.WorkerPayment * Units * Days;
					ClientPayment = payments.ClientPayment * Units * Days;
				}
			}
		}
		private Guid? _PostShiftId { get; set; }

		public bool PostShiftIdEditable => _PostId.HasValue;

		//public DropListSettings PostShiftIdSettings => new DropListSettings(typeof(PostShiftController)
		//,	(w) => w.Parts.Add(new SqlWherePart(nameof(PostShiftModel.PostId), PostId))
		//,   (m) => ((PostShiftModel)m).PostId = PostId!.Value);

		public SelectList<Guid?> PostShiftIdSource => GetPostShiftIdSource();
		private SelectList<Guid?> GetPostShiftIdSource()
		{
			if (!PostId.HasValue)
				return new SelectList<Guid?>();
			return AutoProcedure.Of<PostShiftModel>().GetRange(f => f.PostId, PostId).ToNullableSelectList(f => f.Name, f => f.Id);
		}
		public static SelectList<Guid?> _PostShiftIdSource { get; set; } = AutoProcedure.Of<PostShiftModel>().GetRange().ToNullableSelectList(f => f.Name, f => f.Id);
		#endregion

		#region "Days"
		[Map, Show]
		public int Days
		{
			get => _Days; set
			{
				if (value == 0) value = 1;
				var workerPaymentPerUnit = WorkerPayment / _Days;
				var clientPaymentPerUnit = ClientPayment / _Days;
				_Days = value;
				WorkerPayment = workerPaymentPerUnit * _Days;
				ClientPayment = clientPaymentPerUnit * _Days;
			}
		}
		private int _Days = 1;

		public NumericSettings DaysSettings => new() { MinValue = 1, MaxValue = Math.Ceiling(ProjectDays) };

		[Map] public DateTimeOffset ProjectTaskStartTime { get; set; }
		[Map] public DateTimeOffset ProjectTaskEndTime { get; set; }
		public double ProjectDays => (ProjectTaskEndTime - ProjectTaskStartTime).TotalDays;
		#endregion

		[NumericSettings(MinValue = 1, MaxValue = 10000)]
		[Map, Show]
		public int Units
		{
			get => _Units; set
			{
				var workerPaymentPerUnit = WorkerPayment / _Units;
				var clientPaymentPerUnit = ClientPayment / _Units;
				_Units = value;
				WorkerPayment = workerPaymentPerUnit * _Units;
				ClientPayment = clientPaymentPerUnit * _Units;
			}
		}
		private int _Units { get; set; } = 1;
		[Map, Show]
		public int? Hours
		{
			get => _Hours; set
			{
				_Hours = value;
				if (PostId.HasValue)
				{
					var postModel = AutoProcedure.Of<PostModel>().Get(_PostId!.Value);
					if (postModel is null) return;
					var payments = postModel.GetPlanPayment(Hours ?? 1, _PostShiftId);
					WorkerPayment = payments.WorkerPayment * Units;
					ClientPayment = payments.ClientPayment * Units;
				}
			}
		}
		private int? _Hours { get; set; } = 1;

		[Show(In.Edit | In.Create)]
		public object? HoursWhitespace { get; }

		[Map, Show] public Decimal WorkerPayment { get; set; }

		[Map, Show] public Decimal ClientPayment { get; set; }

		[TextBoxSettings(Multiline = true, ControlSize = 10)]
		[Map, Show] public string? Notes { get; set; }


		[ShowMark(In.Edit)]
		[LabelGroupMark]
		[Show]
		public object? Validation { get; set; }

		[Show, RawSettings] public string UnitsCheck => $"TEST Units: {Users?.Sum(f => f.Units * f.Days) ?? 0}/{Days * Units}";
		[Show, RawSettings] public string PaymentCheck => $"TEST Payment: {(Users?.Sum(f => f.WorkerPayment) ?? 0).ToString("f2")}/{WorkerPayment.ToString("f2")}";
		[Show, RawSettings] public string ProfitCheck => $"TEST Profit: {(ClientPayment - (Users?.Sum(f => f.WorkerPayment) ?? 0)).ToString("f2")}";


		[LabelGroupMark]
		[GridSettings(typeof(ProjectTaskEntryUserController))]
		[Show]
		[MapModel(nameof(ProjectTaskEntryUserModel.ProjectTaskEntryId))]
		public IEnumerable<ProjectTaskEntryUserModel> Users { get; set; } = null!;

		public override string ToString()
		{
			return $"{PostName} ({WorkerPayment.ToString("F2")}/{ClientPayment.ToString("F2")})";
		}
	}

}
