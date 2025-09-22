using eFlex.Common.Extensions;
using frlnet.Integration.API.Serializers;
using frlnet.Models.frl;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace frlnet.Integration.API
{

	public abstract class ComBase
    {

        protected ComBase(ResponseSerializer serializer)
        {
            this.Serializer = serializer;
        }
        public ResponseSerializer Serializer { get; }

        public virtual void ProcessStatusCode(HttpStatusCode code)
        {
            if (code == HttpStatusCode.OK)
                return;
            else
                throw new HttpRequestException(code.ToString());
        }

        protected string ReadFromStream(HttpResponseMessage message)
        {
            var receiveStream = message.Content.ReadAsStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            return readStream.ReadToEnd();
        }

        public IEnumerable<ApiModel> Download<ApiModel>(string url)
            where ApiModel : ModelBase
        {
            var i = 0u;
            while (true)
            {
                var fullUrl = UpdateUrl(url, i);

                var client = new HttpClient();
                var response = client.GetAsync(fullUrl).Result;

                ProcessStatusCode(response.StatusCode);

                var content = ReadFromStream(response);

                var apiModels = Serializer.Deserialize<ApiModel>(content);

                foreach (var f in apiModels)
                {
                    yield return f;
                }

                if (!DownloadNextPage(apiModels))
                    break;

                i++;
            }
        }

        public HttpResponseMessage Update<ApiModel>(string url, ApiModel model)
        {
            var fullUrl = UpdateUrl(url, null);
            var client = new HttpClient();
            var content = Serializer.SerializeToContent(model);
            var response = client.PostAsync(fullUrl, content).Result;
            ProcessStatusCode(response.StatusCode);
            return response;
        }

        public HttpResponseMessage Insert<ApiModel>(string url, ApiModel model)
        {
            var fullUrl = UpdateUrl(url, null);
            var client = new HttpClient();
            var content = Serializer.SerializeToContent(model);
            var response = client.PostAsync(fullUrl, content).Result;
            ProcessStatusCode(response.StatusCode);
            return response;
        }

        private static MethodInfo DownloadGeneric = typeof(ComBase).GetMethods().Where(f => f.Name == nameof(Download) && f.GetGenericArguments().Count() == 2).First();

        protected abstract string UpdateUrl(string url, uint? i);

        public abstract bool DownloadNextPage(IEnumerable<ModelBase> models);

    }

    public abstract class FrlComBase : ComBase
    {
        protected FrlComBase(ApiSyncModel syncModel, ResponseSerializer serializer) : base(serializer)
        {
            this.SyncModel = syncModel;
        }

        public ApiSyncModel SyncModel { get; }

        private readonly Dictionary<Type, FrlComStructure> StructureRegistry = new();
        private readonly Dictionary<Type, Type> TopLevelRegistry = new();
        public void Register<TModel>(FrlComStructure<TModel> structure) where TModel : ApiModelBase
        {
            StructureRegistry.Set(typeof(TModel), structure);
            var allTypes = typeof(TModel).GetBaseTypes();
            foreach (var f in allTypes)
            {
                if (f == typeof(ModelBase))
                    break;
                TopLevelRegistry.Set(f, typeof(TModel));
            }
        }

        private MethodInfo? GetTopLevelMethod<TModel>([CallerMemberName] string? name = null)
        {
            ArgumentNullException.ThrowIfNull(name);

            if (!TopLevelRegistry.TryGetValue(typeof(TModel), out var topType))
                throw new Exception("Invalid API structure request");

            if (topType != typeof(TModel))
            {
                var method = typeof(FrlComBase).GetMethod(name);
                method = method!.MakeGenericMethod(topType);
                return method;
            }
            return null;
        }

        public virtual IEnumerable<TModel> DownloadList<TModel>(ApiModelBase? parentModel) where TModel : ApiModelBase
        {
            var top = GetTopLevelMethod<TModel>();
            if (top != null)
                return (IEnumerable<TModel>)top.Invoke(this, [parentModel])!;

            if (!StructureRegistry.TryGetValue(typeof(TModel), out var structure))
                throw new Exception("Invalid API structure request");

            var url = structure.DownloadListUrl(parentModel);
            var models = Download<TModel>(url).ToArray();
            if (parentModel is not null)
                foreach (var f in models)
                    f.ParentId = parentModel.Id!.Value;
            ((FrlComStructure<TModel>)structure).Populate(models, SyncModel);
            return models;
        }

        public virtual void UpdateSingle<TModel>(ApiModelBase model, ApiModelBase? parentModel) where TModel : ApiModelBase
        {
            var top = GetTopLevelMethod<TModel>();
            if (top != null)
            {
                top.Invoke(this, [model, parentModel]);
                return;
            }

            if (!StructureRegistry.TryGetValue(typeof(TModel), out var structure))
                throw new Exception("Invalid API structure request");

            var apiModel = DownloadList<TModel>(parentModel).FirstOrDefault(f => f.ExternalId == model.ExternalId)!;
            if (apiModel is null)
                return;
            Cast.CopyValues(model, apiModel);
            var url = structure.UpdateSingleUrl(parentModel, apiModel.ExternalId!);
            Update(url, apiModel);
        }

        public virtual void InsertSingle<TModel>(ApiModelBase model, ApiModelBase? parentModel) where TModel : ApiModelBase
        {
            var top = GetTopLevelMethod<TModel>();
            if (top != null)
            {
                top.Invoke(this, [model, parentModel]);
            }
            else
            {
                if (!StructureRegistry.TryGetValue(typeof(TModel), out var structure))
                    throw new Exception("Invalid API structure request");

                var apiModel = Activator.CreateInstance<TModel>();
                Cast.CopyValues(model, apiModel);
                var url = structure.InsertSingleUrl(parentModel);
                var response = Insert(url, apiModel);
                var id = GetInsertResult(response).Trim(); //Trim is required.
                if (string.IsNullOrEmpty(id)) throw new ArgumentNullException();
                model.ExternalId = id;
            }

            if (typeof(TModel) == model.GetType())
                if (model.Id.HasValue)
                    AutoProcedure.Of<TModel>().Update((TModel)model);
                else
                    AutoProcedure.Of<TModel>().Insert((TModel)model);

        }

        public abstract string GetInsertResult(HttpResponseMessage message);
    }

    public abstract class FrlComStructure
    {
        public abstract string DownloadListUrl(ApiModelBase? parentModel);
        public abstract string? InsertSingleUrl(ApiModelBase? parentModel);
        public abstract string? UpdateSingleUrl(ApiModelBase? parentModel, string? id);
        public abstract string? DeleteSingleUrl(ApiModelBase? parentModel, string? id);
    }

    public abstract class FrlComStructure<TModel> : FrlComStructure where TModel : ApiModelBase
    {
        public virtual void Populate(IEnumerable<TModel> models, ApiSyncModel syncModel) { }
    }


    public abstract class ApiModelBase : ModelBase
    {
        public abstract string? ExternalId { get; set; }
        public abstract Guid ParentId { get; set; }
        public virtual Guid? LinkId { get; set; }
        public abstract Guid OrganizationId { get; set; }
        public abstract ApiChildModelMeta[] ChildModels { get; }
    }
    public record ApiChildModelMeta(Type Type, string Key);



    public class PayTraqCom : FrlComBase
    {

        public PayTraqCom(ApiSyncModel syncModel) : base(syncModel, new DeepXmlResponseSerializer())
        {

            Register(new ClientStructure());
            Register(new ClientContactStructure());
            Register(new ClientBankInfoStructure());
        }

        private class CommonPageParams
        {
            public string APIToken; //"brlioC3FuA2NSzrk";
            public string APIKey;//"b3f8cb11-9363-4036-8774-aae45b172688-3259";
            public uint? page = 0;
        }

        protected override string UpdateUrl(string url, uint? i)
        {
            var @params = new CommonPageParams() { APIToken = SyncModel!.Token!, APIKey = SyncModel!.Key! };
            @params.page = i;
            return url.FormatFrom(@params);
        }

        public override bool DownloadNextPage(IEnumerable<ModelBase> models)
        {
            return models.Count() >= 100;
        }

        public override string GetInsertResult(HttpResponseMessage message)
        {
            var content = ReadFromStream(message);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            var root = xmlDoc.DocumentElement;

            // Check if there is only one child node under the root element
            if (root.ChildNodes.Count != 1)
            {
                throw new InvalidOperationException("The XML does not contain exactly one child node.");
            }

            // Get the value of the single child node
            XmlNode singleNode = root.FirstChild;
            string value = singleNode.InnerText;

            return value;
        }
    }

    public class ClientStructure : FrlComStructure<frlnet.Integration.API.PayTraq.Models.ClientModel>
    {
        public override string DownloadListUrl(ApiModelBase? parentModel) => "https://go.paytraq.com/api/clients?APIToken={APIToken}&APIKey={APIKey}&page={page}";
        public override string? InsertSingleUrl(ApiModelBase? parentModel) => $"https://go.paytraq.com/api/client?APIToken={{APIToken}}&APIKey={{APIKey}}";
        public override string? UpdateSingleUrl(ApiModelBase? parentModel, string? id) => $"https://go.paytraq.com/api/client/{id}?APIToken={{APIToken}}&APIKey={{APIKey}}";
        public override string? DeleteSingleUrl(ApiModelBase? parentModel, string? id) => null;

        public override void Populate(IEnumerable<PayTraq.Models.ClientModel> models, ApiSyncModel syncModel)
        {
            base.Populate(models, syncModel);
            models.ForEach(f => f.OrganizationId = syncModel.OrganizationId!.Value);
        }
    }

    public class ClientContactStructure : FrlComStructure<frlnet.Integration.API.PayTraq.Models.ClientContactModel>
    {
        public override string DownloadListUrl(ApiModelBase? parentModel)
        {
            return $"https://go.paytraq.com/api/client/contacts/{parentModel!.ExternalId}?APIToken={{APIToken}}&APIKey={{APIKey}}&page={{page}}";
        }

        public override string? InsertSingleUrl(ApiModelBase? parentModel)
        {
            return $"https://go.paytraq.com/api/client/contact/{parentModel!.ExternalId}?APIToken={{APIToken}}&APIKey={{APIKey}}";
        }

        public override string? UpdateSingleUrl(ApiModelBase? parentModel, string? id)
        {
            return $"https://go.paytraq.com/api/client/contact/{parentModel!.ExternalId}/{id}?APIToken={{APIToken}}&APIKey={{APIKey}}";
        }

        public override string? DeleteSingleUrl(ApiModelBase? parentModel, string? id) => null;
    }

    public class ClientBankInfoStructure : FrlComStructure<frlnet.Integration.API.PayTraq.Models.ClientBankInfoModel>
    {
        public override string DownloadListUrl(ApiModelBase? parentModel)
        {
            return $"https://go.paytraq.com/api/client/banks/{parentModel!.ExternalId}?APIToken={{APIToken}}&APIKey={{APIKey}}&page={{page}}";
        }

        public override string? InsertSingleUrl(ApiModelBase? parentModel)
        {
            return $"https://go.paytraq.com/api/client/bank/{parentModel!.ExternalId}?APIToken={{APIToken}}&APIKey={{APIKey}}";
        }

        public override string? UpdateSingleUrl(ApiModelBase? parentModel, string? id)
        {
            return $"https://go.paytraq.com/api/client/bank/{parentModel!.ExternalId}/{id}?APIToken={{APIToken}}&APIKey={{APIKey}}";
        }

        public override string? DeleteSingleUrl(ApiModelBase? parentModel, string? id) => null;
    }

}
