using eFlex.Index.Base.ActionResults;
using frlnet.Jobs;
using frlnet.Models.frl;

namespace frlnet.Controllers.frl
{
    public class ClientBankInfoController : hAreaIndexController<ClientBankInfoModel>
    {
        public ClientBankInfoController()
        {
            Instructions.Index.Grid.Buttons.Delete.Visible = false;
            Instructions.Create.AfterSave = CreateBuilder.eAfterSave.ReturnToPrevius;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var parentId = IndexNavigation.Current.GetQueryGuid<ClientController>()!;
            where.Parts.Add(new(nameof(ClientBankInfoModel.ParentId), parentId));
            return where;
        }

        public override IndexResult CreateConfirm(ClientBankInfoModel model)
        {
            var res = base.CreateConfirm(model);
            if (!res.HasErrors)
            {
                var clientModel = AutoProcedure.Of<ClientModel>().Get(model.ParentId);
                var apiCom = ApiSync.GetApiCom(clientModel!.OrganizationId);
                if (apiCom != null)
                {
                    apiCom.InsertSingle<ClientContactModel>(model, clientModel);
                    Procedure.Connectivity.Instances.Commit();
                    ApiSync.Update([clientModel], null, apiCom.SyncModel);
                }
            }
            return res;
        }

        public override IndexResult EditConfirm(ClientBankInfoModel model)
        {
            var res = base.EditConfirm(model);
            if (!res.HasErrors)
            {
                var apiCom = ApiSync.GetApiCom(model.OrganizationId);
                if (apiCom != null)
                {
                    var clientModel = AutoProcedure.Of<ClientModel>().Get(model.ParentId);
                    apiCom?.UpdateSingle<ClientBankInfoModel>(model, clientModel);
                    Procedure.Connectivity.Instances.Commit();
                    ApiSync.SyncLinked([model], clientModel);
                }
            }
            return res;
        }
    }
}
