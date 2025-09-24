import create from "zustand"
import type { UIExchange } from "../types/ui/UIExchange"


type LiveExchange = UIExchange & { isPending?: boolean }

type LiveExchangeState = {
    liveExchanges: Record<string, LiveExchange[]> // keyed by threadId
    addExchange: (threadId: string, exchange: LiveExchange) => void
    updateExchange: (threadId: string, id: string, patch: Partial<LiveExchange>) => void
    removeExchange: (threadId: string, id: string) => void
    appendToAssistantMessage: (threadId: string, id: string, text: string) => void
}

export const useLiveExchangesStore = create<LiveExchangeState>((set) => ({
    liveExchanges: {},

    addExchange: (threadId, exchange) =>
        set((state) => ({
            liveExchanges: {
                ...state.liveExchanges,
                [threadId]: [...(state.liveExchanges[threadId] ?? []), exchange],
            },
        })),

    updateExchange: (threadId, id, patch) =>
        set((state) => ({
            liveExchanges: {
                ...state.liveExchanges,
                [threadId]: (state.liveExchanges[threadId] ?? []).map((e) =>
                    e.id === id ? { ...e, ...patch } : e
                ),
            },
        })),

    appendToAssistantMessage: (threadId, id, text) =>
        set((state) => {
            const threadExchanges = state.liveExchanges[threadId] ?? []
            return {
                liveExchanges: {
                    ...state.liveExchanges, // new map
                    [threadId]: threadExchanges.map((e) => {
                        if (e.id !== id || !e.assistant) return e
                        return {
                            ...e, // new exchange object
                            assistant: {
                                ...e.assistant, // new message object
                                text: e.assistant.text + text, // new string
                            },
                        }
                    }),
                },
            }
        }),


    removeExchange: (threadId, id) =>
        set((state) => ({
            liveExchanges: {
                ...state.liveExchanges,
                [threadId]: (state.liveExchanges[threadId] ?? []).filter((e) => e.id !== id),
            },
        })),




}))

export function useLiveExchanges(threadId: string) {
    const exchanges = useLiveExchangesStore((s) => s.liveExchanges[threadId] ?? [])
    const addExchange = useLiveExchangesStore((s) => s.addExchange)
    const updateExchange = useLiveExchangesStore((s) => s.updateExchange)
    const removeExchange = useLiveExchangesStore((s) => s.removeExchange)
    const appendToAssistantMessage = useLiveExchangesStore((s) => s.appendToAssistantMessage)

    return {
        exchanges,
        addExchange: (exchange: LiveExchange) => addExchange(threadId, exchange),
        updateExchange: (id: string, patch: Partial<LiveExchange>) =>
            updateExchange(threadId, id, patch),
        removeExchange: (id: string) => removeExchange(threadId, id),
        appendToAssistantMessage: (id: string, text: string) =>
            appendToAssistantMessage(threadId, id, text),
    }
}
