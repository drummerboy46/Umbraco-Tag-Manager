using Umbraco.Cms.Core.Sections;

namespace Umbraco.Community.TagManager
{
	public class TagManagerSection : ISection
	{
		public string Alias => StringConstants.SectionAlias;

		public string Name => StringConstants.SectionName;
	}
}