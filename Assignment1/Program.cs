
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Assignment1;

class Program {
    static void Main(string[] args) {
        Console.Title = "Water Fee Calculator";
        Function();
    }

    static void Function() {
        string username = GetUsername(0);
        CustomerType customer = GetCustomerType(2);
        double lastMonth = GetWaterUse(4, "Amount of water used last month: ");
        double thisMonth = GetWaterUse(5, "Amount of water used last month: ");

    }

    private static string GetUsername(int line) {
        string? username = null;

        while (username == null) {
            Regex regex = new(@"^[a-zA-Z\s]*$");
            ConsoleUtils.WriteAt(line, "Please enter name: ");
            string? temp = Console.ReadLine();
            if (temp != null && temp.Length > 0) {
                if (regex.IsMatch(temp)) {
                    username = temp;
                    ConsoleUtils.WriteWithColorAt(line, FormatText.Of("Username: ", null), FormatText.Of(username, ConsoleColor.Green));
                } else {
                    ConsoleUtils.WriteWithColorAt(line, FormatText.Of("Only allow character and whitespace", ConsoleColor.Red));
                    Console.ReadKey();
                }
            }
        }
        return username;
    }

    private static CustomerType GetCustomerType(int line) {
        CustomerType[] types = (CustomerType[]) Enum.GetValues(typeof(CustomerType));
        CustomerType type;
        int selected = 0;
    readInput:
        List<FormatText> pairs = types
            .ToList()
            .ToDictionary(x => x, x => x == types[selected] ? ConsoleColor.Green : ConsoleColor.Red)
            .Select(x => new FormatText(x.Key + " ", x.Value))
            .ToList();
        ConsoleUtils.WriteLineAt(line, "Type\t(Tab: Next, Enter: Select)");
        ConsoleUtils.WriteWithColorAt(line + 1, [.. pairs, FormatText.Of($"({types[selected].GetDescription()})", null)]);

        ConsoleKey key = Console.ReadKey().Key;
        Console.Write("\b \b");
        if (key == ConsoleKey.Tab) {
            if (selected < types.Length - 1) {
                selected++;
            } else {
                selected = 0;
            }

            goto readInput;
        } else {
            if (key != ConsoleKey.Enter) {
                goto readInput;
            }
        }

        try {
            type = types[selected];
            ConsoleUtils.WriteWithColorAt(line, FormatText.Of("Type: ", null), FormatText.Of(type, ConsoleColor.Green));
            ConsoleUtils.ClearLine(line + 1);
        } catch (Exception e) {
            goto readInput;
        }
        return type;
    }

    private static double GetWaterUse(int line, string message) {
        double amount = 0.0;

        return amount;
    }

    private static void Sleep(int milis) {
        Thread.Sleep(milis);
    }
}

enum CustomerType {
    Household,
    PublicService,
    Production
}

static class CustomerTypeDescription {
    public static String GetDescription(this CustomerType type) {
        return type switch {
            CustomerType.Household => "Household customer",
            CustomerType.PublicService => "Administrative agency, public services",
            CustomerType.Production => "Production units",
            _ => "Unknown",
        };
    }
}