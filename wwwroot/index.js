const TITLE = document.getElementById('title');
const ARTIST = document.getElementById('artist');
const THUMBNAIL = document.getElementById('thumbnail');
const THUMBNAIL_WRAPPER = document.getElementById('thumbnail-wrapper');
const ERROR = document.getElementById('error');
let noThumbnailCount = 0;

async function main() {
  updateWidget(await fetchMediaProperties());
  setInterval(async () => {
    updateWidget(await fetchMediaProperties());
  }, 1000);
}

main();

function updateWidget(data) {
  if (!data) {
    clear();
    return;
  }
  if (!data.thumbnail && noThumbnailCount < 3) {
    noThumbnailCount++;
    return;
  }
  noThumbnailCount = 0;

  TITLE.textContent = data.title || '';
  ARTIST.textContent = data.artist || '';
  if (data.thumbnail) {
    showThumbnail(data.thumbnail);
  } else {
    hideThumbnail();
  }
}

function clear() {
  TITLE.textContent = '';
  ARTIST.textContent = '';
  hideThumbnail();
}

function showThumbnail({ base64, mimeType }) {
  THUMBNAIL_WRAPPER.classList.remove('empty');
  THUMBNAIL.setAttribute('src', `data:${mimeType};base64,${base64}`);
}

function hideThumbnail() {
  THUMBNAIL_WRAPPER.classList.add('empty');
  THUMBNAIL.setAttribute('src', '');
}

async function fetchMediaProperties() {
  try {
    const response = await fetch('/data');
    ERROR.textContent = '';
    return response.ok ? response.json() : null;
  } catch (error) {
    ERROR.textContent = 'Server is down';
    return null;
  }
}
