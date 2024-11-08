import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChessService {
  private baseUrl = 'http://localhost:5141/api/chess';

  constructor(private http: HttpClient) {}

  getGameState(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/state`);
  }

  makeMove(moveRequest: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/move`, moveRequest);
  }

  resetGame(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/reset`, {});
  }

  // chess.service.ts
  getPossibleMoves(pieceId: number): Observable<{ row: number, column: number }[]> {
    return this.http.get<{ row: number, column: number }[]>(`${this.baseUrl}/moves/${pieceId}`);
  }
}
