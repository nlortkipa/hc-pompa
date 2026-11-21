import { createReadStream, statSync } from 'node:fs';
import { pipeline } from 'node:stream/promises';
import { fileURLToPath } from 'node:url';

const backend = { target: 'http://localhost:5032', secure: false, changeOrigin: true };
const apkPath = fileURLToPath(new URL('./public/downloads/pompa-android.apk', import.meta.url));

export default {
  '/api': backend,
  '/swagger': backend,
  // The dev server sends APKs without a Content-Type, so Android offers "open with"
  // instead of installing. Serve the file ourselves with the correct headers.
  '/downloads/android': {
    target: 'http://localhost:4200',
    bypass: async (req, res) => {
      res.writeHead(200, {
        'Content-Type': 'application/vnd.android.package-archive',
        'Content-Length': statSync(apkPath).size,
        'Content-Disposition': 'attachment; filename="pompa-android.apk"',
        'Cache-Control': 'no-cache',
      });
      await pipeline(createReadStream(apkPath), res);
      return req.url;
    },
  },
};
