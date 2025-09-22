using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.FilterConditions;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Resource;
using eFlex.Index.Base.Types;
using static eFlex.Connectivity.Com.SqlJoinCondition;

namespace frlnet.Models.frl
{
    [MapSource("frl", "InventoryStatus")]
    [MapJoin("frl", "InventoryRequest")]
    [MapJoin("frl", "Inventory", joinTarget: "InventoryRequestInventoryId")]
    [MapJoin("adm", "User", joinType: eJoinType.Left , alias:"Status")]
    [MapJoin("adm", "User", joinTarget:nameof(InventoryRequestUserId), joinType: eJoinType.Left , alias:"Request")]
    public class InventoryStatusModel : ModelBase, IOrganizationModel
    {
        [Map] public Guid InventoryRequestId { get; set; }
        [Map] public Guid InventoryRequestInventoryId { get; set; }

        [ShowMark(In.Filter | In.Grid)]

        [DropListSettings(SourceClassifierCode = Classifiers._InventoryStatusAction.Code)]
        [Map][Show] public Guid ActionClvId { get; set; }
        [Map][Show] public override DateTimeOffset? RowCreated { get; set; }

        [Map] public Guid? InventoryRequestUserId { get; set; }
        [Map("RequestUserName")][Show] public string? RequestedBy { get; set; }

        [Map] public Guid? UserId { get; set; }
        [Map("StatusUserName")][Show] public string? CreatedBy { get; set; }

        [Map][Show] public string? Notes { get; set; }

        [Map("InventoryOrganizationId")][Show] public Guid? OrganizationId { get; set; }
        public static SelectList<Guid> OrganizationIdSource => OrganizationModel.ParentOrganizationIdSource;
    }
}
