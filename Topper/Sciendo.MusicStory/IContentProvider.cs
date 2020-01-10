namespace Sciendo.MusicStory
{
    public interface IContentProvider<T> where T: class, new()
    {
        T GetContent(string subject, ActionType actionType, string additionalParameters, long subjectId = 0, string attrbute="");
    }
}
