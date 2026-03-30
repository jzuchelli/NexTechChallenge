import { test, expect } from '@playwright/test';

test('loads newest stories and supports search/paging', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByText('Newest Stories')).toBeVisible();

  await page.waitForSelector('.story', { timeout: 30_000 });
  await expect.poll(async () => page.locator('.story').count()).toBeGreaterThan(0);

  const searchInput = page.getByPlaceholder('Search title, author, or URL');
  await searchInput.fill('open');

  await page.waitForTimeout(500);
  await expect.poll(async () => page.locator('.story').count()).toBeGreaterThan(0);

  const nextButton = page.getByRole('button', { name: 'Next' });
  if (await nextButton.isEnabled()) {
    await nextButton.click();
    await expect(page.getByText(/Page 2 of/)).toBeVisible();
  }
});
