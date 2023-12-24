namespace Core
{
    public interface IBaseAoc
    {
        TextWriter Out { get; set; }

        event EventHandler<WriteEventArgs> Writing;

        Task PartOneTest(string fileName, ActionLevel level);
        Task PartTwoTest(string fileName, ActionLevel level);

        string? SessionCookie { get; set; }
    }
}