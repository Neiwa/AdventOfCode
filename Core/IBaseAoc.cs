namespace Core
{
    public interface IBaseAoc
    {
        TextWriter Out { get; set; }

        event EventHandler<WriteEventArgs> Writing;

        void PartOneTest(string fileName, ActionLevel level);
        void PartTwoTest(string fileName, ActionLevel level);

        string? SessionCookie { get; set; }
    }
}