#!/usr/bin/env node
/**
 * Enhanced bundle size guard.
 * Now supports multiple entry name patterns: main.*, index.*, app.* (configurable).
 * Environment overrides:
 *   BUNDLE_PATTERNS="main.*,index.*"
 *   BUNDLE_DELTA_KB=30
 */
const fs = require('fs');
const path = require('path');

const targetDir = process.argv[2] || 'dist/assets';
const baselineFile = path.join('scripts', 'bundle-baseline.json');
const allowedDeltaKB = parseInt(process.env.BUNDLE_DELTA_KB || '30', 10);

// Comma-separated glob-ish patterns (literal prefix matching + wildcard hash)
const patternEnv = process.env.BUNDLE_PATTERNS || 'index-*,main-*,main.*,index.*';
const patterns = patternEnv.split(',').map(p => p.trim()).filter(Boolean);

// Convert simple wildcard pattern ( * ) to regex; keep ".js" optional if pattern already ends with .js
function toRegex(p) {
  const hasExt = p.endsWith('.js');
  const base = hasExt ? p.slice(0, -3) : p;
  const esc = base.replace(/[.+?^${}()|[\]\\]/g, '\\$&').replace(/\*/g, '.*');
  return new RegExp('^' + esc + (hasExt ? '\\.js$' : '.*\\.js$'));
}

const regexes = patterns.map(toRegex);

function format(kb) { return `${kb.toFixed(2)}KB`; }

if (!fs.existsSync(targetDir)) {
  console.error(`Bundle directory not found: ${targetDir}`);
  process.exit(0);
}

const allJs = fs.readdirSync(targetDir).filter(f => f.endsWith('.js'));
const candidate = allJs.filter(f => regexes.some(r => r.test(f)));

if (candidate.length === 0) {
  // Fallback: attempt to use index-*.js if present
  const fallback = allJs.find(f => /^index-.*\.js$/.test(f));
  if (fallback) {
    candidate.push(fallback);
    console.warn(`(fallback) Using ${fallback} as entry; adjust BUNDLE_PATTERNS if needed.`);
  } else {
    console.warn(`No entry chunk matched patterns: ${patterns.join(', ')}`);
    console.warn(`Available JS files (truncated): ${allJs.slice(0, 10).join(', ')}${allJs.length > 10 ? ' ...' : ''}`);
    process.exit(0);
  }
}

// Pick largest matching entry (covers index vs main)
let largestFile = '';
let largestSize = -1;
for (const f of candidate) {
  const stat = fs.statSync(path.join(targetDir, f));
  if (stat.size > largestSize) {
    largestSize = stat.size;
    largestFile = f;
  }
}

const sizeKB = largestSize / 1024;
let baselineKB = null;
if (fs.existsSync(baselineFile)) {
  try {
    baselineKB = JSON.parse(fs.readFileSync(baselineFile, 'utf8')).main;
  } catch {
    console.warn('Failed parsing baseline file.');
  }
}

if (baselineKB != null) {
  const diff = sizeKB - baselineKB;
  console.log(`Entry chunk: ${largestFile} size: ${format(sizeKB)} (baseline ${format(baselineKB)}, diff ${format(diff)})`);
  if (diff > allowedDeltaKB) {
    console.error(`ERROR: Size exceeded baseline + ${allowedDeltaKB}KB`);
    process.exit(1);
  }
} else {
  console.log(`Entry chunk: ${largestFile} size: ${format(sizeKB)} (no baseline file)`);
  console.log('To set baseline run (PowerShell example):');
  console.log(`echo {"main": ${sizeKB.toFixed(2)}} > scripts/bundle-baseline.json`);
}
