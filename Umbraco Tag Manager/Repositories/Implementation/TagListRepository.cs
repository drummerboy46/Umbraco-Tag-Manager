using System;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Umbraco.Community.TagManager.Repositories.Implementation
{
    internal class TagListRepository(ILogger<TagManagerRepository> logger, IScopeProvider scopeProvider)
        : ITagListRepository
    {

        public string[] GetTagsByGroup(string group)
        {
            // We're only going to return a string array, because this is best for indexing in Examine. 
            string[] tags = null;
            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    //Simple SQL query to return all tags by group from the cmstags table.
                    var query = new Sql().Select(
                        $"tag AS text FROM cmsTags WHERE [group] = '{group}'");

                    tags = scope.Database.Fetch<string>(query).ToArray();

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error returning a string array in GetTagsByGroupAndSize:", ex);
            }
            return tags;
        }
    }
}
