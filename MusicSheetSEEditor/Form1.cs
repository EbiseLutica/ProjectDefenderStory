using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicSheetSEEditor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			
		}

		int hztemp = 440;

		SoundData[] sound = new SoundData[4];

		

		public bool wavesetrequest = false;

		public ushort[] wavedata = new ushort[1];

		

		/// <summary>
		/// 一周期分の波形データ、周波数、パンポットを指定して、波形データを作成します。
		/// </summary>
		/// <param name="wave">32個の波形データ。</param>
		/// <param name="hz">周波数。</param>
		/// <param name="pan">-100 ～ +100 の範囲で、パンポット。</param>
		/// <returns>生成されたサウンドバッファーのハンドル。</returns>
		public static ushort[] CreateSquare(int hz, int vol)
		{
			int length = 0;
			for (int i = 0; i < 44100; i++)
				if (Math.PI * 2 / 44100 * i * hz * 180 / Math.PI >= 360)
				{
					length = i;
					break;
				}
			float tL = 0;
			float tR = 0;
			float t = 0;
			ushort[] y = new ushort[length];
			for (int i = 0; i < length; i++)
			{
				t = (int)((int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / 180) * 65535) * (vol / 100.0f);
				y[i] = (ushort)t;
			}


			return y;
		}

		public static ushort[] CreateSaw(int hz, int vol)
		{
			int length = 0;
			for (int i = 0; i < 44100; i++)
				if (Math.PI * 2 / 44100 * i * hz * 180 / Math.PI >= 360)
				{
					length = i;
					break;
				}
			float tL = 0;
			float tR = 0;
			float t = 0;
			ushort[] y = new ushort[length];
			for (int i = 0; i < length; i++)
			{
				t = (int)(((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360) * (65535 / 360)) * (vol / 100.0f);
				y[i] = (ushort)t;
			}


			return y;
		}

		public static float CreateSquarePiece(int hz, int i, int vol)
		{
			return (int)((int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / 180) * 65535) * (vol / 100.0f);
		}

		public static float CreateSawPiece(int hz, int i, int vol)
		{
			return (int)(((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360) * (65535 / 360) ) * (vol / 100.0f);
		}

	}

	public struct SoundData
	{
		public int A;
		public int D;
		public byte S;
		public int R;

		public int waveidx;
		public int freq;
		public int startidx;
		public int length;
		public bool enabled;
		public int pan;
	}

}
