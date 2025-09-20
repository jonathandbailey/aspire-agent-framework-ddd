import { BrowserRouter } from "react-router-dom";
import RootLayout from "./layouts/RootLayout";
import streamingService from "./services/chat/streaming.service";

streamingService.initialize();

function App() {

  return (
    <BrowserRouter>
      <RootLayout />
    </BrowserRouter>
  );
}

export default App
