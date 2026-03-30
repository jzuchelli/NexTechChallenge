import { defineConfig } from '@playwright/test';

const isCI = !!process.env.CI;

export default defineConfig({
  testDir: './e2e',
  timeout: 30_000,
  expect: {
    timeout: 10_000
  },
  use: {
    baseURL: 'http://localhost:4200',
    trace: 'on-first-retry'
  },
  webServer: [
    {
      command: 'dotnet run --project ../backend',
      url: 'http://localhost:5129',
      reuseExistingServer: !isCI,
      cwd: __dirname,
      timeout: 60_000
    },
    {
      command: 'npm run start -- --port 4200',
      url: 'http://localhost:4200',
      reuseExistingServer: !isCI,
      cwd: __dirname,
      timeout: 60_000
    }
  ]
});
