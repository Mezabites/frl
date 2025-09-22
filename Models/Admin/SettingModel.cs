using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.FilterConditions;
using eFlex.Index.Base.Types;

namespace eFlex.Index.Base.Models.Admin
{
    [CacheByModelRegister]
    [MapSource("adm", "Setting")]
    public class SettingModel : ModelBase
    {
        [Map(Unique = true)] public string Code { get; set; }

        [Map] public string Name { get; set; }

        [Map] public string Category { get; set; }
        
        [Map] public string? DefaultValue { get; set; }

        [Map(AllowUpdate = false)] public string Type { get; set; }

        [Map] public string? Description { get; set; }

    }

}
