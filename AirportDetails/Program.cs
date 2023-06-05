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

        var countriesAndAirports = countries
            .Select(country => new
            {
                Country = country,
                Airports = database.Airports
                    .Where(airport => airport.Country == country)
                    .ToList()
            })
            .ToList();

        // var countriesAndAirports = await database.Countries
        //     .Select(country => new
        //     {
        //         Country = country.Name,
        //         NumberOfAirports = country.Airports.Count
        //     })
        //     .ToListAsync();

        foreach (var countryAndAirports in countriesAndAirports)
        {
            Console.WriteLine(
                $"{countryAndAirports.Country.Name} has {countryAndAirports.Airports.Count} known airports.");
        }
    }
    else
    {
        var countriesAndAirports = await database.Countries
            .Include(country => country.Airports)
            .OrderBy(country => country.IsoCode)
            .AsSplitQuery()
            .ToListAsync();

        foreach (var countryAndAirports in countriesAndAirports)
        {
            Console.WriteLine($"{countryAndAirports.Name} has {countryAndAirports.Airports.Count} known airports.");
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
}