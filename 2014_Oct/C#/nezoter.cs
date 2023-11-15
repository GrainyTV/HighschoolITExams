using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

class Chair
{
	public char Type { get; init; }
	public int Category { get; init; }
}

class Nezoter
{
	public static bool DynamicSearch(ref Chair[] row, int index)
	{
		if (index == 0)
		{
			return row[index + 1].Type == 'x';
		}
		else if (index == row.Length - 1)
		{
			return row[index - 1].Type == 'x';
		}
		else
		{
			return row[index - 1].Type == 'x' && row[index + 1].Type == 'x';
		}
	}

	public static int FindSingleSeats(ref Chair[] row)
	{
		var counter = 0;

		for (int i = 0; i < row.Length; ++i)
		{
			if (row[i].Type == 'o')
			{
				if (DynamicSearch(ref row, i))
				{
					++counter;
				}
			}
		}

		return counter;
	}

	static void Main(string[] args)
	{
	//
	// Task : 1
	//
		const int ROWS = 15;
		const int COLUMNS = 20;
		using var readerOccupancy = new StreamReader("foglaltsag.txt");
		using var readerCategory = new StreamReader("kategoria.txt");

		var theatre = new Chair[ROWS, COLUMNS];

		for (int i = 0; i < ROWS; ++i)
		{
			var lineOccupancy = readerOccupancy.ReadLine() ?? String.Empty;
			var lineCategory = readerCategory.ReadLine() ?? String.Empty;

			if (lineOccupancy.Equals(String.Empty) == false && lineCategory.Equals(String.Empty) == false)
			{
				for (int j = 0; j < COLUMNS; ++j)
				{
					theatre[i, j] = new Chair()
					{
						Type = lineOccupancy[j],
						Category = lineCategory[j] - '0',
					};
				}
			}
		}
	//
	// Task : 2
	//
		WriteLine("2. feladat:");
		
		Write("Kerek egy sorszamot: ");
		var userInputRow = ReadLine() ?? String.Empty;
		
		Write("Kerek egy szek szamot: ");
		var userInputColumn = ReadLine() ?? String.Empty;

		var requestedRow = -1;
		var requestedColumn = -1;
		
		if (Int32.TryParse(userInputRow, out requestedRow) == false || requestedRow < 1 || requestedRow > ROWS)
		{
			requestedRow = 1;
		}
		
		if (Int32.TryParse(userInputColumn, out requestedColumn) == false || requestedColumn < 1 || requestedColumn > COLUMNS)
		{
			requestedColumn = 1;
		}

		WriteLine($"A(z) {requestedRow} sor {requestedColumn}. szeke {(theatre[requestedRow - 1, requestedColumn - 1].Type == 'o' ? "meg szabad" : "mar foglalt")}.\n");
	//
	// Task : 3
	//
		var soldTickets = theatre.Cast<Chair>()
		.Where(chair => chair.Type == 'x')
		.Count();

		WriteLine("3. feladat:");
		WriteLine($"Az eloadasra eddig {soldTickets} jegyet adtak el, ez a nezoter {soldTickets / ((float) ROWS * COLUMNS) * 100:0}%-a.\n");
	//
	// Task : 4
	//
		var priceCategories = theatre.Cast<Chair>()
		.Where(chair => chair.Type == 'x')
		.GroupBy(chair => chair.Category, chair => chair.Category, (key, group) => new
		{
			PriceRange = key,
			Amount = group.Count(),
		});

		WriteLine("4. feladat:");
		WriteLine($"A legtobb jegyet a(z) {priceCategories.MaxBy(chair => chair.Amount)?.PriceRange}. arkategoriaban ertekesitettek.\n");
	//
	// Task : 5
	//
		var income = priceCategories.Sum(kv => kv.PriceRange switch
		{
			1 => kv.Amount * 5000,
			2 => kv.Amount * 4000,
			3 => kv.Amount * 3000,
			4 => kv.Amount * 2000,
			5 => kv.Amount * 1500,
			_ => throw new ArgumentException("Invalid price range value provided."),
		});

		WriteLine("5. feladat:");
		WriteLine($"A szinhaz bevetele a pillanatnyilag eladott jegyek alapjan {income} HUF lenne.\n");
	//
	// Task : 6
	//
		var numberOfSingleSeats = 0;
		var seats = theatre.Cast<Chair>();

		for (int i = 0; i < ROWS; ++i)
		{
			var currentRow = seats.Skip(i * COLUMNS)
			.Take(20)
			.ToArray();
			
			numberOfSingleSeats += FindSingleSeats(ref currentRow);
		}
		
		WriteLine("6. feladat:");
		WriteLine($"Jelenleg {numberOfSingleSeats} db egyedulallo ures hely van a nezoteren.");
	//
	// Task : 7
	//
		using var writer = new StreamWriter("szabad.txt");

		for (int i = 0; i < ROWS; ++i)
		{
			for (int j = 0; j < COLUMNS; ++j)
			{
				writer.Write(theatre[i, j].Type == 'x' ? "x" : theatre[i, j].Category);
			}

			writer.WriteLine();
		}
	}
}