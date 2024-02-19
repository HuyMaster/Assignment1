using System.Collections.Immutable;
using YamlDotNet.Serialization;

namespace Assignment1;

// Class used to perform functions on files
internal class Database {

	// Singleton object of this class so there is no need to create it multiple times in one run
	private static readonly Database Instance = new();

	// The database filename without full path it will create at current executable file location
	private readonly string filename = "database.txt";

	private List<User> users = [];

	private readonly Serializer serializer = new();
	private readonly Deserializer deserializer = new();

	// Prevent creating new objects from outside this class
	private Database() => Initialize();

	// Returns unique Instance of this class
	public static Database GetInstance() => Instance;

	// Initialize database file
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
		return GetUsers(SortMethod.Random);
	}

	// Returns a unchangeable list of users with the specified SortMethod
	public ImmutableList<User> GetUsers(SortMethod method) {
		Load();
		return users.ToImmutableList().Sort(method.GetComparison());
	}

	// Returns a unchangeable list of users that User.Username contains the `name` string with the specified SortMethod
	public ImmutableList<User> FindUsers(string? name, SortMethod method) {
		Load();
		if (string.IsNullOrEmpty(name)) return GetUsers(method);
		return GetUsers(method).FindAll(x => x.Username.Contains(name, StringComparison.CurrentCultureIgnoreCase));
	}

	// Delete all users saved in database
	public void ClearAll() {
		users = [];
		Save();
	}

	// Delete a user with the specified attributes
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

	// Add users to the database
	public void Add(User user) {
		Load();
		users.Add(user);
		Save();
	}

	// Read list of users from database
	private void Load() {
		string readData = File.ReadAllText(filename);
		try {
			users = deserializer.Deserialize<List<User>>(readData);
		} catch (Exception e) {
			MessageBox.Show($"Load file error ({e.Message})");
		}
		users ??= [];
		Save();
	}

	// Save list of users to database
	private void Save() {
		string saveData = serializer.Serialize(users);
		File.WriteAllText(filename, saveData);
	}
}

// Methods for sorting user lists
internal enum SortMethod {
	ByName,
	ByAddTime,
	ByUseAmount,
	Random
}

// SortMethod extension. Used to get Comparison used in List.Sort()
internal static class SortMethodComparer {

	public static Comparison<User> GetComparison(this SortMethod method) {
		return method switch {
			SortMethod.ByName => (x1, x2) => {
				string s1 = x1.Username;
				string s2 = x2.Username;
				return string.Compare(s1, s2);
			}
			,
			SortMethod.ByAddTime => (x1, x2) => {
				int val = 0;
				long a1 = x1.addTime;
				long a2 = x2.addTime;
				if (a1 > a2) {
					val = -1;
				} else if (a1 < a2) {
					val = 1;
				}
				return val;
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
			_ => (x1, x2) => Random.Shared.Next(-2, 2)
		};
	}
}