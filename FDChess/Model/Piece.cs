﻿// Piece.cs
using System.Text.Json.Serialization;
using FDChess.Model;

public abstract class Piece
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public Position Position { get; set; }
    public string? Color { get; set; }

    // Parameterless constructor for deserialization
    protected Piece() { }

    // Constructor with parameters
    [JsonConstructor]
    protected Piece(int id, string name, Position position, string color)
    {
        Id = id;
        Name = name;
        Position = position;
        Color = color;
    }

    public override string ToString()
    {
        return $"{Name} at {Position}";
    }

    // Abstract method requiring implementation in derived classes
    public abstract bool IsMoveValid(Position newPosition, Board board);

    // Move method to update the position if the move is valid
    public bool Move(Position newPosition, Board board)
    {
        if (IsMoveValid(newPosition, board))
        {
            Position = newPosition;
            return true;
        }
        return false;
    }
}