import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE } from '../core/api-config';

export type Priority = 'low' | 'medium' | 'high';
export type SortBy = 'newest' | 'oldest' | 'priority_asc' | 'priority_desc';
export type TaskFilter = 'all' | 'active' | 'done';

export interface Todo {
  id: string;
  title: string;
  description: string;
  isCompleted: boolean;
  priority: Priority;
  tags: string[];
  createdAt: string;
  scheduleCount: number;
  nextScheduledAt: string | null;
}

export interface PagedTodos {
  items: Todo[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = `${API_BASE}/todo`;

  constructor(private http: HttpClient) {}

  getAll(pageNumber: number, pageSize: number, sortBy: SortBy = 'newest', filter: TaskFilter = 'all'): Observable<PagedTodos> {
    return this.http.get<PagedTodos>(this.apiUrl, {
      params: { pageNumber, pageSize, sortBy, filter }
    });
  }

  getById(id: string): Observable<Todo> {
    return this.http.get<Todo>(`${this.apiUrl}/${id}`);
  }

  create(todo: { title: string; description: string; isCompleted: boolean; priority: Priority; tags: string[] }): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, todo);
  }

  update(id: string, todo: Todo): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, todo);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
