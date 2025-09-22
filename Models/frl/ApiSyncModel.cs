using eFlex.Common.Extensions;
using eFlex.Index.Base.Controllers.Admin;
using eFlex.Index.Base.FilterConditions;
using eFlex.Index.Base.Models.Admin;
using eFlex.Index.Base.Types;

namespace frlnet.Models.frl
{
    [MapSource("frl", "ApiSync")]
    public class ApiSyncModel : ModelBase, IOrganizationModel
    {
        [DropListSettings]
        [Show][Map] public string? Name { get; set; }
        public static SelectList<string> NameSource
        { get; } = eFlex.Index.Base.Initialize.Types
            .Where(f => f.HasBaseType(typeof(frlnet.Integration.API.ComBase)) && !f.IsAbstract)
            .ToNullableSelectList(f => f.Name, f => f.FullName!);

        [Show][Map] public string? Key { get; set; }
        public bool KeyVisible => !string.IsNullOrEmpty(Name);

        [Show][Map] public string? Token { get; set; }
        public bool TokenVisible => !string.IsNullOrEmpty(Name);

        [Map] public Guid? LastSyncId { get; set; }

		[DropListSettings(typeof(OrganizationController))]
        [Show][Map] public Guid? OrganizationId { get; set; }
        //public static SelectList<Guid> OrganizationIdSource => RoleModel.OrganizationIdSource;
    }
}
