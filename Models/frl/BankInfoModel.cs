using eFlex.Common.Extensions;
using eFlex.Index.Base.Controls.Settings;
using System.Diagnostics.CodeAnalysis;

namespace frlnet.Models.frl
{
    [MapSource("frl", "BankInfo")]
    public class BankInfoModel : ModelBase
    {
        [Map] public Guid UserId { get; set; }

        [ShowMark(In.Grid | In.Edit | In.Create)]

        [Show]
        [Map] public string? BankName { get; set; }

        [TextBoxSettings(Regex = Constants.RegExtIban)]
        [Show]
        [Map] public string IBAN { get; set; }

        [TextBoxSettings(Regex = Constants.RegExtSwift)]
        [Show]
        [Map] public string? SWIFT { get; set; }

        [Show]
        [Map] public string? Props { get; set; }

        [SwitchSettings]
        [Show]
        [Map] public bool Default { get; set; }
    }

    public class BankInfoPrrocedure : AutoProcedure<BankInfoModel>
    {

        public override void Insert(IEnumerable<BankInfoModel> models)
        {
            base.Insert(models);
            models.ForEach(ApplyDefaultUpdate);
        }

        public override void Update(BankInfoModel model)
        {
            base.Update(model);
            ApplyDefaultUpdate(model);
        }

        public override void Delete([NotNull] SqlWhereCondition where, bool deleteSubModels = true)
        {
            var models = GetRange(where);
            foreach (var model in models)
            {
                ApplyDefaultDelete(model);
            }
            base.Delete(where, deleteSubModels);
        }

        private void ApplyDefaultUpdate(BankInfoModel model)
        {
            if (model.Default)
            {
                var allModels = GetRange(nameof(BankInfoModel.UserId), model.UserId);
                allModels = allModels.Where(f => f.Id != model.Id).ToArray();
                allModels.ForEach(f => f.Default = false);
                allModels.ForEach(Update);
            }
        }

        private void ApplyDefaultDelete(BankInfoModel model)
        {
            var allModels = GetRange(nameof(BankInfoModel.UserId), model.UserId);
            allModels = allModels.Where(f => f.Id != model.Id).ToArray();
            var defaultModels = allModels.Where(f => f.Default);
            if (defaultModels.Count() > 1)
            {
                foreach (var f in defaultModels)
                {
                    f.Default = false;
                    Update(f);
                }
            }

            if (defaultModels.Count() == 0 && allModels.Any())
            {
                allModels.First().Default = true;
                Update(allModels.First());
            }
        }


    }
}
