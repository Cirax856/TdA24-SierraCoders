namespace aspnetapp
{
    public interface ISaveLoad<T> where T : ISaveLoad<T>
    {
        public static abstract string FileName { get; }

        public abstract void Save(SaveWriter writer);
        public abstract static T Load(SaveReader reader);
    }
}
