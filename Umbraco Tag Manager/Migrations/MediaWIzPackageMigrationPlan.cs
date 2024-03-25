using System;
using Umbraco.Cms.Core.Packaging;

namespace Umbraco.Community.TagManager.Migrations
{
    public class MediaWizPackageMigrationPlan() : PackageMigrationPlan("Umbraco TagManager")
    {
        protected override void DefinePlan()
        {
            From(String.Empty).To<TagManagerMigrationHelper>("UsomeTagManagerMigrationv1-db");;
        }
    }
}
