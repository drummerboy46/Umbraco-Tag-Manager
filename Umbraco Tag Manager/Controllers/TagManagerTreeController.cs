using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
using Umbraco.Community.TagManager.Repositories;
using Constants = Umbraco.Cms.Core.Constants;
using Umbraco.Community.TagManager.Models;
using Umbraco.Cms.Core.Actions;

namespace Umbraco.Community.TagManager.Controllers
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
            if (id == Constants.System.Root.ToInvariantString())
            {
                //top level nodes - generate list of tag groups that this user has access to.       
                var tree = new TreeNodeCollection();
                foreach (var tagGroup in tagManagerRepository.GetTagGroups())
                {
                    var item = CreateTreeNode("tagGroup-" + tagGroup.Group, id, null, tagGroup.Group, "icon-tags", true, queryStrings.GetValue<string>("application"));
                    item.RoutePath = $"{StringConstants.SectionAlias}/{StringConstants.TreeAlias}/{StringConstants.DetailAction}/{tagGroup.Group}";
                    tree.Add(item);
                }

                return tree;
            }
            else
            {
                //List all tags under group
                var group = id.Substring(id.IndexOf('-') + 1);
                var tree = new TreeNodeCollection();
                TagList cmsTags = tagManagerRepository.GetTagsByGroup(group);

                foreach (var tag in cmsTags.TagsInGroup)
                {
                    var item = CreateTreeNode(tag.Id.ToString(), group, queryStrings, $"{tag.Tag}", "icon-tag", false);
                    tree.Add(item);
                }

                return tree;
            }
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            var menu = _menuItemCollectionFactory.Create();
            menu.Items.Clear();

            bool idIsInteger = int.TryParse(id, out var idInt);

            if (idIsInteger)
            {
                if (idInt > 0)
                {
                    menu.Items.Add(new MenuItem("delete", "Delete Tag")
                    {
                        Icon = "delete",
                        SeparatorBefore = true,
                    });
                }
                else
                {
                    menu.Items.Add(new MenuItem("create-group", "Create Tag Group")
                    {
                        Icon = "add",
                        SeparatorBefore = true,
                    });
                }
            }

            if (id.Contains("tagGroup-"))
            {
                // If the node is a tag group, add the "Create" option
                menu.Items.Add(new MenuItem("create", "Create Tag")
                {
                    Icon = "add", // Set the icon to "add"
                    SeparatorBefore = true, // Add a separator before this menu item
                });
                menu.Items.Add(new RefreshNode(LocalizedTextService, false));
            }

            return menu;
        }
    }
}