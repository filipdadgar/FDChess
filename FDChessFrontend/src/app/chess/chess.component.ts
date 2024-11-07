import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChessService } from '../chess.service';
import { FormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-chess',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropModule],
  templateUrl: './chess.component.html',
  styleUrls: ['./chess.component.css']
})
export class ChessComponent implements OnInit {
  gameState: any = { Board: { Pieces: [] } }; // Initialize with default value
  selectedPiece: any;
  targetRow: number = 0;
  targetColumn: number = 0;

  constructor(private chessService: ChessService) { }

  ngOnInit(): void {
    this.getGameState();
  }

  getGameState(): void {
    this.chessService.getGameState().subscribe({
      next: (data) => {
        if (data && data.Board && data.Board.Pieces) {
          console.log('Game state:', data);
          this.gameState = data;
        } else {
          this.gameState = this.initializeGameState(); // Initialize with default values if needed
        }
      },
      error: (error) => {
        this.gameState = this.initializeGameState(); // Initialize with default values if error occurs
        console.error('Error getting game state', error);
      }
    });
  }

  selectPiece(row: number, column: number): void {
    this.selectedPiece = this.gameState.Board.Pieces.find((piece: any) => piece && piece.Position && piece.Position.Row === row && piece.Position.Column === column);
  }

  getPieceName(row: number, column: number): string {
    if (!this.gameState || !this.gameState.Board || !this.gameState.Board.Pieces) {
      return '';
    }

    const piece = this.gameState.Board.Pieces.find((p: any) => p && p.Position && p.Position.Row === row && p.Position.Column === column);
    return piece ? piece.Name : '';
  }

  onDrop(event: any): void {
    console.log('Drop event:', event);
    const previousIndex = event.previousContainer.data.index;
    const currentIndex = event.container.data.index;
    const previousRow = Math.floor(previousIndex / 8);
    const previousCol = previousIndex % 8;
    const currentRow = Math.floor(currentIndex / 8);
    const currentCol = currentIndex % 8;

    // Find the piece to move based on the previous position
    this.selectedPiece = this.gameState.Board.Pieces.find((piece: any) => piece && piece.Position && piece.Position.Row === previousRow && piece.Position.Column === previousCol);

    if (this.selectedPiece) {
      const moveRequest = {
        PieceId: this.selectedPiece.Id,
        NewPosition: { Row: currentRow, Column: currentCol }
      };

      this.chessService.makeMove(moveRequest).subscribe({
        next: (response) => {
          console.log('Move response:', response);
          this.gameState = response; // Update the game state with the new game state
        },
        error: (error) => {
          console.error('Error making move', error);
        }
      });
    }
  }

  resetGame(): void {
    this.chessService.resetGame().subscribe(
      response => {
        this.getGameState(); // Reload game state after resetting
      },
      error => {
        console.error('Error resetting game', error);
      }
    );
  }

  initializeGameState(): any {
    // Initialize the game state with default values
    return {
      Board: {
        Id: 1,
        Name: "Default Board",
        Description: "Initial game state",
        Pieces: [
          { Id: 1, Name: 'Rook', Position: { Row: 0, Column: 0 }, Color: 'white' },
          { Id: 2, Name: 'Knight', Position: { Row: 0, Column: 1 }, Color: 'white' },
          { Id: 3, Name: 'Bishop', Position: { Row: 0, Column: 2 }, Color: 'white' },
          { Id: 4, Name: 'Queen', Position: { Row: 0, Column: 3 }, Color: 'white' },
          { Id: 5, Name: 'King', Position: { Row: 0, Column: 4 }, Color: 'white' },
          { Id: 6, Name: 'Bishop', Position: { Row: 0, Column: 5 }, Color: 'white' },
          { Id: 7, Name: 'Knight', Position: { Row: 0, Column: 6 }, Color: 'white' },
          { Id: 8, Name: 'Rook', Position: { Row: 0, Column: 7 }, Color: 'white' },
          { Id: 9, Name: 'Pawn', Position: { Row: 1, Column: 0 }, Color: 'white' },
          { Id: 10, Name: 'Pawn', Position: { Row: 1, Column: 1 }, Color: 'white' },
          { Id: 11, Name: 'Pawn', Position: { Row: 1, Column: 2 }, Color: 'white' },
          { Id: 12, Name: 'Pawn', Position: { Row: 1, Column: 3 }, Color: 'white' },
          { Id: 13, Name: 'Pawn', Position: { Row: 1, Column: 4 }, Color: 'white' },
          { Id: 14, Name: 'Pawn', Position: { Row: 1, Column: 5 }, Color: 'white' },
          { Id: 15, Name: 'Pawn', Position: { Row: 1, Column: 6 }, Color: 'white' },
          { Id: 16, Name: 'Pawn', Position: { Row: 1, Column: 7 }, Color: 'white' },
          { Id: 17, Name: 'Rook', Position: { Row: 7, Column: 0 }, Color: 'black' },
          { Id: 18, Name: 'Knight', Position: { Row: 7, Column: 1 }, Color: 'black' },
          { Id: 19, Name: 'Bishop', Position: { Row: 7, Column: 2 }, Color: 'black' },
          { Id: 20, Name: 'Queen', Position: { Row: 7, Column: 3 }, Color: 'black' },
          { Id: 21, Name: 'King', Position: { Row: 7, Column: 4 }, Color: 'black' },
          { Id: 22, Name: 'Bishop', Position: { Row: 7, Column: 5 }, Color: 'black' },
          { Id: 23, Name: 'Knight', Position: { Row: 7, Column: 6 }, Color: 'black' },
          { Id: 24, Name: 'Rook', Position: { Row: 7, Column: 7 }, Color: 'black' },
          { Id: 25, Name: 'Pawn', Position: { Row: 6, Column: 0 }, Color: 'black' },
          { Id: 26, Name: 'Pawn', Position: { Row: 6, Column: 1 }, Color: 'black' },
          { Id: 27, Name: 'Pawn', Position: { Row: 6, Column: 2 }, Color: 'black' },
          { Id: 28, Name: 'Pawn', Position: { Row: 6, Column: 3 }, Color: 'black' },
          { Id: 29, Name: 'Pawn', Position: { Row: 6, Column: 4 }, Color: 'black' },
          { Id: 30, Name: 'Pawn', Position: { Row: 6, Column: 5 }, Color: 'black' },
          { Id: 31, Name: 'Pawn', Position: { Row: 6, Column: 6 }, Color: 'black' },
          { Id: 32, Name: 'Pawn', Position: { Row: 6, Column: 7 }, Color: 'black' }
        ]
      }
    };
  }
}
