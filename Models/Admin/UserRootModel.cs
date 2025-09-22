using frlnet.Controllers.Admin;
using frlnet.Controllers.frl;

namespace frlnet.Models.Admin
{

    [CacheByModelRegister]
    [MapSource("adm", "User")]
    public class UserRootModel : ModelBase
    {
        [LabelTabMark]
        [Show(In.Edit)]
        public Link BaseData { get; set; } =  new Link(nameof(ProfileController.Edit), typeof(ProfileController), new {id = UserModel.CurrentId });

        [LabelTabMark]
        [Show(In.Edit)]
        public Link LanguageSkills => Link.ToIndex<LanguageSkillsController>();

        [LabelTabMark]
        [Show(In.Edit)]
        public Link BankInfo => Link.ToIndex<BankInfoController>();

        [LabelTabMark]
        [Show(In.Edit)]
        public Link Inventory => Link.ToIndex<InventoryUserController>();

        [LabelTabMark]
        [Show(In.Edit)]
        public Link Requests => Link.ToIndex<InventoryRequestController>();

        [LabelTabMark]
        [Show(In.Edit)]
        public Link Documents => Link.ToIndex<UserDocumentUserController>();
    }

   
}
