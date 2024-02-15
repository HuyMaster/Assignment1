namespace Assignment1;

internal partial class GUI : Form {
	public static readonly GUI instance = new();
	private static readonly UserType[] usertypes = (UserType[]) Enum.GetValues(typeof(UserType));

	public new void ShowDialog() {
		InitializeComponent();
		Thread t = new(new ThreadStart(() => base.ShowDialog()));
		t.Start();
	}

	private void InitializeComponent() {
		components = new System.ComponentModel.Container();
		name_label = new Label();
		username = new TextBox();
		type_label = new Label();
		type = new ComboBox();
		lastMonth_label = new Label();
		lastMonth = new TextBox();
		errorDisplay = new ErrorProvider(components);
		thisMonth = new TextBox();
		thisMonth_label = new Label();
		listUsers = new ListBox();
		perform = new Button();
		((System.ComponentModel.ISupportInitialize) errorDisplay).BeginInit();
		SuspendLayout();
		//
		// name_label
		//
		name_label.AutoSize = true;
		name_label.Location = new Point(12, 9);
		name_label.Name = "name_label";
		name_label.Size = new Size(60, 15);
		name_label.TabIndex = 7;
		name_label.Text = "Username";
		//
		// username
		//
		username.Location = new Point(88, 6);
		username.Name = "username";
		username.Size = new Size(473, 23);
		username.TabIndex = 0;
		//
		// type_label
		//
		type_label.AutoSize = true;
		type_label.Location = new Point(12, 38);
		type_label.Name = "type_label";
		type_label.Size = new Size(31, 15);
		type_label.TabIndex = 6;
		type_label.Text = "Type";
		//
		// type
		//
		List<UserTypeDataBinding> values = usertypes.Select(x => new UserTypeDataBinding(x)).ToList();
		type.Location = new Point(88, 35);
		type.Name = "type";
		type.Size = new Size(473, 23);
		type.DropDownStyle = ComboBoxStyle.DropDownList;
		type.DataSource = values;
		type.DisplayMember = "Name";
		type.ValueMember = "Type";
		type.TabIndex = 1;
		//
		// lastMonth_label
		//
		lastMonth_label.AutoSize = true;
		lastMonth_label.Location = new Point(12, 67);
		lastMonth_label.Name = "lastMonth_label";
		lastMonth_label.Size = new Size(67, 15);
		lastMonth_label.TabIndex = 5;
		lastMonth_label.Text = "Last month";
		//
		// lastMonth
		//
		lastMonth.Location = new Point(88, 64);
		lastMonth.Name = "lastMonth";
		lastMonth.Size = new Size(159, 23);
		lastMonth.TabIndex = 2;
		//
		// thisMonth_label
		//
		thisMonth_label.AutoSize = true;
		thisMonth_label.Location = new Point(329, 67);
		thisMonth_label.Name = "thisMonth_label";
		thisMonth_label.Size = new Size(67, 15);
		thisMonth_label.Text = "This month";
		//
		// thisMonth
		//
		thisMonth.Location = new Point(402, 64);
		thisMonth.Name = "thisMonth";
		thisMonth.Size = new Size(159, 23);
		thisMonth.TabIndex = 3;
		//
		// errorDisplay
		//
		errorDisplay.ContainerControl = this;
		//
		// listUser
		//
		listUsers.Location = new Point(11, 96);
		listUsers.Size = new Size(550, 221);
		listUsers.ColumnWidth = listUsers.Width;
		listUsers.SelectionMode = SelectionMode.One;
		listUsers.MouseUp += (o, e) => {
			listUsers.SelectedIndex = listUsers.IndexFromPoint(e.X, e.Y);
			UserDataBinding? value = (UserDataBinding?) listUsers.SelectedItem;
			if (e.Button == MouseButtons.Right && value != null) {
				DialogResult choice = MessageBox.Show("Remove this item?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (choice == DialogResult.Yes) {
					User user = value.User;
					Database.Instance.Remove(user.Username, user.Type, user.LastMonth, user.ThisMonth);
					RefreshList();
				}
			}
		};
		listUsers.MouseDoubleClick += (o, e) => {
			UserDataBinding? value = (UserDataBinding?) listUsers.SelectedItem;
			ResultDialog.Create(this, value?.User);
		};
		RefreshList();
		//
		//
		// perform
		//
		perform.Location = new Point(486, 326);
		perform.Name = "perform";
		perform.Size = new Size(75, 23);
		perform.TabIndex = 4;
		perform.Text = "Calculate";
		perform.UseVisualStyleBackColor = true;
		perform.Click += Perform_Click;
		//
		// GUI
		//
		ClientSize = new Size(584, 361);
		Controls.Add(perform);
		Controls.Add(listUsers);
		Controls.Add(thisMonth);
		Controls.Add(thisMonth_label);
		Controls.Add(lastMonth);
		Controls.Add(lastMonth_label);
		Controls.Add(type);
		Controls.Add(type_label);
		Controls.Add(username);
		Controls.Add(name_label);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "GUI";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "Water Fee Calculator";
		FormClosing += Form_Closing;
		((System.ComponentModel.ISupportInitialize) errorDisplay).EndInit();
		ResumeLayout(false);
		type_label.Click += Perform_Click;
		PerformLayout();
	}

	private void RefreshList() {
		int index = listUsers.SelectedIndex;
		listUsers.DataSource = Database.Instance.GetUsers().Sort((a, b) => {
			double va = a.UseAmount();
			double vb = b.UseAmount();
			if (va > vb) {
				return -1;
			} else if (va == vb) {
				return a.Username.CompareTo(b.Username);
			}
			return 1;
		}).Select(x => new UserDataBinding(x)).ToList();
		listUsers.DisplayMember = "Name";
		listUsers.ValueMember = "User";
		listUsers.SelectedIndex = listUsers.Items.Count - 1 < index ? listUsers.Items.Count - 1 : index;
	}

	private void Perform_Click(object? sender, EventArgs e) {
		if (sender == perform) {
			string name = username.Text;
			UserType Type = (UserType) (type.SelectedValue ?? UserType.Household);
			string lastMonthStr = lastMonth.Text;
			string thisMonthStr = thisMonth.Text;

			bool condition = true;
			if (string.IsNullOrEmpty(name)) {
				errorDisplay.SetError(username, "Username can NOT null");
				condition = false;
			} else {
				errorDisplay.SetError(username, "");
			}

			if (!ParseDouble(lastMonthStr, out double lastMonthValue)) {
				errorDisplay.SetError(lastMonth, "Not a number");
				condition = false;
			} else {
				errorDisplay.SetError(lastMonth, "");
			}

			if (!ParseDouble(thisMonthStr, out double thisMonthValue)) {
				errorDisplay.SetError(thisMonth, "Not a number");
				condition = false;
			} else {
				errorDisplay.SetError(thisMonth, "");
			}
			if (thisMonthValue - lastMonthValue <= 0) {
				errorDisplay.SetError(thisMonth, "This month must NOT be less than or equal to the last month");
				condition = false;
			} else {
				errorDisplay.SetError(thisMonth, "");
			}
			if (condition) {
				AddResult(name, Type, lastMonthValue, thisMonthValue);
			}
		}
	}

	private void AddResult(string username, UserType type, double lastMonth, double thisMonth) {
		User user = new();
		user.Username = username;
		user.Type = type;
		user.LastMonth = lastMonth;
		user.ThisMonth = thisMonth;

		Database.Instance.Add(user);
		RefreshList();
	}

	private static bool ParseDouble(string? str, out double output) {
		return double.TryParse(str, out output);
	}

	private void Form_Closing(object? sender, FormClosingEventArgs e) {
		DialogResult result = MessageBox.Show("Close app?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
		if (result == DialogResult.Cancel) {
			e.Cancel = true;
		}
	}

#pragma warning disable 8618
	private System.ComponentModel.IContainer components;
	private ErrorProvider errorDisplay;

	private Label name_label;
	private TextBox username;
	private Label type_label;
	private ComboBox type;
	private Label lastMonth_label;
	private TextBox lastMonth;
	private Label thisMonth_label;
	private TextBox thisMonth;

	private ListBox listUsers;
	private Button perform;
}

internal class UserTypeDataBinding(UserType Type) {
	public string Name { get; } = $"{Type} ({Type.GetDescription()})";
	public UserType Type { get; } = Type;
}

internal class UserDataBinding(User user) {
	public string Name { get; } = user.ToString();
	public User User { get; } = user;
}