const title = document.getElementById('title');
const artist = document.getElementById('artist');
const thumbnailWrapper = document.getElementById('thumbnail-wrapper');
const thumbnail = document.getElementById('thumbnail');
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

  title.textContent = data.title || '';
  artist.textContent = data.artist || '';
  if (data.thumbnail) {
    showThumbnail(data.thumbnail);
  } else {
    hideThumbnail();
  }
}

function clear() {
  title.textContent = '';
  artist.textContent = '';
  hideThumbnail();
}

function showThumbnail({ base64, mimeType }) {
  thumbnailWrapper.classList.remove('empty');
  thumbnail.setAttribute('src', `data:${mimeType};base64,${base64}`);
}

function hideThumbnail() {
  thumbnailWrapper.classList.add('empty');
  thumbnail.setAttribute('src', '');
}

async function fetchMediaProperties() {
  try {
    const response = await fetch('/data');
    return response.ok ? response.json() : null;
  } catch (error) {
    console.error(error);
    return null;
  }
}
