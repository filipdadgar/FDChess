// position.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PositionService {
  private positions: { row: number, column: number }[] = [];

  addPosition(position: { row: number, column: number }): void {
    this.positions.push(position);
  }

  getPositions(): { row: number, column: number }[] {
    return this.positions;
  }

  clearPositions(): void {
    this.positions = [];
  }
}
