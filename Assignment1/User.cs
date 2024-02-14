using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1;
struct User {
	public String Username { get; set; }
	public UserType Type { get; set; }
	public double UseAmount { get; set; }

	public User() {
		Username = "";
		Type = UserType.Household;
		UseAmount = 0;
	}
}

enum UserType {
	Household,
	PublicService,
	Production,
	Business
}

static class UserTypeDescription {
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

static class UserTypeFeeCalculatorMethod {

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
		double[] amountSteps = new double[prices.Length];
		double fee = -1;
		if (amount >= 0) {
			for (int i = 0; i < amountSteps.Length; i++) {
				if (i == amountSteps.Length - 1) {
					amountSteps[i] = amount;
					amount -= amountSteps[i];
				} else {
					amountSteps[i] = Math.Min(10, amount);
					amount -= amountSteps[i];
				}
			}
			fee = 0;
			for (int i = 0; i < amountSteps.Length; i++) {
				fee += amountSteps[i] * prices[i];
				fee += Epf(amountSteps[i], prices[i]);
			}
		}
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

	private static double Epf(double amount, double price) {
		double epf = 0;
		epf += amount * (price * 0.1);
		return epf;
	}

	private static double VAT(double fee) {
		if (fee == -1) {
			return fee;
		}
		return fee + (fee * 0.1);
	}
}