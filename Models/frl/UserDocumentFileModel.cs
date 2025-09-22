using eFlex.Common.Extensions;
using static eFlex.Index.Base.Models.FileUploadModel;

namespace frlnet.Models.frl
{
    [MapSource("frl", "UserDocumentFile")]
    public class UserDocumentFileModel : FileModelBase
    {
        [Map] public Guid UserDocumentId { get; set; }

        private string? _MimeType;
        [Map]
        public string? MimeType
        {
            get
            {
                if (string.IsNullOrEmpty(_MimeType) == false)
                    return _MimeType;

                if (string.IsNullOrEmpty(Extension))
                    return string.Empty;

                return MimeTypeMap.GetMimeType(Extension);
            }
            set => _MimeType = value;
        }

        private string _Extension;

        [Map]
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(_Extension) == false)
                    return _Extension;

                if (string.IsNullOrEmpty(Name))
                    return string.Empty;

                var ext = Path.GetExtension(Name)?.RemovePrefix(".");
                return ext ?? string.Empty;

            }

            set => _Extension = value;
        }
    }
}