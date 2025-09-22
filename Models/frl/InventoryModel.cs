using eFlex.Common.Extensions;
using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.FilterConditions;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Types;
using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
    [MapSource("frl", "Inventory")]
    [MapJoin("adm", "Organization")]
    public class InventoryModel : ModelBase, IOrganizationModel
    {
        [LabelTabMark]
        [Show(In.Edit)]
        public object? BaseData => null;

        [Show][Map] public string Name { get; set; }

        [DropListSettings(SourceClassifierCode = Classifiers._InventoryType.Code)]
        [Show][Map] public Guid TypeClvId { get; set; }

        [TextBoxSettings(ControlSize = 10)]
        [Show][Map] public string? Description { get; set; }

        [NumericSettings(Decimals = 0)]
        [Show][Map] public uint Count { get; set; }

        [NumericSettings(Decimals = 0)]
        [MapQuery("\"Count\" - COALESCE((SELECT SUM(\"Count\") FROM frl.\"InventoryRequest\" WHERE \"InventoryId\" = J1.\"Id\" AND \"Active\" = TRUE), 0)")]
        [Show(visibleIn: In.Filter|In.Grid|In.Edit, editableIn: In.Filter)]
        public uint CountLeft { get; set; }

        [Show][Map] public DateOnly AvailableFrom { get; set; } = DateTime.Now.Date.ToDateOnly();

        [Show][Map] public int ExpireDays { get; set; } = 90;

        [Show][Map] public Guid? OrganizationId { get; set; } = OrganizationModel.CurrentId;
        public static SelectList<Guid> OrganizationIdSource => OrganizationModel.ParentOrganizationIdSource;

        [LabelGroupMark]
        [Show(In.Edit | In.Create)]
        [ImageSettings]
        [MapModel(nameof(InventoryImageModel.InventoryId))]
        public IEnumerable<InventoryImageModel>? Images { get; set; }

        [DynamicUpdateIgnore]
        [LabelGroupMark]
        [Show(In.Edit | In.Create)]
        [MapModel(nameof(InventoryDetailsModel.InventoryId))]
        [GridSettings(typeof(InventoryDetailsController))]
        public IEnumerable<InventoryDetailsModel>? Details { get; set; }

        [LabelTabMark]
        [Show(In.Edit)]
        public Link History => Link.ToIndex<InventoryStatusController>();

    }
}
