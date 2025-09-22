using System.Runtime.CompilerServices;

namespace frlnet.Integration.API
{
    public static class Parser
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ParseSource : Attribute
    {
        public ParseSource(Type modelType)
        {
            ModelType = modelType;
        }
        public Type ModelType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ParseSource<TModel> : ParseSource where TModel : ModelBase
    {
        public ParseSource() : base(typeof(TModel)) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ParseTo : Attribute
    {
        public ParseTo([CallerMemberName] string? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(propertyName);
            PropertyName = propertyName;
        }
        public string PropertyName { get; set; }
    }

}
