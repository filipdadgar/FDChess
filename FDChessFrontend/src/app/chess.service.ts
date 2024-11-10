// chess.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PositionService } from './position.service';

@Injectable({
  providedIn: 'root'
})
export class ChessService {
  private baseUrl = 'http://localhost:5141/api/chess';

  constructor(private http: HttpClient, private positionService: PositionService) {}

  getGameState(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/state`);
  }

  makeMove(moveRequest: any): Observable<any> {
    this.positionService.addPosition(moveRequest.CurrentPosition);
    this.positionService.addPosition(moveRequest.NewPosition);
    return this.http.post<any>(`${this.baseUrl}/move`, moveRequest);
  }

  resetGame(): Observable<any> {
    this.positionService.clearPositions();
    return this.http.post<any>(`${this.baseUrl}/reset`, {});
  }

  getPossibleMoves(pieceId: number): Observable<{ row: number, column: number }[]> {
    return this.http.get<{ row: number, column: number }[]>(`${this.baseUrl}/moves/${pieceId}`);
  }

  simulateGame(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/simulate`, {});
  }

  getRemovedPieces(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/removed`);
  }
}
