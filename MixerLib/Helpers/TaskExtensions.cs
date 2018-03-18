using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MixerLib.Helpers
{
	internal static class TaskHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Forget(this Task task)
		{
			// Empty on purpose!
		}

		private const int DEFAULT_TIMEOUT = 5000;

		public static Task OrTimeout(this Task task, int milliseconds = DEFAULT_TIMEOUT)
		{
			return OrTimeout(task, new TimeSpan(0, 0, 0, 0, milliseconds));
		}

		public static async Task OrTimeout(this Task task, TimeSpan timeout)
		{
			var completed = await Task.WhenAny(task, Task.Delay(timeout));
			if (completed != task)
			{
				throw new TimeoutException();
			}

			await task;
		}

		public static Task<T> OrTimeout<T>(this Task<T> task, int milliseconds = DEFAULT_TIMEOUT)
		{
			return OrTimeout(task, new TimeSpan(0, 0, 0, 0, milliseconds));
		}

		public static async Task<T> OrTimeout<T>(this Task<T> task, TimeSpan timeout)
		{
			var completed = await Task.WhenAny(task, Task.Delay(timeout));
			if (completed != task)
			{
				throw new TimeoutException();
			}

			return await task;
		}
	}
}
