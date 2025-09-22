namespace frlnet.Models.frl
{
    [MapSource("frl", "PostPlan")]
    public class PostPlanModel : EditableModelBase
    {
        [Map] public Guid PostId { get; set; }

		[Map, Show] public string Name { get; set; } = null!;

        [Map, Show] public int AfterHour { get; set; }

        [NumericSettings(Decimals =2)]
        [Map, Show] public decimal WorkerPayment { get; set; }

        [NumericSettings(Decimals =2)]
        [Map, Show] public decimal ClientPayment { get; set; }

		[Map, Show] public string? Description { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
				return base.ToString();
			return Name;
		}
	}
}
