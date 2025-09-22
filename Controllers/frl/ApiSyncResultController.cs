using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Controllers;
using eFlex.Index.Base.Extensions;
using eFlex.Index.Base.Models.Admin;
using frlnet.Jobs;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.frl
{
    [Claim(eClaimName.Read)]
    public class ApiSyncResultController : hAreaIndexController<ApiSyncResultModel>
    {
        public ApiSyncResultController()
        {
            Instructions.Edit.Allow = false;
            Instructions.Create.Allow = false;
            Instructions.Delete.Allow = false;
        }

        public override IndexResult IndexAllow([FromQuery] bool showLayout = true)
        {
            var res = base.IndexAllow(showLayout);
            if (!res.HasErrors)
                return new(new Link(nameof(IndexPartial), this.GetType(), HttpContext.Request.Query.ToDynamic()), false);
            return res;
        }

        public PartialViewResult IndexPartial()
        {
            var result = Index(false);
            var indexModel = extractIndexModel(result);
            return PartialBuilder(Instructions.Index, indexModel);
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);

            var apiCom = ApiSync.GetApiCom(OrganizationModel.CurrentId!.Value);
            if (apiCom is not null && apiCom.SyncModel.LastSyncId.HasValue)
            {
                var controllerType = IndexNavigation.Current.Routes.Last().ControllerType;
                var modelType = hIndexController.GetGenericType(controllerType);

                where.Parts.Add(new(nameof(ApiSyncResultModel.ObjectType), modelType.FullName!));
                where.Parts.Add(new(nameof(ApiSyncResultModel.LastSyncId), apiCom.SyncModel.LastSyncId.Value));
            }
            else
            {
                where.Parts.Add(new(nameof(ApiSyncResultModel.Id), Guid.Empty));
            }

            return where;
        }
    }
}
