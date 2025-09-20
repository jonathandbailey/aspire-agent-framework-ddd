import { Layout } from "antd";
import { useEffect, useState } from "react";
import type { Conversation } from "../types/models/chat/conversation";
import { ConversationService } from "../services/chat/conversation.service";
import ChatPage from "../pages/chat/ChatPage";
import { Route, Routes } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import streamingService from "../services/chat/streaming.service";
import NavigationMenu from "../components/layout/NavigationMenu";

const { Content, Sider } = Layout;

const RootLayout = () => {
    const [conversations, setConversations] = useState<Conversation[]>([]);
    const [loadingNavigation, setLoadingNavigation] = useState<boolean>(false);

    const conversationService = new ConversationService();
    const navigate = useNavigate();

    const handleAddPlanClick = async (e: React.MouseEvent) => {
        e.stopPropagation();

        const newConversaton = await conversationService.CreateConversation();
        setConversations((prevConversations) => [...prevConversations, newConversaton]);
        navigate(`/conversation/${newConversaton.id}`);
    };

    useEffect(() => {

        setLoadingNavigation(true);
        const loadConversations = async () => {
            const convos = await conversationService.LoadConversations();
            setConversations(convos);
            setLoadingNavigation(false);
        };

        loadConversations();
    }, []);

    streamingService.on("command", ({ conversationId, title }) => {
        console.log("Received chat event:", conversationId, title);

        setConversations(prevConversations =>
            prevConversations.map(convo =>
                convo.id === conversationId
                    ? { ...convo, name: title }
                    : convo
            )
        );
    });

    return (
        <Layout style={{ height: "100vh" }}>
            <Sider breakpoint="lg"
                style={{ margin: "0px", height: "100vh", overflow: "auto" }}
                width={250}
                theme="light" >
                <NavigationMenu conversations={conversations} handleAddPlanClick={handleAddPlanClick} loading={loadingNavigation} />
            </Sider>
            <Layout style={{ minHeight: 0, width: "100%" }}>
                <Content
                    style={{
                        minHeight: 0,


                        backgroundColor: "white",
                    }}
                >
                    <Routes>
                        <Route path="/conversation/:id" element={<ChatPage />} />
                    </Routes>
                </Content>
            </Layout>
        </Layout>
    );
};

export default RootLayout;