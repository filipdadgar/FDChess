using System.Text.Json;
using System.Text.Json.Serialization;
using FDChess.Model;

public class PieceConverter : JsonConverter<Piece>
{
    public override Piece? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        if (!root.TryGetProperty("Name", out JsonElement nameElement))
        {
            Console.Error.WriteLine("The given key 'Name' was not present in the JSON.");
            return null;
        }
        string? name = nameElement.GetString()?.ToLower();

        try
        {
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
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deserializing piece: {ex.Message}");
            return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
    {
        try
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error serializing piece: {ex.Message}");
        }
    }
}