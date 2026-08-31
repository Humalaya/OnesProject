import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ToastrService } from 'ngx-toastr';
import { ScheduleService, Schedule } from '../../services/schedule.service';

@Component({
  selector: 'app-task-scheduler',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, DragDropModule],
  templateUrl: './task-scheduler.component.html',
  styleUrls: ['./task-scheduler.component.scss']
})
export class TaskSchedulerComponent implements OnInit, OnChanges {
  @Input({ required: true }) todoId!: string;
  @Output() changed = new EventEmitter<void>();

  schedules: Schedule[] = [];
  isLoading = false;
  isBusy = false;

  selectMode = false;
  selectedIds = new Set<string>();

  constructor(private scheduleService: ScheduleService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.load();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['todoId'] && !changes['todoId'].firstChange) {
      this.load();
    }
  }

  load(): void {
    this.isLoading = true;
    this.scheduleService.getForToDo(this.todoId).subscribe({
      next: (schedules) => {
        this.isLoading = false;
        this.schedules = schedules;
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error('Could not load schedules.');
      }
    });
  }

  add(): void {
    if (this.isBusy) return;
    this.isBusy = true;
    const now = new Date().toISOString();
    this.scheduleService.create(this.todoId, now).subscribe({
      next: (schedule) => {
        this.isBusy = false;
        this.schedules = [...this.schedules, schedule];
        this.changed.emit();
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not add a schedule.');
      }
    });
  }

  updateScheduledAt(schedule: Schedule, value: string): void {
    if (!value || this.isBusy) return;
    const iso = new Date(value).toISOString();
    if (iso === schedule.scheduledAt) return;

    this.isBusy = true;
    this.scheduleService.update(this.todoId, schedule.id, iso).subscribe({
      next: () => {
        this.isBusy = false;
        schedule.scheduledAt = iso;
        this.changed.emit();
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not update the schedule.');
      }
    });
  }

  toDatetimeLocal(iso: string): string {
    const date = new Date(iso);
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }

  toggleSelectMode(): void {
    this.selectMode = !this.selectMode;
    this.selectedIds.clear();
  }

  toggleSelected(id: string): void {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  isSelected(id: string): boolean {
    return this.selectedIds.has(id);
  }

  deleteSelected(): void {
    if (this.selectedIds.size === 0 || this.isBusy) return;
    const ids = Array.from(this.selectedIds);

    this.isBusy = true;
    this.scheduleService.bulkDelete(this.todoId, ids).subscribe({
      next: () => {
        this.isBusy = false;
        this.schedules = this.schedules.filter((s) => !this.selectedIds.has(s.id));
        this.selectedIds.clear();
        this.toastr.success('Schedule(s) deleted.');
        this.changed.emit();
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not delete the selected schedules.');
      }
    });
  }

  drop(event: CdkDragDrop<Schedule[]>): void {
    if (this.isBusy) return;
    moveItemInArray(this.schedules, event.previousIndex, event.currentIndex);

    this.isBusy = true;
    this.scheduleService.reorder(this.todoId, this.schedules.map((s) => s.id)).subscribe({
      next: () => {
        this.isBusy = false;
      },
      error: () => {
        this.isBusy = false;
        this.toastr.error('Could not save the new order.');
        this.load();
      }
    });
  }
}
