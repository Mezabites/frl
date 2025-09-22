using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models.Admin;
using frlnet.Models.frl;


namespace frlnet.Controllers.frl
{
    [NoClaim]
    public class LanguageSkillsController : hAreaIndexController<LanguageSkillsModel>
    {
        public LanguageSkillsController()
        {
            Instructions.Index.Filter.ShowConfigurator = false;
            Instructions.Create.AfterSave = CreateBuilder.eAfterSave.ReturnToPrevius;
            Instructions.Edit.Editable = false;
            Instructions.Edit.Allow = false;
            Instructions.Create.Allow = IndexNavigation.Current.Routes.Any(f => f.ControllerType == typeof(Admin.ProfileController) || f.ControllerType == typeof(Admin.ProfileRootController));
            Instructions.Delete.Allow = IndexNavigation.Current.Routes.Any(f => f.ControllerType == typeof(Admin.ProfileController) || f.ControllerType == typeof(Admin.ProfileRootController));
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var userId = IndexNavigation.Current.GetQueryGuid<Admin.UserRootController>();
            userId ??= IndexNavigation.Current.GetQueryGuid<Public.UserRootController>();
            userId ??= UserModel.CurrentId!.Value;
            where.Parts.Add(new(nameof(LanguageSkillsModel.UserId), userId));
            return where;
        }

        public override IndexResult CreateConfirm(LanguageSkillsModel model)
        {
            model.UserId = UserModel.CurrentId!.Value;
            return base.CreateConfirm(model);
        }

    }
}