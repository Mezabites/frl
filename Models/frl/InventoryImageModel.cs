using static eFlex.Index.Base.Models.FileUploadModel;

namespace frlnet.Models.frl
{
    [MapSource("frl", "InventoryImage")]
    public class InventoryImageModel : FileModelBase
    {
        [Map] public Guid InventoryId { get; set; }
        //[Map] public byte[] File { get; set; }
        //[Map] public string Name { get; set; }
    }
}
