using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace rseuHack
{
    class QueueController
    {
        static QueueController contr;
        Authorization auth = Authorization.GetAuthorization();
        private QueueController()
        {
            new Thread(() => {
                Proceed();
            }).Start();
        }

        public static QueueController GetQueueController()
        {
            if (contr == null)
            {
                contr = new QueueController();
            }
            return contr;
        }

        static List<QueueElement> queue = new List<QueueElement>();

        public void Add(long userId, string type, string fileid) {
            queue.Add(new QueueElement() { userId = userId, fileType = type, fileid = fileid });
        }

        private async void Proceed()
        {
            while (true)
            {
                if (queue.Count>0 && queue.First() != null)
                {
                    var cur = queue.First();
                    await Program.GetFile(cur.userId, cur.fileid);
                    await auth.Magic(Program.filename,cur.fileType ,cur.userId);
                    queue.Remove(cur);
                }
                Thread.Sleep(2000);
            }
        }

        public int[] GetQueues(long userId)
        {
            var all = queue.FindAll(x => x.userId == userId);
            int[] res = new int[all.Count];
            for(int i = 0; i < all.Count; i++)
            {
                res[i] = queue.IndexOf(all[i]);
            }
            return res;
        }

    }

    

    class QueueElement
    {
        public long userId;
        public string fileType;
        public string fileid;
    }
}
