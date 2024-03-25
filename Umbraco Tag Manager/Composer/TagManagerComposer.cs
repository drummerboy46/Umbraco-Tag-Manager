using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.Community.TagManager.Repositories;
using Our.Umbraco.Community.TagManager.Repositories.Implementation;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.Community.TagManager.Composer;

public class TagManagerComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Sections().Append<TagManagerSection>();
        builder.Services.AddScoped<ITagManagerRepository, TagManagerRepository>();
    }
}