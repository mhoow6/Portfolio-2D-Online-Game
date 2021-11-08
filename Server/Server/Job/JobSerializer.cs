using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    // Job을 일렬로 나열해서 실행시키는 클래스
    public class JobSerializer
    {
        JobTimer _timer = new JobTimer(); // 미래에 실행해야 하는 일들을 시간관리
        Queue<IJob> _jobQueue = new Queue<IJob>(); // 당장 실행해야할 일들
        object _lock = new object();

		#region 실행해야할 일들을 큐 또는 JobTimer에 넣기
		// 그냥 Job을 안 만들고 Action으로 건네 주고 싶을 경우
		public void Push(Action action)
		{
			Push(new Job(action));
		}

		public void Push<T1>(Action<T1> action, T1 t1)
		{
			Push(new Job<T1>(action, t1));
		}

		public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
		{
			Push(new Job<T1, T2>(action, t1, t2));
		}

		public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
		{
			Push(new Job<T1, T2, T3>(action, t1, t2, t3));
		}

		// 그냥 Job을 안 만들고 Action으로 건네 주고 싶을 경우
		public void PushAfter(int tickAfter, Action action)
		{
			PushAfter(tickAfter, new Job(action));
		}

		public void PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1)
		{
			PushAfter(tickAfter, new Job<T1>(action, t1));
		}

		public void PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2)
		{
			PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2));
		}

		public void PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
		{
			PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3));
		}
		#endregion

		// 큐에 넣고 실행을 나중에 하길 기대함
		public void Push(IJob job)
        {
			lock (_lock)
            {
				_jobQueue.Enqueue(job);
            }
        }

		// 타이머에 넣고 실행을 나중에 하길 기대함
		public void PushAfter(int tickAfter, IJob job)
		{
			lock (_lock)
			{
				_timer.Push(job, tickAfter);
			}
		}

		// 누군가가 호출하여 일감을 한 번에 처리한다.
		public void Flush()
		{
			_timer.Flush();

			while (true)
			{
				IJob action = Pop();
				if (action == null)
					return;

				action.Execute();
			}
		}

		IJob Pop()
		{
			lock (_lock)
			{
				if (_jobQueue.Count == 0)
				{
					return null;
				}
				return _jobQueue.Dequeue();
			}
		}
	}
}
