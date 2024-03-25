using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.TagManager.Repositories;
using Umbraco.Community.TagManager.Repositories.Implementation;

namespace Umbraco.Community.TagManager.Composer;

public class TagManagerComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Sections().Append<TagManagerSection>();
        builder.Services.AddScoped<ITagManagerRepository, TagManagerRepository>();
    }
}