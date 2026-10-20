import { enhanceDatePickers } from "../components/date-picker";

const dispose = enhanceDatePickers();
if (import.meta.hot) {
  import.meta.hot.accept();
  import.meta.hot.dispose(async () => (await dispose)());
}
