namespace Sciendo.Last.Fm
{
    public interface IContentProvider<T> where T:class, new()
    {
        T GetContent(string methodName, string userName, int currentPage = 1, string additionalParameters = "");
    }
}
