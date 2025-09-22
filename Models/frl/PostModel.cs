using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
	[MapSource("frl", "Post")]
	public class PostModel : ModelBase
	{
		[Show, Map] public string Name { get; set; } = null!;
		[Show, Map] public string? Description { get; set; }

		[Show, Map] public DateOnly? Start { get; set; }
		[Show, Map] public DateOnly? End { get; set; }

		[ShowMark(In.Edit | In.Create)]

		[Show]
		[LabelGroupMark]
		[DynamicUpdateIgnore]
		[MapModel(nameof(PostPlanModel.PostId))]
		[GridSettings(typeof(PostPlanController))]
		public IEnumerable<PostPlanModel> Plan { get; set; }

		[Show]
		[LabelGroupMark]
		[DynamicUpdateIgnore]
		[MapModel(nameof(PostShiftModel.PostId))]
		[GridSettings(typeof(PostShiftController))]
		public IEnumerable<PostShiftModel> Shift { get; set; }

		public record PlanPayment(decimal WorkerPayment, decimal ClientPayment);

		public PlanPayment GetPlanPayment(int hours, Guid? shiftId)
		{
			// 1. Handle the case where no plans are defined
			if (Plan == null || !Plan.Any())
			{
				// No plans, return default payment
				return new PlanPayment(0, 0);
			}

			decimal workerPayment = 0;
			decimal clientPayment = 0;
			int calcHour = 0;

			// 2. Handle shift details if a shift ID is provided
			if (shiftId.HasValue)
			{
				var shiftModel = Shift.FirstOrDefault(f => f.Id == shiftId);
				if (shiftModel != null)
				{
					workerPayment = shiftModel.WorkerPayment;
					clientPayment = shiftModel.ClientPayment;
					calcHour = shiftModel.MaxHours ?? 0;

					// If shift hours are not reported, return the base shift payments
					if (!shiftModel.ReportHours)
					{
						return new PlanPayment(workerPayment, clientPayment);
					}
				}
			}

			// 3. Sort plans by AfterHour to ensure they are in ascending order
			var plans = Plan.OrderBy(f => f.AfterHour).ToArray();

			// 4. Accumulate payments for each hour from calcHour to hours - 1
			for (int h = calcHour; h < hours; h++)
			{
				// Find the applicable plan for the current hour
				var plan = plans.LastOrDefault(p => p.AfterHour <= h);

				// If no plan matches, it means no valid plan is found for this hour
				// This should not happen if plans are correctly defined; handle defensively
				if (plan == null)
				{
					return new PlanPayment(workerPayment, clientPayment);
				}

				// Accumulate payments based on the found plan
				workerPayment += plan.WorkerPayment;
				clientPayment += plan.ClientPayment;
			}

			// 5. Return the accumulated payments
			return new PlanPayment(workerPayment, clientPayment);
		}


		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
				return base.ToString();
			return Name;
		}
	}


}
