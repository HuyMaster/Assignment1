using System.Collections.Immutable;
using YamlDotNet.Serialization;

namespace Assignment1;

internal class Database {
	public static readonly Database Instance = new();
	private readonly string filename = "database.db";

	private List<User> users;

	private readonly Serializer serializer = new();
	private readonly Deserializer deserializer = new();

	private Database() {
		Initialize();
	}

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
		Load();
		return [.. users];
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

	public void Add(User user) {
		Load();
		users.Add(user);
		Save();
	}

	private void Load() {
		string readData = File.ReadAllText(filename);
		users = deserializer.Deserialize<List<User>>(readData);
		users ??= [];
	}

	private void Save() {
		string saveData = serializer.Serialize(users);
		File.WriteAllText(filename, saveData);
	}
}