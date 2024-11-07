using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FDChess.Model;

namespace FDChess.Helper
{
    public class PieceListConverter : JsonConverter<List<Piece>>
    {
        public override List<Piece> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<Piece>();
            var pieceConverter = new PieceConverter();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var piece = pieceConverter.Read(ref reader, typeof(Piece), options);
                if (piece != null)
                {
                    list.Add(piece);
                }
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<Piece> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            var pieceConverter = new PieceConverter();

            foreach (var piece in value)
            {
                pieceConverter.Write(writer, piece, options);
            }

            writer.WriteEndArray();
        }
    }
}