using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TakeUpJewel
{
	public class EventScript
	{
		public enum Token
		{
			LeftBrace,
			Name,
			Coron,
			Args,
			RightBrace
		}

		private readonly List<Command> _script;

		private int _ptr;

		public EventScript(string script)
		{
			_script = string.IsNullOrEmpty(script) ? new List<Command>() : ParseCommand(script);
		}

		public int ProgramCounter
		{
			get { return _ptr; }
			set
			{
				if (value < 0)
					_ptr = 0;
				else if ((_script != null) && (value >= _script.Count))
					_ptr = _script.Count - 1;
				else
					_ptr = value;
			}
		}

		public Command Current => _script.Count == 0 ? null : _script?[ProgramCounter];

		public bool IsEndOfScript => ProgramCounter == _script?.Count - 1;


		public static List<Command> ParseCommand(string cs)
		{
			if (string.IsNullOrEmpty(cs))
				return null;
			var scpt = new List<Command>();
			string name = null;
			List<string> args = null;

			var buffer = "";
			var quotFlag = false;
			var token = Token.LeftBrace;
			for (var i = 0; i < cs.Length; i++)
			{
				var c = cs[i];
				var cm1 = i > 0 ? cs[i - 1] : '\0';
				var cp1 = i < cs.Length - 1 ? cs[i + 1] : '\0';
				if ((token != Token.Args) && (char.IsControl(c) || char.IsSeparator(c) || char.IsWhiteSpace(c)))
					continue;
				switch (token)
				{
					case Token.LeftBrace:
						buffer = "";
						name = "";
						token = Token.Name;
						args = null;
						break;
					case Token.Name:
						name += c;
						switch (cp1)
						{
							case ':':
								token = Token.Coron;
								break;
							case ']':
								token = Token.RightBrace;
								break;
						}
						break;
					case Token.Coron:
						args = new List<string>();
						token = Token.Args;
						break;
					case Token.Args:

						switch (c)
						{
							case '\\':
								switch (cp1)
								{
									case 'n':
										buffer = buffer + '\n';
										break;
									case 'l':
										buffer = buffer + (GameEngine.CurrentGender == PlayerGender.Male ? "アレン" : "ルーシィ");
										break;
									case '"':
										buffer = buffer + '"';
										break;
									case '\\':
										buffer = buffer + '\\';
										break;
									default:
										throw new EventScriptException($@"Script Error: Invalid escape sequence \{cp1}");
								}
								i++;
								break;
							case '"':
								quotFlag = !quotFlag;
								break;
							default:

								if ((c == ',') && !quotFlag)
								{
									args?.Add(buffer);
									buffer = "";
								}
								else
								{
									buffer = buffer + c;
								}


								break;
						}
						if ((cp1 == ']') && !quotFlag)
							token = Token.RightBrace;
						break;
					case Token.RightBrace:
						args?.Add(buffer);
						scpt.Add(new Command
						{
							Name = name?.ToLower(),
							Args = args?.ToArray()
						});
						token = Token.LeftBrace;
						break;
					default:
						throw new EventScriptException($@"Internal Error: Unknown token ""{token}""");
				}
			}
			if (quotFlag)
				throw new EventScriptException("Script Error: Unmatched Double Quotation");
			if (token != Token.LeftBrace)
				throw new EventScriptException($@"Script Error: Unexpected token  ""{token}"" at the end of scripts");
			return scpt;
		}

		[Serializable]
		public class EventScriptException : Exception
		{
			//
			// For guidelines regarding the creation of new exception types, see
			//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
			// and
			//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
			//

			public EventScriptException()
			{
			}

			public EventScriptException(string message) : base(message)
			{
			}

			public EventScriptException(string message, Exception inner) : base(message, inner)
			{
			}

			protected EventScriptException(
				SerializationInfo info,
				StreamingContext context) : base(info, context)
			{
			}
		}

		public class Command
		{
			public string Name { get; set; }
			public string[] Args { get; set; }
		}
	}
}