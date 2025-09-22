using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Models.App;
using frlnet.Controllers.Admin;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
    [NoClaim()]
    public class UserDocumentUserController : hAreaIndexController<UserDocumentUserModel>
    {
        public UserDocumentUserController()
        {
            Instructions.Index.Filter.ShowConfigurator = false;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var userId = IndexNavigation.Current.GetQueryGuid<Admin.UserRootController>();
            userId ??= IndexNavigation.Current.GetQueryGuid<Public.UserRootController>();
            userId ??= UserModel.CurrentId!.Value;
            where.Parts.Add(new(nameof(UserDocumentUserModel.UserId), userId));
            return where;
        }

        public override IndexResult CreateConfirm(UserDocumentUserModel model)
        {
            var error = ValidateModel(model, true);
            if (error != null) return error;

            var currentUserId = UserModel.CurrentId;
            if (currentUserId.HasValue == false)
                return new IndexResult(Messages.IndexEditConfirmFailure);

            var currentOrgId = OrganizationModel.CurrentId;
            if (currentOrgId.HasValue == false)
                return new IndexResult(Messages.IndexEditConfirmFailure);

            model.UserId = currentUserId.Value;
            model.StatusClvId = Classifiers._UserDocumentStatus._Pending.Id;
            return base.CreateConfirm(model);
        }

        public override IndexResult EditConfirm(UserDocumentUserModel model)
        {
            var error = ValidateModel(model, false);
            if (error != null) return error;
            model.StatusClvId = Classifiers._UserDocumentStatus._Pending.Id;
            model.ApproveDate = null;
            model.ApprovedByUserId = null;
            return base.EditConfirm(model);
        }

        private static IndexResult? ValidateModel(UserDocumentUserModel model, bool isCreate)
        {
            var notDeletedImages = model.Images != null ? model.Images.Where(f => !f.IsDeleted).ToArray() : null;

            if(notDeletedImages == null || notDeletedImages.Any() == false)
                return new IndexResult(Messages.DocumentImagesAreNotSet);

            if (model.TypeClvId.HasValue == false)
                return new IndexResult(string.Format(Messages.IndexValidateField_Required, nameof(model.TypeClvId)));

            var ImagesCount = notDeletedImages.Count();
            if (UserDocumentModel.GetFileCountByTypeClvId(model.TypeClvId) != ImagesCount)
                return new IndexResult(string.Format(Messages.ImageSettingRequiredFileCount.ToString().Replace(Constants.ErrorPrefix, string.Empty), UserDocumentModel.GetFileCountByTypeClvId(model.TypeClvId)));

            //for Image
            if (model.IsComplexForm == false)
            {
                model.Nr = null;
                model.IssueDate = null;
                model.Issuer = null;
                model.AdditionalTypeClvIdCreate = null;
                model.ExpirationDate = null;
            }
            else
            {
                if (string.IsNullOrEmpty(model.Nr))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentUserController), nameof(model.Nr)).Text));
                if (model.IssueDate.HasValue == false)
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentUserController), nameof(model.IssueDate)).Text));
                if (string.IsNullOrEmpty(model.Issuer))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentUserController), nameof(model.Issuer)).Text));
                if (model.ExpirationDate.HasValue == false)
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentUserController), nameof(model.ExpirationDate)).Text));

                if (isCreate && (model.AdditionalTypeClvIdCreate == null || model.AdditionalTypeClvIdCreate.Any() == false))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentUserController), nameof(model.AdditionalTypeClvIdCreate)).Text));

                if (isCreate == false && (model.AdditionalTypeClvIdEditGrid == null || model.AdditionalTypeClvIdEditGrid.Any() == false))
                    return new IndexResult(string.Format(Messages.IndexValidateField_Required, LabelFieldModel.GetValue(typeof(UserDocumentUserController), nameof(model.AdditionalTypeClvIdEditGrid)).Text));
            }

            return null;
        }
    }
}