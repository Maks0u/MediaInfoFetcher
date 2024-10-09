async function fetchMediaProperties() {
  try {
    const response = await fetch('/data');
    if (!response.ok) {
      return {};
    }
    return response.json();
  } catch (error) {
    console.error(error);
    return {};
  }
}

let noThumbnailCount = 0;

function updateMediaProperties(data) {
  if (!data.thumbnail && noThumbnailCount < 3) {
    noThumbnailCount++;
    return;
  }
  noThumbnailCount = 0;

  const title = document.getElementById('title');
  const artist = document.getElementById('artist');
  const thumbnail = document.getElementById('thumbnail');
  const thumbnailWrapper = document.getElementById('thumbnail-wrapper');

  title.textContent = data.title || '';
  artist.textContent = data.artist || '';
  if (data.thumbnail) {
    thumbnailWrapper.classList.remove('empty');
    thumbnail.classList.remove('empty');
    thumbnail.setAttribute(
      'src',
      `data:${data.thumbnail?.mimeType};base64,${data.thumbnail?.base64}`
    );
  } else {
    thumbnailWrapper.classList.add('empty');
    thumbnail.classList.add('empty');
  }
}

async function main() {
  updateMediaProperties(await fetchMediaProperties());
  setInterval(async () => {
    updateMediaProperties(await fetchMediaProperties());
  }, 1000);
}

main();
