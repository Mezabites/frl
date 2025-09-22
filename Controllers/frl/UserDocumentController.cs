using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Extensions;
using eFlex.Index.Base.Models.App;
using eFlex.Index.Base.Types;
using frlnet.Models.Admin;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;
using static eFlex.Connectivity.Com.SqlWherePart;
using static eFlex.Index.Base.Builders.ControlBuilder;

namespace frlnet.Controllers.frl
{
	[Claim(eClaimName.Read)]
    [Claim(eClaimName.Update)]
    [Claim(nameof(Approve))]
    public class UserDocumentController : hAreaIndexController<UserDocumentModel>
    {
        public UserDocumentController()
        {
            Instructions.Create.Allow = false;

            Instructions.Index.Grid.UseDefaultGeneratedSortKey = false;

            //{
            //    var btn = IndexInstructions<UserDocumentModel>.DefaultButtons.IndexGenericFromId(Labels.ApproveUserDocument, nameof(ApproveAllow));
            //    btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
            //    Instructions.Buttons.Add(btn);
            //}

            //{
            //    var btn = IndexInstructions<UserDocumentModel>.DefaultButtons.IndexGenericFromId(Labels.DeclineUserDocument, nameof(DeclineAllow));
            //    btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
            //    Instructions.Buttons.Add(btn);
            //}
        }

        public override ViewResult Edit(Guid id)
        {
            {
                var link = new Link(nameof(ApproveAllow), new { id = IndexNavigation.Current.GetQueryValue(this.GetType()) });
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(Labels.ApproveUserDocument, link);
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Edit.Buttons.List.Add(btn);
            }

            {
                var link = new Link(nameof(DeclineAllow), new { id = IndexNavigation.Current.GetQueryValue(this.GetType()) });
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(Labels.DeclineUserDocument, link);
                btn.Theme = Kendo.Mvc.UI.ThemeColor.Tertiary;
                Instructions.Edit.Buttons.List.Add(btn);
            }

            return base.Edit(id);
        }

        public override IndexResult EditConfirm(UserDocumentModel model)
        {
            var invalid = ValidateModel(model, false);
            if (invalid != null) return invalid;
            return base.EditConfirm(model);
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            //AdditionalTypeClvIdFilter add filter for.
            var where = base.TranslateFilter(indexModel);
            var documentValid = where.Parts.FirstOrDefault(f => f.Name == nameof(UserDocumentModel.DocumentValid));

            if (documentValid != null)
            {
                var documentValidWhere = new SqlWherePart(nameof(UserDocumentModel.ExpirationDate), (DateOnly)documentValid.Values[0], SqlWherePart.CommonPatterns.DbDateTimeLargerOrNull);
                where.Parts.Remove(documentValid);
                where.Parts.Add(documentValidWhere);
            }

            var additionalTypeClvIdFilter = where.Parts.FirstOrDefault(f => f.Name == nameof(UserDocumentModel.AdditionalTypeClvIdFilter));
            if (additionalTypeClvIdFilter != null)
            {
                var docIds = AutoProcedure.Of<UserDocumentClvModel>().GetRangeSpecific<Guid>(nameof(UserDocumentClvModel.UserDocumentId),
                    new SqlWhereCondition(nameof(UserDocumentClvModel.AdditionalTypeClvId), additionalTypeClvIdFilter.Values.Select(f => Guid.Parse(f.ToString()!)).ToArray(), CommonPatterns.GuidIntIncludes)).Distinct();

                var addTypeWhere = new SqlWherePart(nameof(UserDocumentModel.Id), docIds.ToArray(), SqlWherePart.CommonPatterns.GuidIntIncludes);
                where.Parts.Remove(additionalTypeClvIdFilter);
                where.Parts.Add(addTypeWhere);
            }

            return where;
        }

        [Claim(nameof(Approve))]
        public IndexResult ApproveAllow(Guid id)
        {
            var res = DeclineAllow(id);
            if (!res.HasLink)
                return res;

          return  ApproveConfirmInner(new ApproveModel() { Id = id}, true);
        }

        [Claim(nameof(Approve))]
        public IndexResult DeclineAllow(Guid id)
        {
            var res = EditAllow(id);
            if (!res.HasLink)
                return res;

            var link = new Link(nameof(Approve), this.GetType(), HttpContext.Request.Query.ToDynamic());
            return new IndexResult(link, false, new { dialogTitle = Labels.DeclineUserDocument.ToString() });
        }

        [Audit]
        [Claim(nameof(Approve))]
        public PartialViewResult Approve(Guid id)
        {
            var instructions = IndexInstructions.CreateInstructions();
            instructions.Context.ShowHeader = false;
            instructions.Edit.DefaultControlSize = eControlSize.c12;
            instructions.Edit.Buttons.List.Clear();

            //instructions.Buttons.Add(IndexInstructions<ApproveModel>.DefaultButtons.EditGeneric(Labels.ApproveUserDocument, nameof(ApproveConfirm)));
            instructions.Edit.Buttons.List.Add(ButtonBuilder.DefaultButtons.EditGeneric(Labels.DeclineUserDocument, nameof(DeclineConfirm)));

            var indexModel = new IndexModel<ApproveModel>(this.GetType(), nameof(Approve));
            indexModel.Instructions = instructions;
            indexModel.Model = new ApproveModel() { Id = id };
            
            return PartialBuilder(instructions.Edit, indexModel);
        }

        [NonAction]
        private IndexResult ApproveConfirmInner(ApproveModel model, bool approve)
        {
            var docModel = Procedure.Get(model.Id!.Value)!;

            //Update status.
            if (approve)
            {
                docModel.StatusClvId = Classifiers._UserDocumentStatus._Approved.Id;
                docModel.ApprovedByUserId = UserModel.CurrentId;
                docModel.ApproveDate = DateTimeOffset.Now.ToDateOnly();
                docModel.DeclinedReason = null;
            }
            else
            {
                docModel.DeclinedReason = model.Notes;
                docModel.StatusClvId = Classifiers._UserDocumentStatus._Declined.Id;
            }

            AutoProcedure.Of<UserDocumentModel>().Update(docModel);

            var link = new Link(nameof(Edit), this.GetType(), new { id = model.Id });
            return new IndexResult(link, true, new IndexResult.Message(Messages.DocumentApprovedSuccess, IndexResult.Message.eLevel.Success));
        }

        //[HttpPost]
        //[Audit]
        //[Claim(nameof(Approve))]
        //public IndexResult ApproveConfirm(ApproveModel model) => ApproveConfirmInner(model, true);

        [HttpPost]
        [Audit]
        [Claim(nameof(Approve))]
        public IndexResult DeclineConfirm(ApproveModel model) => ApproveConfirmInner(model, false);

        private static IndexResult? ValidateModel(UserDocumentModel model, bool isCreate)
        {
            var notDeletedImages = model.Images != null ? model.Images.Where(f => !f.IsDeleted).ToArray() : null;

            if (notDeletedImages == null || notDeletedImages.Any() == false)
                return new IndexResult(Messages.DocumentImagesAreNotSet);

            if (model.TypeClvId.HasValue == false)
                return new IndexResult(string.Format(Messages.IndexValidateField_Required, nameof(model.TypeClvId)));

            var ImagesCount = notDeletedImages.Count();
            if (UserDocumentModel.GetFileCountByTypeClvId(model.TypeClvId) != ImagesCount)
                return new IndexResult(string.Format(Messages.ImageSettingRequiredFileCount.ToString().Replace(Constants.ErrorPrefix, string.Empty), UserDocumentModel.GetFileCountByTypeClvId(model.TypeClvId)));

            if (model.IsComplexForm == false)
            {
                model.Nr = null;
                model.IssueDate = null;
                model.Issuer = null;
                model.ExpirationDate = null;
            }
            else
            {
                if (string.IsNullOrEmpty(model.Nr))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentController), nameof(model.Nr)).Text));
                if (model.IssueDate.HasValue == false)
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentController), nameof(model.IssueDate)).Text));
                if (string.IsNullOrEmpty(model.Issuer))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentController), nameof(model.Issuer)).Text));
                if (model.ExpirationDate.HasValue == false)
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentController), nameof(model.ExpirationDate)).Text));
                if (isCreate == false && (model.AdditionalTypeClvIdEditGrid == null || model.AdditionalTypeClvIdEditGrid.Any() == false))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentController), nameof(model.AdditionalTypeClvIdEditGrid)).Text));
            }

            return null;
        }
    }
}
