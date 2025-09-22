using eFlex.Index.Base.Controls.Settings;
using frlnet.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.Admin
{
    [NoClaim]
    public class ProfileController : eFlex.Index.Base.Controllers.Admin.hProfileController<UserModel>
    {
        public ProfileController()
        {
            Instructions.Edit.MonitorChanges = false; //Temp solution to form structure.
            Instructions.Edit.Buttons.Return.Visible = false;
        }

        public override ViewResult Edit(Guid id)
        {
            var res = base.Edit(id);
            var indexModel = extractIndexModel(res);
            if (indexModel != null)
            {

                indexModel.Instructions.Edit.IndexModel = indexModel;
                var structure = indexModel.Instructions.Edit.GetControlStructure();
                structure.RemoveControl(nameof(UserModel.ForcePasswordChange));
                var rolesCtrl = structure.GetAllControls().FirstOrDefault(f => f.Key == nameof(UserModel.Roles));
                if (rolesCtrl?.Settings is ListBoxSettings listBoxSettings)
                {
                    listBoxSettings.Height = 100;
                }
            }
            return res;
        }
    }
}
