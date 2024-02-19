namespace Assignment1;

// Class used to replace Console.Write() and Console.WriteLine() and add some interesting functionality
internal class ConsoleUtils {
	private static int currentLine = 0;

	private ConsoleUtils() {
	}

	// Clear all content on console and set currentLine = 0
	public static void ClearAll() {
		currentLine = 0;
		Console.SetCursorPosition(0, 0);
		Console.Write(new String(' ', Console.BufferWidth * Console.BufferHeight));
		Console.SetCursorPosition(0, 0);
	}

	// Clear the specified line
	private static void ClearLine(int line) {
		Console.SetCursorPosition(0, line);
		Console.Write(new String(' ', Console.BufferWidth));
		Console.SetCursorPosition(0, line);
	}

	public static int CurrentLine() {
		return currentLine;
	}

	// Write content to the console on the currentLine
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

	//	Write content to the console on the currentLine and add line break at end
	public static void WriteLine(params FormattedText[] text) {
		Write(text);
		Console.WriteLine();
		currentLine++;
	}

	//	Write content to the console on `line` and add line
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

	//	Write content to the console on `line` and add line break at end
	public static void WriteLineAt(int line, params FormattedText[] text) {
		WriteAt(line, text);
		Console.WriteLine();
	}
}

// struct is used to format text with the desired color
public readonly struct FormattedText(object text, ConsoleColor? color) {
	public readonly object text = text;
	public readonly ConsoleColor? color = color;
}

// Extension to provide Format() method to get FormattedText easily for every object
public static class Formatter {

	public static FormattedText Format(this object text) {
		return new FormattedText(text, null);
	}

	public static FormattedText Format(this object text, ConsoleColor color) {
		return new FormattedText(text, color);
	}
}