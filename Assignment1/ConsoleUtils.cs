
namespace Assignment1;
internal class ConsoleUtils {
    public static void ClearLine(int line) {
        if (line >= Console.BufferHeight) {
            return;
        }

        Console.CursorLeft = 0;
        Console.CursorTop = line;

        Console.Write(new String(' ', Console.BufferWidth));
        Console.CursorLeft = 0;
        Console.CursorTop = line;
    }

    public static void WriteLineAt(int line, object text) {
        ClearLine(line);
        Console.WriteLine(text);
    }

    public static void WriteAt(int line, object text) {
        ClearLine(line);
        Console.Write(text);
    }

    public static void WriteWithColorAt(int line, params FormatText[] texts) {
        ClearLine(line);
        foreach (FormatText text in texts) {
            object st = text.text;
            ConsoleColor? color = text.color;
            if (color == null) {
                Console.ResetColor();
            } else {
                Console.ForegroundColor = color.Value;
            }
            Console.Write(st);
        }
        Console.ResetColor();
    }
}

struct FormatText {
    public object text;
    public ConsoleColor? color;

    private FormatText(object text, ConsoleColor? color) {
        this.text = text;
        this.color = color;
    }

    public static FormatText Of(object text, ConsoleColor? color) {
        return new FormatText(text, color);
    }
}
