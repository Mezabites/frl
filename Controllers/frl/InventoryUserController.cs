using eFlex.Index.Base.Models.Admin;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
    public class InventoryUserController : InventoryController
    {

        public InventoryUserController()
        {
            Instructions.Create.Allow = false;
            Instructions.Delete.Allow = false;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var ids = default(Guid[]?);
            {
                var where = new SqlWhereCondition();
                var userId = IndexNavigation.Current.GetQueryGuid<Admin.UserRootController>();
                userId ??= IndexNavigation.Current.GetQueryGuid<Public.UserRootController>();
                userId ??= UserModel.CurrentId!.Value;
                where.Parts.Add(new(nameof(InventoryRequestModel.UserId), userId));
                where.Parts.Add(new(nameof(InventoryRequestModel.Active), true));
                var requests = AutoProcedure.Of<InventoryRequestModel>().GetRange(where);
                ids = requests.Select(f => f.InventoryId).Distinct().ToArray();
            }

            {
                var where = base.TranslateFilter(indexModel);
                where.Parts.Add(new(nameof(InventoryModel.Id), ids));
                return where;
            }
        }
    }
}