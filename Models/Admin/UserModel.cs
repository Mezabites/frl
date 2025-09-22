using DocumentFormat.OpenXml.Office2010.Excel;
using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.Controls.Settings.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace frlnet.Models.Admin
{

    [CacheByModelRegister]
    [MapSource("adm", "User")]
    public class UserModel : eFlex.Index.Base.Models.Admin.UserModel
    {

        [IndexOffset(nameof(UserModel.LastName))]
        [Show][Map] public DateOnly DateOfBirth { get; set; } = new DateOnly(2000, 1, 1);
        public static DatePickerSettings DateOfBirthSettings => new() { MaxDate = DateTime.Now.AddYears(-18), Required = IRequiredSettings.Require.True };

        [MapQuery("SELECT COUNT(*) FROM (SELECT DISTINCT \"OrganizationId\" FROM adm.\"Role\" r INNER JOIN adm.\"UserRole\" ur ON ur.\"RoleId\" = r.\"Id\" AND ur.\"UserId\" = J0.\"Id\" ) c")]
        [Show(In.Grid | In.Filter)]
        public int OrganizationCount { get; set; }

    }

    public class UserProcedure : AutoProcedure<UserModel>
    {
        readonly eFlex.Index.Base.Models.Admin.UserProcedure baseProcedure = (eFlex.Index.Base.Models.Admin.UserProcedure)Of<eFlex.Index.Base.Models.Admin.UserModel>();

        public override void Insert(IEnumerable<UserModel> models)
        {
            baseProcedure.Insert(models);
            foreach (var f in models)
                base.Update(f);
        }

        public override void Update(UserModel model)
        {
            base.Update(model);
            baseProcedure.Update(model);
        }
        public override void Delete([NotNull] SqlWhereCondition where, bool deleteSubModels = true)
        {
            baseProcedure.Delete(where, deleteSubModels);
        }

    }
}
