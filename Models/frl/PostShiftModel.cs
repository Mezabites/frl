namespace frlnet.Models.frl
{
    [MapSource("frl", "PostShift")]
    public class PostShiftModel : EditableModelBase
    {
        [Map] public Guid PostId { get; set; }

        [Show, Map] public string Name { get; set; } = null!;

        [Show, Map] public Decimal WorkerPayment { get; set; }

        [Show, Map] public Decimal ClientPayment { get; set; }

        [Show, Map]
        public bool ReportHours
        {
            get
            {
                return _ReportHours;
            }
            set
            {
                _ReportHours = value;
            }
        }
        private bool _ReportHours = false;

        [Show]
        [Map] public int? MaxHours { get => ReportHours ? _MaxHours : null; set => _MaxHours = value; }
        private int? _MaxHours = null;

        [Show, Map] public string? Description { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
				return base.ToString();
			return Name;
		}
	}
}
