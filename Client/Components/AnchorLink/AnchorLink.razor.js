export function ScrollToElementById(elementId) {
    if (elementId && elementId.length != 0) {
        var element = document.getElementById(elementId);
        if (element instanceof HTMLElement) {
            element.scrollIntoView(true);
        }
    }
}
//# sourceMappingURL=AnchorLink.razor.js.map