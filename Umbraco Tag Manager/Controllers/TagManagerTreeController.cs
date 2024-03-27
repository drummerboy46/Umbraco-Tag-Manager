using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.Community.TagManager.Models;
using Our.Umbraco.Community.TagManager.Repositories;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
using Constants = Umbraco.Cms.Core.Constants;

namespace Our.Umbraco.Community.TagManager.Controllers
{
    [PluginController(StringConstants.PluginAlias)]
    [Tree(StringConstants.SectionAlias, StringConstants.TreeAlias, TreeGroup = StringConstants.TreeGroup)]
    public class TagManagerTreeController(
        ILocalizedTextService localizedTextService,
        UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
        IEventAggregator eventAggregator,
        ITagManagerRepository tagManagerRepository,
        IMenuItemCollectionFactory menuItemCollectionFactory)
        : TreeController(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
    {
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory = menuItemCollectionFactory ?? throw new ArgumentNullException(nameof(menuItemCollectionFactory));

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                //top level nodes - generate list of tag groups.       
                foreach (var tagGroup in tagManagerRepository.GetTagGroups())
                {
                    var item = CreateTreeNode("tagGroup-" + tagGroup.Group, id, null, tagGroup.Group, "icon-tags", true, queryStrings.GetValue<string>("application"));
                    item.RoutePath = $"{StringConstants.SectionAlias}/{StringConstants.TreeAlias}/{StringConstants.DetailAction}/{tagGroup.Group}";
                    nodes.Add(item);
                }
            }
            else
            {
                var group = id.Substring(id.IndexOf('-') + 1);
                TagList cmsTags = tagManagerRepository.GetTagsByGroup(group);

                //List all tags under group
                foreach (var tag in cmsTags.TagsInGroup)
                {
                    var item = CreateTreeNode(tag.Id.ToString(), group, queryStrings, $"{tag.Tag}", "icon-tag", false);
                    nodes.Add(item);
                }
            }

            return nodes;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            var menu = _menuItemCollectionFactory.Create();

            bool idIsInteger = int.TryParse(id, out var idInt);

            menu.Items.Clear();

            // Add root menu item
            if (id == Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add(new MenuItem("create-group", "Create Tag Group")
                {
                    Icon = "icon-add",
                    OpensDialog = true,
                    SeparatorBefore = true,
                    UseLegacyIcon = false
                });
            }

            // Add tag group menu items
            if (id.Contains("tagGroup-"))
            {
                menu.Items.Add(new MenuItem("create", "Create Tag")
                {
                    Icon = "icon-add", // Set the icon to "add"
                    OpensDialog = true,
                    SeparatorBefore = true, // Add a separator before this menu item
                    UseLegacyIcon = false
                });
                menu.Items.Add(new RefreshNode(LocalizedTextService, false));
            }

            // Add children menu items
            if (idIsInteger)
            {
                if (idInt > 0)
                {
                    menu.Items.Add(new MenuItem("delete", "Delete Tag")
                    {
                        Icon = "icon-delete",
                        OpensDialog = true,
                        SeparatorBefore = true,
                        UseLegacyIcon = false,
                    });
                }
            }

            return menu;
        }
    }
}