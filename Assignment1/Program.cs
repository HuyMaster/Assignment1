using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Linq;

namespace Assignment1;

internal static class Program {
	private static readonly List<string> Action = ["Add", "Remove", "Query"];

	public static void Main(string[] args) {
		Database.GetInstance();
		ChoiceAction();
	}

	private static void ChoiceAction() {
		int choice = 0;
		ConsoleUtils.ClearAll();
		while (true) {
			ConsoleUtils.ClearAll();
			Console.Title = "Water Fee Calculator";

			Dictionary<string, bool> paintDic = [];
			foreach (string item in Action) {
				paintDic.Add(item, false);
			}

			KeyValuePair<string, bool> entry = paintDic.ElementAt(choice);
			paintDic[entry.Key] = true;

			ConsoleUtils.WriteAt(0, "Tab: Next - Enter: Select - ESC: Exit".Format());
			ConsoleUtils.WriteAt(2, paintDic.Select(x => $"{x.Key}{new string(' ', 5)}".Format(Paint(x.Value))).ToArray());

			ConsoleKey key = Console.ReadKey().Key;
			if (key == ConsoleKey.Tab) {
				choice = (choice + 1) % Action.Count;
			} else if (key == ConsoleKey.Enter) {
				switch (Action[choice]) {
					case "Add":
						AddUser();
						break;

					case "Remove":
						RemoveUser();
						break;

					case "Query":
						QueryUser();
						break;

					default:
						break;
				}
			} else if (key == ConsoleKey.Escape) {
				break;
			}
		}
	}

	private static ConsoleColor Paint(bool paint) {
		return paint ? ConsoleColor.Green : ConsoleColor.Red;
	}

	private static void AddUser() {
		Console.Title = "User Adder";
		ConsoleUtils.ClearAll();

		User user = new();
		ConsoleUtils.WriteAt(0, "Username: ".Format());
		user.Username = Console.ReadLine() ?? "";

		user.Type = TypeChooser(1);
		ConsoleUtils.WriteAt(1, $"Type: {user.Type}".Format());

		double thisMonth;

		GetNumberInput(2, "Last month", out double lastMonth);
		while (true) {
			GetNumberInput(3, "This month", out thisMonth);

			if (lastMonth >= thisMonth) {
				ConsoleUtils.WriteAt(3, "Last month must NOT be less than or equal to this month".Format(ConsoleColor.Red));
				Console.ReadKey();
			} else {
				break;
			}
		}

		user.LastMonth = lastMonth;
		user.ThisMonth = thisMonth;

		if (!string.IsNullOrEmpty(user.Username)) {
			Database.GetInstance().Add(user);
		}
	}

	public static void RemoveUser() {
		Console.Title = "User Remover";
		ConsoleUtils.ClearAll();

		ImmutableList<User> list = Database.GetInstance().GetUsers(SortMethod.ByName);
		if (list.Count <= 0) {
			ConsoleUtils.WriteLine("Nothing to remove".Format(ConsoleColor.Red));
			Console.ReadKey();
			return;
		}

		for (int i = 0; i < list.Count; i++) {
			ConsoleUtils.WriteLine($"{i}: ".Format(), list[i].ToString().Format(ConsoleColor.Green));
		}
		ConsoleUtils.WriteLine();
		ConsoleUtils.WriteLine("< 0: ".Format(), "Exit".Format(ConsoleColor.Red));
		while (true) {
			GetNumberInput(ConsoleUtils.CurrentLine(), "Remove (index)", out int index);
			if (index >= list.Count) {
				ConsoleUtils.WriteAt(ConsoleUtils.CurrentLine(), "Index out of bound".Format(ConsoleColor.Red));
				Console.ReadKey();
			} else if (index < 0) {
				return;
			} else {
				User user = list[index];
				Database.GetInstance().Remove(user.Username, user.Type, user.LastMonth, user.ThisMonth);
				break;
			}
		}
	}

	private static void QueryUser() {
		Console.Title = "User Query";

		List<SortMethod> sortModes = Enum.GetValues(typeof(SortMethod)).Cast<SortMethod>().ToList();

		SortMethod currentSortMethod = SortMethod.Default;
		string? filter = "";
		ImmutableList<User> list = Database.GetInstance().GetUsers(currentSortMethod);

		while (true) {
			ConsoleUtils.ClearAll();
			ConsoleUtils.WriteLine("Current sort method: ".Format(), currentSortMethod.ToString().Format(ConsoleColor.Green));
			ConsoleUtils.WriteLine("Current filter: ".Format(), $"{(string.IsNullOrEmpty(filter) ? "<all>" : filter)}".Format(ConsoleColor.Green));
			ConsoleUtils.WriteLine();
			ConsoleUtils.WriteLine("Ctrl + M: Change sort method".Format());
			ConsoleUtils.WriteLine("Ctrl + F: Find user by name".Format());
			ConsoleUtils.WriteLine("Ctrl + A: Show all".Format());
			ConsoleUtils.WriteLine("Ctrl + D: View Detail".Format());
			ConsoleUtils.WriteLine("Ctrl + X: Exit".Format());
			ConsoleUtils.WriteLine();

			if (list.Count <= 0) {
				ConsoleUtils.WriteLine($"\t< Empty >".Format(ConsoleColor.Red));
			} else {
				for (int i = 0; i < list.Count; i++) {
					ConsoleUtils.WriteLine($"{i}: ".Format(), list[i].ToString().Format(ConsoleColor.Green));
				}
			}

			ConsoleKeyInfo key = Console.ReadKey();
			if (key.Modifiers == ConsoleModifiers.Control) {
				if (key.Key == ConsoleKey.M) {
					currentSortMethod = Chooser(ConsoleUtils.CurrentLine() + 1, sortModes, "Choose method");
					list = list.Sort(currentSortMethod.GetComparison());
				} else if (key.Key == ConsoleKey.F) {
					ConsoleUtils.WriteLine();
					ConsoleUtils.Write("Filter: ".Format());
					filter = Console.ReadLine();
					list = Database.GetInstance().FindUsers(filter, currentSortMethod);
				} else if (key.Key == ConsoleKey.A) {
					filter = "";
					list = Database.GetInstance().GetUsers(currentSortMethod);
				} else if (key.Key == ConsoleKey.D) {
					while (true) {
						GetNumberInput(ConsoleUtils.CurrentLine(), "Index", out int index);
						if (index >= list.Count) {
							ConsoleUtils.WriteAt(ConsoleUtils.CurrentLine(), "Index out of bound".Format(ConsoleColor.Red));
							Thread.Sleep(1000);
						} else {
							User view = list[index];
							ConsoleUtils.ClearAll();
							ConsoleUtils.WriteLine("============================User detail============================".Format());
							ConsoleUtils.WriteLine("Username: ".Format(), view.Username.Format(ConsoleColor.Green));
							ConsoleUtils.WriteLine("Type: ".Format(), view.Type.ToString().Format(ConsoleColor.Green), $" ({view.Type.GetDescription()})".Format());
							ConsoleUtils.WriteLine("Last month: ".Format(), String.Format("{0:#,0}", view.LastMonth).Format(ConsoleColor.Green));
							ConsoleUtils.WriteLine("This month: ".Format(), String.Format("{0:#,0}", view.ThisMonth).Format(ConsoleColor.Green));
							ConsoleUtils.WriteLine();
							ConsoleUtils.WriteLine("Use amount: ".Format(), String.Format("{0:#,0}", view.UseAmount()).Format(ConsoleColor.Green));
							ConsoleUtils.WriteLine("Total: ".Format(), String.Format("{0:0,0} VND", view.Type.CalculateFee(view.UseAmount())).Format(ConsoleColor.Green));
							ConsoleUtils.WriteLine("===================================================================".Format());
							Console.ReadKey();
							break;
						}
					}
				} else if (key.Key == ConsoleKey.X) {
					ConsoleUtils.ClearAll();
					break;
				} else {
					ConsoleUtils.WriteAt(ConsoleUtils.CurrentLine(), "Invalid key".Format(ConsoleColor.Red));
					Thread.Sleep(1000);
				}
			}
		}
	}

	private static T Chooser<T>(int line, ICollection<T> list, string prompt) {
		int choice = 0;
		Type type = typeof(T);

		while (true) {
			Dictionary<string, bool> paintDic = [];
			foreach (T item in list) {
				if (item == null) continue;
				paintDic.Add(item.ToString(), false);
			}
			KeyValuePair<string, bool> entry = paintDic.ElementAt(choice);
			paintDic[entry.Key] = true;

			ConsoleUtils.WriteAt(line, $"{prompt}: ".Format());
			ConsoleUtils.Write(
				paintDic.Select(x => $"{x.Key}{new string(' ', 3)}".Format(Paint(x.Value))).ToArray());

			ConsoleKey key = Console.ReadKey().Key;
			if (key == ConsoleKey.Tab) {
				choice = (choice + 1) % list.Count;
			} else if (key == ConsoleKey.Enter) {
				return list.ElementAt(choice);
			}
		}
	}

	private static UserType TypeChooser(int line) {
		UserType[] types = (UserType[]) Enum.GetValues(typeof(UserType));
		return Chooser(line, types, "Type");
	}

	private static void GetNumberInput(int line, string prompt, out double output) {
		while (true) {
			ConsoleUtils.WriteAt(line, $"{prompt}: ".Format());
			string? tmp = Console.ReadLine() ?? "";
			if (!double.TryParse(tmp, out output)) {
				ConsoleUtils.WriteAt(line, $"{prompt}: ".Format(), "NaN (Not a Number)".Format(ConsoleColor.Red));
				Console.ReadKey();
			} else {
				break;
			}
		}
	}

	private static void GetNumberInput(int line, string prompt, out int output) {
		while (true) {
			ConsoleUtils.WriteAt(line, $"{prompt}: ".Format());
			string? tmp = Console.ReadLine() ?? "";
			if (!int.TryParse(tmp, out output)) {
				ConsoleUtils.WriteAt(line, $"{prompt}: ".Format(), "NaN (Not a Number)".Format(ConsoleColor.Red));
				Console.ReadKey();
			} else {
				break;
			}
		}
	}
}