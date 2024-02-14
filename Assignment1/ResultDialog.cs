using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1;

internal partial class ResultDialog : Form {
	private readonly User user;

	public static ResultDialog Create(Form parent, User user) {
		ResultDialog dialog = new(user);
		dialog.ShowDialog(parent);
		return dialog;
	}

	private ResultDialog(User user) => this.user = user;

	private new void ShowDialog(Form parent) {
		InitializeComponent();
		Thread t = new(new ThreadStart(() => base.ShowDialog(parent)));
		t.Start();
	}

	private void InitializeComponent() {
		username_label = new Label();
		type_label = new Label();
		username = new TextBox();
		type = new TextBox();
		used = new TextBox();
		used_label = new Label();
		total = new TextBox();
		total_label = new Label();
		SuspendLayout();
		//
		// username_label
		//
		username_label.AutoSize = true;
		username_label.Location = new Point(12, 9);
		username_label.Name = "username_label";
		username_label.Size = new Size(60, 15);
		username_label.Text = "Username";
		//
		// username
		//
		username.Location = new Point(78, 6);
		username.Name = "username";
		username.ReadOnly = true;
		username.Size = new Size(294, 23);
		username.Text = user.Username;
		//
		// type_label
		//
		type_label.AutoSize = true;
		type_label.Location = new Point(12, 38);
		type_label.Name = "type_label";
		type_label.Size = new Size(31, 15);
		type_label.Text = "Type";
		//
		// type
		//
		type.Location = new Point(78, 35);
		type.Name = "type";
		type.ReadOnly = true;
		type.Size = new Size(294, 23);
		type.Text = $"{user.Type.GetDescription()}";
		//
		// used_label
		//
		used_label.AutoSize = true;
		used_label.Location = new Point(12, 67);
		used_label.Name = "used_label";
		used_label.Size = new Size(33, 15);
		used_label.Text = "Used";
		//
		// used
		//
		used.Location = new Point(78, 64);
		used.Name = "used";
		used.ReadOnly = true;
		used.Size = new Size(294, 23);
		used.Text = $"{user.UseAmount()}";
		//
		// total_label
		//
		total_label.AutoSize = true;
		total_label.Location = new Point(12, 96);
		total_label.Name = "total_label";
		total_label.Size = new Size(32, 15);
		total_label.Text = "Total";
		//
		// total
		//
		total.Location = new Point(78, 93);
		total.Name = "total";
		total.ReadOnly = true;
		total.Size = new Size(294, 23);
		total.Text = $"{user.Type.CalculateFee(user.UseAmount())} VND";
		//
		// ResultDialog
		//
		ClientSize = new Size(384, 125);
		Controls.Add(total);
		Controls.Add(total_label);
		Controls.Add(used);
		Controls.Add(used_label);
		Controls.Add(type);
		Controls.Add(username);
		Controls.Add(type_label);
		Controls.Add(username_label);
		DoubleBuffered = true;
		FormBorderStyle = FormBorderStyle.FixedSingle;
		StartPosition = FormStartPosition.CenterParent;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "ResultDialog";
		Text = "Result";
		ResumeLayout(false);
		PerformLayout();
	}

	private Label username_label;
	private Label type_label;
	private TextBox username;
	private TextBox type;
	private TextBox used;
	private Label used_label;
	private TextBox total;
	private Label total_label;
}