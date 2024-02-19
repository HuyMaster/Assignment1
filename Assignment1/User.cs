namespace Assignment1;

// The Plain Old CLR Object(POCO) class to store user data
internal struct User {
	public String Username { get; set; }
	public UserType Type { get; set; }
	public double LastMonth { get; set; }
	public double ThisMonth { get; set; }
	public long addTime { get; set; }

	public User() {
		Username = "";
		Type = UserType.Household;
		LastMonth = 0;
		ThisMonth = 0;
	}

	public double UseAmount() {
		return ThisMonth - LastMonth;
	}

	// Used to display basic information
	// Ex: [P] User[Huy] Used: 15
	public new string ToString() => $"[{Type.GetShortName()}]\tUser[{Username}] Used: {UseAmount()}";
}

// Provide specific functions for each type of user
internal enum UserType {
	Household,
	PublicService,
	Production,
	Business
}

// UserType extension returns the short name of the UserType
internal static class UserTypeShortName {

	public static string GetShortName(this UserType type) {
		return type switch {
			UserType.Household => "H",
			UserType.PublicService => "PS",
			UserType.Production => "P",
			UserType.Business => "B",
			_ => "?",
		};
	}
}

// UserType extension returns the description of the UserType
internal static class UserTypeDescription {

	public static string GetDescription(this UserType type) {
		return type switch {
			UserType.Household => "Household customer",
			UserType.PublicService => "Administrative agency, public services",
			UserType.Production => "Production units",
			UserType.Business => "Business services",
			_ => "Unknown",
		};
	}
}

// UserType extension returns the fee calculating method of the UserType
internal static class UserTypeFeeCalculatorMethod {

	/// <param name="type"></param>
	/// <param name="amount"></param>
	/// <returns>-1 if can't calc, else return fee</returns>
	public static double CalculateFee(this UserType type, double amount) {
		return type switch {
			UserType.Household => Household(amount),
			UserType.PublicService => PublicService(amount),
			UserType.Production => Production(amount),
			UserType.Business => Business(amount),
			_ => -1
		};
	}

	private static double Household(double amount) {
		double[] prices = [5973, 7052, 8699, 15929];
		double fee = -1;

		double price;
		if (amount < 0) {
			return fee;
		} else if (amount < 10) {
			price = prices[0];
		} else if (amount < 20) {
			price = prices[1];
		} else if (amount < 30) {
			price = prices[2];
		} else {
			price = prices[3];
		}

		fee = price * amount;
		fee += Epf(amount, price);

		return VAT(fee);
	}

	private static double PublicService(double amount) {
		double price = 9955;
		double fee = -1;
		if (amount >= 0) {
			fee = amount * price;
			fee += Epf(amount, price);
		}
		return VAT(fee);
	}

	private static double Production(double amount) {
		double price = 11615;
		double fee = -1;
		if (amount >= 0) {
			fee = amount * price;
			fee += Epf(amount, price);
		}
		return VAT(fee);
	}

	private static double Business(double amount) {
		double price = 22068;
		double fee = -1;
		if (amount >= 0) {
			fee = amount * price;
			fee += Epf(amount, price);
		}
		return VAT(fee);
	}

	// Environment protection fee
	private static double Epf(double amount, double price) {
		double epf = 0;
		epf += amount * (price * 0.1); // 10% of price
		return epf;
	}

	//
	private static double VAT(double fee) {
		if (fee == -1) {
			return fee;
		}
		return fee + (fee * 0.1); // Calculate VAT and add to fee and return final fee
	}
}