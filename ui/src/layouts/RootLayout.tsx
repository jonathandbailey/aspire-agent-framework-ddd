import { Button, Layout, Result } from "antd";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { Conversation } from "../types/models/chat/conversation";
import { ConversationService } from "../services/chat/conversation.service";
import ChatPage from "../pages/chat/ChatPage";
import { Route, Routes } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import streamingService from "../services/chat/streaming.service";
import NavigationMenu from "../components/layout/NavigationMenu";

const { Content, Sider } = Layout;

const RootLayout = () => {
    const conversationService = new ConversationService();
    const queryClient = useQueryClient();
    const {
        data: conversations = [],
        isLoading: loadingNavigation,
        isError,
        error
    } = useQuery<Conversation[]>({
        queryKey: ["conversations"],
        queryFn: () => conversationService.LoadConversations(),
        retry: false,
    });
    const navigate = useNavigate();

    // Mutation for creating a conversation
    const { mutate: createConversation } = useMutation({
        mutationFn: () => conversationService.CreateConversation(),
        onSuccess: (newConversation: Conversation) => {
            navigate(`/conversation/${newConversation.id}`);
            // Optionally, invalidate or refetch conversations query here
        }
    });

    const handleAddPlanClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        createConversation();
    };

    // Conversations are loaded by React Query

    // If you need to update the UI when a conversation title changes, consider using a query invalidation or refetch
    streamingService.on("command", ({ conversationId, title }) => {
        queryClient.setQueryData(["conversations"], (oldConversations: Conversation[] = []) =>
            oldConversations.map(convo =>
                convo.id === conversationId ? { ...convo, name: title } : convo
            )
        );
        queryClient.invalidateQueries({ queryKey: ["conversations"] });
    });

    if (isError) {
        return <Result
            status="500"
            title="500"
            subTitle="Sorry, something went wrong."
            extra={<Button type="primary">Back Home</Button>}
        />
    }

    return (
        <Layout style={{ height: "100vh" }}>
            <Sider breakpoint="lg"
                style={{ margin: "0px", height: "100vh", overflow: "auto" }}
                width={250}
                theme="light" >
                <NavigationMenu conversations={conversations} handleAddPlanClick={handleAddPlanClick} loading={loadingNavigation} />
            </Sider>
            <Layout style={{ minHeight: 0, width: "100%" }}>
                <Content style={{ minHeight: 0, backgroundColor: "white" }} >
                    <Routes>
                        <Route path="/conversation/:id" element={<ChatPage />} />
                    </Routes>
                </Content>
            </Layout>
        </Layout>
    );
};

export default RootLayout;