using System.Collections.Generic;
using System.Linq;
using Examine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.TagManager.Repositories;
using Umbraco.Community.TagManager.Repositories.Implementation;
using static Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.TagManager.Composer;

public class TagListComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<ContentSavingNotification, UpdateContentTagsForIndex>();
        builder.AddNotificationHandler<MediaSavingNotification, UpdateMediaTagsForRelations>();
        builder.Services.AddScoped<ITagListRepository, TagListRepository>();
    }
}

public class UpdateMediaTagsForRelations(
    IDataTypeService dataTypeService,
    ITagRepository tagRepository,
    IExamineManager examineManager) : INotificationHandler<MediaSavingNotification>
{
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
                bool externalIndexAvailable = examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex externalIndex);

                // check for internal index and add data
                bool internalIndexAvailable = examineManager.TryGetIndex(UmbracoIndexes.InternalIndexName, out IIndex internalIndex);

                if (property.GetValue() != null)
                {
                    // get property tag data from content, preValues (group) from datatype
                    string[] items = JsonConvert.DeserializeObject<string[]>((string)property.GetValue()!);
                    IDataType dataType = dataTypeService.GetDataType(property.PropertyType.DataTypeId);
                    Dictionary<string, object> preValues = (Dictionary<string, object>)dataType!.Configuration!;
                    string group = (string)preValues["group"];

                    // Create the tags
                    IEnumerable<ITag> tags = items.Select(text => new Tag
                    {
                        Text = text,
                        Group = group
                    });

                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
                    tagRepository.Assign(item.Id, property.PropertyTypeId, tags, false);
                    // Need to make a suggestion this API includes getAllTags (with unused tags) and Remove/RemoveAll/Assign updates the index.

                    if (externalIndexAvailable)
                        externalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));

                    if (internalIndexAvailable)
                        internalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));
                }
                else
                {
                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
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
        updatedValues[key] = [value];
        e.SetValues(updatedValues.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value));
    }
}

public class UpdateContentTagsForIndex(IDataTypeService dataTypeService, 
    ITagRepository tagRepository, 
    IExamineManager examineManager) : INotificationHandler<ContentSavingNotification>
{
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
                bool externalIndexAvailable = examineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex externalIndex);

                // check for internal index and add data
                bool internalIndexAvailable = examineManager.TryGetIndex(UmbracoIndexes.InternalIndexName, out IIndex internalIndex);

                if (property.GetValue() != null)
                {
                    // get property tag data from content, preValues (group) from datatype
                    string[] items = JsonConvert.DeserializeObject<string[]>((string)property.GetValue()!);
                    IDataType dataType = dataTypeService.GetDataType(property.PropertyType.DataTypeId);
                    Dictionary<string, object> preValues = (Dictionary<string, object>)dataType!.Configuration!;
                    string group = (string)preValues["group"];

                    // Create the tags
                    IEnumerable<ITag> tags = items.Select(text => new Tag
                    {
                        Text = text,
                        Group = group
                    });

                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
                    tagRepository.Assign(item.Id, property.PropertyTypeId, tags, false);
                    // Need to make a suggestion this API includes getAllTags (with unused tags) and Remove/RemoveAll/Assign updates the index.

                    if (externalIndexAvailable)
                        externalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));

                    if (internalIndexAvailable)
                        internalIndex.TransformingIndexValues += (sender, e) => AddData(sender, e, property.Alias, string.Join(",", items));
                }
                else
                {
                    // Wow, I found an API that actually works adding to the Tags table without having to revert to raw SQL - BUT IT DOESN'T UPDATE THE INDEX, So we'll have to do that separately :-(
                    tagRepository.RemoveAll(item.Id, property.PropertyTypeId);
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
        updatedValues[key] = [value];
        e.SetValues(updatedValues.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value));
    }
}