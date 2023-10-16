using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

class Cimek
{
	public static int DynamicSearch(int index, ref string[] segments, char direction)
	{
		if (index < 0 || index > 7)
		{
			return 0;
		}
		else if (segments[index] == "0" && direction == '0')
		{
			return 1 + DynamicSearch(index - 1, ref segments, '-') + DynamicSearch(index + 1, ref segments, '+');
		}
		else if (segments[index] == "0" && direction == '-')
		{
			return 1 + DynamicSearch(index - 1, ref segments, '-');
		}
		else if (segments[index] == "0" && direction == '+')
		{
			return 1 + DynamicSearch(index + 1, ref segments, '+');
		}
		else
		{
			return 0;
		}
	}

	public static string HardAbbreviate(string ip)
	{
		var addressParts = ip.Split(':');
		var best = 0;
		var index = -1;

		for (int i = 0; i < addressParts.Length; ++i)
		{
			var counter = DynamicSearch(i, ref addressParts, '0');

			if (counter > 1 && counter > best)
			{
				best = counter;
				index = i;
			}
		}

		var result = new List<string>();
		var skipElements = false;

		for (int i = 0; i < addressParts.Length; ++i)
		{
			if (i == index)
			{
				skipElements = true;
			}

			if (!skipElements)
			{
				result.Add(addressParts[i]);
			}
			else
			{
				--best;

				if (best == 0)
				{
					result.Add("");
					skipElements = false;
				}
			}
		}

		return String.Join(":", result.ToArray());
	}

	public static string SoftAbbreviate(string ip)
	{
		var builder = new StringBuilder();

		foreach (var addressPart in ip.Split(':'))
		{
			if (addressPart.StartsWith("000"))
			{
				builder.Append(addressPart[3]);
			}
			else if (addressPart.StartsWith("00"))
			{
				builder.Append(addressPart.AsSpan().Slice(2));
			}
			else if (addressPart.StartsWith("0"))
			{
				builder.Append(addressPart.AsSpan().Slice(1));
			}
			else
			{
				builder.Append(addressPart);
			}

			builder.Append(':');
		}

		builder.Remove(builder.Length - 1, 1);
		return builder.ToString();
	}

	static void Main(string[] args)
	{
	//
	// Task : 1
	//
		var ipAddresses = new List<string>();
		using var reader = new StreamReader("ip.txt");

		while (reader.EndOfStream == false)
		{
			var line = reader.ReadLine() ?? String.Empty;

			if (line.Equals(String.Empty) == false)
			{
				ipAddresses.Add(line.TrimEnd('\r'));
			}
		}
	//
	// Task : 2
	//
		WriteLine("2. feladat:");
		WriteLine($"Az allomanyban {ipAddresses.Count} darab adatsor van.\n");
	//
	// Task : 3
	//
		var lowest = ipAddresses.Select(ip => new
		{
			Address = ip,
			AsciiValue = ip.Sum(character => character),
		})
		.MinBy(ip => ip.AsciiValue);
		
		WriteLine("3. feladat:");
		WriteLine("A legalacsonyabb tarolt IP-cim:");
		WriteLine($"{lowest?.Address}\n");
	//
	// Task : 4
	//
		var documentAddress = ipAddresses.Where(ip => ip.StartsWith("2001:0db8")).Count();
		var globalAddress = ipAddresses.Where(ip => ip.StartsWith("2001:0e")).Count();
		var localAddress = ipAddresses.Where(ip => ip.StartsWith("fc") || ip.StartsWith("fd")).Count();
		
		WriteLine("4. feladat:");
		WriteLine($"Dokumentacios cim: {documentAddress} darab");
		WriteLine($"Globalis egyedi cim: {globalAddress} darab");
		WriteLine($"Helyi egyedi cim: {localAddress} darab\n");
	//
	// Task : 5
	//
		using var writer = new StreamWriter("sok.txt");

		for (int i = 0; i < ipAddresses.Count; ++i)
		{
			if (ipAddresses[i].Count(character => character == '0') >= 18)
			{
				writer.WriteLine($"{i + 1} {ipAddresses[i]}");
			}
		}
	//
	// Task : 6
	//
		WriteLine("6. feladat:");
		Write("Kerek egy sorszamot: ");

		var choice = -1;
		var userInput = ReadLine() ?? String.Empty;
		
		if (Int32.TryParse(userInput, out choice) == false || choice < 1 || choice > ipAddresses.Count)
		{
			choice = 1;
		}

		var abbreviatedAddress = SoftAbbreviate(ipAddresses[choice - 1]);

		WriteLine($"{ipAddresses[choice - 1]}");
		WriteLine($"{abbreviatedAddress}\n");
	//
	// Task : 7
	//
		WriteLine("7. feladat:");

		var evenMoreAbbreviatedAddress = HardAbbreviate(abbreviatedAddress);

		WriteLine("{0}", evenMoreAbbreviatedAddress.Equals(abbreviatedAddress) ? "Nem roviditheto tovabb." : evenMoreAbbreviatedAddress);
	}
}
