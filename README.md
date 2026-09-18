# DirectionFinder

Direction Finder is a .NET console application that reads a hierarchical list of directions and items from `Data.txt`.

The application:

- parses the direction hierarchy into a tree structure;
- validates the input data format;
- displays the available items in alphabetical order;
- lets the user select an item by its displayed number;
- prints the directions required to reach the selected item.

![DirectionFinder console app](image.png)

# Requirements

- .NET 10 SDK

# Build

Run the following command from the repository root:

```bash
dotnet build DirectionFinder/DirectionFinder.csproj
```

# Run

Run the application with:

```bash
dotnet run --project DirectionFinder/DirectionFinder.csproj
```

The application uses `DirectionFinder/Data.txt` (default) as its input file. The file is copied to the build output directory automatically.

When prompted, enter the number of the item for which directions should be displayed.

# Project Structure

- `Program.cs` contains the console application flow and output handling.
- `Parsers/` contains the input file parser.
- `Validators/` contains validation rules for the input format.
- `Models/` contains the parsed tree and node models.
- `Data.txt` contains the input direction hierarchy.
