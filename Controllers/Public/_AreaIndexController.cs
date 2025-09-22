using eFlex.Index.Base.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace frlnet.Controllers.Public
{
    [Area(nameof(Public))]
    [Route("[area]/[controller]/[action]", Name = "[area]_[controller]_[action]")]
    public abstract class hAreaIndexController<TModel> : hIndexController<TModel> where TModel : ModelBase
    {
    }
}