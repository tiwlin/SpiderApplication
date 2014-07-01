using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace CommonHelper
{
	public class RecordHandler : ICallHandler
	{
		/// <summary>
		/// 执行顺序
		/// </summary>
		public int Order { get; set; }
		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			string strLog = input.MethodBase.Name;

			for (var i = 0; i < input.Arguments.Count; i++)
			{
				//LogHelper.Log.Info(string.Format("{0}: {1}", input.Arguments.ParameterName(i), input.Arguments[i]));

				strLog += "\n" + string.Format("参数{0}: {1}", input.Arguments.ParameterName(i), input.Arguments[i]);
			}

			var retvalue = getNext()(input, getNext);
			watch.Stop();

			strLog += string.Format("总耗时：{0}", watch.ElapsedMilliseconds);

			LogHelper.Log.Info(strLog);

			return retvalue;
		}
	}

	public class RecordHandlerAttribute : HandlerAttribute
	{
		public override ICallHandler CreateHandler(IUnityContainer container)
		{
			return new RecordHandler();
		}
	}

	public class TimerRecord
	{
		public static void Initialize()
		{
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			Time("", 1, () => { });
		}

		public static void LogRecordTimeMessage(string methodName,string desc, Action action)
		{
			Dictionary<string, string> dctParams = new Dictionary<string, string>();
			dctParams.Add("方法名:{0}", methodName);
			dctParams.Add("描述:{0}", desc);
			LogRecordTimeMessage(dctParams, action);
		}

		public static void LogRecordTimeMessage(IDictionary<string,string> dctParams, Action action)
		{
			if (dctParams == null || dctParams.Count == 0)
			{
				return;
			}

			string message = string.Empty;

			foreach (KeyValuePair<string, string> kp in dctParams)
			{
				message += kp.Key + kp.Value + "\n";
			}

			Stopwatch watch = new Stopwatch();
			watch.Start();
			action();
			watch.Stop();
			message += string.Format("总耗时：{0}", watch.ElapsedMilliseconds);

			LogHelper.Log.Info(message);
		}

		public static void Time(string name, int iteration, Action action)
		{
			if (string.IsNullOrEmpty(name)) return;

			ConsoleColor currentForeColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(name);

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
			int[] gcCounts = new int[GC.MaxGeneration + 1];
			for (int i = 0; i <= GC.MaxGeneration; i++)
			{
				gcCounts[i] = GC.CollectionCount(i);
			}

			Stopwatch watch = new Stopwatch();
			watch.Start();
			//ulong cycleCount = GetCycleCount();
			long ticksFst = GetCurrentThreadTimes();
			for (int i = 0; i < iteration; i++) action();
			//ulong cpuCycles = GetCycleCount() - cycleCount;
			long ticks = GetCurrentThreadTimes() - ticksFst;
			watch.Stop();

			Console.ForegroundColor = currentForeColor;
			Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
			//Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));
			Console.WriteLine("\tTime Elapsed (one time):" +
			   (watch.ElapsedMilliseconds / iteration).ToString("N0") + "ms");

			Console.WriteLine("\tCPU time:\t\t" + (ticks * 100).ToString("N0")
			   + "ns");
			Console.WriteLine("\tCPU time (one time):\t" + (ticks * 100 /
			   iteration).ToString("N0") + "ns");

			for (int i = 0; i <= GC.MaxGeneration; i++)
			{
				int count = GC.CollectionCount(i) - gcCounts[i];
				Console.WriteLine("\tGen " + i + ": \t\t" + count);
			}

			Console.WriteLine();
		}

		private static ulong GetCycleCount()
		{
			ulong cycleCount = 0;
			QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
			return cycleCount;
		}

		private static long GetCurrentThreadTimes()
		{
			long l;
			long kernelTime, userTimer;
			GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime, out userTimer);

			return kernelTime + userTimer;
		}

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetCurrentThread();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);
	}
}
