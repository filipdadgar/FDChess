import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChessService {
  private baseUrl = 'http://localhost:5141/api/chess'; // Update this to match your backend URL

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
}
