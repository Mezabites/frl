using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.frl
{
    public class InventoryController : hAreaIndexController<InventoryModel>
    {

        [Claim(eClaimName.Read)]
        public ViewResult EditReadOnly(Guid id)
        {
            var result = Edit(id);
            Instructions.Edit.Allow = false;
            Instructions.Edit.Buttons.List.Clear();
            return result;
        }

        public override ViewResult Edit(Guid id)
        {
            {//Request.
                var btn = ButtonBuilder.DefaultButtons.IndexCreate();
                btn.Title = Labels.InventoryRequest;
                btn.Args.Set("url", new Link(nameof(InventoryRequestController.CreateAllow), typeof(InventoryRequestController)));
                Instructions.Edit.Buttons.List.Add(btn);
            }

            return base.Edit(id);
        }

        public override IndexResult DeleteAllow([FromQuery] Guid[] id)
        {
            var models = Procedure.GetRange(id);
            if(models.Any(f=> f.CountLeft < f.Count))
                return new IndexResult(Messages.InventoryDeleteBadCount);
            return base.DeleteAllow(id);
        }
    }
}