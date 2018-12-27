using System;
using System.Linq;
using System.Collections.Concurrent;
using System.IO;

namespace ChineseAbs.ABSManagement.ResourcePool
{
    public static class ResourcePool
    {
        public static Resource RegisterMemoryStream(string userName, string name, MemoryStream obj)
        {
            CollectResourse();

            var resource = new Resource();
            resource.Guid = Guid.NewGuid();
            resource.UserName = userName;
            resource.Name = name;
            resource.Instance = obj;
            resource.TimeStamp = DateTime.Now;
            resource.Type = ResourceType.MemoryStream;

            lock(m_lockObj)
            {
                m_resources[resource.Guid] = resource;
            }

            return resource;
        }

        public static Resource RegisterFilePath(string userName, string name, string filePath)
        {
            CollectResourse();

            var resource = new Resource();
            resource.Guid = Guid.NewGuid();
            resource.UserName = userName;
            resource.Name = name;
            resource.Instance = filePath;
            resource.TimeStamp = DateTime.Now;
            resource.Type = ResourceType.FilePath;

            lock (m_lockObj)
            {
                m_resources[resource.Guid] = resource;
            }

            return resource;
        }

        public static Resource Release(string userName, Guid guid)
        {
            CollectResourse();

            lock(m_lockObj)
            {
                Resource resource;
                m_resources.TryRemove(guid, out resource);
                if (resource != null
                    && userName.Equals(resource.UserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return resource;
                }
            }

            return null;
        }

        private static void CollectResourse()
        {
            lock(m_lockObj)
            {
                var now = DateTime.Now;
                var expiredKeys = m_resources.Where(x => (now - x.Value.TimeStamp).TotalMilliseconds > m_expiredTime)
                    .Select(x => x.Key);

                Resource resource;
                expiredKeys.ToList().ForEach(x => m_resources.TryRemove(x, out resource));
            }
        }

        private static object m_lockObj = new object();

        //设置生成文件在内存中缓存时间为2分钟，过期则在下次触发CollectResourse时回收
        private static readonly int m_expiredTime = 1000 * 60 * 2;

        private static ConcurrentDictionary<Guid, Resource> m_resources = new ConcurrentDictionary<Guid, Resource>();
    }

    public class Resource
    {
        public string Name { get; set; }

        public Guid Guid { get; set; }

        public DateTime TimeStamp { get; set; }

        public ResourceType Type { get; set; }

        public string UserName { get; set; }

        public object Instance { get; set; }
    }

    public enum ResourceType
    {
        Undefined = 0,
        MemoryStream = 1,
        FilePath = 2
    }
}
