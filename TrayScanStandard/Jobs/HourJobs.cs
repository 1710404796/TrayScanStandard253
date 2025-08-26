using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace TrayScanStandard.Jobs
{
    internal class HourJobs() : IJob
    {
        /// <summary>
        /// 每日执行任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("11111111111111");


            var files = new DirectoryInfo("Data2D").GetFiles().OrderByDescending(s => s.CreationTime).ToArray();

            

            foreach (var file in files.Skip(500))
            {
                File.Delete(file.FullName);

            }
            foreach (var file in files.Take(500))
            {
                if (DateTime.Now - file.CreationTime > TimeSpan.FromDays(MainStorage.Saves.LogDeleteDay))
                {
                    File.Delete(file.FullName);
                }
            }


           var dirs = new DirectoryInfo("DataFrame").GetDirectories().OrderByDescending(s => s.CreationTime).ToArray();
            foreach (var dir in dirs.Skip(500))
            {
             
                    Directory.Delete(dir.FullName, true);
            }
            foreach (var dir in dirs.Take(500))
            {
                if (DateTime.Now - dir.CreationTime > TimeSpan.FromDays(MainStorage.Saves.LogDeleteDay))
                {
                    Directory.Delete(dir.FullName, true);
                }
            }
            return Task.CompletedTask;
        }
    }
}
