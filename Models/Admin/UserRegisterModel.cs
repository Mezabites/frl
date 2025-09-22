using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.Controls.Settings.Interfaces;

namespace frlnet.Models.Admin
{
    [MapSource("adm", "User")]
    public class UserRegisterModel : eFlex.Index.Base.Models.Admin.UserRegisterModel
    {
        [IndexOffset(-3)]
        [Show(In.Create, In.Create)][Map] public DateOnly DateOfBirth { get; set; } = new DateOnly(2000, 1, 1);
        public static DatePickerSettings DateOfBirthSettings => new() { MaxDate = DateTime.Now.AddYears(-18), Required= IRequiredSettings.Require.True };
    }

}
