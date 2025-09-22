using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Types;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;
using eFlex.Index.Base.Extensions;
using Classifiers = eFlex.Index.Demo.Resource.Classifiers;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Builders;

namespace frlnet.Controllers.frl
{

    [Claim(eClaimName.Read)]
    [Claim(eClaimName.Create)]
    [Claim(nameof(Approve))]
    [Claim(nameof(Close))]
    public class InventoryRequestController : hAreaIndexController<InventoryRequestModel>
    {

        public InventoryRequestController()
        {
            Instructions.Delete.Allow = false;
            Instructions.Edit.AfterSave = EditBuilder.eAfterSave.RefreshPage;

            Instructions.Index.Grid.Buttons.Create.Visible = false;

            {
                var btn = ButtonBuilder.DefaultButtons.IndexGenericFromId(Labels.ApproveRequest, nameof(ApproveAllow));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Index.Grid.Buttons.List.Add(btn);
            }

            {
                var btn = ButtonBuilder.DefaultButtons.IndexGenericFromId(Labels.DeclineRequest, nameof(DeclineAllow));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Index.Grid.Buttons.List.Add(btn);
            }

            {
                var btn = ButtonBuilder.DefaultButtons.IndexGenericFromId(Labels.ReceiveRequest, nameof(ReceiveAllow));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Index.Grid.Buttons.List.Add(btn);
            }

            {
                var btn = ButtonBuilder.DefaultButtons.IndexGenericFromId(Labels.CloseRequest, nameof(CloseAllow));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Index.Grid.Buttons.List.Add(btn);
            }
        }

        private string? ValidateInventory()
        {
            var inventoryId = IndexNavigation.Current.GetQueryGuid<InventoryController>();
            if (!inventoryId.HasValue)
                return Messages.InventoryRequestSourceBadRoute;

            var inventoryModel = AutoProcedure.Of<InventoryModel>().Get(inventoryId.Value);

            if (inventoryModel is null)
                return Messages.InventoryRequestSourceBadRoute;

            if (inventoryModel.CountLeft <= 0)
                return Messages.InventoryRequestSourceBadCount;

            if (inventoryModel.AvailableFrom > DateTime.Now.ToDateOnly())
                return Messages.InventoryRequestSourceBadDate;

            return null;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var userId = IndexNavigation.Current.GetQueryGuid<Admin.UserRootController>();
            userId ??= IndexNavigation.Current.GetQueryGuid<Public.UserRootController>();
            userId ??= UserModel.CurrentId!.Value;
            where.Parts.Add(new(nameof(InventoryRequestModel.UserId), userId));
            return where;
        }

        protected override InventoryRequestModel CreateViewModel()
        {
            var model = base.CreateViewModel();
            model.InventoryId = IndexNavigation.Current.GetQueryGuid<InventoryController>()!.Value;
            model.UserId = UserModel.CurrentId!.Value;
            model.InventoryCountLeft = AutoProcedure.Of<InventoryModel>().Get(model.InventoryId)!.CountLeft;
            return model;
        }

        public override IndexResult CreateAllow()
        {
            var validate = ValidateInventory();
            if (validate is not null)
                return new IndexResult(validate);

            return base.CreateAllow();
        }

        public override IndexResult CreateConfirm(InventoryRequestModel model)
        {
            { //Validations.
                var validate = ValidateInventory();
                if (validate is not null)
                    return new IndexResult(validate);

                var inventoryId = IndexNavigation.Current.GetQueryGuid<InventoryController>();
                var inventoryModel = AutoProcedure.Of<InventoryModel>().Get(inventoryId!.Value);

                if (model.Count > inventoryModel!.CountLeft)
                    return new IndexResult(Messages.InventoryRequestBadCount);
            }

            var result = base.CreateConfirm(model);

            //Insert status.
            if (!result.Messages.Any(f => f.Level == IndexResult.Message.eLevel.Error!))
            {
                var statusModel = new InventoryStatusModel()
                {
                    InventoryRequestId = model.Id!.Value,
                    ActionClvId = Classifiers._InventoryStatusAction._Requested.Id,
                    Notes = model.Notes

                };

                var statusId = AutoProcedure.Of<InventoryStatusModel>().Insert(statusModel);
                model.InventoryStatusId = statusId;
                Procedure.Update(model);
            }

            if (result.HasLink)
                return new IndexResult(Link.ToIndex<InventoryController>(), false, result.Messages);
            return result;
        }

        public override ViewResult Edit(Guid id)
        {
            {
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(Labels.ApproveRequest,
                new Link(nameof(ApproveAllow), new { id = IndexNavigation.Current.GetQueryGuid<InventoryRequestController>() }));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Edit.Buttons.List.Add(btn);
            }
            {
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(Labels.DeclineRequest,
                new Link(nameof(DeclineAllow), new { id = IndexNavigation.Current.GetQueryGuid<InventoryRequestController>() }));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Edit.Buttons.List.Add(btn);
            }
            {
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(Labels.ReceiveRequest,
                new Link(nameof(ReceiveAllow), new { id = IndexNavigation.Current.GetQueryGuid<InventoryRequestController>() }));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Edit.Buttons.List.Add(btn);
            }
            {
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(Labels.CloseRequest,
                new Link(nameof(CloseAllow), new { id = IndexNavigation.Current.GetQueryGuid<InventoryRequestController>() }));
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Edit.Buttons.List.Add(btn);
            }

            return base.Edit(id);
        }


        [Claim(nameof(Approve))]
        public IndexResult ApproveAllow(Guid id)
        {
            var result = DeclineAllow(id);
            if (!result.HasLink)
                return result;

            return ApproveConfirmInner(new ApproveModel() { Id = id }, true);
        }

        [Claim(nameof(Approve))]
        public IndexResult DeclineAllow(Guid id)
        {
            var where = new SqlWhereCondition();
            where.Parts.Add(new(nameof(InventoryStatusModel.InventoryRequestId), id));
            var statusModels = AutoProcedure.Of<InventoryStatusModel>().GetRange(where);
            if (statusModels.Any(f => f.ActionClvId == Classifiers._InventoryStatusAction._Received.Id))
                return new IndexResult(Messages.InventoryRequestActionInvalid);

            var link = new Link(nameof(Approve), this.GetType(), HttpContext.Request.Query.ToDynamic());
            return new IndexResult(link, false, new { dialogTitle = Labels.DeclineRequest.ToString() });
        }

        [Audit]
        [Claim(nameof(Approve))]
        public PartialViewResult Approve(Guid id)
        {
            var dialog = CreateDilaog<ApproveModel>(id);
            var indexModel = extractIndexModel<ApproveModel>(dialog)!;
            indexModel.Instructions.Edit.Buttons.List.Add(ButtonBuilder.DefaultButtons.EditGeneric(Labels.DeclineRequest, nameof(DeclineConfirm)));
            return dialog;
        }


        //[HttpPost]
        //[Audit]
        //[Claim(nameof(Approve))]
        //public IndexResult ApproveConfirm(ApproveModel model) => ApproveConfirmInner(model, true);

        [HttpPost]
        [Audit]
        [Claim(nameof(Approve))]
        public IndexResult DeclineConfirm(ApproveModel model) => ApproveConfirmInner(model, false);

        [NonAction]
        private IndexResult ApproveConfirmInner(ApproveModel model, bool approve)
        {
            var id = model.Id!.Value;
            CompareAllow(ApproveAllow(id), id, nameof(Approve));

            var statusClvId = approve ? Classifiers._InventoryStatusAction._Approved.Id : Classifiers._InventoryStatusAction._Declined.Id;
            return UpdateStatus(model, statusClvId);
        }


        [Claim(nameof(Receive))]
        public IndexResult ReceiveAllow(Guid id)
        {
            var where = new SqlWhereCondition();
            where.Parts.Add(new(nameof(InventoryStatusModel.InventoryRequestId), id));
            var statusModels = AutoProcedure.Of<InventoryStatusModel>().GetRange(where).OrderByDescending(f => f.RowCreated);
            if (statusModels.Any(f => f.ActionClvId == Classifiers._InventoryStatusAction._Received.Id))
                return new IndexResult(Messages.InventoryRequestActionInvalid);

            var lastApprove = statusModels.FirstOrDefault(f => f.ActionClvId == Classifiers._InventoryStatusAction._Approved.Id || f.ActionClvId == Classifiers._InventoryStatusAction._Declined.Id);
            if (lastApprove is null || lastApprove.ActionClvId != Classifiers._InventoryStatusAction._Approved.Id)
                return new IndexResult(Messages.InventoryRequestActionInvalid);

            var link = new Link(nameof(Receive), this.GetType(), HttpContext.Request.Query.ToDynamic());
            return new IndexResult(link, false, new { dialogTitle = Labels.ReceiveRequest.ToString() });
        }

        [Audit]
        [Claim(nameof(Receive))]
        public PartialViewResult Receive(Guid id)
        {
            var dialog = CreateDilaog<ApproveModel>(id);
            var indexModel = extractIndexModel<ApproveModel>(dialog)!;
            indexModel.Instructions.Edit.Buttons.List.Add(ButtonBuilder.DefaultButtons.EditGeneric(Labels.ReceiveRequest, nameof(ReceiveConfirm)));
            return dialog;
        }

        [HttpPost]
        [Audit]
        [Claim(nameof(Receive))]
        public IndexResult ReceiveConfirm(ApproveModel model)
        {
            var id = model.Id!.Value;
            CompareAllow(ReceiveAllow(id), id, nameof(Receive));
            return UpdateStatus(model, Classifiers._InventoryStatusAction._Received.Id);
        }


        [Claim(nameof(Close))]
        public IndexResult CloseAllow(Guid id)
        {
            var where = new SqlWhereCondition();
            where.Parts.Add(new(nameof(InventoryStatusModel.InventoryRequestId), id));
            var statusModels = AutoProcedure.Of<InventoryStatusModel>().GetRange(where);
            if (!statusModels.Any(f => f.ActionClvId == Classifiers._InventoryStatusAction._Received.Id))
                return new IndexResult(Messages.InventoryRequestActionInvalid);
            if (statusModels.Any(f => f.ActionClvId == Classifiers._InventoryStatusAction._Closed.Id))
                return new IndexResult(Messages.InventoryRequestActionInvalid);

            var link = new Link(nameof(Close), this.GetType(), HttpContext.Request.Query.ToDynamic());
            return new IndexResult(link, false, new { dialogTitle = Labels.CloseRequest.ToString() });
        }

        [Audit]
        [Claim(nameof(Close))]
        public PartialViewResult Close(Guid id)
        {
            var dialog = CreateDilaog<ApproveModel>(id);
            var indexModel = extractIndexModel<ApproveModel>(dialog)!;
            indexModel.Instructions.Edit.Buttons.List.Add(ButtonBuilder.DefaultButtons.EditGeneric(Labels.CloseRequest, nameof(CloseConfirm)));
            return dialog;
        }

        [HttpPost]
        [Audit]
        [Claim(nameof(Close))]
        public IndexResult CloseConfirm(ApproveModel model)
        {
            var id = model.Id!.Value;
            CompareAllow(CloseAllow(id), id, nameof(Close));
            return UpdateStatus(model, Classifiers._InventoryStatusAction._Closed.Id);
        }


        [NonAction]
        private IndexResult UpdateStatus(ApproveModel model, Guid status)
        {
            var id = model.Id!.Value;
            var statusModel = new InventoryStatusModel()
            {
                InventoryRequestId = id,
                ActionClvId = status,
                Notes = model.Notes
            };
            var statusId = AutoProcedure.Of<InventoryStatusModel>().Insert(statusModel);

            var requestModel = Procedure.Get(id);
            requestModel!.InventoryStatusId = statusId;
            requestModel.Active = true;

            if (status == Classifiers._InventoryStatusAction._Declined.Id)
                requestModel.Active = false;

            if (status == Classifiers._InventoryStatusAction._Received.Id)
                requestModel.ReceiveDate = DateTime.Now.ToDateOnly();

            Procedure.Update(requestModel);

            var link = default(Link?);
            if (IndexNavigation.Current.Routes.FirstOrDefault()?.ActionName == nameof(Edit))
                link = IndexNavigation.Current.Routes.First().ToLink();
            return new IndexResult(link, false, Messages.InventoryRequestStatusUpdated);

        }

        [NonAction]
        private InventoryStatusModel? GetStatus(Guid requerstId)
        {
            var requestModel = Procedure.Get(requerstId);
            if (requestModel is null)
                return null;

            if (!requestModel.InventoryStatusId.HasValue)
                return null;

            var statusModel = AutoProcedure.Of<InventoryStatusModel>().Get(requestModel.InventoryStatusId.Value);
            if (statusModel is null)
                return null;

            return statusModel;
        }

        internal PartialViewResult CreateDilaog<TModel>(Guid id) where TModel : ModelBase
        {
            var instructions = IndexInstructions.CreateInstructions();
            instructions.Context.ShowHeader = false;
            instructions.Edit.DefaultControlSize = ControlBuilder.eControlSize.c12;
            instructions.Edit.Buttons.List.Clear();
            var indexModel = new IndexModel<TModel>(this.GetType(), nameof(Approve));
            indexModel.Instructions= instructions;
            indexModel.Model = (ModelBase)Activator.CreateInstance(typeof(TModel))!;
            indexModel.Model.Id = id;
            return PartialBuilder(instructions.Edit, indexModel);
        }

    }
}