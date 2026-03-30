import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HackerNewsApiService } from './hn-api.service';
import { HackerNewsStory } from './hn-story.model';

@Component({
  selector: 'app-newest-stories-page',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './newest-stories.page.html',
  styleUrl: './newest-stories.page.css'
})
export class NewestStoriesPage implements OnInit {
  private readonly api = inject(HackerNewsApiService);

  stories: HackerNewsStory[] = [];
  filteredStories: HackerNewsStory[] = [];
  pagedStories: HackerNewsStory[] = [];

  query = '';
  page = 1;
  pageSize = 10;
  totalCount = 0;

  loading = true;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = null;

    this.api.getNewestStories(100).subscribe({
      next: (stories) => {
        this.stories = stories ?? [];
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load Hacker News stories. Please try again.';
        this.loading = false;
      }
    });
  }

  onQueryChange(): void {
    this.page = 1;
    this.applyFilters();
  }

  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page += 1;
      this.applyPaging();
    }
  }

  previousPage(): void {
    if (this.page > 1) {
      this.page -= 1;
      this.applyPaging();
    }
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  private applyFilters(): void {
    const term = this.query.trim().toLowerCase();

    if (!term) {
      this.filteredStories = [...this.stories];
    } else {
      this.filteredStories = this.stories.filter((story) =>
        this.buildSearchText(story).includes(term)
      );
    }

    this.totalCount = this.filteredStories.length;
    this.applyPaging();
  }

  private applyPaging(): void {
    const start = (this.page - 1) * this.pageSize;
    this.pagedStories = this.filteredStories.slice(start, start + this.pageSize);
  }

  private buildSearchText(story: HackerNewsStory): string {
    const title = story.title?.toLowerCase() ?? '';
    const by = story.by?.toLowerCase() ?? '';
    const url = story.url?.toLowerCase() ?? '';
    return `${title} ${by} ${url}`.trim();
  }
}
