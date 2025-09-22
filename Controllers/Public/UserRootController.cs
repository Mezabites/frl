using eFlex.Index.Base.Controllers;
using frlnet.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.Public
{
    [ClaimFrom(typeof(UserController))]
    public class UserRootController : hAreaIndexController<UserRootModel>
    {
        public UserRootController()
        {
            Instructions.Edit.Editable = false;
        }

        public override ViewResult Edit(Guid id)
        {
            var res = base.Edit(id);
            var model = extractViewModel(res);
            if (model is not null)
                model.BaseData = new Link(nameof(UserController.Edit), typeof(UserController), new { id });
            return res;
        }
    }
}
