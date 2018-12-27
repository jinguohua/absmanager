
namespace ChineseAbs.ABSManagement.Utils
{
    public class SysUtils
    {
        static public string GetDllVersion()
        {
            var assemblyLocation = typeof(CommUtils).Assembly.Location;
            var verInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation);
            var version = (verInfo.FilePrivatePart > 0 ? verInfo.FileVersion
                : string.Format("{0}.{1}.{2}", verInfo.FileMajorPart, verInfo.FileMinorPart, verInfo.FileBuildPart));
            return version;
        }
    }
}
