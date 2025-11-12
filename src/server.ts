import express from 'express';
import path from 'path';

const app = express();
const PORT = process.env.PORT || 3000;

// Serve compiled JS files with correct MIME type
app.use('/dist', express.static(path.join(__dirname, '.'), {
  setHeaders: (res, filepath) => {
    if (filepath.endsWith('.js')) {
      res.setHeader('Content-Type', 'application/javascript');
    }
  }
}));

// Serve assets from the dist/assets directory
app.use('/assets', express.static(path.join(__dirname, 'assets')));

// Serve static files from public directory
app.use(express.static(path.join(__dirname, '../src/public')));

// Serve index.html only for non-file routes
app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, '../src/public/index.html'));
});

app.listen(PORT, () => {
  console.log(`🎮 Ladders & Slides game server running at http://localhost:${PORT}`);
  console.log(`Open your browser to start playing!`);
});
