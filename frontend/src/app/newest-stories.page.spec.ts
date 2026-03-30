import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { NewestStoriesPage } from './newest-stories.page';
import { HackerNewsStory } from './hn-story.model';

const mockStories: HackerNewsStory[] = [
  { id: 1, title: 'Hello World', by: 'Ada', url: 'https://example.com', time: 1700000000, score: 10 },
  { id: 2, title: 'Second Story', by: 'Bob', url: null, time: 1700001000, score: 5 }
];

describe('NewestStoriesPage', () => {
  let fixture: ComponentFixture<NewestStoriesPage>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewestStoriesPage],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    fixture = TestBed.createComponent(NewestStoriesPage);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('loads and renders stories', () => {
    fixture.detectChanges();

    const req = httpMock.expectOne('/api/hackernews/newest?count=100');
    expect(req.request.method).toBe('GET');
    req.flush(mockStories);

    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Hello World');
    expect(compiled.textContent).toContain('Second Story');
  });

  it('shows an error state on failure', () => {
    fixture.detectChanges();

    const req = httpMock.expectOne('/api/hackernews/newest?count=100');
    req.flush('Server error', { status: 500, statusText: 'Server Error' });

    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Unable to load Hacker News stories');
  });

  it('filters stories with case-insensitive search', () => {
    fixture.detectChanges();

    const req = httpMock.expectOne('/api/hackernews/newest?count=100');
    req.flush(mockStories);

    fixture.detectChanges();

    const input = fixture.nativeElement.querySelector('input') as HTMLInputElement;
    input.value = 'hello';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Hello World');
    expect(compiled.textContent).not.toContain('Second Story');
  });

  it('paginates results', () => {
    const stories: HackerNewsStory[] = Array.from({ length: 12 }, (_, index) => ({
      id: index + 1,
      title: `Story ${index + 1}`,
      by: 'Tester',
      url: null,
      time: 1700000000 + index,
      score: 1
    }));

    fixture.detectChanges();
    const req = httpMock.expectOne('/api/hackernews/newest?count=100');
    req.flush(stories);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Story 1');
    expect(compiled.textContent).toContain('Story 10');
    expect(compiled.textContent).not.toContain('Story 11');

    const nextButton = fixture.nativeElement.querySelector('.pager button:last-child') as HTMLButtonElement;
    nextButton.click();
    fixture.detectChanges();

    expect(compiled.textContent).toContain('Story 11');
  });

  it('renders a fallback when url is missing', () => {
    fixture.detectChanges();

    const req = httpMock.expectOne('/api/hackernews/newest?count=100');
    req.flush(mockStories);

    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('No link');
  });
});
