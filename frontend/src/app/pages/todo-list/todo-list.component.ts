import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ToastrService } from 'ngx-toastr';
import { TodoService, Todo, Priority, SortBy, TaskFilter } from '../../services/todo.service';
import { ScheduleService } from '../../services/schedule.service';
import { TaskSchedulerComponent } from '../../shared/task-scheduler/task-scheduler.component';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, TaskSchedulerComponent],
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

  // Sort / filter
  sortBy: SortBy = 'newest';
  filter: TaskFilter = 'all';

  // New-task sidebar form
  newTitle = '';
  newDescription = '';
  newPriority: Priority = 'medium';
  newTags: string[] = [];
  tagInput = '';
  newDueDate = '';

  editId: string | null = null;
  editTitle = '';
  editDescription = '';

  expandedId: string | null = null;

  // Guards against double-clicking while a request is in flight (item 3)
  isBusy = false;

  constructor(
    private todoService: TodoService,
    private scheduleService: ScheduleService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadTodos();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  get remainingCount(): number {
    return this.todos.filter((t) => !t.isCompleted).length;
  }

  loadTodos(): void {
    this.todoService.getAll(this.pageNumber, this.pageSize, this.sortBy, this.filter).subscribe({
      next: (data) => {
        this.todos = data.items;
        this.totalCount = data.totalCount;
      },
      error: () => this.toastr.error('Could not load your tasks.')
    });
  }

  onPageSizeChange(): void {
    this.pageNumber = 1;
    this.loadTodos();
  }

  onSortChange(sortBy: SortBy): void {
    this.sortBy = sortBy;
    this.pageNumber = 1;
    this.loadTodos();
  }

  onFilterChange(filter: TaskFilter): void {
    this.filter = filter;
    this.pageNumber = 1;
    this.loadTodos();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || this.isBusy) return;
    this.pageNumber = page;
    this.loadTodos();
  }

  setNewPriority(priority: Priority): void {
    this.newPriority = priority;
  }

  addTag(): void {
    const tag = this.tagInput.trim().replace(/^#/, '');
    if (!tag || this.newTags.includes(tag)) {
      this.tagInput = '';
      return;
    }
    this.newTags.push(tag);
    this.tagInput = '';
  }

  removeTag(tag: string): void {
    this.newTags = this.newTags.filter((t) => t !== tag);
  }

  createTodo(): void {
    if (!this.newTitle.trim() || this.isBusy) return;

    const newTodo = {
      title: this.newTitle,
      description: this.newDescription,
      isCompleted: false,
      priority: this.newPriority,
      tags: this.newTags
    };
    const dueDate = this.newDueDate;

    this.isBusy = true;
    this.todoService.create(newTodo).subscribe({
      next: (created) => {
        const afterCreate = () => {
          this.isBusy = false;
          this.toastr.success('Task added.');
          this.newTitle = '';
          this.newDescription = '';
          this.newPriority = 'medium';
          this.newTags = [];
          this.tagInput = '';
          this.newDueDate = '';
          this.pageNumber = 1;
          this.loadTodos();
        };

        if (dueDate) {
          this.scheduleService.create(created.id, new Date(dueDate).toISOString()).subscribe({
            next: afterCreate,
            error: () => {
              this.toastr.error('Task added, but the due date could not be saved.');
              afterCreate();
            }
          });
        } else {
          afterCreate();
        }
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not add task.');
      }
    });
  }

  toggleComplete(todo: Todo): void {
    if (this.isBusy) return;

    const updated: Todo = {
      ...todo,
      isCompleted: !todo.isCompleted
    };

    this.isBusy = true;
    this.todoService.update(todo.id, updated).subscribe({
      next: () => {
        this.isBusy = false;
        this.loadTodos();
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not update task status.');
      }
    });
  }

  toggleExpand(todo: Todo): void {
    this.expandedId = this.expandedId === todo.id ? null : todo.id;
  }

  onScheduleChanged(): void {
    // A schedule was added/edited/deleted/reordered - refresh the collapsed-row summaries.
    this.loadTodos();
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
    if (!this.editTitle.trim() || this.isBusy) return;

    const updated: Todo = {
      ...todo,
      title: this.editTitle,
      description: this.editDescription
    };

    this.isBusy = true;
    this.todoService.update(todo.id, updated).subscribe({
      next: () => {
        this.isBusy = false;
        this.toastr.success('Task updated.');
        this.cancelEdit();
        this.loadTodos();
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not update task.');
      }
    });
  }

  deleteTodo(id: string): void {
    if (this.isBusy) return;
    if (!confirm('Are you sure you want to delete this task?')) return;

    this.isBusy = true;
    this.todoService.delete(id).subscribe({
      next: () => {
        this.isBusy = false;
        this.toastr.success('Task deleted.');
        if (this.todos.length === 1 && this.pageNumber > 1) {
          this.pageNumber -= 1;
        }
        this.loadTodos();
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not delete task.');
      }
    });
  }
}
