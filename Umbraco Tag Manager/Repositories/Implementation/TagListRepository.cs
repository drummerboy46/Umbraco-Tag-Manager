using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NPoco;
using Our.Umbraco.Community.TagManager.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using static Umbraco.Cms.Core.Constants;

namespace Our.Umbraco.Community.TagManager.Repositories.Implementation
{
    internal class TagListRepository : ITagListRepository
    {
        private readonly ILogger<TagManagerRepository> _logger;
        private readonly IScopeProvider _scopeProvider;
        private readonly ILocalizationService _localizationService;

        public TagListRepository(ILogger<TagManagerRepository> logger, IScopeProvider scopeProvider, ILocalizationService localizationService)
        {
            _logger = logger;
            _scopeProvider = scopeProvider;
            _localizationService = localizationService;
        }

        public string[] GetTagsByGroup(string group, string culture)
        {
            // We're only going to return a string array, because this is best for indexing in Examine. 
            string[] tags = null;

            try
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    if (culture == "null" || culture == "undefined")
                    {
                        string sql = "SELECT tag AS text FROM cmsTags WHERE [group] = @0 AND languageId IS NULL";
                        tags = scope.Database.Fetch<string>(sql, group).ToArray();
                    }
                    else
                    {
                        IEnumerable<ILanguage> languages = _localizationService.GetAllLanguages().ToList();
                        int languageId = languages.FirstOrDefault(x => x.IsoCode == culture)!.Id;

                        string sql = "SELECT tag AS text FROM cmsTags WHERE [group] = @0 AND languageId = @1";
                        tags = scope.Database.Fetch<string>(sql, group, languageId).ToArray();
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTagsByGroup | Exception: {0} | Message: {1} | Stack Trace: {2}",
                    ex.InnerException != null ? ex.InnerException!.ToString() : "",
                    ex.Message, ex.StackTrace);
            }
            return tags;
        }
    }
}
