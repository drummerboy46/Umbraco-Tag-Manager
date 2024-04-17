using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Examine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Our.Umbraco.Community.TagManager.Repositories;
using Our.Umbraco.Community.TagManager.Repositories.Implementation;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Services;
using static Umbraco.Cms.Core.Constants;

namespace Our.Umbraco.Community.TagManager.Composer;

public class TagListComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<ContentSavingNotification, UpdateContentTagsForRelationsAndIndex>();
        builder.AddNotificationHandler<MediaSavingNotification, UpdateMediaTagsForRelationsAndIndex>();
        builder.Services.AddScoped<ITagListRepository, TagListRepository>();
    }
}
public class UpdateContentTagsForRelationsAndIndex : INotificationHandler<ContentSavingNotification>
{
    private readonly IDataTypeService _dataTypeService;
    private readonly ITagRepository _tagRepository;
    private readonly IExamineManager _examineManager;
    private readonly ILocalizationService _localizationService;

    public UpdateContentTagsForRelationsAndIndex(IDataTypeService dataTypeService,
        ITagRepository tagRepository,
        IExamineManager examineManager,
        ILocalizationService localizationService)
    {
        _dataTypeService = dataTypeService;
        _tagRepository = tagRepository;
        _examineManager = examineManager;
        _localizationService = localizationService;
    }

    public void Handle(ContentSavingNotification notification)
    {
        // return if there's no TagList properties in this content
        if (!notification.SavedEntities.Any(content => content.Properties.Any(prop => prop.PropertyType.PropertyEditorAlias == "TagList"))) return;

        // iterate through the content
        foreach (IContent item in notification.SavedEntities)
        {
            // get all properties using TagList Property Editor
            IEnumerable<IProperty> properties = item.Properties
                .Where(x => x.PropertyType.PropertyEditorAlias == "TagList");

            // iterate over the properties
            foreach (IProperty property in properties)
            {
                // Check for an external index and add data
                bool externalIndexAvailable = _examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex externalIndex);

                // check for internal index and add data
                bool internalIndexAvailable = _examineManager.TryGetIndex(UmbracoIndexes.InternalIndexName, out IIndex internalIndex);

                IEnumerable<ITag> tags = new List<ITag>();
                List<ITag> allTags = new List<ITag>();
                int? languageId = null;

                if (property.Values.Any())
                {
                    foreach (IPropertyValue value in property.Values)
                    {
                        // get property tag data from content, preValues (group) from datatype
                        if (value.EditedValue != null)
                        {
                            string[] items = JsonConvert.DeserializeObject<string[]>((string)value.EditedValue);
                            IDataType dataType = _dataTypeService.GetDataType(property.PropertyType.DataTypeId);
                            Dictionary<string, object> preValues = (Dictionary<string, object>)dataType!.Configuration!;
                            string group = (string)preValues["group"];

                            IEnumerable<ILanguage> languages = _localizationService.GetAllLanguages().ToList();

                            if (value.Culture != null)
                                languageId = languages.Count() > 1 ? languages.FirstOrDefault(x => x.IsoCode.ToLower() == value.Culture)!.Id : null;

                            // Create the tags
                            tags = items.Select(text => new Tag
                            {
                                Text = text,
                                LanguageId = languageId,
                                Group = group
                            }).ToList();

                            allTags.AddRange(tags);

                            if (externalIndexAvailable)
                                externalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));

                            if (internalIndexAvailable)
                                internalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));
                        }
                    }

                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    _tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
                    if (allTags.Any())
                        _tagRepository.Assign(item.Id, property.PropertyTypeId, allTags, false);
                    // Need to make a suggestion this API includes getAllTags (with unused tags) and Remove/RemoveAll/Assign updates the index.
                }
                else
                {
                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    _tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
                    // Need to make a suggestion this API includes getAllTags (with unused tags) and Remove/RemoveAll/Assign updates the index.

                    if (externalIndexAvailable)
                        externalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Empty);

                    if (internalIndexAvailable)
                        internalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Empty);
                }
            }
        }
    }

    private void AddData(object sender, IndexingItemEventArgs e, string key, string value)
    {
        Dictionary<string, List<object>> updatedValues = e.ValueSet.Values.ToDictionary(x => x.Key, x => x.Value.ToList());
        updatedValues[key] = new List<object> { value };
        e.SetValues(updatedValues.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value));
    }
}
public class UpdateMediaTagsForRelationsAndIndex : INotificationHandler<MediaSavingNotification>
{
    private readonly IDataTypeService _dataTypeService;
    private readonly ITagRepository _tagRepository;
    private readonly IExamineManager _examineManager;

    public UpdateMediaTagsForRelationsAndIndex(IDataTypeService dataTypeService,
        ITagRepository tagRepository,
        IExamineManager examineManager)
    {
        _dataTypeService = dataTypeService;
        _tagRepository = tagRepository;
        _examineManager = examineManager;
    }

    public void Handle(MediaSavingNotification notification)
    {
        if (!notification.SavedEntities.Any(media =>
                media.Properties.Any(prop => prop.PropertyType.PropertyEditorAlias == "TagList"))) return;

        foreach (IMedia item in notification.SavedEntities)
        {
            // get all properties using TagList Property Editor
            IEnumerable<IProperty> properties = item.Properties
                .Where(x => x.PropertyType.PropertyEditorAlias == "TagList");

            // iterate over the properties
            foreach (IProperty property in properties)
            {
                // Check for an external index and add data
                bool externalIndexAvailable = _examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex externalIndex);

                // check for internal index and add data
                bool internalIndexAvailable = _examineManager.TryGetIndex(UmbracoIndexes.InternalIndexName, out IIndex internalIndex);

                if (property.GetValue() != null)
                {
                    // get property tag data from content, preValues (group) from datatype
                    string[] items = JsonConvert.DeserializeObject<string[]>((string)property.GetValue()!);
                    IDataType dataType = _dataTypeService.GetDataType(property.PropertyType.DataTypeId);
                    Dictionary<string, object> preValues = (Dictionary<string, object>)dataType!.Configuration!;
                    string group = (string)preValues["group"];

                    // Create the tags
                    IEnumerable<ITag> tags = items.Select(text => new Tag
                    {
                        Text = text,
                        Group = group
                    });

                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    _tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
                    _tagRepository.Assign(item.Id, property.PropertyTypeId, tags, false);
                    // Need to make a suggestion this API includes getAllTags (with unused tags) and Remove/RemoveAll/Assign updates the index.

                    if (externalIndexAvailable)
                        externalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));

                    if (internalIndexAvailable)
                        internalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));
                }
                else
                {
                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    _tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
                    // Need to make a suggestion this API includes getAllTags (with unused tags) and Remove/RemoveAll/Assign updates the index.

                    if (externalIndexAvailable)
                        externalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Empty);

                    if (internalIndexAvailable)
                        internalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Empty);
                }
            }
        }
    }

    private void AddData(object sender, IndexingItemEventArgs e, string key, string value)
    {
        Dictionary<string, List<object>> updatedValues = e.ValueSet.Values.ToDictionary(x => x.Key, x => x.Value.ToList());
        updatedValues[key] = new List<object> { value };
        e.SetValues(updatedValues.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value));
    }
}