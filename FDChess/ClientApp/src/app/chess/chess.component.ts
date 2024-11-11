import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChessService } from '../chess.service';
// Remove PositionService import as it's not used
import { FormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-chess',
  templateUrl: './chess.component.html',
  styleUrls: ['./chess.component.css'],
  imports: [CommonModule, FormsModule, DragDropModule],
  standalone: true
})
export class ChessComponent implements OnInit {
  gameState: any = { Board: { Pieces: [] } }; // Initialize with default value
  selectedPiece: any;
  possibleMoves: { row: number, column: number }[] = []; // Update to lowercase properties
  removedPieces: any[] = []; // Declare removedPieces property
  moveHistory: any[] = []; // Track move history

  constructor(private chessService: ChessService) { } // Remove PositionService from constructor

  ngOnInit(): void {
    this.getGameState();
    this.getRemovedPieces();
  }

  getGameState(): void {
    this.chessService.getGameState().subscribe({
      next: (gameState: any) => {
        this.gameState = gameState;
        console.log('Game state:', this.gameState);
      },
      error: (error: any) => {
        console.error('Error getting game state:', error);
      }
    });
  }

  selectPiece(piece: any): void {
    this.selectedPiece = piece;
    this.getPossibleMoves(piece);
  }

  getPossibleMoves(piece: any): void {
    this.chessService.getPossibleMoves(piece.Id).subscribe({
      next: (moves: { row: number, column: number }[]) => {
        // No need to map, just assign directly
        this.possibleMoves = moves;
        console.log('Possible moves for piece', piece, ':', this.possibleMoves);
      },
      error: (error: any) => {
        console.error('Error getting possible moves:', error);
      }
    });
  }

  makeMove(row: number, column: number): void {
    console.log('Possible move:', { row, column });

    const possibleMove = this.possibleMoves.find(move => move.row === row && move.column === column);
    if (this.selectedPiece && possibleMove) {
      const moveRequest = {
        CurrentPosition: {
          Row: this.selectedPiece.Position.Row,
          Column: this.selectedPiece.Position.Column
        },
        NewPosition: {
          Row: row,
          Column: column
        },
        PieceId: this.selectedPiece.Id
      };

      this.chessService.makeMove(moveRequest).subscribe({
        next: (updatedGameState: any) => {
          this.gameState = updatedGameState;
          this.selectedPiece = null;
          this.possibleMoves = [];
          this.moveHistory.push(moveRequest); // Add move to history
          console.log('Move made:', moveRequest);
        },
        error: (error: any) => {
          console.error('Error making move:', error);
        }
      });
    } else {
      console.log('Invalid move');
    }
  }

  getRemovedPieces(): void {
    this.chessService.getRemovedPieces().subscribe({
      next: (data: any[]) => {
        console.log('Removed pieces:', data);
        this.removedPieces = data.map((piece: any) => ({
          ...piece,
          removedAt: piece.RemovedAt ? { Row: piece.RemovedAt.Row, Column: piece.RemovedAt.Column } : null
        })); // Correctly update the removedPieces property
      },
      error: (error) => {
        console.error('Error getting removed pieces:', error);
      }
    });
  }

  isPossibleMove(row: number, column: number): boolean {
    return this.possibleMoves.some(move => move.row === row && move.column === column);
  }

  getPieceColor(row: number, column: number): string {
    const piece = this.gameState.Board.Pieces.find((p: any) => p.Position.Row === row && p.Position.Column === column);
    return piece ? piece.Color : '';
  }

  getPieceType(row: number, column: number): string {
    const piece = this.gameState.Board.Pieces.find((p: any) => p.Position.Row === row && p.Position.Column === column);
    return piece ? piece.Type : '';
  }

  isSelected(row: number, column: number): boolean {
    return this.selectedPiece?.Position.Row === row && this.selectedPiece?.Position.Column === column;
  }

  isDraggable(row: number, column: number): boolean {
    const piece = this.gameState.Board.Pieces.find((p: any) => p.Position.Row === row && p.Position.Column === column);
    return piece && piece.Color === 'White';
  }

  onDrop(event: any): void {
    const { previousContainer, previousIndex, currentIndex, item } = event;
    const { row, column } = item.data;
    this.makeMove(row, column);
  }
}
