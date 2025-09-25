import { Menu } from "antd";
import { Skeleton } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";
import type { ConversationSummary } from "../../types/models/chat/conversationSummary";

interface NavigationMenuProps {
    conversations: ConversationSummary[];
    handleAddPlanClick: (e: React.MouseEvent) => void;
    loading: boolean;
}

const NavigationMenu: React.FC<NavigationMenuProps> = ({ conversations, handleAddPlanClick, loading }) => {
    return (
        <Skeleton active loading={!!loading} style={{ padding: "16px" }}>
            <Menu style={{ paddingTop: "32px", paddingLeft: "8px" }}>
                <Menu.ItemGroup
                    key="g3"
                    title={
                        <div style={{ color: "gray", paddingLeft: 12, display: "flex", alignItems: "center", justifyContent: "space-between", gap: 8 }}>
                            <span>Conversations</span>
                            <PlusOutlined style={{ cursor: "pointer" }} onClick={handleAddPlanClick} />
                        </div>
                    }
                >
                    {conversations?.map((conversation: ConversationSummary) => (
                        <Menu.Item key={conversation.id}>
                            <Link to={`/conversation/${conversation.id}`}>
                                <span style={{ textOverflow: "ellipsis", overflow: "hidden", whiteSpace: "nowrap", display: "block", width: "24ch" }}>
                                    {conversation.title}
                                </span>
                            </Link>
                        </Menu.Item>
                    ))}
                </Menu.ItemGroup>
            </Menu>
        </Skeleton>
    )
}
export default NavigationMenu;