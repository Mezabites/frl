using eFlex.Common.Extensions;

namespace frlnet.Models.frl
{
    [MapSource("frl", "Post")]
    public class PostCopyModel : ModelBase
    {
        [Show, Map] public string Name { get; set; } = null!;


        [DatePickerSettings()]
        [Show, Map] public DateOnly? Start { get; set; }
        public DatePickerSettings StartSettings
        {
            get
            {
                var minDate = DateTime.Now.ToDateOnly();
                if (End.HasValue)
                    minDate = End.Value;
                return new() { MinDate = minDate.ToDateTime() };
            }
        }

        [Show(In.None), Map] public DateOnly? End { get; set; }
    }
}
