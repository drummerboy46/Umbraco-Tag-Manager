namespace Our.Umbraco.Community.TagManager.Repositories
{
    public interface ITagListRepository
    {
        string[] GetTagsByGroup(string group);
    }
}
