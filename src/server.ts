import express from 'express';
import path from 'path';

const app = express();
const PORT = process.env.PORT || 3000;

// Serve static files from public directory
app.use(express.static(path.join(__dirname, '../src/public')));

// Serve assets
app.use('/assets', express.static(path.join(__dirname, '../src/assets')));

// Serve compiled JS
app.use('/dist', express.static(path.join(__dirname, '../dist')));

// Serve index.html for all routes (SPA)
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, '../src/public/index.html'));
});

app.listen(PORT, () => {
  console.log(`🎮 Ladders & Slides game server running at http://localhost:${PORT}`);
  console.log(`Open your browser to start playing!`);
});
