namespace frlnet.Models.frl
{
    [MapSource("frl", "InventoryDetails")]
    public class InventoryDetailsModel : EditableModelBase
    {
        [Map] public Guid InventoryId { get; set; }

        [Show][Map] public string Name { get; set; }

        [Show][Map] public string Value { get; set; }

        [Show][Map] public string? Description { get; set; }
    }
}
