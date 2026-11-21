import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'ge.pompa.mobile',
  appName: 'პომპა',
  webDir: 'dist/frontend/browser',
  server: {
    url: 'https://frontend-three-theta-3t6i7lsyzw.vercel.app',
    cleartext: false,
    androidScheme: 'https',
  },
  android: {
    backgroundColor: '#f3f6f8',
  },
};

export default config;
