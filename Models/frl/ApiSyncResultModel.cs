using frlnet.Integration.API;

namespace frlnet.Models.frl
{

    [MapSource("frl", "ApiSyncResult")]
    public class ApiSyncResultModel : ModelBase
    {
        [Map] public Guid ApiSyncId { get; set; }
        [Map] public Guid LastSyncId { get; set; }
        [Show, Map] public string ObjectName { get; set; }
        [Show, Map] public string ObjectType  { get; set; }
        [Show, Map] public eAction Action { get; set; }
        public enum eAction
        {
            Update,
            Insert,
            Delete,
            Sync
        }
        public bool HasError => !string.IsNullOrEmpty(Error);
        [Show, Map] public string? Error { get; set; }
        public ApiModelBase? Model { get; set; }
    }
}