namespace FDChess.Model;

public class PromotionRequest
{
    public Position Position { get; set; }
    public string NewPieceType { get; set; }
}