using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.FilterConditions;
using eFlex.Index.Base.Types;

namespace eFlex.Index.Base.Models.Admin
{
    [CacheByModelRegister]
    [MapSource("adm", "SettingValue")]
    [MapJoin("adm", "Setting", alias: "")]
    [MapJoin("adm", "Organization")]
    public class SettingValueModel : ModelBase, IOrganizationModel
    {
        [ShowMark(editableIn: In.Filter)]

        [Map] public Guid SettingId { get; set; }
        [Map] public Guid? OrganizationId { get; set; }

        [Map] public string Code { get; set; }

        [Show]
        [Map] public string Name { get; set; }

        [DropListSettings(nameof(CategorySource))]
        [Show]
        [Map] public string Category { get; set; }
        public static SelectList<string> CategorySource { get; set; } = AutoProcedure.Of<SettingValueModel>().GetRange().Select(f => f.Category).Distinct().ToSelectList(f => f, f => f);

        [Show]
        [Map] public string OrganizationName { get; set; }

        [Show(editableIn:In.Filter | In.Edit)]
        [Map] public string? Value { get; set; }

        [Show(editableIn: In.Filter)]
        [DropListSettings(nameof(TypeSource))]
        [Map] public string Type { get; set; }

        public static Type[] TypesSupported = new Type[] { typeof(byte), typeof(sbyte), typeof(string), typeof(bool), typeof(short), typeof(int), typeof(ushort), typeof(uint), typeof(decimal) };
        public static SelectList<string> TypeSource { get; } = new SelectList<string>(TypesSupported.Select(f => new SelectValue<string>(f?.Name, f?.FullName)).ToArray());

        [Show(In.Edit)]
        [Map] public string? Description { get; set; }

    }

}
