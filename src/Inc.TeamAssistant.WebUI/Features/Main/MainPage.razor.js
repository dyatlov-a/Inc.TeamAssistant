function playVideo(videoId) {
    let video = document.getElementById(videoId);
    video.muted = "muted";
    video.playsinline = "playsinline";
    video.play();
}

export function onLoad(){
    window.playVideo = playVideo;
}