using eFlex.Index.Base.Controllers;
using frlnet.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.Admin
{
    [NoClaim]
    public class ProfileRootController : hIndexController<UserRootModel>
    {
        public ProfileRootController()
        {
            Instructions.Edit.Editable = false;
        }
    }
}
