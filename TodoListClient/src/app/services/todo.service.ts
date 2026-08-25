import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Todo {
  id: string;
  title: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
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
  private apiUrl = 'http://localhost:5000/api/todo';

  constructor(private http: HttpClient) {}

  getAll(pageNumber: number, pageSize: number): Observable<PagedTodos> {
    return this.http.get<PagedTodos>(this.apiUrl, {
      params: { pageNumber, pageSize }
    });
  }

  getById(id: string): Observable<Todo> {
    return this.http.get<Todo>(`${this.apiUrl}/${id}`);
  }

  create(todo: { title: string; description: string; isCompleted: boolean }): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, todo);
  }

  update(id: string, todo: Todo): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, todo);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
