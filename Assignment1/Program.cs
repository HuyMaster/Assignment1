
using System.Collections.Immutable;
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
        double lastMonth = GetWaterUse(4, "Amount of water used last month: ", "Last month: ");
        double thisMonth = GetWaterUse(5, "Amount of water used this month: ", "This month: ");

        double lastMonthFee = customer.GetCalcMethod().Invoke(lastMonth);
        double thisMonthFee = customer.GetCalcMethod().Invoke(thisMonth);
        bool useMore = thisMonthFee > lastMonthFee;

        ConsoleUtils.WriteWithColorAt(7,
            FormatText.Of(String.Format("Last month: {0,11:0,0} VND", lastMonthFee), null));
        ConsoleUtils.WriteWithColorAt(8,
            FormatText.Of(String.Format("This month: {0,11:0,0} VND", thisMonthFee), null),
            FormatText.Of(String.Format(" ({0}{1:0,0} VND)", useMore ? "+" : "", thisMonthFee - lastMonthFee), useMore ? ConsoleColor.Red : ConsoleColor.Green));
        Console.ReadKey();
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
            .Select(x => FormatText.Of(x.Key + "  ", x.Value))
            .ToList();
        ConsoleUtils.WriteLineAt(line, "Type\t(Tab: Next, Enter: Select)");
        ConsoleUtils.WriteWithColorAt(line + 1, [.. pairs, FormatText.Of($"({types[selected].GetDescription()}) ", null)]);

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
        } catch (Exception) {
            goto readInput;
        }
        return type;
    }

    private static double GetWaterUse(int line, string message, string successText) {
        double amount = 0.0;

    input:
        ConsoleUtils.WriteAt(line, message);
        string? input = Console.ReadLine();
        if (input == null) {
            ConsoleUtils.WriteWithColorAt(line, FormatText.Of("Require input", ConsoleColor.Red));
            Console.ReadKey();
            goto input;
        } else {
            if (!double.TryParse(input, out amount)) {
                ConsoleUtils.WriteWithColorAt(line, FormatText.Of("Invalid number", ConsoleColor.Red));
                Console.ReadKey();
                goto input;
            } else {
                ConsoleUtils.WriteWithColorAt(line, FormatText.Of(successText, null), FormatText.Of(amount, ConsoleColor.Green));
            }
        }

        return amount;
    }

    private static void Sleep(int milis) {
        Thread.Sleep(milis);
    }
}

enum CustomerType {
    Household,
    PublicService,
    Production,
    Business
}

static class CustomerTypeDescription {
    public static String GetDescription(this CustomerType type) {
        return type switch {
            CustomerType.Household => "Household customer",
            CustomerType.PublicService => "Administrative agency, public services",
            CustomerType.Production => "Production units",
            CustomerType.Business => "Business service",
            _ => "Unknown",
        };
    }
}

static class CustomerFeeCalcMethod {
    private static double EnviromentProtectionFees(double price) {
        return price * 0.1;
    }
    private static double VAT(double fee) {
        return fee * 0.1;
    }

    public static Func<double, double> GetCalcMethod(this CustomerType type) {
        return type switch {
            CustomerType.Household => (Func<double, double>) (amount => {
                IList<double> price = ((double[]) [5973, 7052, 8699, 15929]).ToImmutableList();
                IList<double> epf = price.Select(x => x * 0.1).ToImmutableList();

                double[] group = new double[price.Count];

                double fee = 0.0;

                group[0] = Math.Min(amount, 10);
                amount -= group[0];
                group[1] = Math.Min(amount, 10);
                amount -= group[1];
                group[2] = Math.Min(amount, 10);
                amount -= group[2];
                group[3] = amount;
                amount -= group[3];

                if (amount == 0) {
                    for (int i = 0; i < group.Length; i++) {
                        double tempAmount = group[i];
                        fee += tempAmount * price.ElementAt(i);
                        fee += tempAmount * epf.ElementAt(i);
                    }
                }

                fee += VAT(fee);

                return fee;
            }
                ),
            CustomerType.PublicService => (Func<double, double>) (amount => {
                double price = 9955;
                double fee = 0.0;
                double epf = 995.5;

                fee += price * amount;
                fee += epf * amount;
                fee += VAT(fee);

                return fee;
            }),
            CustomerType.Production => (Func<double, double>) (amount => {
                double price = 11615;
                double fee = 0.0;
                double epf = 1161.5;

                fee += price * amount;
                fee += epf * amount;
                fee += VAT(fee);

                return fee;
            }),
            CustomerType.Business => (Func<double, double>) (amount => {
                double price = 22068;
                double fee = 0.0;
                double epf = 2206.8;

                fee += price * amount;
                fee += epf * amount;
                fee += VAT(fee);

                return fee;
            }),
            _ => throw new NotImplementedException()
        };
    }


}