using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Community.TagManager.Models;

namespace Umbraco.Community.TagManager.Repositories
{
    public interface ITagListRepository
    {
        string[] GetTagsByGroup(string group);
    }
}
