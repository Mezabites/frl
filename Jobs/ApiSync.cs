using eFlex.Common.Extensions;
using eFlex.Index.Base.Extensions;
using eFlex.Index.Base.Jobs;
using eFlex.Index.Base.Models.Admin;
using frlnet.Integration.API;
using frlnet.Models.frl;
using System.Reflection;

namespace frlnet.Jobs
{
	public class ApiSync : JobBase
    {
        public override WorkStatusReporter Work()
        {
            var results = SyncAll<ClientModel>();

            var count = (uint)results.Count();
            var errored = (uint)results.Where(f => f.HasError).Count();
            var status = new WorkStatusReporter(count, count - errored, errored);

            return status;
        }

        public IEnumerable<ApiSyncResultModel> SyncAll<TModel>() where TModel : ApiModelBase
        {
            var results = new List<ApiSyncResultModel>();
            var apiSyncModels = AutoProcedure.Of<ApiSyncModel>().GetRange();
            foreach (var fSyncModel in apiSyncModels)
            {
                if (fSyncModel.Name is null)
                    continue;

                var orgModel = AutoProcedure.Of<OrganizationModel>().Get(fSyncModel.OrganizationId!.Value)!;
                var res = Sync<TModel>(fSyncModel, orgModel);

                fSyncModel.LastSyncId = Guid.NewGuid();
                AutoProcedure.Of<ApiSyncModel>().Update(fSyncModel);
                res.ForEach(f =>
                {
                    f.ObjectType = typeof(TModel).FullName!;
                    f.ObjectName = f.Model?.ToString() ?? "All";
                    f.ApiSyncId = fSyncModel.Id!.Value;
                    f.LastSyncId = fSyncModel.LastSyncId!.Value;
                });
                AutoProcedure.Of<ApiSyncResultModel>().Delete(nameof(ApiSyncResultModel.ApiSyncId), fSyncModel.Id!.Value);
                AutoProcedure.Of<ApiSyncResultModel>().Insert(res);
                results.AddRange(res);
            }
            return results;
        }

        public static IEnumerable<ApiSyncResultModel> Sync<TModel>(ApiSyncModel syncModel, ModelBase parentModel) where TModel : ApiModelBase
        {
            var apiCom = GetApiCom(syncModel.OrganizationId!.Value);
            if (apiCom is null)
                return Enumerable.Empty<ApiSyncResultModel>();

            var procedure = new AutoProcedure<TModel>();

            //Find unique maps.
            var uniqueProperties = typeof(TModel).GetProperties().Where(f => f.GetCustomAttribute<Map>()?.Unique == true).ToArray();
            if (uniqueProperties.Length == 0)
                throw new ArgumentException($"Cannot sync data, because model '{typeof(TModel).FullName}' does not contain properties with {nameof(Map)} attribute using {nameof(Map.Unique)} = true.");

            //Get data.
            try
            {
                var apiModels = apiCom.DownloadList<TModel>(parentModel as ApiModelBase);
                var map = Mappings<TModel>.GetMap(nameof(ApiModelBase.ParentId))!.Map;
                var where = new SqlWhereCondition(map.ColumnName, parentModel.Id!.Value);
                var baseModels = procedure.GetRange(where);

                var compare = apiModels.PrepareWith(baseModels,
                    f => string.Join("-", uniqueProperties.Select(ff => ff.GetValue(f)?.GetHashCode().ToString())).GetHashCode(),
                    (@new, old) =>
                    {
                        @new.Id = old.Id;
                        @new.ParentId = old.ParentId;
                        @new.LinkId = old.LinkId;
                        return @new;
                    });

                //Sync data.
                var resultList = new List<ApiSyncResultModel>(apiModels.Count());
                resultList.AddRange(Delete(compare.ToDelete, parentModel, syncModel));
                resultList.AddRange(Insert(compare.ToInsert, parentModel, syncModel));
                resultList.AddRange(Update(compare.ToUpdate, parentModel, syncModel));


                return resultList;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException is not null)
                    message = ex.InnerException.Message;
                var res = new ApiSyncResultModel()
                {
                    Action = ApiSyncResultModel.eAction.Sync,
                    ApiSyncId = apiCom.SyncModel.Id!.Value,
                    Error = message
                };

                return [res];
            }

        }

        public static IEnumerable<ApiSyncResultModel> Update<TModel>(IEnumerable<TModel> models, ModelBase? parentModel, ApiSyncModel syncModel) where TModel : ApiModelBase
        {
            var procedure = new AutoProcedure<TModel>();
            var results = SyncAction(models, parentModel, f =>
            {
                procedure.Update(f);
                SyncChildAction(f, syncModel);
            });
            results.ForEach(f => f.Action = ApiSyncResultModel.eAction.Update);
            return results;
        }
        public static IEnumerable<ApiSyncResultModel> Insert<TModel>(IEnumerable<TModel> models, ModelBase? parentModel, ApiSyncModel syncModel) where TModel : ApiModelBase
        {
            var procedure = new AutoProcedure<TModel>();
            var results = SyncAction(models, parentModel, f =>
            {
                f.Id = null;
                procedure.Insert(f);
                SyncChildAction(f, syncModel);
            });
            results.ForEach(f => f.Action = ApiSyncResultModel.eAction.Insert);
            return results;
        }

        public static IEnumerable<ApiSyncResultModel> Delete<TModel>(IEnumerable<TModel> models, ModelBase? parentModel, ApiSyncModel syncModel) where TModel : ApiModelBase
        {
            var procedure = new AutoProcedure<TModel>();
            var results = SyncAction(models, parentModel, f =>
                procedure.Delete(f.Id!.Value)
            );
            results.ForEach(f => f.Action = ApiSyncResultModel.eAction.Delete);
            return results;
        }


        private static IEnumerable<ApiSyncResultModel> SyncAction<TModel>(IEnumerable<TModel> models, ModelBase? parentModel, Action<TModel> action) where TModel : ApiModelBase
        {
            var procedure = new AutoProcedure<TModel>();
            if (models.Any())
            {
                var resultList = new List<ApiSyncResultModel>(models.Count());
                var errorModels = new HashSet<TModel>();
                foreach (var f in models)
                {
                    var result = new ApiSyncResultModel() { Model = f };
                    try
                    {
                        action.Invoke(f);
                        SyncLinked([(TModel)result.Model], parentModel);
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex.Message;
                        errorModels.Add(f);
                    }
                    finally
                    {
                        resultList.Add(result);
                    }
                }
                if (errorModels.Any())
                {
                    procedure.Connectivity.Instances.Rollback();
                    var validModels = models.Where(f => !errorModels.Contains(f)).ToArray();
                    foreach (var f in validModels)
                    {
                        action.Invoke(f);
                        SyncLinked(resultList.Where(f => !f.HasError).Select(f => (TModel)f.Model), parentModel);
                    }

                }
                procedure.Connectivity.Instances.Commit();
                return resultList;
            }
            return Enumerable.Empty<ApiSyncResultModel>();
        }

        private static void SyncChildAction<TModel>(TModel m, ApiSyncModel syncModel) where TModel : ApiModelBase
        {
            foreach (var fChildType in m.ChildModels)
            {
                //Sync.
                var method = typeof(ApiSync).GetMethod(nameof(Sync), BindingFlags.Public | BindingFlags.Static);
                method = method!.MakeGenericMethod(fChildType.Type);
                var results = (IEnumerable<ApiSyncResultModel>)method.Invoke(null, [syncModel, m])!;

                //Sync linked.
                method = typeof(ApiSync).GetMethod(nameof(SyncLinkedRedirect), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                method = method!.MakeGenericMethod(fChildType.Type);
                method.Invoke(null, [results.Where(f => !f.HasError).Select(f => f.Model), m]);

                //Create linked.
                if (m.LinkId.HasValue)
                {
                    method = typeof(ApiSync).GetMethod(nameof(CreateChildLinks), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    method = method!.MakeGenericMethod(typeof(TModel), fChildType.Type);
                    method.Invoke(null, [m.LinkId!.Value, fChildType.Key]);
                }
            }
        }

        public static void CreateChildLinks<TParent, TModel>(Guid parentLinkId, string keyName)
            where TParent : ApiModelBase
            where TModel : ApiModelBase
        {
            var keyProperty = typeof(TModel).GetProperty(keyName);
            ArgumentNullException.ThrowIfNull(keyProperty, nameof(keyName));
            var getKey = new Func<TModel, object>(f => keyProperty.GetValue(f)!);

            var parentProcedure = new AutoProcedure<TParent>();
            var childProcedure = new AutoProcedure<TModel>();
            var parentModels = parentProcedure.GetRange(nameof(ApiModelBase.LinkId), parentLinkId);

            var childModels = childProcedure.GetRange(nameof(ApiModelBase.ParentId), parentModels.Select(f => f.Id!.Value).ToArray());
            var childGroups = childModels.GroupBy(f => f.ParentId);

            foreach (var fGroup in childGroups)
            {
                var targetChilds = fGroup.ToHashSet();
                var refChilds = childModels.Where(f => !targetChilds.Contains(f)).ToArray();

                var compare = refChilds.PrepareWith(targetChilds, getKey, (n, o) =>
                {
                    return o;
                });

                var parentModel = parentModels.First(f => f.Id == fGroup.Key);
                var apiCom = GetApiCom(parentModel.OrganizationId);

                //Insert childs.
                if (compare.ToInsert.Any())
                {
                    var models = compare.ToInsert.ToArray();
                    foreach (var f in models)
                    {
                        EnsureLink(f);
                        f.Id = null;
                        f.ParentId = parentModel.Id!.Value;
                        f.ExternalId = null;

                        apiCom?.InsertSingle<ClientContactModel>(f, parentModel);
                    }
                    if (apiCom is null)
                        childProcedure.Insert(models);
                }
            }
        }

        public static void EnsureLink<TModel>(TModel model) where TModel : ApiModelBase
        {
            if (!model.LinkId.HasValue)
            {
                model.LinkId = Guid.NewGuid();
                AutoProcedure.Of<TModel>().Update(model);
            }
        }

        private static void SyncLinkedRedirect<TModel>(IEnumerable<ApiModelBase> models, ModelBase? parentModel) where TModel : ApiModelBase
        {
            SyncLinked(models.Select(f => (TModel)f), parentModel);
        }
        public static void SyncLinked<TModel>(IEnumerable<TModel> models, ModelBase? parentModel) where TModel : ApiModelBase
        {
            var linked = models.Where(f => f.LinkId.HasValue).ToHashSet();
            if (linked.Count == 0)
                return;

            var procedure = new AutoProcedure<TModel>();
            var allLinks = linked.Select(f => f.LinkId).Distinct().ToArray();

            var linkedModels = procedure.GetRange(new SqlWhereCondition(nameof(ApiModelBase.LinkId), allLinks))
                .Where(f => !linked.Contains(f))
                .GroupBy(f => f.LinkId!.Value)
                .ToDictionary(f => f.Key, f => f.ToArray());

            foreach (var fResult in linked)
            {

                if (!linkedModels.TryGetValue(fResult.LinkId!.Value, out var fLinkedModels))
                    continue;

                fLinkedModels = fLinkedModels.Where(f => f.Id != fResult.Id).ToArray();

                foreach (var fLinkedModel in fLinkedModels)
                {
                    SyncLinkedInner((TModel)fResult, fLinkedModel, parentModel);
                }
            }
        }

        public class DummyModel : ApiModelBase
        {
            public override string? ExternalId { get; set; }
            public override Guid ParentId { get; set; }

            public override ApiChildModelMeta[] ChildModels { get; } = [];
            public override Guid OrganizationId { get; set; }
        }

        private static void SyncLinkedInner<TModel>(TModel source, TModel target, ModelBase? parentModel) where TModel : ApiModelBase
        {
            var dummy = Cast.CopyValues(target, new DummyModel());
            Cast.CopyValues(source, target, BindingFlags.Public |  BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Cast.CopyValues(dummy, target, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var apiCom = GetApiCom(target.OrganizationId);
            if (apiCom is not null)
            {
                if (parentModel is not null)
                {
                    var parentType = parentModel.GetType().GetBaseTypes().First(f => f.GetCustomAttribute<MapSource>() is not null);
                    var procedure = AutoProcedure.Of(parentType);
                    parentModel = procedure.Get(target.ParentId);
                }

                apiCom.UpdateSingle<TModel>(target, parentModel as ApiModelBase);
            }
            new AutoProcedure<TModel>().Update(target);
        }


        //public static void Update<TModel>(TModel model) where TModel : ApiModelBase, IOrganizationModel
        //{

        //}


        public static Integration.API.FrlComBase? GetApiCom(Guid orgId)
        {
            var model = AutoProcedure.Of<ApiSyncModel>().GetBy(f => f.OrganizationId, orgId);
            if (model == null)
                return null;
            var com = Activator.CreateInstance(Type.GetType(model!.Name!)!, model);
            return (Integration.API.FrlComBase)com!;
        }




        public class UpdateRequest
        {
            public ApiModelBase Model { get; init; }

            public eOperastion Operastion { get; init; }
            public enum eOperastion
            {
                Insert,
                Update,
                Delete
            }
        }

    }
}
