using eFlex.Common.Extensions;
using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Extensions;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Models.App;
using frlnet.Integration.API;
using frlnet.Jobs;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace frlnet.Controllers.frl
{
    public class ClientController : hAreaIndexController<ClientModel>
    {
        public ClientController()
        {
            Instructions.Index.Filter.Expanded = true;
            Instructions.Index.Grid.Buttons.Delete.Visible = false;

            var btn = ButtonBuilder.DefaultButtons.DefaultHref(
                    Labels.ClientShowSyncLog, new Link(
                        nameof(ApiSyncResultController.IndexAllow), typeof(ApiSyncResultController)));
            btn.Script = "indexDefaultGet";
            Instructions.Index.Grid.Buttons.List.Add(btn);
        }

        public override ViewResult Index([FromQuery] bool showLayout = true)
        {
            var res = base.Index(showLayout);
            var indexModel = extractIndexModel(res);
            if (indexModel is not null)
            {
                var apiSyncModel = AutoProcedure.Of<ApiSyncModel>().GetBy(f => f.OrganizationId, OrganizationModel.CurrentId!.Value);
                if (apiSyncModel is not null)
                {
                    // var apiSyncResultModels
                }
            }
            return res;
        }

        public override IndexResult CreateConfirm(ClientModel model)
        {
            var res = base.CreateConfirm(model);
            if (!res.HasErrors)
            {
                var apiCom = ApiSync.GetApiCom(model.OrganizationId);
                apiCom?.InsertSingle<ClientModel>(model, null);
            }
            return res;
        }

        public override ViewResult Edit(Guid id)
        {

            var res = base.Edit(id);
            var model = extractViewModel(res);

            //Add link button.
            if (model is not null && !model.LinkId.HasValue)
            {
                var title = LabelFieldModel.GetValue(this.GetType(), nameof(Link)).Text;
                var link = new Link(nameof(Link), this.GetType());
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(title, link);
                Instructions.Edit.Buttons.List.Add(btn);
            }
            else
            {
                var title = LabelFieldModel.GetValue(this.GetType(), nameof(Unlink)).Text;
                var link = new Link(nameof(Unlink), this.GetType());
                var btn = ButtonBuilder.DefaultButtons.EditGeneric(title, link);
                Instructions.Edit.Buttons.List.Add(btn);
            }

            return res;
        }

        public override IndexResult EditConfirm(ClientModel model)
        {
            var res = base.EditConfirm(model);
            if (!res.HasErrors)
            {
                var apiCom = ApiSync.GetApiCom(model.OrganizationId);
                if (apiCom != null)
                {
                    apiCom.UpdateSingle<ClientModel>(model, null);
                    Procedure.Connectivity.Instances.Commit();
                    ApiSync.SyncLinked([model], null);
                }
            }
            return res;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            where.Parts.Add(new(nameof(ClientModel.OrganizationId), OrganizationModel.CurrentId!));
            return where;
        }

        [HttpPost]
        [Claim(eClaimName.Create)]
        public IndexResult Search(ClientModel model)
        {
            var regNr = model.RegNumber;
            if (string.IsNullOrEmpty(regNr))
                return new(Messages.ClientSearchRegEmpty.ToString());

            var refModel = GetLinkableModel(regNr);

            if (refModel is null)
                return new(Messages.ClientSearchNotFound.ToString(), IndexResult.Message.eLevel.Warning);

            if (refModel?.OrganizationId == OrganizationModel.CurrentId)
                return new(Messages.ClientSearchRegExists.ToString(), IndexResult.Message.eLevel.Warning);

            var refId = refModel.Id;
            refModel.Id = null;
            refModel.ExternalId = null;
            refModel.OrganizationId = OrganizationModel.CurrentId!.Value;

            ModelState.Clear();
            var res = CreateConfirm(refModel);

            if (!res.HasErrors)
                SyncChilds(refModel);

            return res;
        }

        [HttpPost]
        [Claim(eClaimName.Update)]
        public IndexResult Link(ClientModel model)
        {
            var regNr = model.RegNumber;
            if (string.IsNullOrEmpty(regNr))
                return new(Messages.ClientSearchRegEmpty.ToString());

            if (model.IsLinked)
                return new(Messages.ClientLinkActive.ToString(), IndexResult.Message.eLevel.Warning);

            var refModel = GetLinkableModel(regNr);

            if (refModel is null)
                return new(Messages.ClientSearchNotFound.ToString(), IndexResult.Message.eLevel.Warning);

            if (refModel?.OrganizationId == OrganizationModel.CurrentId)
                return new(Messages.ClientSearchRegExists.ToString(), IndexResult.Message.eLevel.Warning);

            var sourceId = model.Id;
            var sourceOrg = model.OrganizationId;
            var extId = model.ExternalId;
            Cast.CopyValues(refModel, model);
            model.Id = sourceId;
            model.OrganizationId = sourceOrg;
            model.ExternalId = extId;
            Procedure.Update(model);

            SyncChilds(refModel);

            return new IndexResult(IndexNavigation.Current.Routes.First().ToLink(), false, new IndexResult.Message(Messages.ClientLinkCreateSuccess, IndexResult.Message.eLevel.Success));
        }

        private void SyncChilds(ClientModel model)
        {
            Procedure.Connectivity.Instances.Commit();

            foreach (var fChildType in model.ChildModels)
            {
                var method = typeof(ApiSync).GetMethod(nameof(ApiSync.CreateChildLinks), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                method = method!.MakeGenericMethod(typeof(ClientModel), fChildType.Type);
                method.Invoke(null, [model.LinkId!.Value, fChildType.Key]);
            }
        }

        public IndexResult Unlink(ClientModel model)
        {
            model.LinkId = null;
            Procedure.Update(model);

            {
                var subProcedure = AutoProcedure.Of<ClientContactModel>();
                var subModels = subProcedure.GetRange(new SqlWhereCondition(nameof(ApiModelBase.ParentId), model.Id!.Value));
                subModels.ForEach(f => f.LinkId = null);
                subModels.ForEach(subProcedure.Update);
            }

            {
                var subProcedure = AutoProcedure.Of<ClientContactModel>();
                var subModels = subProcedure.GetRange(new SqlWhereCondition(nameof(ApiModelBase.ParentId), model.Id!.Value));
                subModels.ForEach(f => f.LinkId = null);
                subModels.ForEach(subProcedure.Update);
            }

            return new IndexResult(IndexNavigation.Current.Routes.First().ToLink(), false, new IndexResult.Message(Messages.ClientLinkRemoveSuccess, IndexResult.Message.eLevel.Success));
        }

        private static void EnsureLink<TModel>(TModel model) where TModel : ApiModelBase
        {
            if (!model.LinkId.HasValue)
            {
                model.LinkId = Guid.NewGuid();
                AutoProcedure.Of<TModel>().Update(model);
            }
        }


        private ClientModel? GetLinkableModel(string regNr)
        {
            var relatedOrgs = OrganizationModel.TreeIds;
            var where = new SqlWhereCondition();
            where.Parts.Add(new(nameof(ClientModel.RegNumber), regNr));
            where.Parts.Add(new(nameof(ClientModel.OrganizationId), relatedOrgs!));
            var models = Procedure.GetRange(where);

            var curentOrgModel = models.FirstOrDefault(f => f.OrganizationId == OrganizationModel.CurrentId);

            var refModel = models.FirstOrDefault(f => f.LinkId.HasValue);
            refModel ??= models.FirstOrDefault(f => f != curentOrgModel);

            if (refModel is not null)
            {
                if (!refModel.LinkId.HasValue)
                {
                    refModel.LinkId = Guid.NewGuid();
                    Procedure.Update(refModel);
                }
            }

            return refModel;
        }

        //public override IndexResult EditConfirm(ClientModel model)
        //{
        //    var result = base.EditConfirm(model);
        //    if (!result.Messages.Any(f => f.Level == IndexResult.Message.eLevel.Error))
        //        UpdateLinkedClients(model);
        //    return result;
        //}

        //private static void UpdateLinkedClients(ClientModel model)
        //{
        //    if (model.IsLinked)
        //    {
        //        var linkedModels = Procedure.GetRange(new SqlWhereCondition(nameof(ClientModel.LinkId), model.LinkId!.Value));
        //        linkedModels = linkedModels.Where(f => f.Id != model.Id).ToArray();

        //        foreach (var f in linkedModels)
        //        {
        //            var apicom = GetApiCom(f.OrganizationId!.Value);
        //            if (apicom != null)
        //            {
        //                var sourceId = f.Id;
        //                var orgId = f.OrganizationId;
        //                var extId = f.ExternalId;
        //                Cast.CopyValues(model, f);
        //                f.Id = sourceId;
        //                f.OrganizationId = orgId;
        //                f.ExternalId = extId;
        //                apicom.UpdateSingle<ClientModel>(f, null);
        //            }
        //            AutoProcedure.Of<ClientModel>().Update(f);
        //        }
        //    }
        //}
    }
}
