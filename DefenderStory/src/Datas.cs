﻿using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace TakeUpJewel.Data
{
	/// <summary>
	///     レベルに関する情報を表します。
	/// </summary>
	[DataContract]
	public class LevelData
	{
		/// <summary>
		///     このレベルの最初のエリア番号を取得します。
		/// </summary>
		[DataMember]
		public int FirstArea { get; set; }

		/// <summary>
		///     このレベルに設定された時間を取得します。
		/// </summary>
		[DataMember]
		public int Time { get; set; }

		/// <summary>
		///     このレベルの説明を取得します。
		/// </summary>
		[DataMember]
		public string Desc { get; set; }
	}

	/// <summary>
	///     エリアに関する情報を表します。
	/// </summary>
	public class AreaInfo
	{
		/// <summary>
		///     このエリアが使用するマップチップ名を取得します。
		/// </summary>
		public string Mpt { get; set; }

		/// <summary>
		///     このエリアの BGM 名を取得します。
		/// </summary>
		public string Music { get; set; }

		/// <summary>
		///     このエリアの BG 画像名を取得します。
		/// </summary>
		public string Bg { get; set; }

		/// <summary>
		///     このエリアの BG のスクロール速度を取得します。
		/// </summary>
		public int ScrollSpeed { get; set; }

		/// <summary>
		///     このエリアの FG 画像名を取得します。
		/// </summary>
		public string Fg { get; set; }

		/// <summary>
		///     このエリアの FG のスクロール速度を取得します。
		/// </summary>
		public int FgScrollSpeed { get; set; }
	}

	/// <summary>
	///     ベクトルを表します。
	/// </summary>
	public struct Vector
	{
		public static Vector Zero = new Vector(0, 0);

		public float X { get; set; }
		public float Y { get; set; }

		public float Direction => (float) Math.Atan2(Y, X);

		public void SetValueByVelocity(float direction, float speed)
		{
			X = (float) Math.Cos(direction) * speed;
			Y = (float) Math.Sin(direction) * speed;
		}

		public Vector(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static PointF operator +(PointF a, Vector b)
		{
			var p = new PointF();
			p.X = a.X + b.X;
			p.Y = a.Y + b.Y;
			return p;
		}

		public static Vector operator *(Vector v, float f) => new Vector(v.X * f, v.Y * f);

		public static Vector operator /(Vector v, float f) => new Vector(v.X / f, v.Y / f);

		public static PointF operator -(PointF a, Vector b)
		{
			var p = new PointF();
			p.X = a.X - b.X;
			p.Y = a.Y - b.Y;
			return p;
		}
	}
}