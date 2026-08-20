// Generates src/environments/environment.ts from Vercel/CI environment variables
// before the Angular production build runs, so the API URL is configurable per
// environment without hardcoding it in source.
const fs = require('fs');
const path = require('path');

const apiUrl = process.env['NG_APP_API_URL'] || 'http://localhost:5000/api';

const content = `export const environment = {
  production: true,
  apiUrl: '${apiUrl}'
};
`;

const targetPath = path.join(__dirname, 'src/environments/environment.ts');
fs.writeFileSync(targetPath, content);
console.log(`environment.ts written with apiUrl: ${apiUrl}`);
