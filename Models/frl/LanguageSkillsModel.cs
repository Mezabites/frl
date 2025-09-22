using eFlex.Index.Base.Controls.Settings;
using eFlex.Index.Base.FilterConditions;

namespace frlnet.Models.frl
{
    [MapSource("frl", "LanguageSkills")]
    public class LanguageSkillsModel : ModelBase
    {
        [Map] public Guid UserId { get; set; }

        [ShowMark(In.Grid | In.Edit | In.Create)]

        [DropListSettings(SourceClassifierCode = Classifiers._LanguageSkillsLanguage.Code)]
        [Show]
        [Map] public Guid? LanguageClvId { get; set; }

        [DropListSettings(SourceClassifierCode = Classifiers._LanguageSkillsLevel.Code)]
        [Show]
        [Map] public Guid? LevelClvId { get; set; }

        [TextBoxSettings(Multiline = true)]
        [Show]
        [Map] public string? Notes { get; set; }
    }
}
