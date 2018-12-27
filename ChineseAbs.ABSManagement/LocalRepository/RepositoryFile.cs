using ChineseAbs.ABSManagement.LocalRepository.Core;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.LocalRepository
{
    public class RepositoryFile : RepositoryElement
    {
        protected override string GetRepositoryRootPath()
        {
            return WebConfigUtils.RepositoryFilePath;
        }

        public string GetFilePath()
        {
            return GetAbsoluteFilePath();
        }
    }
}
