using eFlex.Index.Base.Controllers.Admin;
using eFlex.Index.Base.Models.Admin;
using frlnet.Controllers.Admin;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
    [ClaimFromAttribute<Admin.ProfileController>]
    public class InventoryRequestUserController : hAreaIndexController<InventoryRequestModel>
    {

        public InventoryRequestUserController()
        {
            Instructions.Edit.Allow= false;
            Instructions.Create.Allow = false;
            Instructions.Delete.Allow = false;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var userId = IndexNavigation.Current.GetQueryGuid<Admin.UserController>();
            userId ??= UserModel.CurrentId!.Value;

            where.Parts.Add(new(nameof(InventoryRequestModel.UserId), userId));
            where.Parts.Add(new(nameof(InventoryRequestModel.Active), true));

            return where;
        }


    }
}