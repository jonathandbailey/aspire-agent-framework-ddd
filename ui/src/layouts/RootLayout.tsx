import { Button, Layout, Result } from "antd";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { Conversation } from "../types/models/chat/conversation";
import { ConversationService } from "../services/chat/conversation.service";
import ChatPage from "../pages/chat/ChatPage";
import { Route, Routes } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import streamingService from "../services/chat/streaming.service";
import NavigationMenu from "../components/layout/NavigationMenu";
import type { ConversationSummary } from "../types/models/chat/conversationSummary";

const { Content, Sider } = Layout;

const RootLayout = () => {
    const conversationService = new ConversationService();
    const queryClient = useQueryClient();
    const {
        data: conversations = [],
        isLoading: loadingNavigation,
        isError,
        error
    } = useQuery<ConversationSummary[]>({
        queryKey: ["conversations"],
        queryFn: () => conversationService.LoadConversationSummaries(),
        retry: false,
    });
    const navigate = useNavigate();

    const { mutate: createConversation } = useMutation({
        mutationFn: () => conversationService.CreateConversation(),
        onSuccess: (id: string) => {
            navigate(`/conversation/${id}`);
        }
    });

    const handleAddPlanClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        createConversation();
    };

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
        <Layout hasSider >
            <Sider
                breakpoint="lg"
                style={{
                    margin: "0px",
                    height: "100vh",
                    position: "sticky",
                    top: 0,
                    scrollbarWidth: "thin",
                    scrollbarGutter: "stable"
                }}
                width={250}
                theme="light" >
                <NavigationMenu conversations={conversations} handleAddPlanClick={handleAddPlanClick} loading={loadingNavigation} />
            </Sider>
            <Layout style={{ minHeight: 0, width: "100%" }}>
                <Content style={{ minHeight: 0, backgroundColor: "white", overflow: 'initial' }} >
                    <Routes>
                        <Route path="/conversation/:id" element={<ChatPage />} />
                    </Routes>
                </Content>
            </Layout>
        </Layout>
    );
};

export default RootLayout;