const CACHE_NAME = 'emac-app-v1';
const urlsToCache = [
    '/',
    '/css/emac_style.css',
    '/js/main.js',
    '/images/emac_logo.png'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(urlsToCache);
            })
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                return response || fetch(event.request);
            })
    );
});