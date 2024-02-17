using System.Collections.Immutable;
using YamlDotNet.Serialization;

namespace Assignment1;

internal class Database {
	private static readonly Database Instance = new();
	private readonly string filename = "database.txt";

	private List<User> users;

	private readonly Serializer serializer = new();
	private readonly Deserializer deserializer = new();

	private Database() => Initialize();

	public static Database GetInstance() => Instance;

	private void Initialize() {
		users = [];
		if (!File.Exists(filename)) {
			ConsoleUtils.WriteLine("File not found. Creating new".Format(ConsoleColor.Red));
			File.Create(filename).Close();
		}
		if (File.Exists(filename)) {
			ConsoleUtils.WriteLine($"Working database {Path.GetFullPath(filename)}".Format(ConsoleColor.Green));
		}
	}

	public ImmutableList<User> GetUsers() {
		return GetUsers(SortMethod.Default);
	}

	public ImmutableList<User> GetUsers(SortMethod method) {
		Load();
		return users.ToImmutableList().Sort(method.GetComparison());
	}

	public ImmutableList<User> FindUsers(string? name, SortMethod method) {
		Load();
		if (string.IsNullOrEmpty(name)) return GetUsers(method);
		return GetUsers(method).FindAll(x => x.Username.ToLower().Contains(name.ToLower())).ToImmutableList();
	}

	public void ClearAll() {
		users = [];
		Save();
	}

	public void Remove(string username, UserType type, double lastMonth, double thisMonth) {
		_ = users.FindAll(x => x.Username == username && x.Type == type && x.LastMonth == lastMonth && x.ThisMonth == thisMonth)
		   .First(users.Remove);
		Save();
	}

	public void Add(params User[] users) {
		foreach (User user in users) {
			Add(user);
		}
	}

	public void Add(User user) {
		Load();
		users.Add(user);
		Save();
	}

	private void Load() {
		string readData = File.ReadAllText(filename);
		try {
			users = deserializer.Deserialize<List<User>>(readData);
		} catch (Exception e) {
			ConsoleUtils.WriteLine($"Load file error ({e.Message})".Format(ConsoleColor.Red));
		}
		users ??= [];
		Save();
	}

	private void Save() {
		string saveData = serializer.Serialize(users);
		File.WriteAllText(filename, saveData);
	}
}

internal enum SortMethod {
	ByName,
	ByUseAmount,
	Default
}

internal static class SortMethodComparer {

	public static Comparison<User> GetComparison(this SortMethod method) {
		return method switch {
			SortMethod.ByName => (x1, x2) => {
				string s1 = x1.Username;
				string s2 = x2.Username;
				return string.Compare(s1, s2);
			}
			,
			SortMethod.ByUseAmount => (x1, x2) => {
				int val = 0;
				double a1 = x1.UseAmount();
				double a2 = x2.UseAmount();
				if (a1 > a2) {
					val = -1;
				} else if (a1 < a2) {
					val = 1;
				}
				return val;
			}
			,
			_ => (x1, x2) => 0
		};
	}
}