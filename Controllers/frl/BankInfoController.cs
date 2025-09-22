using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Builders;
using eFlex.Index.Base.Models.Admin;
using frlnet.Models.frl;


namespace frlnet.Controllers.frl
{
    [NoClaim]
    public class BankInfoController : hAreaIndexController<BankInfoModel>
    {
        public BankInfoController()
        {
            Instructions.Index.Filter.ShowConfigurator = false;
            Instructions.Create.AfterSave = CreateBuilder.eAfterSave.ReturnToPrevius;
            Instructions.Create.Allow = IndexNavigation.Current.Routes.Any(f => f.ControllerType == typeof(Admin.ProfileController) || f.ControllerType == typeof(Admin.ProfileRootController));
            Instructions.Delete.Allow = IndexNavigation.Current.Routes.Any(f => f.ControllerType == typeof(Admin.ProfileController) || f.ControllerType == typeof(Admin.ProfileRootController));
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var userId = IndexNavigation.Current.GetQueryGuid<Admin.UserRootController>();
            userId ??= IndexNavigation.Current.GetQueryGuid<Public.UserRootController>();
            userId ??= UserModel.CurrentId!.Value;
            where.Parts.Add(new(nameof(BankInfoModel.UserId), userId));
            return where;
        }

        public override IndexResult CreateConfirm(BankInfoModel model)
        {
            model.UserId = UserModel.CurrentId!.Value;
            model.IBAN = model.IBAN!.Replace(" ", "");
            if (!IsIbanChecksumValid(model.IBAN))
                return new(Messages.BankInfoIbanChecksumFailed.ToString());
            return base.CreateConfirm(model);
        }


        static bool IsIbanChecksumValid(string iban)
        {
            if (iban.Length < 4 || iban[0] == ' ' || iban[1] == ' ' || iban[2] == ' ' || iban[3] == ' ') return false;

            var checksum = 0;
            var ibanLength = iban.Length;
            for (int charIndex = 0; charIndex < ibanLength; charIndex++)
            {
                var c = iban[(charIndex + 4) % ibanLength];
                if (c == ' ') continue;

                int value;
                if (c >= '0' && c <= '9')
                {
                    value = c - '0';
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    value = c - 'A';
                    checksum = (checksum * 10 + value / 10 + 1) % 97;
                    value %= 10;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    value = c - 'a';
                    checksum = (checksum * 10 + value / 10 + 1) % 97;
                    value %= 10;
                }
                else return false;

                checksum = (checksum * 10 + value) % 97;
            }
            return checksum == 1;
        }

    }
}