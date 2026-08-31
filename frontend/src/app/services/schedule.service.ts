import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE } from '../core/api-config';

export interface Schedule {
  id: string;
  scheduledAt: string;
  order: number;
}

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  constructor(private http: HttpClient) {}

  private baseUrl(todoId: string): string {
    return `${API_BASE}/todo/${todoId}/schedules`;
  }

  getForToDo(todoId: string): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(this.baseUrl(todoId));
  }

  create(todoId: string, scheduledAt: string): Observable<Schedule> {
    return this.http.post<Schedule>(this.baseUrl(todoId), { scheduledAt });
  }

  update(todoId: string, scheduleId: string, scheduledAt: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl(todoId)}/${scheduleId}`, { scheduledAt });
  }

  bulkDelete(todoId: string, scheduleIds: string[]): Observable<void> {
    return this.http.post<void>(`${this.baseUrl(todoId)}/bulk-delete`, { scheduleIDs: scheduleIds });
  }

  reorder(todoId: string, orderedScheduleIds: string[]): Observable<void> {
    return this.http.put<void>(`${this.baseUrl(todoId)}/reorder`, { orderedScheduleIDs: orderedScheduleIds });
  }
}
