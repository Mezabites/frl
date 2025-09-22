using eFlex.Index.Base.Types;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;
using static eFlex.Index.Base.Builders.GridBuilder;

namespace frlnet.Controllers.frl
{
    [Area(nameof(frl))]
    [ClaimFrom<InventoryController>]
    public class InventoryStatusController : hAreaIndexController<InventoryStatusModel>
    {
        public InventoryStatusController()
        {
            Instructions.Edit.Allow = false;
            Instructions.Create.Allow = false;
            Instructions.Edit.Allow = false;
            Instructions.Index.Grid.Sorts.Add(nameof(InventoryStatusModel.RowCreated), eDirection.Descending);
        }


        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var requestId = IndexNavigation.Current.GetQueryGuid<InventoryRequestController>();
            var inventoryId = IndexNavigation.Current.GetQueryGuid<InventoryController>();
            if (requestId.HasValue)
                where.Parts.Add(new( nameof(InventoryStatusModel.InventoryRequestId), requestId));
            else if(inventoryId.HasValue)
                where.Parts.Add(new( nameof(InventoryStatusModel.InventoryRequestInventoryId), inventoryId));

            return where;
        }
    }
}