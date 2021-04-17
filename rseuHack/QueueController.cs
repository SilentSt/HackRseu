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

        public void Add(long userId, string type) {
            queue.Add(new QueueElement() { userId = userId, fileType = type });
        }

        private async void Proceed()
        {
            while (true)
            {
                if (queue.Count>0 && queue.First() != null)
                {
                    var cur = queue.First();
                    Program.GetFile(cur.userId);
                    switch (cur.fileType)
                    {
                        case "PDF":
                            await auth.PDF(Program.document.FileName, cur.userId);
                            break;
                        case "RTF":
                            await auth.RTF(Program.document.FileName, cur.userId);
                            break;
                        case "DOCX":
                            await auth.DOCX(Program.document.FileName, cur.userId);
                            break;
                        case "SVG":
                            await auth.SVG(Program.document.FileName, cur.userId);
                            break;
                    }
                    queue.Remove(cur);
                }
                Thread.Sleep(20);
            }
        }
    }

    

    class QueueElement
    {
        public long userId;
        public string fileType;
    }
}
