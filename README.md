# CsvSerializer: The Fastest CSV Serializer for .NET

[![NuGet](https://img.shields.io/nuget/v/RG.CsvSerializer.svg)](https://www.nuget.org/packages/RG.CsvSerializer/)

## Installation
Package Manager Console:
```
Install-Package RG.CsvSerializer
```

## Usage
```csharp
using Csv;

// Serialize collection to CSV string
string csv = CsvSerializer.Serialize(items, withHeaders: true);

// Deserialize CSV string to array
Item[] items = CsvSerializer.Deserialize<Item>(csv, hasHeaders: true);
```

With custom delimiter:
```csharp
Item[] items = CsvSerializer.Deserialize<Item>(csv, hasHeaders: true, separator: ';');
```

With custom header name and date format:
```csharp
class Product {
    public string Name { get; set; }
    
    [CsvColumn("Price")]
    public decimal PriceBeforeTaxes { get; set; }
    
    [CsvColumn("Added", DateFormat = "dd/MM/yyyy")]
    public DateTime Added { get; set; }
}
```

Serializing to stream:
```csharp
CsvSerializer.Serialize(streamWriter, items, withHeaders: true);
```

Deserializing from stream:
```csharp
IEnumerable<Item> items = CsvSerializer.Deserialize<Item>(streamReader, hasHeaders: true);
```

## Supported Property Types
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
Uri // serialized as quoted string
Enum // serialized as unquoted string
Enum? // serialized as unquoted string
```

Not yet supported:
```csharp
char
char?
DateTimeOffset
DateTimeOffset?
TimeSpan
TimeSpan?
Guid
Guid?
any Object to string (serialize only)
any Object to JSON
any Object to MessagePack Base64
any Object to MessagePack Hex string
```
