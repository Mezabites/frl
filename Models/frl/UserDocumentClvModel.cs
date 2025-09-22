namespace frlnet.Models.frl
{
    [MapSource("frl", "UserDocumentClv", false)]
    public class UserDocumentClvModel : ModelBase
    {
        [Map] public Guid UserDocumentId { get; set; }
        [Map] public Guid AdditionalTypeClvId { get; set; }
    }
}
