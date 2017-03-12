using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Text;

public class LuaBigTick : MonoBehaviour
{
	// min 50ms 
	public long minDumpMs = 33;
#if UNITY_EDITOR_WIN || UNITY_ANDROID
	[StructLayout(LayoutKind.Sequential)]
	protected struct Proto 
	{
		  public IntPtr next; 
		  public byte tt;
		  public byte marked;
		  public IntPtr k;  /* constants used by the function */
		  public IntPtr code;
		  public IntPtr p;  /* functions defined inside the function */
		  public IntPtr lineinfo;  /* map from opcodes to source lines */
		  public IntPtr locvars;  /* information about local variables */
		  public IntPtr upvalues;  /* upvalue names */
		  public IntPtr source;
		  public int sizeupvalues;
		  public int sizek;  /* size of `k' */
		  public int sizecode;
		  public int sizelineinfo;
		  public int sizep;  /* size of `p' */
		  public int sizelocvars;
		  public int linedefined;
		  public int lastlinedefined;
		  public IntPtr gclist;
		  public byte nups;  /* number of upvalues */
		  public byte numparams;
		  public byte is_vararg;
		  public byte maxstacksize;
		  // performance informations
		  public int perfcount;/* perf call count */
		  public UInt64 perftime;/*perf start time*/
		  public UInt64 perftotaltime;/*perf total time*/
		  public UInt64 perfcurrenttime;/* perf current time*/
		  public int perflevel;/* stack level */
		  public int perfcalltick;/* call tick */
		  public int perfmaxlevel;/* max stack level per frame*/
		  // add:O(1) delete:O(1) tick:O(n) search:O(n)
		  public IntPtr perflast;/* perf functions stacks */
		  public IntPtr perfnext;/* perf functions stacks */
		  public IntPtr perfcslast;/* call stacks last */
		  public IntPtr perfcsnext;/* call stacks next */
	}

	protected const int maxListLength = 400;
	protected const int maxTextLength = 1024;

	public static byte[] mTextCache;
	public static List<string> mFrameCache;
	public static int mLineCount;
	protected static int mBuilderLength = 0;

	protected static long mDeltaTick = 0;

	[HideInInspector]
	public long mLastTick = 0;
	protected System.IntPtr m_L = System.IntPtr.Zero;

	public static void LuaDumperer(IntPtr ptr, IntPtr textPtr)
	{
		if (++mLineCount >= maxListLength)
			return;

		var buffer = mTextCache;
		int count = 0;
		int plus = 0;
		for (int i = 0; i < maxTextLength; ++i)
		{
			byte b = Marshal.ReadByte(textPtr,i*sizeof(byte));
			if (b != '\0')
			{
				buffer[i+plus] = b;
				++count;
			}
			else
			{
				break;
			}
		}

		if(ptr == IntPtr.Zero)
		{
			buffer[count++] = (byte)'\n';
		}

		string text = System.Text.Encoding.UTF8.GetString(buffer, 0, count);
		if(ptr == IntPtr.Zero)
		{
			double delta = ((double)mDeltaTick * ULUA.Perf.cpu_factor) / 1000;
			ulong uDelta = (ulong)delta;
			text = string.Format("{0}{1}\r\n", text, uDelta);
			mFrameCache.Add(text);
			mBuilderLength += text.Length;
			return;
		}

		Proto proto = (Proto)Marshal.PtrToStructure(ptr, typeof(Proto));
		ulong perfcurrenttime = (ulong)(proto.perfcurrenttime * ULUA.Perf.cpu_factor) / 1000;
		ulong perftotaltime = (ulong)(proto.perftotaltime * ULUA.Perf.cpu_factor) / 1000;
		double percent = (double)proto.perftotaltime;
		mDeltaTick = mDeltaTick == 0 ? 1 : mDeltaTick;
		percent /= (double)(mDeltaTick);
		percent *= 100.0;
		string str = string.Format("{0}({1})\t{2}%\t{3}\t{4}\t{5}\t{6}\r\n", 
			text, proto.linedefined, percent.ToString("f4"),proto.perfcount, proto.perfmaxlevel, perfcurrenttime, perftotaltime);
		//mTextBuilder.Append(str);

		mBuilderLength += str.Length;
		mFrameCache.Add(str);
	}

	public static unsafe void LuaTimerFunc(IntPtr timer)
	{
		Marshal.WriteInt64(timer, ULUA.Perf.GetTick());
	}

	public static unsafe UInt64 LuaSecond(UInt64 marker)
	{
		return (UInt64)((UInt64)ULUA.Perf.GetTick() - marker);
	}

	protected virtual void SetDumper()
	{
		LuaDLL.lua_set_bigtick_dumper(m_L, LuaDumperer);
	}

	protected virtual void Init()
	{
		if(m_L!=System.IntPtr.Zero)
		{
			return;
		}
		m_L = LuaMgr.m_L;
		if (m_L == System.IntPtr.Zero)
			return;

		LuaDLL.lua_set_bigtick(m_L, 1);
		LuaDLL.lua_set_bigtick_timer(m_L, LuaTimerFunc, LuaSecond);
		SetDumper();

		mTextCache = new byte[maxTextLength];
		mFrameCache = new List<string>(maxListLength);
		mLastTick = ULUA.Perf.GetTick();
	}

	void Awake()
	{
		Init();
	}

	protected virtual void OnRelease()
	{
		var L = LuaMgr.m_L;
		if (L == System.IntPtr.Zero)
			return;
		LuaDLL.lua_set_bigtick(L, 0);
	}

	void OnDestroy()
	{
		OnRelease();
	}

	protected virtual void onUpdate()
	{
		if (m_L == System.IntPtr.Zero)
			return;

		LuaDLL.lua_update_bigtick(m_L);
		mLineCount = 0;
		mLastTick = ULUA.Perf.GetTick();
		mBuilderLength = 0;
	}

	protected virtual void OnPrintBigtick()
	{
		StringBuilder builder = new StringBuilder(mBuilderLength);
		for (int i = 0; i < mFrameCache.Count; ++i)
		{
			builder.Append(mFrameCache[i]);
		}
		PG.LogTool.LogWarning(builder.ToString());
		mFrameCache.Clear();
	}

	protected virtual void Tick()
	{
		if (m_L == IntPtr.Zero)
		{
			Init();
			return;
		}

		var tick = ULUA.Perf.GetTick();
		mDeltaTick = tick - mLastTick;
		if (mDeltaTick < (long)((double)(minDumpMs * 1000000.0) / ULUA.Perf.cpu_factor))
		{
			onUpdate();
			return;
		}

		LuaDLL.lua_dump_bigtick(m_L, IntPtr.Zero);
		OnPrintBigtick();
		onUpdate();
	}

	void Update()
	{
		Tick();
	}
#endif
}


