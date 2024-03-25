using Umbraco.Cms.Core.Sections;

namespace Our.Umbraco.Community.TagManager
{
	public class TagManagerSection : ISection
	{
		public string Alias => StringConstants.SectionAlias;

		public string Name => StringConstants.SectionName;
	}
}