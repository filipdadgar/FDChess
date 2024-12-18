import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChessService } from '../chess.service';
import { PositionService } from '../position.service';
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
  possibleMoves: { Row: number, Column: number }[] = [];
  removedPieces: any[] = []; // Declare removedPieces property
  moveHistory: any[] = []; // Track move history

  constructor(private chessService: ChessService, private positionService: PositionService) { }

  ngOnInit(): void {
    this.getGameState();
    this.getRemovedPieces(); // Fetch removed pieces on initialization
    this.loadMoveHistory(); // Load move history from local storage
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
    if (!this.gameState || !this.gameState.Board || !this.gameState.Board.Pieces) {
      console.error('Game state or board is not properly initialized');
      return;
    }
    console.log(`Piece selected at row: ${row}, column: ${column}`);
    this.selectedPiece = this.gameState.Board.Pieces.find((piece: any) => piece && piece.Position && piece.Position.Row === row && piece.Position.Column === column);
    if (this.selectedPiece) {
      console.log(`Selected piece: ${this.selectedPiece.Name}`);
      this.getPossibleMoves(this.selectedPiece);
    } else {
      console.log('No piece found at the selected position');
    }
  }

  getPossibleMoves(piece: any): void {
    this.chessService.getPossibleMoves(piece.Id).subscribe({
      next: (moves: { row: number, column: number }[]) => {
        // Map the API response to the expected format
        this.possibleMoves = moves.map(move => ({ Row: move.row, Column: move.column }));
        console.log('Possible moves for piece', piece, ':', this.possibleMoves);
      },
      error: (error: any) => {
        console.error('Error getting possible moves', error);
      }
    });
  }

  movePiece(row: number, column: number): void {
    console.log('Attempting to move piece to row:', row, 'column:', column);
    console.log('Current possible moves:', this.possibleMoves);
    this.possibleMoves.forEach(move => {
      console.log('Possible move:', move);
    });

    const possibleMove = this.possibleMoves.find(move => move.Row === row && move.Column === column);
    if (this.selectedPiece && possibleMove) {
      const moveRequest = {
        CurrentPosition: {
          Row: this.selectedPiece.Position.Row,
          Column: this.selectedPiece.Position.Column
        },
        NewPosition: {
          Row: row,
          Column: column
        }
      };

      console.log('Move request:', moveRequest);

      this.chessService.makeMove(moveRequest).subscribe({
        next: (response) => {
          console.log('Move response:', response);
          if (response.gameState) {
            this.gameState = response.gameState; // Update the game state with the response
          } else {
            this.gameState = response; // Fallback to the entire response if gameState is not present
          }

          // Add move to move history
          if (this.selectedPiece) {
            this.moveHistory.push({
              piece: this.selectedPiece.Name,
              color: this.selectedPiece.Color,
              from: moveRequest.CurrentPosition,
              to: moveRequest.NewPosition,
              message: response.message
            });
            this.saveMoveHistory(); // Save move history to local storage
          }

          this.selectedPiece = null; // Deselect the piece after the move
          this.possibleMoves = []; // Clear possible moves

          // Check for special conditions
          if (response.message === 'Check') {
            alert('Check!');
          } else if (response.message === 'Checkmate') {
            alert('Checkmate! Game over.');
            this.resetGame(); // Reset the game after checkmate
          } else if (response.message === 'Stalemate') {
            alert('Stalemate! Game over.');
          }

          // Update the game state after making the move
          this.getGameState();
          this.getRemovedPieces(); // Update removed pieces after making the move
        },
        error: (error) => {
          console.error('Error making move', error);
        }
      });
    } else {
      console.log('Invalid move. Selected piece:', this.selectedPiece, 'Possible move:', possibleMove);
    }
  }

  handleCellClick(row: number, column: number): void {
    console.log(`Cell clicked at row: ${row}, column: ${column}`);
    if (this.selectedPiece) {
      this.movePiece(row, column);
    } else {
      this.selectPiece(row, column);
    }
  }

  getPieceName(row: number, column: number): string {
    if (!this.gameState || !this.gameState.Board || !this.gameState.Board.Pieces) {
      return '';
    }

    const piece = this.gameState.Board.Pieces.find((p: any) => p && p.Position && p.Position.Row === row && p.Position.Column === column);
    if (piece) {
      // console.log(`Piece found at row: ${row}, column: ${column} - ${piece.Name}`);
    }
    return piece ? piece.Name : '';
  }

  isPossibleMove(row: number, column: number): boolean {
    return this.possibleMoves.some(move => move.Row === row && move.Column === column);
  }

  getPieceColor(row: number, column: number): string {
    if (!this.gameState || !this.gameState.Board || !this.gameState.Board.Pieces) {
      return '';
    }

    const piece = this.gameState.Board.Pieces.find((p: any) => p && p.Position && p.Position.Row === row && p.Position.Column === column);
    return piece ? piece.Color : '';
  }

  resetGame(): void {
    this.chessService.resetGame().subscribe({
      next: () => {
        this.getGameState(); // Refresh the game state after resetting
        this.getRemovedPieces(); // Refresh removed pieces after resetting
        this.moveHistory = []; // Clear move history
        this.saveMoveHistory(); // Save the cleared move history to local storage
      },
      error: (error) => {
        console.error('Error resetting game', error);
      }
    });
  }

  simulateGame(): void {
    this.chessService.simulateGame().subscribe(response => {
      this.gameState = response.gameState;
      this.getRemovedPieces(); // Refresh removed pieces after simulating game
    });
  }

  getRemovedPieces(): void {
    this.chessService.getRemovedPieces().subscribe({
      next: (data) => {
        console.log('Removed pieces:', data);
        this.removedPieces = data.map((piece: any) => ({
          ...piece,
          removedAt: piece.removedAt ? { Row: piece.removedAt.row, Column: piece.removedAt.column } : null
        })); // Correctly update the removedPieces property
      },
      error: (error) => {
        console.error('Error getting removed pieces', error);
      }
    });
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

  saveMoveHistory(): void {
    localStorage.setItem('moveHistory', JSON.stringify(this.moveHistory));
  }

  loadMoveHistory(): void {
    const savedHistory = localStorage.getItem('moveHistory');
    if (savedHistory) {
      this.moveHistory = JSON.parse(savedHistory);
    }
  }
}
