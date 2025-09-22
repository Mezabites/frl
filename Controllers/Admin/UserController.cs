using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Extensions;
using frlnet.Models.Admin;

namespace frlnet.Controllers.Admin
{
    public class UserController : eFlex.Index.Base.Controllers.Admin.hUserController<UserModel>
    {
        public UserController()
        {
            Instructions.Edit.MonitorChanges = false; //Temp solution to form structure.
            Instructions.Edit.Buttons.Return.Visible = false;
        }

        public override IndexResult EditAllow(Guid id)
        {
            var res = base.EditAllow(id);
            if (res.Link is not null)
            {
                var link = new Link(nameof(Edit), typeof(UserRootController), HttpContext.Request.Query.ToDynamic());
                return new IndexResult(link, res.AddToReturn, res.Messages);
            }
            return res;
        }
    }
}
