using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUglify.Helpers;
using Our.Umbraco.Community.TagManager.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Our.Umbraco.Community.TagManager.Repositories.Implementation
{
    internal class TagManagerRepository(IScopeProvider scopeProvider, IContentService contentService,
        IMediaService mediaService, ITagService tagService, ILogger<TagManagerRepository> logger) : ITagManagerRepository
    {
        public PagedContent GetPagedContent(int id, int offset = 0, int limit = 10)
        {
            List<TaggedContent> taggedContent = GetTaggedContent(id).Skip(offset).Take(limit).ToList();
            int totalRecords = GetTaggedContent(id).Count;
            PagedContent pagedContent = new PagedContent
            {
                TaggedContent = taggedContent,
                TotalRecords = totalRecords
            };
            return pagedContent;
        }

        public PagedMedia GetPagedMedia(int id, int offset = 0, int limit = 10)
        {
            List<TaggedMedia> taggedMedia = GetTaggedMedia(id).Skip(offset).Take(limit).ToList();
            int totalRecords = GetTaggedMedia(id).Count;
            PagedMedia pagedMedia = new PagedMedia
            {
                TaggedMedia = taggedMedia,
                TotalRecords = totalRecords
            };
            return pagedMedia;
        }

        public TagList GetTagsById(int id)
        {
            TagList tagList = new TagList();

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    string sql = "SELECT id, tag, [group] FROM cmsTags WHERE cmsTags.id = @0";
                    TagItem tagItem = scope.Database.Single<TagItem>(sql, id);
                    tagList.TagItem = tagItem;

                    sql = "SELECT nodeId, tagId, propertyTypeId FROM cmsTagRelationship";
                    List<TagRelationship> tagRelationships = scope.Database.Fetch<TagRelationship>(sql);

                    List<TaggedContent> taggedContent = GetTaggedContent(id);
                    tagItem.TaggedContent = taggedContent;
                    List<TaggedMedia> taggedMedia = GetTaggedMedia(id);
                    tagItem.TaggedMedia = taggedMedia;

                    tagItem.TagRelationships = tagRelationships.Where(x => x.TagId == id).ToList();
                    tagItem.TagRelationshipCount = tagItem.TagRelationships.Count;

                    sql = "SELECT id, tag, [group] FROM cmsTags WHERE cmsTags.[group] = @0";
                    tagList.TagsInGroup = scope.Database.Fetch<TagItem>(sql, tagItem.Group);

                    if (tagList.TagsInGroup != null)
                    {
                        foreach (TagItem tag in tagList.TagsInGroup)
                        {
                            List<TaggedContent> taggedContentGrp = GetTaggedContent(tag.Id);
                            tag.TaggedContent = taggedContentGrp;
                            List<TaggedMedia> taggedMediaGrp = GetTaggedMedia(id);
                            tag.TaggedMedia = taggedMediaGrp;
                            tag.TagRelationships = tagRelationships.Where(x => x.TagId == tag.Id).ToList();
                            tag.TagRelationshipCount = tag.TagRelationships.Count;
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in GetTagById:", ex);
            }

            return tagList;
        }

        public TagList GetTagsByGroup(string group)
        {
            TagList tagList = new TagList();

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    string sql = "SELECT id, tag, [group] FROM cmsTags WHERE cmsTags.[group] = @0";
                    tagList.TagsInGroup = scope.Database.Fetch<TagItem>(sql, group);

                    sql = "SELECT nodeId, tagId, propertyTypeId FROM cmsTagRelationship";
                    List<TagRelationship> tagRelationships = scope.Database.Fetch<TagRelationship>(sql);

                    if (tagList.TagsInGroup != null)
                    {
                        foreach (TagItem tag in tagList.TagsInGroup)
                        {
                            List<TaggedContent> taggedContent = GetTaggedContent(tag.Id);
                            tag.TaggedContent = taggedContent;
                            List<TaggedMedia> taggedMedia = GetTaggedMedia(tag.Id);
                            tag.TaggedMedia = taggedMedia;
                            tag.TagRelationships = tagRelationships.Where(x => x.TagId == tag.Id).ToList();
                            tag.TagRelationshipCount = tag.TagRelationships.Count;
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in GetTagById:", ex);
            }

            return tagList;
        }

        public List<TagGroup> GetTagGroups()
        {
            List<TagGroup> tagGroups = new List<TagGroup>(); ;

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    string sql = "SELECT [group] FROM cmsTags GROUP BY [group] ORDER BY [group];";
                    tagGroups = scope.Database.Fetch<TagGroup>(sql);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in GetTagGroups:", ex);
            }

            return tagGroups;
        }

        public int CreateGroup(string group = "default")
        {
            var success = 0;

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    success = scope.Database.ExecuteScalar<int>("INSERT INTO cmsTags (tag, [group]) VALUES (@0, @1); " +
                                                                "SELECT CAST(SCOPE_IDENTITY() AS INT)",
                        "default", group);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Create:");
            }

            return success;
        }

        public int CreateTag(TagItem tagItem)
        {
            var success = 0;

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    if (tagItem.Id == 0)
                    {
                        success = scope.Database.ExecuteScalar<int>("INSERT INTO cmsTags (tag, [group]) VALUES (@0, @1); " +
                                                                    "SELECT CAST(SCOPE_IDENTITY() AS INT)",
                            tagItem.Tag, tagItem.Group);
                    }
                    scope.Complete();

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Create:");
            }

            return success;
        }

        public int SaveTag(TagList tagList)
        {
            var success = 0;

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    success = scope.Database.Execute("UPDATE cmsTags SET tag = @0 WHERE id = @1",
                        tagList.TagItem.Tag, tagList.TagItem.Id);

                    if (success == 1 && tagList.MergeTag != null && tagList.TagItem.Id != tagList.MergeTag.Id)
                    {
                        List<TagItem> mergeTagForUpdate = [tagList.MergeTag];

                        scope.Database.Execute("UPDATE cmsTagRelationship SET tagID = @0 WHERE tagID = @1 AND" +
                                               " nodeId NOT IN (SELECT nodeId FROM cmsTagRelationship WHERE tagId = @0);",
                            tagList.TagItem.Id, tagList.MergeTag.Id);

                        scope.Database.Execute("DELETE FROM cmsTagRelationship WHERE tagId = @0", tagList.MergeTag.Id);
                        scope.Database.Execute("DELETE FROM cmsTags WHERE id = @0", tagList.MergeTag.Id);

                        UpdateContent(mergeTagForUpdate);
                        UpdateMedia(mergeTagForUpdate);
                    }

                    List<TagItem> tagForUpdate = [tagList.TagItem];

                    UpdateContent(tagForUpdate);
                    UpdateMedia(tagForUpdate);

                    scope.Complete();

                    success = 1;
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error in Save:", ex);
            }

            return success;
        }


        public int DeleteTag(TagList tagList)
        {
            var success = 0;

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    var sqlQuery1 = $"DELETE FROM cmsTagRelationship WHERE tagId = @0;";
                    var sqlQuery2 = $"DELETE FROM cmsTags WHERE id = @0;";
                    scope.Database.Execute(sqlQuery1, tagList.TagItem.Id);
                    scope.Database.Execute(sqlQuery2, tagList.TagItem.Id);

                    List<TagItem> tags = [tagList.TagItem];

                    UpdateContent(tags);
                    UpdateMedia(tags);

                    scope.Complete();
                }

                success = 1;
            }
            catch (Exception ex)
            {
                logger.LogError("Error in Delete:", ex);
            }

            return success;
        }


        public int DeleteTags(TagList tagList)
        {
            var success = 0;

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    List<string> tagIds = tagList.TagsInGroup.Where(x => x.TagSelected).Select(x => x.Id.ToString()).ToList();
                    IEnumerable<TagItem> tagItems = tagList.TagsInGroup.Where(x => x.TagSelected).ToList();

                    var sqlQuery1 = $"DELETE FROM cmsTagRelationship WHERE tagId IN (@0);";
                    var sqlQuery2 = $"DELETE FROM cmsTags WHERE id IN (@0);";
                    scope.Database.Execute(sqlQuery1, tagIds);
                    scope.Database.Execute(sqlQuery2, tagIds);

                    UpdateContent(tagItems);
                    UpdateMedia(tagItems);

                    scope.Complete();
                }

                success = 1;
            }
            catch (Exception ex)
            {
                logger.LogError("Error in Delete Tags:", ex);
            }

            return success;
        }

        private List<TaggedContent> GetTaggedContent(int tagId)
        {
            List<TaggedContent> contentList = new List<TaggedContent>();

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    var query = $"SELECT nodeId AS Id FROM cmsTagRelationship WHERE tagId = @0;";
                    var results = scope.Database.Fetch<TaggedContent>(query, tagId);

                    foreach (var result in results)
                    {
                        var n = contentService.GetById(result.Id);
                        if (n != null)
                        {
                            if (!string.IsNullOrWhiteSpace(n.Name))
                            {
                                var content = new TaggedContent
                                {
                                    Id = result.Id,
                                    Name = n.Name,
                                    Url = $"#/content/content/edit/{result.Id}"
                                };
                                contentList.Add(content);
                            }
                        }
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in GetTaggedDocumentNodeIds:", ex);
            }

            return contentList;
        }

        private List<TaggedMedia> GetTaggedMedia(int tagId)
        {
            var mediaList = new List<TaggedMedia>();

            try
            {
                using (var scope = scopeProvider.CreateScope())
                {
                    var query = $"SELECT nodeId AS Id FROM cmsTagRelationship WHERE tagId = @0;";
                    var results = scope.Database.Fetch<TaggedMedia>(query, tagId);

                    foreach (var result in results)
                    {
                        var n = mediaService.GetById(result.Id);
                        if (n != null)
                        {
                            if (!string.IsNullOrWhiteSpace(n.Name))
                            {
                                var media = new TaggedMedia
                                {
                                    Id = result.Id,
                                    Name = n.Name,
                                    Url = $"#/media/media/edit/{result.Id}"
                                };
                                mediaList.Add(media);
                            }
                        }
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in GetTaggedMediaNodeIds:", ex);
            }

            return mediaList;
        }

        private void UpdateContent(IEnumerable<TagItem> tags)
        {
            List<TagItem> tagItems = tags.ToList();

            foreach (TagItem tag in tagItems)
            {
                List<TagRelationship> tagRelations = tag.TagRelationships;

                foreach (var item in tag.TaggedContent)
                {
                    IContent content = contentService.GetById(item.Id);
                    string propertyAlias;

                    content!.Properties.ForEach(bItem =>
                    {
                        if (tagRelations.Any(x => x.PropertyTypeId == bItem.PropertyTypeId))
                        {
                            propertyAlias = bItem.Alias;
                            string tagsVal = content.GetValue<string>(propertyAlias!);
                            string tagsFormat = tagsVal.Contains("[") ? "json" : "csv";

                            IEnumerable<string> tagsToUpdate = tagService.GetTagsForEntity(item.Id, tagItems.FirstOrDefault()!.Group).Select(x => x.Text).ToList();

                            if (tagsFormat == "csv")
                            {
                                string csvTags = string.Join(',', tagItems);
                                content.SetValue(propertyAlias, csvTags);
                            }
                            else
                            {
                                string jsonTags = JsonConvert.SerializeObject(tagsToUpdate.ToArray(), Formatting.None);
                                content.SetValue(propertyAlias, jsonTags);
                            }

                            contentService.SaveAndPublish(content);
                        }
                    });
                }
            }
        }

        private void UpdateMedia(IEnumerable<TagItem> tags)
        {
            List<TagItem> tagItems = tags.ToList();

            foreach (TagItem tag in tagItems)
            {
                List<TagRelationship> tagRelations = tag.TagRelationships;

                foreach (var item in tag.TaggedMedia)
                {
                    IMedia media = mediaService.GetById(item.Id);
                    string propertyAlias;

                    media!.Properties.ForEach(bItem =>
                    {
                        if (tagRelations.Any(x => x.PropertyTypeId == bItem.PropertyTypeId))
                        {
                            propertyAlias = bItem.Alias;
                            string tagsVal = media.GetValue<string>(propertyAlias!);
                            string tagsFormat = tagsVal.Contains("[") ? "json" : "csv";

                            IEnumerable<string> tagsToUpdate = tagService.GetTagsForEntity(item.Id, tagItems.FirstOrDefault()!.Group).Select(x => x.Text).ToList();

                            if (tagsFormat == "csv")
                            {
                                string csvTags = string.Join(',', tagItems);
                                media.SetValue(propertyAlias, csvTags);
                            }
                            else
                            {
                                string jsonTags = JsonConvert.SerializeObject(tagsToUpdate.ToArray(), Formatting.None);
                                media.SetValue(propertyAlias, jsonTags);
                            }
                            mediaService.Save(media);
                        }
                    });
                }
            }
        }
    }
}
