namespace frlnet.Models.frl
{
    [MapSource("frl", "UserDocument")]
    public class UserDocumentExpiredJobModel : ModelBase
    {
        [Map] public DateOnly? ExpirationDate { get; set; }
        [Map] public Guid StatusClvId { get; set; }
    }
}

