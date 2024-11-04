using System.Text.Json;
using System.Text.Json.Serialization;
using FDChess.Model;

namespace FDChess.Helper;

public class PieceConverter : JsonConverter<Piece>
{
    public override Piece? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        if (!root.TryGetProperty("Name", out JsonElement nameElement))
        {
            // Handle missing property gracefully
            Console.Error.WriteLine("The given key 'Name' was not present in the JSON.");
            return null; // or provide a default Piece object
        }
        string? name = nameElement.GetString()?.ToLower();

        return name switch
        {
            "pawn" => JsonSerializer.Deserialize<Pawn>(root.GetRawText(), options),
            "rook" => JsonSerializer.Deserialize<Rook>(root.GetRawText(), options),
            "knight" => JsonSerializer.Deserialize<Knight>(root.GetRawText(), options),
            "bishop" => JsonSerializer.Deserialize<Bishop>(root.GetRawText(), options),
            "queen" => JsonSerializer.Deserialize<Queen>(root.GetRawText(), options),
            "king" => JsonSerializer.Deserialize<King>(root.GetRawText(), options),
            _ => throw new NotSupportedException($"Piece type '{name}' is not supported")
        };
    }



    public override void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}