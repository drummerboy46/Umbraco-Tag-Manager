using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Packaging;

namespace Our.Umbraco.Community.TagManager.Migrations
{
    public class TagManagerMigrationHelper(
        IPackagingService packagingService,
        IMediaService mediaService,
        MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGenerators,
        IShortStringHelper shortStringHelper,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
        IMigrationContext context,
        IOptions<PackageMigrationSettings> packageMigrationsSettings)
        : PackageMigrationBase(packagingService, mediaService, mediaFileManager, mediaUrlGenerators, shortStringHelper,
            contentTypeBaseServiceProvider, context, packageMigrationsSettings)
    {
        private readonly string[] _userGroups = { "admin", "editor", "writer" };

        protected override void Migrate()
        {
            var dbContext = Context.Database;
            if (TableExists("umbracoUserGroup2App"))
            {
                foreach (var groupAlias in _userGroups)
                {
                    var groupId =
                        dbContext.ExecuteScalar<int?>("select id from umbracoUserGroup where userGroupAlias = @0",
                            groupAlias);
                    if (groupId.HasValue && groupId != 0)
                    {
                        var rows = dbContext
                            .ExecuteScalar<int>(
                                "select count(*) from umbracoUserGroup2App where userGroupId = @0 and app = @1",
                                groupId.Value, StringConstants.SectionAlias);
                        if (rows == 0)
                        {
                            dbContext.Execute("insert umbracoUserGroup2App values (@0, @1)", groupId,
                                StringConstants.SectionAlias);
                        }
                    }
                }
            }
        }



    }
}