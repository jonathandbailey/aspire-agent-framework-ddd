import { BrowserRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import RootLayout from "./layouts/RootLayout";
import streamingService from "./services/chat/streaming.service";

streamingService.initialize();

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      retry: 3,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <RootLayout />
      </BrowserRouter>

    </QueryClientProvider>
  );
}

export default App;
