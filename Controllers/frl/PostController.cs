using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models.App;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.frl
{
    public class PostController : hAreaIndexController<PostModel>
    {
		public PostController()
		{
			Instructions.Edit.AfterSave = EditBuilder.eAfterSave.ReturnToPrevius;
			Instructions.Create.AfterSave = CreateBuilder.eAfterSave.ReturnToPrevius;
			Instructions.Edit.UseDynamicUpdate = false;
			Instructions.Edit.MonitorChanges = false;
			Instructions.Create.UseDynamicUpdate = false;
		}

		public override ViewResult Create()
        {
            var result = base.Create();
            var model = extractViewModel(result);
            if (model is not null)
            {
                var mainPlan = new PostPlanModel()
                {
                    Id = Guid.NewGuid(),
                    IsNew = true,
                    Name = LabelFieldModel.GetValue(typeof(PostController), "Hourly rate").Text,
                    AfterHour = 0,
                    WorkerPayment = 20u,
                    ClientPayment = 30u,
                };
                var overtimePlan = new PostPlanModel()
                {
                    Id = Guid.NewGuid(),
                    IsNew = true,
                    Name = LabelFieldModel.GetValue(typeof(PostController), "Overtime rate").Text,
                    AfterHour = 8,
                    WorkerPayment = 40u,
                    ClientPayment = 60u,
                };

                model.Plan = [mainPlan, overtimePlan];

                var shifts = new List<PostShiftModel>(14);
                model.Shift = shifts;

                for (var i = 1; i <= 12; i++)
                {
                    shifts.Add(new PostShiftModel()
                    {
                        Id = Guid.NewGuid(),
                        IsNew = true,
                        Name = i.ToString(),
                        MaxHours = i,
                        ReportHours = true,
                    });
                }

                shifts.Add(new PostShiftModel()
                {
                    Id = Guid.NewGuid(),
                    IsNew = true,
                    Name = "Day",
                    MaxHours = null,
                    ReportHours = false,
                });

                shifts.Add(new PostShiftModel()
                {
                    Id = Guid.NewGuid(),
                    IsNew = true,
                    Name = "Week",
                    MaxHours = null,
                    ReportHours = false,
                });

            }
            return result;
        }

        public override IndexResult CreateConfirm(PostModel model)
        {
            var shiftBackup = model.Shift;
            model.Shift = model.Shift.Where(f => f.WorkerPayment > 0);
            var result = base.CreateConfirm(model);
            if(result.HasErrors)
            {
                model.Shift = shiftBackup;
            }
            return result;
        }

        //[Audit]
        //[Claim(eClaimName.Create)]
        //public PartialViewResult Copy()
        //{
        //   var id = IndexNavigation.Current.GetQueryGuid<PostController>();
        //    if (!id.HasValue)
        //        throw new Exception("Faild to find id from navigation.");
        //}
    }
}
