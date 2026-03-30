import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { HackerNewsApiService } from './hn-api.service';
import { HackerNewsStory } from './hn-story.model';

describe('HackerNewsApiService', () => {
  let service: HackerNewsApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HackerNewsApiService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(HackerNewsApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('requests newest stories with default count', () => {
    const response: HackerNewsStory[] = [];

    service.getNewestStories().subscribe((stories) => {
      expect(stories).toEqual(response);
    });

    const req = httpMock.expectOne('/api/hackernews/newest?count=100');
    expect(req.request.method).toBe('GET');
    req.flush(response);
  });

  it('requests newest stories with custom count', () => {
    const response: HackerNewsStory[] = [];

    service.getNewestStories(25).subscribe();

    const req = httpMock.expectOne('/api/hackernews/newest?count=25');
    expect(req.request.method).toBe('GET');
    req.flush(response);
  });
});
