using System.Diagnostics;
using System.Reflection;

namespace Assignment1;
class ConsoleUtils {
	private static int currentLine = 0;
	private ConsoleUtils() { }

	private static void ClearLine(int line) {
		Console.SetCursorPosition(0, line);
		Console.Write(new String(' ', Console.BufferWidth));
		Console.SetCursorPosition(0, line);
	}

	public static int CurrentLine() {
		return currentLine;
	}

	public static void Write(params FormattedText[] text) {
		Console.CursorTop = currentLine;
		foreach (FormattedText item in text) {
			if (item.color != null) {
				Console.ForegroundColor = (ConsoleColor) item.color;
			} else {
				Console.ResetColor();
			}
			Console.Write(item.text);
		}
		Console.ResetColor();
	}
	public static void WriteLine(params FormattedText[] text) {
		Write(text);
		Console.WriteLine();
		currentLine++;
	}

	public static void WriteAt(int line, params FormattedText[] text) {
		ClearLine(line);
		if (line > currentLine) {
			currentLine = line;
		}

		foreach (FormattedText item in text) {
			if (item.color != null) {
				Console.ForegroundColor = (ConsoleColor) item.color;
			} else {
				Console.ResetColor();
			}
			Console.Write(item.text);
		}
		Console.ResetColor();
	}

	public static void WriteLineAt(int line, params FormattedText[] text) {
		WriteAt(line, text);
		Console.WriteLine();
	}
}

public struct FormattedText {
	public readonly object text;
	public readonly ConsoleColor? color;

	public static FormattedText Of(object text) {
		return new FormattedText(text, null);
	}

	public static FormattedText Of(object text, ConsoleColor color) {
		return new(text, color);
	}

	private FormattedText(object text, ConsoleColor? color) {
		this.text = text;
		this.color = color;
	}
}

public static class Formatter {
	public static FormattedText Format(this object text) {
		return FormattedText.Of(text);
	}

	public static FormattedText Format(this object text, ConsoleColor color) {
		return FormattedText.Of(text, color);
	}
}