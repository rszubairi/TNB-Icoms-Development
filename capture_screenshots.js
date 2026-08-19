const { chromium } = require('playwright');
const fs = require('fs');
const path = require('path');

const pages = [
  { name: 'login.png', url: 'https://tnb-icoms-design.vercel.app/login' },
  { name: 'index.png', url: 'https://tnb-icoms-design.vercel.app/' },
  { name: 'outage-status.png', url: 'https://tnb-icoms-design.vercel.app/outage-status' },
  { name: 'outage-list.png', url: 'https://tnb-icoms-design.vercel.app/outage-list' },
  { name: 'outage-creation.png', url: 'https://tnb-icoms-design.vercel.app/outage-creation' },
  { name: 'emergency-outage.png', url: 'https://tnb-icoms-design.vercel.app/emergency-outage' },
  { name: 'change-request.png', url: 'https://tnb-icoms-design.vercel.app/change-request' },
  { name: 'pending-review.png', url: 'https://tnb-icoms-design.vercel.app/pending-review' },
  { name: 'confirmation.png', url: 'https://tnb-icoms-design.vercel.app/confirmation' },
  { name: 'authorization.png', url: 'https://tnb-icoms-design.vercel.app/authorization' },
  { name: 'authorization-list.png', url: 'https://tnb-icoms-design.vercel.app/authorization-list' },
  { name: 'data-repository.png', url: 'https://tnb-icoms-design.vercel.app/data-repository' },
  { name: 'reports.png', url: 'https://tnb-icoms-design.vercel.app/reports' },
  { name: 'calendar.png', url: 'https://tnb-icoms-design.vercel.app/calendar' },
  { name: 'off-point-list.png', url: 'https://tnb-icoms-design.vercel.app/off-point-list' }
];

const outputDir = path.join(__dirname, 'Design');

async function captureScreenshots() {
  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1440, height: 900 },
    deviceScaleFactor: 2 // High DPI Retina quality screenshot
  });

  for (const pageInfo of pages) {
    console.log(`Capturing screenshot of ${pageInfo.url}...`);
    const page = await context.newPage();
    try {
      await page.goto(pageInfo.url, { waitUntil: 'networkidle', timeout: 30000 });
      await page.waitForTimeout(1500); // Wait for animations & fonts to render
      
      const filePath = path.join(outputDir, pageInfo.name);
      await page.screenshot({ path: filePath, fullPage: true });
      
      const stats = fs.statSync(filePath);
      console.log(`Saved screenshot ${pageInfo.name} (${(stats.size / 1024).toFixed(1)} KB)`);
    } catch (err) {
      console.error(`Error capturing screenshot for ${pageInfo.url}:`, err.message);
    } finally {
      await page.close();
    }
  }

  await browser.close();
  console.log('Finished capturing and saving all page screenshots.');
}

captureScreenshots();
