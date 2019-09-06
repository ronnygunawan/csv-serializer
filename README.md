[![NuGet](https://img.shields.io/nuget/v/RG.CsvSerializer.svg)](https://www.nuget.org/packages/RG.CsvSerializer/)

# Installation
Package Manager Console:
```
Install-Package RG.CsvSerializer
```

# Usage
```csharp
using Csv;

// Serialize collection to CSV string
string csv = CsvSerializer.Serialize(items, withHeaders: true);

// Deserialize CSV string to array
Item[] items = CsvSerializer.Deserialize<Item>(csv, hasHeaders: true);
```

# Custom Header Name and Date Format
```csharp
class Product {
    public string Name { get; set; }
    
    [CsvColumn("Price")]
    public decimal PriceBeforeTaxes { get; set; }
    
    [CsvColumn("Added", DateFormat = "dd/MM/yyyy")]
    public DateTime Added { get; set; }
}
```

# Supported Property Types
```csharp
bool
bool?
byte
byte?
sbyte
sbyte?
short
short?
ushort
ushort?
int
int?
uint
uint?
long
long?
ulong
ulong?
float
float?
double
double?
decimal
decimal?
string
DateTime
DateTime?
```

## Not yet supported:
```csharp
char
char?
DateTimeOffset
DateTimeOffset?
TimeSpan
TimeSpan?
Uri
Guid
Guid?
any Enum values
any Object to string (serialize only)
any Object to JSON
any Object to MessagePack Base64
any Object to MessagePack Hex string
```
