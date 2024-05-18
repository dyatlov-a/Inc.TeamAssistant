export function play(hostElement) {
    hostElement.muted = "muted";
    hostElement.playsinline = "playsinline";
    hostElement.play();
}