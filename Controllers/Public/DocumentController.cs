using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.Public
{
    [Area(nameof(Public))]
    [Claim(eClaimName.Read)]
    [Claim(eClaimName.Update)]
    [Claim(nameof(Approve))]
    public class UserDocumentController : frlnet.Controllers.frl.UserDocumentController
    {
        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var parts = where.Parts.Where(f => !f.Name.Contains(nameof(UserDocumentModel.OrganizationId)));
            where = new SqlWhereCondition(parts.ToList());
            where.Parts.Add(new(nameof(UserDocumentModel.OrganizationId), eFlex.Index.Base.Initialize.IndexConfiguration.UserRegistrationOrganizationId!.Value));

            foreach (var f in where.Parts)
                if (f.Name == nameof(UserDocumentModel.OrganizationId))
                    f.Name = "UserOrganizationId";
            return where;
        }
    }
}
