using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Models
{
    public class TemplateTaskContainer : List<TemplateTask>
    {
        public TemplateTaskContainer()
        {
        }

        public TemplateTaskContainer(List<TemplateTask> templateTasks)
        {
            AddRange(templateTasks);
        }

        public TemplateTaskContainer SortByDependency()
        {
            if (Count == 0)
            {
                return this;
            }

            //排序后的id
            List<int> sortedIds = new List<int>();
            var allIds = ConvertAll(x => x.TemplateTaskId);
            while (sortedIds.Count < Count)
            {
                var unsortedIds = allIds.Except(sortedIds).ToList();
                var unsortedTasks = this.Where(x => unsortedIds.Contains(x.TemplateTaskId)).ToList();
                var topTasks = unsortedTasks.Where(x => !(x.PrevIds.Intersect(unsortedIds).GetEnumerator().MoveNext())).ToList();
                //未排序的Task中，处于顶层的id（对其它task没有依赖）
                var topIds = topTasks.ConvertAll(x => x.TemplateTaskId);
                if (topIds.Count == 0)
                {
                    throw new ApplicationException("根据依赖关系排序TemplateTask失败！");
                }

                sortedIds.AddRange(topIds);
            }

            TemplateTaskContainer container = new TemplateTaskContainer();
            foreach (var id in sortedIds)
            {
                container.Add(this.First(x => x.TemplateTaskId == id));
            }

            return container;
        }
    }
}
