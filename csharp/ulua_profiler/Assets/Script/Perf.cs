#if !PG_DISABLE_PERF
#   define PG_ENABLE_PERF
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ULUA
{
	public struct Perf : System.IDisposable
    {
#if UNITY_EDITOR_WIN
		[DllImport("ulua")]
		static extern long ULUATool_cpu_counter();
		[DllImport("ulua")]
		static extern long ULUATool_cpu_freq();
		public static long freq = ULUATool_cpu_freq();

		public static long GetTick()
		{
			return ULUATool_cpu_counter();
		}
#else
		public static long freq = System.Diagnostics.Stopwatch.Frequency;

		public static long GetTick()
		{
			return System.Diagnostics.Stopwatch.GetTimestamp();
		}
#endif
		public static double cpu_factor = 1000000000.0 / freq;
    }


}
