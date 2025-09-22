using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.FilterConditions;
using frlnet.Controllers.frl;

namespace frlnet.Models.frl
{
    [MapSource("frl", "InventoryRequest")]
    [MapJoin("frl", "Inventory")]
    [MapJoin("frl", "InventoryStatus")]
    [MapJoin("adm", "User")]
    public class InventoryRequestModel : ModelBase, IOrganizationModel
    {
        [Map] public Guid InventoryId { get; set; }

        [Show(editableIn: In.Filter, visibleIn: In.Filter | In.Grid)]
        [Map] public string InventoryName { get; set; }

        [LabelGroupMark()]
        [Show(In.Edit | In.Create)] public object? Request { get; }

        [Show(editableIn: In.Filter | In.Create | In.Edit)]
        [Map] public uint Count { get; set; } = 1;
        public NumericSettings CountSettings => new() { MinValue = 1, MaxValue = InventoryCountLeft };

        [TextBoxSettings(Multiline = true)]
        [Show(In.Create)] public string Notes { get; set; }

        [ShowMark(visibleIn: In.Filter | In.Grid | In.Edit, editableIn: In.Filter)]

        [Label("Requested at")]
        [Map][Show] public override DateTimeOffset? RowCreated { get; set; }

        [Map] public Guid UserId { get; set; }
        [Label("Requested by")]
        [Map()][Show] public string UserUserName { get; set; }

        [Show(editableIn: In.Filter, visibleIn: In.Filter | In.Grid | In.Edit)]
        [Map] public DateOnly? ReceiveDate { get; set; }

        [MapQuery("CASE WHEN J3.\"ReceiveDate\" IS NULL then NULL ELSE (J3.\"ReceiveDate\" - CURRENT_DATE) + J3.\"InventoryExpireDays\" END")]
        [Show(In.Filter | In.Grid | In.Edit)]
        public int? DaysExpire { get; set; }

        [MapQuery("\"Count\" - COALESCE((SELECT SUM(\"Count\") FROM frl.\"InventoryRequest\" WHERE \"InventoryId\" = J3.\"Id\" AND \"Active\" = TRUE), 0)")]
        public uint InventoryCountLeft { get; set; }
        [Map] public DateTime InventoryAvailableFrom { get; set; }
        [Map] public int? InventoryExpireDays { get; set; }

        [Map("InventoryOrganizationId")] public Guid? OrganizationId { get; set; }


        [ShowMark(editableIn: In.Filter, visibleIn: In.Filter | In.Grid | In.Edit)]


        [Map] public Guid? InventoryStatusId { get; set; }

        [DropListSettings(SourceClassifierCode = Classifiers._InventoryStatusAction.Code)]
        [Map][Show] public Guid InventoryStatusActionClvId { get; set; }

        [DropListSettings]
        [Map][Show] public bool Active { get; set; } = true;



        [Map("InventoryStatusNotes")][Show] public string StatusNotes { get; set; }

        [LabelGroupMark]
        [Show(visibleIn: In.Edit | In.Create, editableIn: In.None)]
        public Link Inventory => new Link(nameof(InventoryController.EditReadOnly), typeof(InventoryController), new { Id = InventoryId });

    }

}
