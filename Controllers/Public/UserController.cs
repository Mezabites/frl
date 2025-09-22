using eFlex.Index.Base.ActionResults;
using eFlex.Index.Base.Extensions;
using frlnet.Controllers.Admin;
using frlnet.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.Public
{
    [Claim(eClaimName.Read)]
    [Claim(eClaimName.Update)]
    [Area(nameof(Public))]
    public class UserController : Admin.UserController
    {
        public UserController()
        {
            Instructions.Create.Allow = false;
        }

        protected override SqlWhereCondition TranslateFilter(hIndexModel indexModel)
        {
            var where = base.TranslateFilter(indexModel);
            var parts = where.Parts.Where(f => !f.Name.Contains(nameof(UserModel.OrganizationId)));
            where = new SqlWhereCondition(parts.ToList());
            where.Parts.Add(new(nameof(UserModel.OrganizationId), eFlex.Index.Base.Initialize.IndexConfiguration.UserRegistrationOrganizationId!));
            return where;
        }

        public override IndexResult EditAllow(Guid id)
        {
            var res = base.EditAllow(id);
            if (res.Link is not null)
            {
                var link = new Link(nameof(Edit), typeof(UserRootController), HttpContext.Request.Query.ToDynamic());
                return new IndexResult(link, res.AddToReturn, res.Messages);
            }
            return res;
        }

        //Desision ar Andri 18.04.2024
        //public override ViewResult Edit(Guid id)
        //{
        //    var res = base.Edit(id);
        //    var indexModel = extractIndexModel(res);
        //    if(indexModel != null)
        //    {
        //        var structure = indexModel.GetControlStructure(In.Edit);
        //        structure.RemoveControl(nameof(UserModel.ReplacePassword));
        //        structure.RemoveControl(nameof(UserModel.ForcePasswordChange));
        //        structure.RemoveControl(nameof(UserModel.Status));
        //    }
        //    return res;
        //}
    }
}
