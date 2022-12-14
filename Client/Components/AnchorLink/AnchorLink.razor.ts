
export function ScrollToElementById(elementId: string): void {
    if (elementId && elementId.length != 0) {
        const element = document.getElementById(elementId);

        if (element instanceof HTMLElement) {
            element.scrollIntoView(true);
        }
    }
}