using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.Community.TagManager.Models;
using Our.Umbraco.Community.TagManager.Repositories;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
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
    public class TagManagerTreeController : TreeController
    {
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;
        private readonly ITagManagerRepository _tagManagerRepository;
        private readonly ILocalizationService _localizationService;

        public TagManagerTreeController(ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IEventAggregator eventAggregator,
            ITagManagerRepository tagManagerRepository,
            IMenuItemCollectionFactory menuItemCollectionFactory,
            ILocalizationService localizationService) : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _tagManagerRepository = tagManagerRepository;
            _localizationService = localizationService;
            _menuItemCollectionFactory = menuItemCollectionFactory ?? throw new ArgumentNullException(nameof(menuItemCollectionFactory));
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                //top level nodes - generate list of tag groups.       
                foreach (var tagGroup in _tagManagerRepository.GetTagGroups())
                {
                    var item = CreateTreeNode("tagGroup-" + tagGroup.Group, id, null, tagGroup.Group, "icon-tags", true, queryStrings.GetValue<string>("application"));
                    item.RoutePath = $"{StringConstants.SectionAlias}/{StringConstants.TreeAlias}/{StringConstants.DetailAction}/{tagGroup.Group}";
                    nodes.Add(item);
                }
            }
            else
            {
                var group = id.Substring(id.IndexOf('-') + 1);
                TagList cmsTags = _tagManagerRepository.GetTagsByGroup(group);
                IEnumerable<ILanguage> languages = _localizationService.GetAllLanguages().ToList();

                //List all tags under group
                foreach (var tag in cmsTags.TagsInGroup)
                {

                    string isoCode = tag.LanguageId != null ? languages.Where(x => x.Id == tag.LanguageId).Select(x => x.CultureName).FirstOrDefault() : "Invariant";
                    var item = CreateTreeNode(tag.Id.ToString(), group, queryStrings, $"{tag.Tag} - {isoCode}", "icon-tag", false);
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