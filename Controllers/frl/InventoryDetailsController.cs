using eFlex.Index.Base.Controllers;
using frlnet.Models.frl;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.frl
{
    [Area(nameof(frl))]
    [Route("[area]/[controller]/[action]", Name = "[area]_[controller]_[action]")]
    public class InventoryDetailsController: hGridController<InventoryDetailsModel>
    {
    }
}