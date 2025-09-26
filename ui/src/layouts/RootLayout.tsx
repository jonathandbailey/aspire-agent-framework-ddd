import { useMemo } from "react";
import { Button, Flex, Layout, Result } from "antd";
import { useMutation } from "@tanstack/react-query";
import { Route, Routes, useNavigate } from "react-router-dom";
import ChatPage from "../pages/chat/ChatPage";
import NavigationMenu from "../components/layout/NavigationMenu";
import { ConversationService } from "../services/chat/conversation.service";
import { useConversationSummaries } from "../hooks/useConversationSummaries";
import { useCommandStreamHandler } from "../hooks/useCommandStreamHandler";
import type { ConversationSummary } from "../types/models/chat/conversationSummary";
import styles from "./RootLayout.module.css";

const { Content, Sider } = Layout;

const RootLayout = () => {
    const navigate = useNavigate();
    const conversationService = useMemo(() => new ConversationService(), []);
    const {
        data: conversations = [] as ConversationSummary[],
        isLoading: loadingNavigation,
        isError,
    } = useConversationSummaries();

    useCommandStreamHandler();

    const { mutate: createConversation } = useMutation({
        mutationFn: () => conversationService.CreateConversation(),
        onSuccess: (id: string) => {
            navigate(`/conversation/${id}`);
        },
    });

    const handleAddPlanClick = (e: React.MouseEvent) => {
        e.stopPropagation();
        createConversation();
    };

    if (isError) {
        return (
            <Result
                status="500"
                title="500"
                subTitle="Sorry, something went wrong."
                extra={<Button type="primary">Back Home</Button>}
            />
        );
    }

    return (
        <Flex vertical gap="middle">
            <Layout hasSider className={styles.layout}>
                <Sider
                    breakpoint="lg"
                    className={styles.sider}
                    width={250}
                    theme="light"
                >
                    <NavigationMenu
                        conversations={conversations}
                        handleAddPlanClick={handleAddPlanClick}
                        loading={loadingNavigation}
                    />
                </Sider>
                <Layout className={styles.innerLayout}>
                    <Content className={styles.content}>
                        <Routes>
                            <Route path="/conversation/:id" element={<ChatPage />} />
                        </Routes>
                    </Content>
                </Layout>
            </Layout>
        </Flex>
    );
};

export default RootLayout;
