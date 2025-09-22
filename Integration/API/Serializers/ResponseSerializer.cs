namespace frlnet.Integration.API.Serializers
{
    public abstract class ResponseSerializer
    {
        public abstract string Serialize<T>(IEnumerable<T> obj);
        public abstract string Serialize<T>(T obj);

        public abstract IEnumerable<T> Deserialize<T>(string str);

        public abstract HttpContent SerializeToContent<T>(T obj);
    }
}
