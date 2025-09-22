using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Models.Admin;

namespace eFlex.Index.Base.Controllers.Admin
{
    [Claim(eClaimName.Read)]
    [Claim(eClaimName.Update)]
    public abstract class SettingController<TModel> : hAreaIndexController<TModel> where TModel : SettingValueModel
    {
        public SettingController()
        {
            Instructions.Create.Allow = false;
            Instructions.Delete.Allow = false;
        }

        public override IndexResult EditConfirm(TModel model)
        {

            if (!string.IsNullOrEmpty(model.Type))
            {
                var type = Type.GetType(model.Type);
                try
                {
                    Convert.ChangeType(model.Value, type!);
                }
                catch
                {
                    return new IndexResult(Messages.ParameterValueBadType);
                }
            }
            return base.EditConfirm(model);
        }
    }

    public class SettingController : SettingController<SettingValueModel> { }

}
