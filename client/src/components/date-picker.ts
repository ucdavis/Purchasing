import "cally";
import type { CalendarDate } from "cally";

// Enhance the existing form input. It remains usable if this module never loads.
export async function enhanceDatePickers() {
  await Promise.all([
    customElements.whenDefined("calendar-date"),
    customElements.whenDefined("calendar-month"),
  ]);

  const cleanup: Array<() => void> = [];
  for (const picker of document.querySelectorAll<HTMLElement>(
    "[data-date-picker]",
  )) {
    const input = picker.querySelector<HTMLInputElement>('input[type="date"]');
    const details = picker.querySelector<HTMLDetailsElement>("details");
    const calendar =
      picker.querySelector<InstanceType<typeof CalendarDate>>("calendar-date");
    if (!input || !details || !calendar) continue;

    const sync = () => {
      calendar.value = input.value;
      // Keep date-only values as strings; UTC conversion would shift some dates.
      if (input.value) calendar.focusedDate = input.value;
    };
    const select = () => {
      input.value = calendar.value;
      input.dispatchEvent(new Event("change", { bubbles: true }));
      details.open = false;
      input.focus();
    };
    const close = (event: Event) => {
      if (event instanceof KeyboardEvent && event.key === "Escape") {
        details.open = false;
        details.querySelector("summary")?.focus();
      } else if (
        event.type === "pointerdown" &&
        !picker.contains(event.target as Node)
      ) {
        details.open = false;
      }
    };
    sync();
    details.hidden = false;
    input.addEventListener("change", sync);
    calendar.addEventListener("change", select);
    details.addEventListener("keydown", close);
    document.addEventListener("pointerdown", close);
    cleanup.push(() => {
      input.removeEventListener("change", sync);
      calendar.removeEventListener("change", select);
      details.removeEventListener("keydown", close);
      document.removeEventListener("pointerdown", close);
    });
  }
  return () => cleanup.forEach((dispose) => dispose());
}
