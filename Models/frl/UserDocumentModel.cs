using eFlex.Common.Extensions;
using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.Controls.Settings.Interfaces;
using eFlex.Index.Base.FilterConditions;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Models.App;
using eFlex.Index.Base.Types;

namespace frlnet.Models.frl
{
    [MapSource("frl", "UserDocument")]
    [MapJoin("adm", "User")]
    public class UserDocumentModel : ModelBase, IOrganizationModel
    {
        [Show(In.Filter, In.Filter)]
        [Map("UserOrganizationId")] public Guid? OrganizationId { get; set; }
        public static SelectList<Guid> OrganizationIdSource => OrganizationModel.ParentOrganizationIdSource;

        [Show(In.Edit | In.Grid)][Map] public string Name { get; set; }

        [Map] public Guid UserId { get; set; }

        //special claims?
        [DropListSettings(FilterType = Kendo.Mvc.UI.FilterType.Contains)]
        [Show(In.Filter | In.Grid | In.Edit, In.Filter)][Map] public Guid? ApprovedByUserId { get; set; }
        public static SelectList<Guid?> ApprovedByUserIdSource => GetUserSource().ToNullableSelectList(f => $"{f.FirstName} {f.LastName} ({f.PersonCode})", f => f.Id);

        [Show(In.Edit, In.Edit, visibleIf: nameof(IsComplexForm))][Map] public string? Nr { get; set; }
        [Show(In.Edit, In.Edit, visibleIf: nameof(IsComplexForm))][Map] public DateOnly? IssueDate { get; set; }
        [Show(In.Edit, In.Edit, visibleIf: nameof(IsComplexForm))][Map] public string? Issuer { get; set; }

        private static IEnumerable<UserModel> GetUserSource() => AutoProcedure.Of<UserModel>().GetRange();

        [DropListSettings(SourceClassifierCode = Classifiers._UserDocumentType.Code, FilterType = Kendo.Mvc.UI.FilterType.Contains)]
        [Show][Map] public Guid? TypeClvId { get; set; }

        //only for filter
        [MultiDropListSettings(nameof(AdditionalTypeClvIdFullSource), FilterType = Kendo.Mvc.UI.FilterType.Contains)]
        [Show(In.Filter)][Map(IsRequired = false)] public StringList<Guid>? AdditionalTypeClvIdFilter { get; set; }

        [Show(In.Edit | In.Grid, In.Edit, visibleIf: nameof(IsComplexForm))]
        [ListBoxSettings(nameof(AdditionalTypeClvIdSource), Required =  IRequiredSettings.Require.True)]
        [MapQuery("SELECT string_agg(\"AdditionalTypeClvId\"::text, '|') FROM frl.\"UserDocumentClv\" WHERE \"UserDocumentId\" = J1.\"Id\"")]
        public StringList<Guid>? AdditionalTypeClvIdEditGrid { get; set; }

        public static readonly Guid[] ClassifierWithAdditionalTypes = { Classifiers._DriversLicense.Id, Classifiers._PersonDocument.Id, Classifiers._Education.Id, Classifiers._Qualification.Id };
        public static SelectList<Guid> AdditionalTypeClvIdFullSource => AutoProcedure.Of<ClassifierValueModel>().GetRange(nameof(ClassifierValueModel.ClassifierId), ClassifierWithAdditionalTypes).ToSelectList(f => f.Name!, f => f.Id!.Value);

        public SelectList<Guid> AdditionalTypeClvIdSource => GetAdditionalClassifierValues(TypeClvId).ToSelectList(f => f.Name!, f => f.Id!.Value);

        public static IEnumerable<ClassifierValueModel> GetAdditionalClassifierValues(Guid? classifierId)
        {
            if (classifierId.HasValue == false) return Enumerable.Empty<ClassifierValueModel>();
            var link = GetAdditionalLinksClvId(classifierId.Value);
            if (link.HasValue == false) return Enumerable.Empty<ClassifierValueModel>();
            return AutoProcedure.Of<ClassifierValueModel>().GetRange(nameof(ClassifierValueModel.ClassifierId), link, SqlWherePart.CommonPatterns.GuidEqual);
        }

        public static Guid? GetAdditionalLinksClvId(Guid classifierId)
        {
            if (classifierId == Classifiers._UserDocumentType._DriversLicense.Id)
                return Classifiers._DriversLicense.Id;
            else if (classifierId == Classifiers._UserDocumentType._PersonDocument.Id)
                return Classifiers._PersonDocument.Id;
            else if (classifierId == Classifiers._UserDocumentType._EducationDocument.Id)
                return Classifiers._Education.Id;
            else if (classifierId == Classifiers._UserDocumentType._Qualification.Id)
                return Classifiers._Qualification.Id;
            return null;
        }

        [DropListSettings(SourceClassifierCode = Classifiers._UserDocumentStatus.Code)]
        [Show][Map] public Guid StatusClvId { get; set; }

        [Show(In.Grid | In.Edit, In.Edit, visibleIf: nameof(IsComplexForm))][Map] public DateOnly? ExpirationDate { get; set; }
        [Show(In.Filter)][Map(IsRequired = false)] public DateOnly? DocumentValid { get; set; }

        [Map] public DateOnly? StatusDate { get; set; }
        [Map] public DateOnly? ApproveDate { get; set; }

        [Show(In.Edit | In.Grid, In.Edit)][Map] public string? Description { get; set; }
        [Show(In.Edit | In.Grid, In.Edit)][Map] public string? DeclinedReason { get; set; }

        //value is required count of files
        public static readonly Dictionary<Guid, int> ComplexClassifiersDic = new()
        {
            { Classifiers._UserDocumentType._DriversLicense.Id, 2},
            { Classifiers._UserDocumentType._PersonDocument.Id, 2},
            { Classifiers._UserDocumentType._EducationDocument.Id, 2},
            { Classifiers._UserDocumentType._Qualification.Id, 2},
        };

        public static int GetFileCountByTypeClvId(Guid? TypeClvId) => TypeClvId.HasValue && ComplexClassifiersDic.TryGetValue(TypeClvId.Value, out int value) ? value : 1;

        public bool IsComplexForm => TypeClvId.HasValue && UserDocumentModel.ComplexClassifiersDic.ContainsKey(TypeClvId.Value);

        [LabelGroupMark]
        [Show(In.Edit, In.Edit)]
        [MapModel(nameof(UserDocumentFileModel.UserDocumentId))]
        public IEnumerable<UserDocumentFileModel>? Images { get; set; }

        public ImageSettings ImagesSettings => SharedImageSettings(TypeClvId);

        public static ImageSettings SharedImageSettings(Guid? typeClvId) => new(new string[] { "jpg", "jpeg", "png" }
            , 3
            , requiredFileCount: UserDocumentModel.GetFileCountByTypeClvId(typeClvId));
    }

    public class UserDocumentModelProcedure : AutoProcedure<UserDocumentModel>
    {
        public override void Update(UserDocumentModel model)
        {
            var additionalClvModels = model.AdditionalTypeClvIdEditGrid != null
            ? model.AdditionalTypeClvIdEditGrid.Select(ff => new UserDocumentClvModel() { UserDocumentId = model.Id!.Value, AdditionalTypeClvId = ff }).ToArray()
            : Enumerable.Empty<UserDocumentClvModel>().ToArray();

            var where = new SqlWhereCondition(nameof(UserDocumentClvModel.UserDocumentId), model.Id!.Value, SqlWherePart.CommonPatterns.GuidEqual);

            AutoProcedure.Of<UserDocumentClvModel>().Delete(where);


            if (model.Images is not null)
            {
                model.Images.ForEach(f => { if (f.IsDeleted == false) f.IsNew = true; });
                AutoProcedure.Of<UserDocumentFileModel>().Delete(where);
            }

            AutoProcedure.Of<UserDocumentClvModel>().Insert(additionalClvModels);

            var dbModel = AutoProcedure.Of<UserDocumentModel>().Get(model.Id!.Value);
            if (dbModel == null) return;
            if (model.StatusClvId != dbModel.StatusClvId)
                model.StatusDate = DateTimeOffset.Now.ToDateOnly();

            base.Update(model);
        }
    }
}

