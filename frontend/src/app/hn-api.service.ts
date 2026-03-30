import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HackerNewsStory } from './hn-story.model';

@Injectable({ providedIn: 'root' })
export class HackerNewsApiService {
  private readonly http = inject(HttpClient);

  getNewestStories(count = 100): Observable<HackerNewsStory[]> {
    return this.http.get<HackerNewsStory[]>(`/api/hackernews/newest?count=${count}`);
  }
}
