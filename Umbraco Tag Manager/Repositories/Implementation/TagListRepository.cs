using System;
using Microsoft.Extensions.Logging;
using NPoco;
using Our.Umbraco.Community.TagManager.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Our.Umbraco.Community.TagManager.Repositories.Implementation
{
    internal class TagListRepository : ITagListRepository
    {
        private readonly ILogger<TagManagerRepository> _logger;
        private readonly IScopeProvider _scopeProvider;

        public TagListRepository(ILogger<TagManagerRepository> logger, IScopeProvider scopeProvider)
        {
            _logger = logger;
            _scopeProvider = scopeProvider;
        }

        public string[] GetTagsByGroup(string group)
        {
            // We're only going to return a string array, because this is best for indexing in Examine. 
            string[] tags = null;
            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    string sql = "SELECT tag AS text FROM cmsTags WHERE [group] = @0";
                    tags = scope.Database.Fetch<string>(sql, group).ToArray();

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error returning a string array in GetTagsByGroupAndSize:", ex);
            }
            return tags;
        }
    }
}
