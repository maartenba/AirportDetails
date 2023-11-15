using System.Globalization;
using AirportDetails;
using CsvHelper;
using CsvHelper.Configuration;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

var useBulkInsert = false;
var useDatabaseFiltering = false;
var useSlowCommand = true;

// Insert data (plain vs. bulk)
await EnsureDatabase(useBulkInsert);

// Query data (get all vs. specific query)
using (var database = new Database())
{
    if (!useDatabaseFiltering)
    {
        var airports = await database.Airports.ToListAsync();
        var brusselsAirport = airports.First(it => it.NormalizedIata == "BRU");

        Console.WriteLine($"{brusselsAirport.Country} ({brusselsAirport.Timezone})");
    }
    else
    {
        var brusselsAirport = await database.Airports.FirstAsync(it => it.NormalizedIata == "BRU");

        Console.WriteLine($"{brusselsAirport.Country} ({brusselsAirport.Timezone})");
    }
}

// Slow command
using (var database = new Database())
{
    if (useSlowCommand)
    {
        var countries = await database.Countries
            .OrderBy(country => country.IsoCode)
            .ToListAsync();

        var countriesAndAirportsAndAirlines = countries
            .Select(country => new
            {
                Country = country,
                Airports = database.Airports
                    .Where(airport => airport.Country == country)
                    .ToList(),
                Airlines = database.Airlines
                    .Where(airline => airline.Country == country)
                    .ToList()
            })
            .ToList();

        foreach (var countryDetails in countriesAndAirportsAndAirlines)
        {
            Console.WriteLine(
                $"{countryDetails.Country.Name} has {countryDetails.Airports.Count} known airports.");
            Console.WriteLine(
                $"{countryDetails.Country.Name} has {countryDetails.Airlines.Count} known airlines.");
        }
    }
    else
    {
        var countriesAndAirportsAndAirlines = await database.Countries
            .Include(country => country.Airports)
            .Include(country => country.Airlines)
            .OrderBy(country => country.IsoCode)
            .AsSplitQuery()
            .ToListAsync();

        foreach (var countryDetails in countriesAndAirportsAndAirlines)
        {
            Console.WriteLine(
                $"{countryDetails.Name} has {countryDetails.Airports.Count} known airports.");
            Console.WriteLine(
                $"{countryDetails.Name} has {countryDetails.Airlines.Count} known airlines.");
        }
    }
}

async Task EnsureDatabase(bool useBulkInsert)
{
    await using var database = new Database();
    database.Database.Migrate();

    // Helper functions
    static string? ValueOrNull(string? input) => input == null || input == "\\N" ? null : input;
    static decimal ValueOrZero(string? input) => input == null || input == "\\N" || !decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? 0 : value;

    // Seed if none exist
    var bulkInsertCountries = new List<Country>();
    if (await database.Countries.AnyAsync() == false)
    {
        var countriesTsvPath = Path.Combine("Data", "countries.tsv");

        using var reader = new StreamReader(countriesTsvPath);
        using var csv = new CsvReader(new CsvParser(reader, new CsvConfiguration(CultureInfo.GetCultureInfo("en-US"))
        {
            Delimiter = "\t"
        }, leaveOpen: false));

        await csv.ReadAsync();
        while (csv.Parser.RawRecord.StartsWith("#"))
        {
            await csv.ReadAsync();
        }

        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var country = new Country
            {
                Name = csv.GetField<string>("Country")!,
                NormalizedName = csv.GetField<string>("Country")!.ToUpperInvariant(),
                IsoCode = csv.GetField<string>("ISO"),
                NormalizedIsoCode = csv.GetField<string>("ISO")?.ToUpperInvariant()
            };

            bulkInsertCountries.Add(country);
            if (!useBulkInsert)
            {
                database.Add(country);
            }
        }

        if (useBulkInsert)
        {
            database.BulkInsert(bulkInsertCountries);
        }
        await database.SaveChangesAsync();
    }
    else
    {
        bulkInsertCountries.AddRange(await database.Countries.ToListAsync());
    }

    // Seed airports if none exist
    var bulkInsertAirports = new List<Airport>();
    if (await database.Airports.AnyAsync() == false)
    {
        var airportsCsvPath = Path.Combine("Data", "airports.csv");

        using var reader = new StreamReader(airportsCsvPath);
        using var csv = new CsvReader(new CsvParser(reader, new CsvConfiguration(CultureInfo.GetCultureInfo("en-US"))
        {
            Delimiter = ","
        }, leaveOpen: false));

        await csv.ReadAsync();

        while (csv.Parser.RawRecord.StartsWith("#"))
        {
            await csv.ReadAsync();
        }

        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var country = bulkInsertCountries.FirstOrDefault(it =>
                it.NormalizedName == csv.GetField<string>("Country")!.ToUpperInvariant());

            var airport = new Airport
            {
                Name = csv.GetField<string>("Name")!,
                NormalizedName = csv.GetField<string>("Name")!.ToUpperInvariant(),
                City = csv.GetField<string>("City")!,
                NormalizedCity = csv.GetField<string>("City")!.ToUpperInvariant(),
                Country = country,
                Iata = csv.GetField<string>("IATA"),
                NormalizedIata = csv.GetField<string>("IATA")!.ToUpperInvariant(),
                Latitude = ValueOrZero(csv.GetField<string>("Latitude")),
                Longitude = ValueOrZero(csv.GetField<string>("Longitude")),
                Timezone = ValueOrNull(csv.GetField<string>("TzDataZone"))
            };

            bulkInsertAirports.Add(airport);
            if (!useBulkInsert)
            {
                database.Add(airport);
            }
        }

        if (useBulkInsert)
        {
            database.BulkInsert(bulkInsertAirports);
        }

        await database.SaveChangesAsync();
    }
    else
    {
        bulkInsertAirports.AddRange(await database.Airports.ToListAsync());
    }

    // Seed airlines if none exist
    var bulkInsertAirlines = new List<Airline>();
    if (await database.Airlines.AnyAsync() == false)
    {
        var airlinesCsvPath = Path.Combine("Data", "airlines.csv");

        using var reader = new StreamReader(airlinesCsvPath);
        using var csv = new CsvReader(new CsvParser(reader, new CsvConfiguration(CultureInfo.GetCultureInfo("en-US"))
        {
            Delimiter = ","
        }, leaveOpen: false));

        await csv.ReadAsync();

        while (csv.Parser.RawRecord.StartsWith("#"))
        {
            await csv.ReadAsync();
        }

        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var country = bulkInsertCountries.FirstOrDefault(it =>
                it.NormalizedName == csv.GetField<string>("Country")!.ToUpperInvariant());

            var airports = bulkInsertAirports
                .Where(it => it.Country == country).Take(Random.Shared.Next(1, 2))
                .ToList();

            var airline = new Airline
            {
                Name = csv.GetField<string>("Name")!,
                NormalizedName = csv.GetField<string>("Name")!.ToUpperInvariant(),
                Country = country,
                Iata = ValueOrNull(csv.GetField<string>("IATA")),
                NormalizedIata = ValueOrNull(csv.GetField<string>("IATA"))?.ToUpperInvariant(),
                Callsign = ValueOrNull(csv.GetField<string>("Callsign")),
                Airports = airports
            };

            bulkInsertAirlines.Add(airline);
            if (!useBulkInsert)
            {
                database.Add(airline);
            }
        }

        if (useBulkInsert)
        {
            database.BulkInsert(bulkInsertAirlines);
        }

        await database.SaveChangesAsync();
    }
    else
    {
        bulkInsertAirlines.AddRange(await database.Airlines.ToListAsync());
    }
}