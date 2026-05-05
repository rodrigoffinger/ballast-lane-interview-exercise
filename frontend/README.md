# TaskPlanner Frontend

React, Vite, TypeScript, and Tailwind CSS frontend for TaskPlanner.

## Development

Run the frontend directly during UI development:

```bash
npm install
npm run dev
```

The Vite dev server proxies `/api` requests to the local ASP.NET Core API.

## Production Build

```bash
npm run build
```

The root `run-dev.bat` script builds this frontend and copies `dist` into the API project's `wwwroot` directory for the single-endpoint demo.

