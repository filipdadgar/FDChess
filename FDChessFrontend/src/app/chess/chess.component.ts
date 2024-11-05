import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChessService } from '../chess.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chess',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
    this.chessService.getGameState().subscribe(
      data => {
        console.log('Fetched game state:', data); // Log the fetched data
        if (data && data.Board && data.Board.Pieces) {
          this.gameState = data;
          console.log('Updated game state:', this.gameState); // Log the updated game state
        } else {
          console.error('Invalid game state received', data);
          this.gameState = this.initializeGameState(); // Initialize with default values if needed
        }
      },
      error => {
        console.error('Error fetching game state', error);
        this.gameState = this.initializeGameState(); // Initialize with default values if error occurs
      }
    );
  }

  selectPiece(row: number, column: number): void {
    this.selectedPiece = this.gameState.Board.Pieces.find((piece: any) => piece.Position.Row === row && piece.Position.Column === column);
    if (this.selectedPiece) {
      console.log(`Selected piece: ${this.selectedPiece.Name} at (${row}, ${column})`);
    } else {
      console.log(`No piece at (${row}, ${column})`);
    }
  }

  getPieceName(row: number, column: number): string {
    if (!this.gameState || !this.gameState.Board || !this.gameState.Board.Pieces) {
        return '';
    }

    const piece = this.gameState.Board.Pieces.find((p: any) => p && p.Position && p.Position.Row === row && p.Position.Column === column);
    return piece ? piece.Name : '';
}


  makeMove(): void {
    if (!this.selectedPiece) {
      console.error('No piece selected');
      return;
    }
  
    const moveRequest = {
      board: {
        id: this.gameState.Board.Id,
        name: this.gameState.Board.Name,
        description: this.gameState.Board.Description,
        pieces: this.gameState.Board.Pieces.map((piece: any) => piece ? {
          id: piece.Id,
          name: piece.Name,
          position: {
            row: piece.Position.Row,
            column: piece.Position.Column
          },
          color: piece.Color
        } : null)
      },
      newPosition: { row: this.targetRow, column: this.targetColumn }
    };
  
    this.chessService.makeMove(moveRequest).subscribe(
      response => {
        this.getGameState(); // Reload game state after making a move
      },
      error => {
        console.error('Error making move', error);
      }
    );
  }
  

  initializeGameState(): any {
    // Initialize the game state with default values
    return {
      Board: {
        Pieces: [
          // Example board initialization
          { Id: 1, Name: 'Rook', Position: { Row: 0, Column: 0 }, Color: 'white' },
          { Id: 2, Name: 'Knight', Position: { Row: 0, Column: 1 }, Color: 'white' },
          // Add other pieces...
          { Id: 17, Name: 'Rook', Position: { Row: 7, Column: 0 }, Color: 'black' },
          { Id: 18, Name: 'Knight', Position: { Row: 7, Column: 1 }, Color: 'black' },
          // Add other pieces...
        ]
      }
    };
  }
}
