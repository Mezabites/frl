using eFlex.Index.Base.Types;
using frlnet.Controllers.Admin;
using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
	[MapSource("frl", "ProjectTaskEntryUser")]
	public class ProjectTaskEntryUserModel : EditableModelBase
	{
		[Map] public Guid ProjectTaskEntryId { get; set; }

		[Map, Show] public DateTimeOffset DateTime { get; set; }

		[DropListSettings(controllerType: typeof(UserController), AllowCreate = false)]
		[Map, Show] public Guid UserId { get; set; }

		[NumericSettings(MinValue = 1, MaxValue = 10000)]
		[Map, Show]
		public int Units
		{
			get => _Units; set
			{
				_Units = value;
				WorkerPayment = _Units;
			}
		}
		private int _Units = 1;
		[Map, Show] public int Days { get; set; } = 1;
		[Map, Show] public int? Hours { get; set; }

		[Map, Show] public Decimal WorkerPayment { get; set; }

		[Map, Show] public string? Notes { get; set; }

	}

}
