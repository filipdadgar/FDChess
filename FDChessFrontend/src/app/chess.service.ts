import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChessService {
  private apiUrl = 'http://localhost:5141/api/Chess';

  constructor(private http: HttpClient) { }

  getGameState(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/state`);
  }

  makeMove(moveRequest: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/move`, moveRequest);
  }
}
