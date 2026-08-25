import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService, Todo } from '../../services/todo.service';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.scss']
})
export class TodoListComponent implements OnInit {
  todos: Todo[] = [];

  // Pagination
  pageSizeOptions = [5, 10, 20];
  pageSize = 10;
  pageNumber = 1;
  totalCount = 0;

  // Form models
  newTitle = '';
  newDescription = '';

  editId: string | null = null;
  editTitle = '';
  editDescription = '';

  constructor(private todoService: TodoService) {}

  ngOnInit(): void {
    this.loadTodos();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  loadTodos(): void {
    this.todoService.getAll(this.pageNumber, this.pageSize).subscribe({
      next: (data) => {
        this.todos = data.items;
        this.totalCount = data.totalCount;
      },
      error: (err) => {
        console.error('Error fetching todos:', err);
      }
    });
  }

  onPageSizeChange(): void {
    this.pageNumber = 1;
    this.loadTodos();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageNumber = page;
    this.loadTodos();
  }

  createTodo(): void {
    if (!this.newTitle.trim()) return;

    const newTodo = {
      title: this.newTitle,
      description: this.newDescription,
      isCompleted: false
    };

    this.todoService.create(newTodo).subscribe({
      next: () => {
        this.newTitle = '';
        this.newDescription = '';
        this.pageNumber = 1;
        this.loadTodos();
      },
      error: (err) => console.error('Error creating todo:', err)
    });
  }

  toggleComplete(todo: Todo): void {
    const updated: Todo = {
      ...todo,
      isCompleted: !todo.isCompleted
    };

    this.todoService.update(todo.id, updated).subscribe({
      next: () => this.loadTodos(),
      error: (err) => console.error('Error updating status:', err)
    });
  }

  startEdit(todo: Todo): void {
    this.editId = todo.id;
    this.editTitle = todo.title;
    this.editDescription = todo.description || '';
  }

  cancelEdit(): void {
    this.editId = null;
    this.editTitle = '';
    this.editDescription = '';
  }

  saveEdit(todo: Todo): void {
    if (!this.editTitle.trim()) return;

    const updated: Todo = {
      ...todo,
      title: this.editTitle,
      description: this.editDescription
    };

    this.todoService.update(todo.id, updated).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadTodos();
      },
      error: (err) => console.error('Error updating todo:', err)
    });
  }

  deleteTodo(id: string): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.todoService.delete(id).subscribe({
        next: () => {
          if (this.todos.length === 1 && this.pageNumber > 1) {
            this.pageNumber -= 1;
          }
          this.loadTodos();
        },
        error: (err) => console.error('Error deleting todo:', err)
      });
    }
  }
}
