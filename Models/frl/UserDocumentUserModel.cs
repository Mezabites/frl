using eFlex.Common.Extensions;
using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.Controls.Settings.Interfaces;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Models.App;
using eFlex.Index.Base.Types;

namespace frlnet.Models.frl
{
    [MapSource("frl", "UserDocument", false)]
    public class UserDocumentUserModel : ModelBase
    {
        [Map] public Guid UserId { get; set; }
        [Map] public Guid? ApprovedByUserId { get; set; }

        [Show(In.Grid | In.Create | In.Edit)]
        [TextBoxSettings(true)]
        [Map] public string Name { get; set; }

        [Show(In.Grid | In.Edit | In.Create, visibleIf: nameof(IsComplexForm))]
        [Map] public string? Nr { get; set; }

        [Show(In.Grid | In.Edit | In.Create, visibleIf: nameof(IsComplexForm))]
        [Map] public DateOnly? IssueDate { get; set; }

        [Show(In.Edit | In.Create, visibleIf: nameof(IsComplexForm))]
        [Map] public string? Issuer { get; set; }

        [Show(In.Grid | In.Edit | In.Create, In.Create)]
        [Map] public Guid? TypeClvId { get; set; }

        public DropListSettings TypeClvIdSettings => new(TypeClvId.HasValue ? nameof(TypeClvIdAllowedSource) : nameof(TypeClvIdAllSource), true) { FilterType = Kendo.Mvc.UI.FilterType.Contains };

        public static SelectList<Guid?> TypeClvIdAllowedSource => GetTypes(false);
        public static SelectList<Guid?> TypeClvIdAllSource => GetTypes(true);

        private static SelectList<Guid?> GetTypes(bool filter)
        {
            var classifiers = Classifiers._UserDocumentType.Model.Values?.ToArray();
            var currentUser = UserModel.Current;

            if (classifiers is null || classifiers.Length != 0 == false || currentUser is null || currentUser.Id.HasValue == false)
                return Array.Empty<ClassifierValueModel>().ToSelectList(f => f.Name!, f => f.Id);

            if (filter)
            {
                var where = new SqlWhereCondition(nameof(UserDocumentUserModel.UserId), currentUser.Id, SqlWherePart.CommonPatterns.GuidEqual);
                var existedDocuments = AutoProcedure.Of<UserDocumentUserModel>().GetRangeSpecific<Guid>(nameof(UserDocumentUserModel.TypeClvId), where).Where(f => f != Classifiers._UserDocumentType._Other.Id).ToHashSet();
                classifiers = classifiers.Where(f => existedDocuments.Contains(f.Id!.Value) == false).ToArray();
            }

            return classifiers.ToNullableSelectList(f => f.Name!, f => f.Id);
        }

        [Show(In.Edit | In.Grid, In.None, visibleIf: nameof(IsComplexForm))]
        [MultiDropListSettings(nameof(AdditionalTypeClvIdEditGridSource))]
        [MapQuery("SELECT string_agg(\"AdditionalTypeClvId\"::text, '|') FROM frl.\"UserDocumentClv\" WHERE \"UserDocumentId\" = J0.\"Id\"")]
        public StringList<Guid>? AdditionalTypeClvIdEditGrid { get; set; }

        public static SelectList<Guid> AdditionalTypeClvIdEditGridSource => UserDocumentModel.AdditionalTypeClvIdFullSource;

        [DropListSettings(SourceClassifierCode = Classifiers._UserDocumentStatus.Code)]
        [Show(In.Filter | In.Grid | In.Edit, In.Filter)][Map] public Guid StatusClvId { get; set; }

        [Map] public DateOnly? StatusDate { get; set; }
        [Map] public DateOnly? ApproveDate { get; set; }

        [Show(In.Create, In.Create, visibleIf: nameof(IsComplexForm))]
        [ListBoxSettings(nameof(AdditionalTypeClvIdCreateSource), Required = IRequiredSettings.Require.True)]
        public StringList<Guid>? AdditionalTypeClvIdCreate { get; set; }

        public SelectList<Guid> AdditionalTypeClvIdCreateSource => UserDocumentModel.GetAdditionalClassifierValues(TypeClvId).ToSelectList(f => f.Name!, f => f.Id!.Value);

        [Show(visibleIf: nameof(IsComplexForm))]
        [Map] public DateOnly? ExpirationDate { get; set; }

        [TextBoxSettings(ControlSize = 10)]
        [Show(In.Edit | In.Create)][Map] public string? Description { get; set; }
        [Show(In.Edit | In.Grid, In.None)][Map] public string? DeclinedReason { get; set; }

        public bool IsComplexForm => TypeClvId.HasValue && ComplexClassifiersDic.ContainsKey(TypeClvId.Value);

        [LabelGroupMark]
        [Show(In.Edit | In.Create, In.Create)]
        [MapModel(nameof(UserDocumentFileModel.UserDocumentId))]
        public IEnumerable<UserDocumentFileModel>? Images { get; set; }

        public ImageSettings ImagesSettings => new(new string[] { "jpg", "jpeg", "png" }
            , uint.Parse(Parameters._UserDocument._UserDocumentMaxFileLengthInMB.Value)
            , requiredFileCount: GetFileCountByTypeClvId);

        //value is required count of files
        private static readonly Dictionary<Guid, int> ComplexClassifiersDic = new()
        {
            { Classifiers._UserDocumentType._DriversLicense.Id, 2},
            { Classifiers._UserDocumentType._PersonDocument.Id, 2},
            { Classifiers._UserDocumentType._EducationDocument.Id, 2},
            { Classifiers._UserDocumentType._Qualification.Id, 2},
        };

        public int GetFileCountByTypeClvId => TypeClvId.HasValue && ComplexClassifiersDic.TryGetValue(TypeClvId.Value, out int value) ? value : 1;
    }

    public class UserModelUserDocumentProcedure : AutoProcedure<UserDocumentUserModel>
    {
        public override void Insert(IEnumerable<UserDocumentUserModel> models)
        {
            models.ForEach(f => f.StatusDate = DateTimeOffset.Now.ToDateOnly());
            base.Insert(models);

            var additionalClvModels = models.SelectMany(f => f.AdditionalTypeClvIdCreate != null
            ? f.AdditionalTypeClvIdCreate.Select(ff => new UserDocumentClvModel() { UserDocumentId = f.Id!.Value, AdditionalTypeClvId = ff })
            : Enumerable.Empty<UserDocumentClvModel>()).ToArray();
            AutoProcedure.Of<UserDocumentClvModel>().Insert(additionalClvModels);
        }

        public override void Update(UserDocumentUserModel model)
        {
            model.StatusDate = DateTimeOffset.Now.ToDateOnly();
            base.Update(model);
        }
    }
}
