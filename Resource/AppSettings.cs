public class AppSettings : eFlex.Index.Base.Resource.AppSettings
{
    static AppSettings()
    {
        //Override example.
        ShortProjectName = eFlex.Index.Base.Initialize.Configuration["Constants:ProjectName"] ?? "IndexDemoApp";
    }


    //Define example.
    public static string Example => eFlex.Index.Base.Initialize.Configuration["TestGroup:TestVariable"] ?? string.Empty;
}