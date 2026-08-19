const { chromium } = require('playwright');
const fs = require('fs');
const path = require('path');

const pages = [
  { name: 'login.html', url: 'https://tnb-icoms-design.vercel.app/login' },
  { name: 'index.html', url: 'https://tnb-icoms-design.vercel.app/' },
  { name: 'outage-status.html', url: 'https://tnb-icoms-design.vercel.app/outage-status' },
  { name: 'outage-list.html', url: 'https://tnb-icoms-design.vercel.app/outage-list' },
  { name: 'outage-creation.html', url: 'https://tnb-icoms-design.vercel.app/outage-creation' },
  { name: 'emergency-outage.html', url: 'https://tnb-icoms-design.vercel.app/emergency-outage' },
  { name: 'change-request.html', url: 'https://tnb-icoms-design.vercel.app/change-request' },
  { name: 'pending-review.html', url: 'https://tnb-icoms-design.vercel.app/pending-review' },
  { name: 'confirmation.html', url: 'https://tnb-icoms-design.vercel.app/confirmation' },
  { name: 'authorization.html', url: 'https://tnb-icoms-design.vercel.app/authorization' },
  { name: 'authorization-list.html', url: 'https://tnb-icoms-design.vercel.app/authorization-list' },
  { name: 'data-repository.html', url: 'https://tnb-icoms-design.vercel.app/data-repository' },
  { name: 'reports.html', url: 'https://tnb-icoms-design.vercel.app/reports' },
  { name: 'calendar.html', url: 'https://tnb-icoms-design.vercel.app/calendar' },
  { name: 'off-point-list.html', url: 'https://tnb-icoms-design.vercel.app/off-point-list' }
];

const outputDir = path.join(__dirname, 'Design');

async function downloadPages() {
  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1440, height: 900 }
  });

  for (const pageInfo of pages) {
    console.log(`Fetching ${pageInfo.url}...`);
    const page = await context.newPage();
    try {
      await page.goto(pageInfo.url, { waitUntil: 'networkidle', timeout: 30000 });
      // Extra wait for any client side animations/state to stabilize
      await page.waitForTimeout(1000);
      const content = await page.content();
      const filePath = path.join(outputDir, pageInfo.name);
      fs.writeFileSync(filePath, content, 'utf8');
      console.log(`Saved ${pageInfo.name} (${content.length} bytes)`);
    } catch (err) {
      console.error(`Error fetching ${pageInfo.url}:`, err.message);
    } finally {
      await page.close();
    }
  }

  await browser.close();
  console.log('Finished saving all HTML reference files.');
}

downloadPages();
