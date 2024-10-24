const cultureKey = "current_culture";

export function changeCulture(culture) {
    window.localStorage.setItem(cultureKey, culture);
    window.location.reload();
}

export function currentCulture() {
    let culture = window.localStorage.getItem(cultureKey);

    return culture ?? "en";
}