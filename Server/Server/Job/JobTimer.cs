using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ServerCore;

namespace Server
{
    struct JobTimerNode : IComparable<JobTimerNode>
    {
        public int executeTick; // 실행시간
        public IJob job; // 처리해야할 일

        public int CompareTo(JobTimerNode other)
        {
            return other.executeTick - executeTick; // <0 일 경우 A.CompareTo(B)를 했을 때 A가 B보다 작음
        }
    }

    // Job을 언제 실행할 지 정해주는 클래스
    public class JobTimer
    {
        PriorityQueue<JobTimerNode> _priortiyQueue = new PriorityQueue<JobTimerNode>(); // 우선순위가 높은 아이가 반드시 맨 앞에 있는 트리구조
        object _lock = new object();

        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerNode node;
            node.executeTick = System.Environment.TickCount + tickAfter;
            node.job = job;

            lock (_lock)
            {
                _priortiyQueue.Push(node);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerNode jobNode;

                // 실행할 수 있는 일감은 실행
                lock (_lock)
                {
                    if (_priortiyQueue.Count == 0)
                        break;

                    jobNode = _priortiyQueue.Peek();
                    if (jobNode.executeTick > now) // 실행할 시간이 왔어~
                        break;

                    _priortiyQueue.Pop();
                }

                jobNode.job.Execute();
            }
        }
    }
}
