using eFlex.Index.Base.Controls.Settings;

namespace frlnet.Models.frl
{

    public class ApproveModel: ModelBase
    {
        [TextBoxSettings(Multiline =true)]
        [Show(In.Edit)]
        public string? Notes { get; set; }
    }
}
