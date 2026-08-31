import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'light' | 'dark';

const THEME_KEY = 'todo_theme';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  theme = signal<Theme>(this.readStoredTheme());

  constructor() {
    this.applyTheme(this.theme());
  }

  toggle(): void {
    const next: Theme = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(next);
    this.applyTheme(next);
    if (this.isBrowser) localStorage.setItem(THEME_KEY, next);
  }

  private applyTheme(theme: Theme): void {
    if (!this.isBrowser) return;
    document.documentElement.setAttribute('data-theme', theme);
  }

  private readStoredTheme(): Theme {
    if (!this.isBrowser) return 'light';
    const stored = localStorage.getItem(THEME_KEY);
    return stored === 'dark' ? 'dark' : 'light';
  }
}
